using FastEndpoints;
using Nimble.Modulith.Reporting.Services;
using Microsoft.AspNetCore.Http;

namespace Nimble.Modulith.Reporting.Endpoints.Reports;

public class CustomerOrdersReportRequest
{
    public int CustomerId { get; set; }
    public string? Format { get; set; }
}

public class CustomerOrdersReport(IReportService reportService) : Endpoint<CustomerOrdersReportRequest, CustomerOrderHistoryReport>
{
    public override void Configure()
    {
        Get("/reports/customers/{customerId}/orders");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CustomerOrdersReportRequest req, CancellationToken ct)
    {

        var customerId = Route<int>("customerId");
        if (customerId == 0) customerId = req.CustomerId;

        var report = await reportService.GetCustomerOrderHistoryAsync(customerId);

        if (report == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var acceptsCsv = HttpContext.Request.Headers.Accept.ToString().Contains("text/csv");
        if (req.Format?.Equals("csv", StringComparison.OrdinalIgnoreCase) == true || acceptsCsv)
        {
            var csv = CsvFormatter.ToCsv(new[] { report });
            HttpContext.Response.ContentType = "text/csv";
            await HttpContext.Response.WriteAsync(csv, ct);
            return;
        }

        Response = report;
    }
}