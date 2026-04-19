using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Nimble.Modulith.Reporting.Services;

public class ReportService(IConfiguration configuration) : IReportService
{
    private readonly string _connectionString = configuration.GetConnectionString("ReportingDb")!;

    public async Task<IEnumerable<OrderSummaryReport>> GetOrdersReportAsync(DateTime startDate, DateTime endDate)
    {
        using var connection = new SqlConnection(_connectionString);
        
        var sql = @"
            SELECT 
                f.OrderId,
                f.OrderNumber,
                MIN(d.Date) as OrderDate,
                c.Name as CustomerName,
                MAX(f.OrderTotalAmount) as TotalAmount,
                SUM(f.Quantity) as ItemCount
            FROM Reporting.FactOrders f
            JOIN Reporting.DimDate d ON f.DateKey = d.DateKey
            JOIN Reporting.DimCustomer c ON f.CustomerId = c.CustomerId
            WHERE d.Date >= @StartDate AND d.Date <= @EndDate
            GROUP BY f.OrderId, f.OrderNumber, c.Name
            ORDER BY OrderDate DESC";

        return await connection.QueryAsync<OrderSummaryReport>(sql, new { StartDate = startDate, EndDate = endDate });
    }

    public async Task<IEnumerable<ProductSalesReportItem>> GetProductSalesReportAsync(DateTime startDate, DateTime endDate)
    {
        using var connection = new SqlConnection(_connectionString);
        
        var sql = @"
            SELECT 
                p.ProductId,
                p.Name as ProductName,
                SUM(f.Quantity) as TotalQuantitySold,
                SUM(f.TotalPrice) as TotalRevenue,
                COUNT(DISTINCT f.OrderId) as OrderCount
            FROM Reporting.FactOrders f
            JOIN Reporting.DimProduct p ON f.ProductId = p.ProductId
            JOIN Reporting.DimDate d ON f.DateKey = d.DateKey
            WHERE d.Date >= @StartDate AND d.Date <= @EndDate
            GROUP BY p.ProductId, p.Name
            ORDER BY TotalRevenue DESC";

        return await connection.QueryAsync<ProductSalesReportItem>(sql, new { StartDate = startDate, EndDate = endDate });
    }

    public async Task<CustomerOrderHistoryReport?> GetCustomerOrderHistoryAsync(int customerId)
    {
        using var connection = new SqlConnection(_connectionString);
        
        var sql = @"
            SELECT 
                c.CustomerId,
                c.Name as CustomerName,
                c.Email,
                COUNT(DISTINCT f.OrderId) as TotalOrders,
                SUM(f.TotalPrice) as TotalSpent,
                MIN(d.Date) as FirstOrderDate,
                MAX(d.Date) as LastOrderDate
            FROM Reporting.DimCustomer c
            LEFT JOIN Reporting.FactOrders f ON c.CustomerId = f.CustomerId
            LEFT JOIN Reporting.DimDate d ON f.DateKey = d.DateKey
            WHERE c.CustomerId = @CustomerId
            GROUP BY c.CustomerId, c.Name, c.Email";

        return await connection.QueryFirstOrDefaultAsync<CustomerOrderHistoryReport>(sql, new { CustomerId = customerId });
    }
}