using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins;

public static class HttpRequestUserAgentChecker
{
    /// <summary>
    /// 是有效來源 (非 A10 / k8s 等 LB 檢查器)
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsValidUser(string? input)
    {
        return string.IsNullOrEmpty(input) || IsNotA10OrK8S(input);
    }

    /// <summary>
    /// 不是 A10 或 k8s
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <remarks>因為要支援多版 dotnet，因此先忽略舊版無法使用的 SYSLIB1045 提醒</remarks>
    [SuppressMessage("Performance", "SYSLIB1045:轉換為 \'GeneratedRegexAttribute\'。")]
    private static bool IsNotA10OrK8S(string input)
    {
        return !Regex.IsMatch(input, @"^a10hm/\d+.\d+|^kube-probe/\d+.\d+", RegexOptions.IgnoreCase);
    }
}