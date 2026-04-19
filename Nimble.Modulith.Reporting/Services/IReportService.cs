namespace Nimble.Modulith.Reporting.Services;

public interface IReportService
{
    Task<IEnumerable<OrderSummaryReport>> GetOrdersReportAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<ProductSalesReportItem>> GetProductSalesReportAsync(DateTime startDate, DateTime endDate);
    Task<CustomerOrderHistoryReport?> GetCustomerOrderHistoryAsync(int customerId);
}