namespace Nimble.Modulith.Reporting.Services;

public record OrderSummaryReport(
    int OrderId,
    string OrderNumber,
    DateTime OrderDate,
    string CustomerName,
    decimal TotalAmount,
    int ItemCount
);

public record ProductSalesReportItem(
    int ProductId,
    string ProductName,
    int TotalQuantitySold,
    decimal TotalRevenue,
    int OrderCount
);

public record CustomerOrderHistoryReport(
    int CustomerId,
    string CustomerName,
    string Email,
    int TotalOrders,
    decimal TotalSpent,
    DateTime FirstOrderDate,
    DateTime LastOrderDate
);