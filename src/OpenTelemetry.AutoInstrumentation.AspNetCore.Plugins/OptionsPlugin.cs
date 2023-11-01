using Microsoft.Extensions.Hosting;
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

        //k8s 部署檔案會將 NODE NAME 設定到 NODE_NAME 環境參數
        var nodeName = Environment.GetEnvironmentVariable("NODE_NAME");

        //取得服務執行時設定的環境參數，理論上會存在，除非服務自己把它刪掉
        var aspNetCoreEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environments.Production;

        builder.AddAttributes(new KeyValuePair<string, object>[]
        {
            new("host.name", nodeName ?? Environment.MachineName),
            new("container.name", Environment.MachineName),
            new("aspnetcore.environment", aspNetCoreEnv)
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

        // via. https://github.com/open-telemetry/opentelemetry-dotnet/tree/main/src/OpenTelemetry.Instrumentation.AspNetCore#enrich
        options.EnrichWithHttpRequest = ActivitySourceExtenstion.EnrichHttpRequest();

        //如果在服務中有使用 UseExceptionHandling() 或是在 ActionFilter 的時候把 Exception 處理過的話，可能會導致 otel 無法正確輸出例外資料
        //所以這邊另外檢查回應內容並另外打包給 AspNetCoreInstrumentation 工具
        //TODO: 需要多測試效果
        options.EnrichWithHttpResponse = ActivitySourceExtenstion.EnrichHttpResponse();
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