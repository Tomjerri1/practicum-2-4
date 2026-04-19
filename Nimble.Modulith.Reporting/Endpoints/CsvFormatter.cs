using System.Reflection;
using System.Text;

namespace Nimble.Modulith.Reporting.Endpoints;

public static class CsvFormatter
{
    public static string ToCsv<T>(IEnumerable<T> data)
    {
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var sb = new StringBuilder();

        sb.AppendLine(string.Join(",", properties.Select(p => Escape(p.Name))));

        foreach (var item in data)
        {
            var values = properties.Select(p => Escape(p.GetValue(item)?.ToString()));
            sb.AppendLine(string.Join(",", values));
        }

        return sb.ToString();
    }

    private static string Escape(string? value)
    {
        if (string.IsNullOrEmpty(value)) return "";
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }
        return value;
    }
}