using OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins;

namespace OpenTelemetry.AutoInstrumentation.AspNetCore.PluginsTests;

[TestClass]
public class HttpRequestUserAgentCheckerTests
{
    [TestMethod]
    [Owner("YuChia")]
    [TestCategory(nameof(HttpRequestUserAgentChecker))]
    [TestProperty(nameof(HttpRequestUserAgentChecker), nameof(HttpRequestUserAgentChecker.IsValidUser))]
    [DataRow("", true)]
    [DataRow("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/107.0.0.0 Safari/537.36", true)]
    [DataRow("a10hm/1.0 Mozilla/5.0 ", false)]
    [DataRow("A10HM/1.0 Mozilla/5.0 ", false)]
    [DataRow("kube-probe/1.24 Mozilla/5.0 ", false)]
    [DataRow("kube-probe/1.25 Mozilla/5.0 ", false)]
    [DataRow("kube-probe/1.26 Mozilla/5.0 ", false)]
    [DataRow("KUBE-PROBE/1.26 Mozilla/5.0 ", false)]
    [DataRow(null, true)]
    [DataRow("a10hm/1.0", false)]
    [DataRow("kube-probe/1.24", false)]
    public void 檢查輸入的User_Agent是否為來源(string? input, bool expected)
    {
        var actual = HttpRequestUserAgentChecker.IsValidUser(input);

        Assert.AreEqual(expected, actual);
    }
}