using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Instrumentation.Http;
using OpenTelemetry.Instrumentation.SqlClient;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins;

public class OptionsPlugin
{
    // To configure metrics SDK after Auto Instrumentation configured SDK
    public MeterProviderBuilder AfterConfigureMeterProvider(MeterProviderBuilder builder)
    {
        //dotnet 8 metrics
        // 在 open telemetry auto instrumentation 1.2.0 中，不用手動注入，已經被包含在以下位置
        // https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/blob/v1.2.0/src/OpenTelemetry.AutoInstrumentation/Configurations/EnvironmentConfigurationMetricHelper.cs#L97C56-L97C56
        // builder.AddMeter("Microsoft.AspNetCore.Hosting",
        //                  "Microsoft.AspNetCore.Server.Kestrel");
        return builder;
    }

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
    public void ConfigureTracesOptions(AspNetCoreTraceInstrumentationOptions options)
    {
        options.RecordException = true;
        options.Filter = context => HttpRequestUserAgentChecker.IsValidUser(context.Request.Headers[HeaderNames.UserAgent]);

        // via. https://github.com/open-telemetry/opentelemetry-dotnet/tree/main/src/OpenTelemetry.Instrumentation.AspNetCore#enrich
        options.EnrichWithHttpRequest = ActivitySourceExtenstion.EnrichHttpRequest();

        //如果在服務中有使用 UseExceptionHandling() 或是在 ActionFilter 的時候把 Exception 處理過的話，可能會導致 otel 無法正確輸出例外資料
        //所以這邊另外檢查回應內容並另外打包給 AspNetCoreInstrumentation 工具
        options.EnrichWithHttpResponse = ActivitySourceExtenstion.EnrichHttpResponse();
    }

    /// <summary>
    /// HttpClient 追蹤 (內往外的要求)
    /// </summary>
    /// <param name="options"></param>
    public void ConfigureTracesOptions(HttpClientTraceInstrumentationOptions options)
    {
        options.RecordException = true;

        // for .net / .net core
        // Request 過濾器
        // options.FilterHttpRequestMessage = httpRequestMessage => HttpClientRequestHostChecker.IsValidHost(httpRequestMessage.RequestUri?.Host);

        // for .net framework
        // options.FilterHttpWebRequest = request => true;
    }

    /// <summary>
    /// should be rename to ConfigureTracesOptions when OpenTelemetry.DotNet.Instrumentation upgrade to ^0.5.1
    /// </summary>
    /// <param name="options"></param>
    public void ConfigureTracesOptions(SqlClientTraceInstrumentationOptions options)
    {
        options.RecordException = true;
    }

    /// <summary>
    /// To configure plugin, before OTel SDK configuration is called.
    /// </summary>
    public void Initializing()
    {
    }
}