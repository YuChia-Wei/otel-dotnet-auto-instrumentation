using System.Text.RegularExpressions;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Instrumentation.Http;
using OpenTelemetry.Instrumentation.SqlClient;

namespace OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins;

public partial class OptionsPlugin
{
    /// <summary>
    /// should be rename to ConfigureTracesOptions when OpenTelemetry.DotNet.Instrumentation upgrade to ^0.5.1
    /// </summary>
    /// <param name="options"></param>
    public void ConfigureOptions(AspNetCoreInstrumentationOptions options)
    {
        options.RecordException = true;

        options.Filter = context => !HealthCheckUserAgentRegex().IsMatch(context.Request.Headers.UserAgent);
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

    [GeneratedRegex("^a10hm/\\d+.\\d+|^kube-probe/\\d+.\\d+")]
    private static partial Regex HealthCheckUserAgentRegex();
}