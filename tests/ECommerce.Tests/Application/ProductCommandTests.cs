using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Products.Commands;
using ECommerce.Application.Features.Products.Queries;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.ValueObjects;
using AutoMapper;
using ECommerce.Application.Common.Mappings;
using FluentAssertions;
using Moq;

namespace ECommerce.Tests.Application;

public class ProductCommandTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IProductRepository> _productRepoMock;
    private readonly Mock<ICategoryRepository> _categoryRepoMock;
    private readonly IMapper _mapper;
    private static readonly Guid TestCategoryId = Guid.NewGuid();

    public ProductCommandTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _productRepoMock = new Mock<IProductRepository>();
        _categoryRepoMock = new Mock<ICategoryRepository>();
        _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Categories).Returns(_categoryRepoMock.Object);

        // Mock category exists
        var testCategory = Category.Create("Electronics", "Electronic devices");
        _categoryRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(testCategory);

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task CreateProduct_UniqueSkU_ShouldSucceed()
    {
        _productRepoMock.Setup(r => r.GetBySkuAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);
        _productRepoMock.Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product p, CancellationToken _) => p);

        var handler = new CreateProductCommandHandler(_unitOfWorkMock.Object);
        var command = new CreateProductCommand("Test", "Desc", 10m, 50, "UNIQUE-SKU", TestCategoryId, null);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateProduct_DuplicateSku_ShouldFail()
    {
        var existing = Product.Create("Existing", "Desc", 10m, 10, "DUP-SKU", TestCategoryId);
        _productRepoMock.Setup(r => r.GetBySkuAsync("DUP-SKU", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var handler = new CreateProductCommandHandler(_unitOfWorkMock.Object);
        var command = new CreateProductCommand("New", "Desc", 10m, 50, "DUP-SKU", TestCategoryId, null);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("already exists");
    }

    [Fact]
    public async Task GetAllProducts_ShouldReturnMappedDtos()
    {
        var products = new List<Product>
        {
            Product.Create("Product 1", "Desc1", 10m, 10, "SKU1", TestCategoryId),
            Product.Create("Product 2", "Desc2", 20m, 20, "SKU2", TestCategoryId),
        };
        _productRepoMock.Setup(r => r.GetActiveProductsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        var handler = new GetAllProductsQueryHandler(_unitOfWorkMock.Object, _mapper);
        var result = await handler.Handle(new GetAllProductsQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
        result[0].Name.Should().Be("Product 1");
        result[1].Price.Should().Be(20m);
    }
}
