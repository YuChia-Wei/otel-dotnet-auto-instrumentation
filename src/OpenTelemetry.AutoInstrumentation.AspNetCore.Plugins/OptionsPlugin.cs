using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Instrumentation.Http;
using OpenTelemetry.Instrumentation.SqlClient;

namespace OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins;

public class OptionsPlugin
{
    /// <summary>
    /// should be rename to ConfigureTracesOptions when OpenTelemetry.DotNet.Instrumentation upgrade to ^0.5.1
    /// </summary>
    /// <param name="options"></param>
    public void ConfigureOptions(AspNetCoreInstrumentationOptions options)
    {
        options.RecordException = true;

        options.Filter = context => HttpRequestUserAgentChecker.IsValidUser(context.Request.Headers.UserAgent);
    }

    /// <summary>
    /// should be rename to ConfigureTracesOptions when OpenTelemetry.DotNet.Instrumentation upgrade to ^0.5.1
    /// </summary>
    /// <param name="options"></param>
    public void ConfigureOptions(HttpClientInstrumentationOptions options)
    {
        options.RecordException = true;
    }

    /// <summary>
    /// should be rename to ConfigureTracesOptions when OpenTelemetry.DotNet.Instrumentation upgrade to ^0.5.1
    /// </summary>
    /// <param name="options"></param>
    public void ConfigureOptions(SqlClientInstrumentationOptions options)
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