using System.Text.RegularExpressions;

namespace OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins;

public static class HttpClientRequestHostChecker
{
    /// <summary>
    /// 要求對象是需要收集追蹤資料的目標 (非 exceptionless / flagsmith / loki)
    /// </summary>
    /// <param name="host"></param>
    /// <returns></returns>
    public static bool IsValidHost(string? host)
    {
        return !IsIgnoreHost(host ?? string.Empty);
    }

    /// <summary>
    /// 以驗證是否為不需要紀錄追蹤的對外要求；目前驗證 loki 的相關網址
    /// </summary>
    /// <param name="host"></param>
    /// <returns></returns>
    private static bool IsIgnoreHost(string host)
    {
        return Regex.IsMatch(host,
                             @"^loki.*$",
                             // @"^loki\.kube-monitor\.svc\.cluster\.local$",
                             RegexOptions.IgnoreCase);
    }
}