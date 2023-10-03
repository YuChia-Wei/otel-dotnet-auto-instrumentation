using Microsoft.AspNetCore.Http;
using OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins;

namespace OpenTelemetry.AutoInstrumentation.AspNetCore.PluginsTests;

[TestClass]
public class HttpClientRequestHostCheckerTests
{
    [TestMethod]
    [Owner("YuChia")]
    [TestCategory(nameof(HttpClientRequestHostChecker))]
    [TestProperty(nameof(HttpClientRequestHostChecker), nameof(HttpClientRequestHostChecker.IsValidHost))]
    [DataRow("http://other-service.other-namespace.svc.cluster.local/logo.png", true)]
    public void 檢查是否為需要啟動追蹤的路徑(string host, bool expected)
    {
        var uri = new Uri(host);
        var actual = HttpClientRequestHostChecker.IsValidHost(uri.Host);
        Assert.AreEqual(expected, actual);
    }
}