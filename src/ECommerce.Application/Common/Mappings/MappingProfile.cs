using AutoMapper;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Category, Features.Categories.Queries.CategoryDto>();

        CreateMap<Product, Features.Products.Queries.ProductDto>()
            .ForMember(d => d.Price, opt => opt.MapFrom(s => s.Price.Amount))
            .ForMember(d => d.Currency, opt => opt.MapFrom(s => s.Price.Currency))
            .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category != null ? s.Category.Name : string.Empty));

        CreateMap<Order, Features.Orders.Queries.OrderDto>()
            .ForMember(d => d.TotalAmount, opt => opt.MapFrom(s => s.TotalAmount.Amount))
            .ForMember(d => d.Currency, opt => opt.MapFrom(s => s.TotalAmount.Currency))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.PaymentStatus, opt => opt.MapFrom(s => s.PaymentStatus.ToString()))
            .ForMember(d => d.ShippingStreet, opt => opt.MapFrom(s => s.ShippingAddress.Street))
            .ForMember(d => d.ShippingCity, opt => opt.MapFrom(s => s.ShippingAddress.City))
            .ForMember(d => d.ShippingState, opt => opt.MapFrom(s => s.ShippingAddress.State))
            .ForMember(d => d.ShippingZipCode, opt => opt.MapFrom(s => s.ShippingAddress.ZipCode))
            .ForMember(d => d.ShippingCountry, opt => opt.MapFrom(s => s.ShippingAddress.Country));

        CreateMap<OrderItem, Features.Orders.Queries.OrderItemDto>()
            .ForMember(d => d.UnitPrice, opt => opt.MapFrom(s => s.UnitPrice.Amount))
            .ForMember(d => d.Subtotal, opt => opt.MapFrom(s => s.Subtotal.Amount));

        CreateMap<Customer, Features.Customers.Queries.CustomerDto>()
            .ForMember(d => d.ShippingStreet, opt => opt.MapFrom(s => s.ShippingAddress != null ? s.ShippingAddress.Street : null))
            .ForMember(d => d.ShippingCity, opt => opt.MapFrom(s => s.ShippingAddress != null ? s.ShippingAddress.City : null))
            .ForMember(d => d.ShippingState, opt => opt.MapFrom(s => s.ShippingAddress != null ? s.ShippingAddress.State : null))
            .ForMember(d => d.ShippingZipCode, opt => opt.MapFrom(s => s.ShippingAddress != null ? s.ShippingAddress.ZipCode : null))
            .ForMember(d => d.ShippingCountry, opt => opt.MapFrom(s => s.ShippingAddress != null ? s.ShippingAddress.Country : null));

        CreateMap<Employee, Features.Employees.Queries.EmployeeDto>();

        CreateMap<Shipper, Features.Shippers.Queries.ShipperDto>();

        CreateMap<Shipment, Features.Shipments.Queries.ShipmentDto>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.ShipperName, opt => opt.MapFrom(s => s.Shipper != null ? s.Shipper.CompanyName : string.Empty));

        CreateMap<Payment, Features.Payments.Queries.PaymentDto>()
            .ForMember(d => d.Amount, opt => opt.MapFrom(s => s.Amount.Amount))
            .ForMember(d => d.Currency, opt => opt.MapFrom(s => s.Amount.Currency))
            .ForMember(d => d.Method, opt => opt.MapFrom(s => s.Method.ToString()))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));

        CreateMap<Review, Features.Reviews.Queries.ReviewDto>()
            .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.Product != null ? s.Product.Name : string.Empty))
            .ForMember(d => d.CustomerName, opt => opt.MapFrom(s => s.Customer != null ? $"{s.Customer.FirstName} {s.Customer.LastName}" : string.Empty));

        CreateMap<Coupon, Features.Coupons.Queries.CouponDto>()
            .ForMember(d => d.DiscountType, opt => opt.MapFrom(s => s.DiscountType.ToString()));
    }
}
