using Microsoft.AspNetCore.Http;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Instrumentation.Http;
using OpenTelemetry.Instrumentation.SqlClient;

namespace OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins;

public class OptionsPlugin
{
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
        options.FilterHttpRequestMessage = httpRequestMessage => HttpClientRequestHostChecker.IsValidHost(httpRequestMessage.RequestUri?.Host);

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
        options.SetDbStatementForText = true;
    }

    // To configure plugin, before OTel SDK configuration is called.
    public void Initializing()
    {
        // My custom logic here
    }
}