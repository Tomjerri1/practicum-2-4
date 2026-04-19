using FastEndpoints;
using Nimble.Modulith.Reporting.Services;
using Microsoft.AspNetCore.Http;

namespace Nimble.Modulith.Reporting.Endpoints.Reports;

public class OrdersReportRequest
{
    public DateTime StartDate { get; set; } = DateTime.UtcNow.AddMonths(-1);
    public DateTime EndDate { get; set; } = DateTime.UtcNow;
    public string? Format { get; set; }
}

public class OrdersReport(IReportService reportService) : Endpoint<OrdersReportRequest, IEnumerable<OrderSummaryReport>>
{
    public override void Configure()
    {
        Get("/reports/orders");
        AllowAnonymous();
    }

    public override async Task HandleAsync(OrdersReportRequest req, CancellationToken ct)
    {
        var report = await reportService.GetOrdersReportAsync(req.StartDate, req.EndDate);

        var acceptsCsv = HttpContext.Request.Headers.Accept.ToString().Contains("text/csv");
        if (req.Format?.Equals("csv", StringComparison.OrdinalIgnoreCase) == true || acceptsCsv)
        {
            var csv = CsvFormatter.ToCsv(report);
            HttpContext.Response.ContentType = "text/csv";
            await HttpContext.Response.WriteAsync(csv, ct);
            return;
        }

        Response = report;
    }
}