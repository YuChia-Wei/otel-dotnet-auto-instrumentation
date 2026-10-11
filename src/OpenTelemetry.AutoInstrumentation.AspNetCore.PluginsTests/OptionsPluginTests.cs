using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins;
using OpenTelemetry.AutoInstrumentation.PluginApi;
using OpenTelemetry.AutoInstrumentation.PluginApi.Telemetry;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Instrumentation.Http;
using OpenTelemetry.Instrumentation.SqlClient;
using OpenTelemetry.Resources;

namespace OpenTelemetry.AutoInstrumentation.AspNetCore.PluginsTests;

public class OptionsPluginTests
{
    [Theory]
    [InlineData("kube-probe/1.34", false)]
    [InlineData("A10HM/1.0", false)]
    [InlineData("Mozilla/5.0", true)]
    [InlineData("", true)]
    public void PluginApi配置保留健康檢查過濾(string userAgent, bool expected)
    {
        var plugin = CreatePlugin();
        var options = new AspNetCoreTraceInstrumentationOptions();
        ((IConfigureTracesOptions<AspNetCoreTraceInstrumentationOptions>)plugin).ConfigureTracesOptions(options);
        var context = new DefaultHttpContext();
        context.Request.Headers[HeaderNames.UserAgent] = userAgent;

        Assert.Equal(expected, options.Filter!(context));
        Assert.NotNull(options.EnrichWithHttpRequest);
        Assert.NotNull(options.EnrichWithHttpResponse);
        Assert.True(options.RecordException);
    }

    [Fact]
    public void PluginApi配置保留用戶端例外記錄()
    {
        var plugin = CreatePlugin();
        var httpOptions = new HttpClientTraceInstrumentationOptions();
        var sqlOptions = new SqlClientTraceInstrumentationOptions();
        ((IConfigureTracesOptions<HttpClientTraceInstrumentationOptions>)plugin).ConfigureTracesOptions(httpOptions);
        ((IConfigureTracesOptions<SqlClientTraceInstrumentationOptions>)plugin).ConfigureTracesOptions(sqlOptions);

        Assert.True(httpOptions.RecordException);
        Assert.True(sqlOptions.RecordException);
    }

    [Fact]
    public void PluginApi配置保留Resource標籤()
    {
        var plugin = CreatePlugin();
        var resource = ((ITelemetryPlugin)plugin).ConfigureResource(ResourceBuilder.CreateEmpty()).Build();
        var attributes = resource.Attributes.ToDictionary(attribute => attribute.Key, attribute => attribute.Value);

        Assert.Equal(Environment.GetEnvironmentVariable("NODE_NAME") ?? Environment.MachineName, attributes["host.name"]);
        Assert.Equal(Environment.MachineName, attributes["container.name"]);
        Assert.Equal(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production", attributes["aspnetcore.environment"]);
    }

    private static IPlugin CreatePlugin()
    {
        // Match the assembly-qualified name configured in both Dockerfiles.
        const string typeName = "OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins.OptionsPlugin, OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
        var type = Type.GetType(typeName, throwOnError: true)!;
        var plugin = Assert.IsAssignableFrom<IPlugin>(Activator.CreateInstance(type));
        plugin.Initializing();
        plugin.Initialized();
        return plugin;
    }
}
