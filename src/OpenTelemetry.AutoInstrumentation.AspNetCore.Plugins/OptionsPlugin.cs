using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Instrumentation.Http;
using OpenTelemetry.Instrumentation.SqlClient;
using OpenTelemetry.Resources;

namespace OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins;

public class OptionsPlugin
{
    /// <summary>
    /// 自訂資料標籤，補上一些可能需要的資料
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public ResourceBuilder ConfigureResource(ResourceBuilder builder)
    {
        // My custom logic here
        // Please note this method is common to set the resource for trace, logs and metrics.
        // This method could be overridden by ConfigureTracesOptions, ConfigureMeterProvider and ConfigureLogsOptions
        // by calling SetResourceBuilder with new object.

        builder.AddAttributes(new KeyValuePair<string, object>[]
        {
            new("container.name", Environment.MachineName)
        });

        return builder;
    }

    /// <summary>
    /// AspNetCore 追蹤 (外往內的要求)
    /// </summary>
    /// <param name="options"></param>
    public void ConfigureTracesOptions(AspNetCoreInstrumentationOptions options)
    {
        options.RecordException = true;
        options.Filter = context => HttpRequestUserAgentChecker.IsValidUser(context.Request.Headers.UserAgent);
    }

    /// <summary>
    /// HttpClient 追蹤 (內往外的要求)
    /// </summary>
    /// <param name="options"></param>
    public void ConfigureTracesOptions(HttpClientInstrumentationOptions options)
    {
        options.RecordException = true;

        // for .net / .net core
        // options.FilterHttpRequestMessage = httpRequestMessage => HttpClientRequestHostChecker.IsValidHost(httpRequestMessage.RequestUri?.Host);

        // for .net framework
        // options.FilterHttpWebRequest = request => true;
    }

    /// <summary>
    /// should be rename to ConfigureTracesOptions when OpenTelemetry.DotNet.Instrumentation upgrade to ^0.5.1
    /// </summary>
    /// <param name="options"></param>
    public void ConfigureTracesOptions(SqlClientInstrumentationOptions options)
    {
        options.RecordException = true;

        // 這邊建議視情況開啟，可以用環境參數去控制開關
        options.SetDbStatementForText = true;
    }

    /// <summary>
    /// To configure plugin, before OTel SDK configuration is called.
    /// </summary>
    public void Initializing()
    {
    }
}