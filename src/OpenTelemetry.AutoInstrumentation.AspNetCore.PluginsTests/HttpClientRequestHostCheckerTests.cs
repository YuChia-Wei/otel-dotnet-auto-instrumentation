using OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins;
using Xunit;

namespace OpenTelemetry.AutoInstrumentation.AspNetCore.PluginsTests;

public class HttpClientRequestHostCheckerTests
{
    [Theory]
    [InlineData("http://other-service.other-namespace.svc.cluster.local/logo.png", true)]
    public void 檢查是否為需要啟動追蹤的路徑(string host, bool expected)
    {
        var uri = new Uri(host);
        var actual = HttpClientRequestHostChecker.IsValidHost(uri.Host);
        Assert.Equal(expected, actual);
    }
}