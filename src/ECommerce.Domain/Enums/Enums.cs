namespace ECommerce.Domain.Enums;

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Processing = 2,
    Shipped = 3,
    Delivered = 4,
    Cancelled = 5,
    Refunded = 6
}

public enum PaymentStatus
{
    Pending = 0,
    Completed = 1,
    Failed = 2,
    Refunded = 3
}

public enum ProductCategory
{
    Electronics = 0,
    Clothing = 1,
    Books = 2,
    HomeAndGarden = 3,
    Sports = 4,
    Food = 5,
    Other = 6
}

public enum ShipmentStatus
{
    Pending = 0,
    Shipped = 1,
    InTransit = 2,
    OutForDelivery = 3,
    Delivered = 4,
    Returned = 5
}

public enum PaymentMethod
{
    CreditCard = 0,
    DebitCard = 1,
    PayPal = 2,
    BankTransfer = 3,
    CashOnDelivery = 4
}

public enum DiscountType
{
    Percentage = 0,
    FixedAmount = 1
}
