namespace Nimble.Modulith.Customers.Domain.OrderAggregate;

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount => Items.Sum(i => i.TotalPrice);
    public List<OrderItem> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public void AddItem(OrderItem item)
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException($"Cannot add items to order in {Status} status");
        }
        
        Items.Add(item);
    }
}

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Processing = 2,
    Shipped = 3,
    Delivered = 4,
    Cancelled = 5
}