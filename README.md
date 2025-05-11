# OpenTelemetry .NET Automatic Instrumentation Image

OpenTelemetry .NET Automatic Instrumentation
source: [opentelemetry-dotnet-instrumentation](https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation)

:::waring
I'm stop publish the \.net 7 version image, You can fork this repo and build it by you self If you need.
:::

## current version

### dotnet support

- dotnet 6.0
- dotnet 8.0
- dotnet 9.0

### OpenTelemetry dotnet instrumentation version

- [1.11.0](https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/releases/tag/v1.11.0)

### published images

- ghcr.io/yuchia-wei/otel-dotnet-auto-instrumentation:6.0
- ghcr.io/yuchia-wei/otel-dotnet-auto-instrumentation:6.0-alpine
- ghcr.io/yuchia-wei/otel-dotnet-auto-instrumentation:6.0-bookworm-slim
- ghcr.io/yuchia-wei/otel-dotnet-auto-instrumentation:8.0
- ghcr.io/yuchia-wei/otel-dotnet-auto-instrumentation:8.0-alpine
- ghcr.io/yuchia-wei/otel-dotnet-auto-instrumentation:8.0-bookworm-slim
- ghcr.io/yuchia-wei/otel-dotnet-auto-instrumentation:9.0
- ghcr.io/yuchia-wei/otel-dotnet-auto-instrumentation:9.0-alpine
- ghcr.io/yuchia-wei/otel-dotnet-auto-instrumentation:9.0-bookworm-slim

## Introduction

此容器基於微軟官方 mcr.microsoft.com/dotnet/aspnet 容器，預先安裝好 [OpenTelemetry .NET Automatic Instrumentation](https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation) 1.11.0 版套件，並另外製作專用 plugin 後重新打包的版本。

使用 plugin 所需的參數已經設定完畢，其他執行時需要設定的環境參數於下一章節中有簡單整理，但我會建議去 [open-telemetry dotnet instrumentation documentation](https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/blob/v1.1.0/docs/README.md) 查閱文件會更好。

這邊預裝的 plugin 功能如下

1. 自動排除 a10 / kube-probe 等來源的流量追蹤。此功能主要目的是要忽略 kubernetes 與網路設定的 health check 流量，以減少無用的追蹤資料。

---

> This translation is done using ChatGPT. If you have any questions, feel free to contact me.
 
This container is based on the official Microsoft mcr.microsoft.com/dotnet/aspnet container, with the pre-installed [OpenTelemetry .NET Automatic Instrumentation](https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation) version 1.11.0 package. It has been repackaged after incorporating a custom plugin.

The necessary parameters for using the plugin have been pre-configured. Other environment variables that need to be set during runtime will be briefly summarized in the next chapter, but I would recommend referring to the [open-telemetry dotnet instrumentation documentation](https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/blob/v1.11.0/docs/README.md) for better understanding.

The pre-installed plugin provides the following functionalities:

1. Automatic exclusion of outbound HTTP traffic from URLs starting with 'loki' (Since I have already transitioned to using open-telemetry for logging, this feature is no longer needed. I will re-evaluate if the official configuration parameters include similar functionality and remove this setting if applicable.)
2. Automatic exclusion of traffic from sources like 'a10' and 'kube-probe'. This functionality is primarily aimed at ignoring Kubernetes and network configuration health check traffic to reduce unnecessary tracking data.

## 執行時需要的環境參數

- [官方說明文件](https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/blob/v1.11.0/docs/README.md)
- [官方設定參數說明](https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/blob/v1.11.0/docs/config.md)

### Open Telemetry 執行參數

- CORECLR_PROFILER_PATH 有關的環境參數的部分，本專案在建置時有複製一份檔案回到舊版位置，所以部署檔可改可不改
- 目前 OpenTelemetry dotnet instrumentation 並不直接支援 serilog，如果應用服務使用 serilog 作為主要的 Log 輸出工具，在採用此 base image 時會無法正常輸出 log，需要在 UseSerilog 時額外設定 writeToProviders = true。

| Environment variable       | .NET version                          | Value                                                                          |
|----------------------------|---------------------------------------|--------------------------------------------------------------------------------|
| `COR_ENABLE_PROFILING`     | .NET Framework                        | `1`                                                                            |
| `COR_PROFILER`             | .NET Framework                        | `{918728DD-259F-4A6A-AC2B-B85E1B658318}`                                       |
| `COR_PROFILER_PATH_32`     | .NET Framework                        | `/otel-dotnet-auto/win-x86/OpenTelemetry.AutoInstrumentation.Native.dll`       |
| `COR_PROFILER_PATH_64`     | .NET Framework                        | `/otel-dotnet-auto/win-x64/OpenTelemetry.AutoInstrumentation.Native.dll`       |
| `CORECLR_ENABLE_PROFILING` | .NET                                  | `1`                                                                            |
| `CORECLR_PROFILER`         | .NET                                  | `{918728DD-259F-4A6A-AC2B-B85E1B658318}`                                       |
| `CORECLR_PROFILER_PATH`    | .NET All Linux (custom in this linux) | `/otel-dotnet-auto/OpenTelemetry.AutoInstrumentation.Native.so`                |
| `CORECLR_PROFILER_PATH`    | .NET on Linux glibc                   | `/otel-dotnet-auto/linux-x64/OpenTelemetry.AutoInstrumentation.Native.so`      |
| `CORECLR_PROFILER_PATH`    | .NET on Linux musl                    | `/otel-dotnet-auto/linux-musl-x64/OpenTelemetry.AutoInstrumentation.Native.so` |
| `CORECLR_PROFILER_PATH`    | .NET on macOS                         | `/otel-dotnet-auto/osx-x64/OpenTelemetry.AutoInstrumentation.Native.dylib`     |
| `CORECLR_PROFILER_PATH_32` | .NET on Windows                       | `/otel-dotnet-auto/win-x86/OpenTelemetry.AutoInstrumentation.Native.dll`       |
| `CORECLR_PROFILER_PATH_64` | .NET on Windows                       | `/otel-dotnet-auto/win-x64/OpenTelemetry.AutoInstrumentation.Native.dll`       |
| `DOTNET_ADDITIONAL_DEPS`   | .NET                                  | `/otel-dotnet-auto/AdditionalDeps`                                             |
| `DOTNET_SHARED_STORE`      | .NET                                  | `/otel-dotnet-auto/store`                                                      |
| `DOTNET_STARTUP_HOOKS`     | .NET                                  | `/otel-dotnet-auto/net/OpenTelemetry.AutoInstrumentation.StartupHook.dll`      |
| `OTEL_DOTNET_AUTO_HOME`    | All versions                          | `/otel-dotnet-auto`                                                            |

### 其他建議設定參數

| Environment variable                        | Value Sample                                                                                                |
|---------------------------------------------|-------------------------------------------------------------------------------------------------------------|
| OTEL_DOTNET_AUTO_METRICS_ADDITIONAL_SOURCES | sample-api                                                                                                  |
| OTEL_DOTNET_AUTO_TRACES_ADDITIONAL_SOURCES  | sample-api                                                                                                  |
| OTEL_EXPORTER_OTLP_ENDPOINT                 | http://otel.observability.svc.cluster.local:4317                                                            |
| OTEL_EXPORTER_OTLP_PROTOCOL                 | grpc                                                                                                        |
| OTEL_RESOURCE_ATTRIBUTES                    | service.version=docker-image-name:imagetag, service.namespace=service-namespace, deployment.environment=dev |
| OTEL_SERVICE_NAME                           | sample-api                                                                                                  | 

### Resource detectors

copy from [otel config](https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/blob/v1.11.0/docs/config.md)

| Environment variable                             | Description                                                                                                                                                                                           | Default value | Status                                                                                                                            |
|--------------------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|---------------|-----------------------------------------------------------------------------------------------------------------------------------|
| `OTEL_DOTNET_AUTO_RESOURCE_DETECTOR_ENABLED`     | Enables all resource detectors.                                                                                                                                                                       | `true`        | [Experimental](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/versioning-and-stability.md) |
| `OTEL_DOTNET_AUTO_{0}_RESOURCE_DETECTOR_ENABLED` | Configuration pattern for enabling a specific resource detector, where `{0}` is the uppercase id of the resource detector you want to enable. Overrides `OTEL_DOTNET_AUTO_RESOURCE_DETECTOR_ENABLED`. | `true`        | [Experimental](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/versioning-and-stability.md) |

The following resource detectors are included and enabled by default:

| ID                | Description                | Documentation                                                                                                                                                                                                                         | Status                                                                                                                            |
|-------------------|----------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------|
| `AZUREAPPSERVICE` | Azure App Service detector | [Azure resource detector documentation](https://github.com/open-telemetry/opentelemetry-dotnet-contrib/blob/Resources.Azure-1.0.0-beta.9/src/OpenTelemetry.Resources.Azure/README.md)                                                 | [Experimental](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/versioning-and-stability.md) |
| `CONTAINER`       | Container detector         | [Container resource detector documentation](https://github.com/open-telemetry/opentelemetry-dotnet-contrib/blob/Resources.Container-1.0.0-beta.9/src/OpenTelemetry.Resources.Container/README.md) **Not supported on .NET Framework** | [Experimental](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/versioning-and-stability.md) |
| `HOST`            | Host detector              | [Host resource detector documentation](https://github.com/open-telemetry/opentelemetry-dotnet-contrib/blob/Resources.Host-0.1.0-beta.3/src/OpenTelemetry.Resources.Host/README.md)                                                    | [Experimental](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/versioning-and-stability.md) |
| `OPERATINGSYSTEM` | Operating System detector  | [Operating System resource detector documentation](https://github.com/open-telemetry/opentelemetry-dotnet-contrib/blob/Resources.OperatingSystem-0.1.0-alpha.4/src/OpenTelemetry.Resources.OperatingSystem/README.md)                 | [Experimental](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/versioning-and-stability.md) |
| `PROCESS`         | Process detector           | [Process resource detector documentation](https://github.com/open-telemetry/opentelemetry-dotnet-contrib/blob/Resources.Process-0.1.0-beta.3/src/OpenTelemetry.Resources.Process/README.md)                                           | [Experimental](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/versioning-and-stability.md) |
| `PROCESSRUNTIME`  | Process Runtime detector   | [Process Runtime resource detector documentation](https://github.com/open-telemetry/opentelemetry-dotnet-contrib/blob/Resources.ProcessRuntime-0.1.0-beta.2/src/OpenTelemetry.Resources.ProcessRuntime/README.md)                     | [Experimental](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/versioning-and-stability.md) |

### Instrumentation options

copy from [otel config](https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/blob/v1.11.0/docs/config.md)

| Environment variable                                                              | Description                                                                                                                                                                                                                                                                                          | Default value | Status                                                                                                                            |
|-----------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|---------------|-----------------------------------------------------------------------------------------------------------------------------------|
| `OTEL_DOTNET_AUTO_ENTITYFRAMEWORKCORE_SET_DBSTATEMENT_FOR_TEXT`                   | Whether the Entity Framework Core instrumentation can pass SQL statements through the `db.statement` attribute. Queries might contain sensitive information. If set to `false`, `db.statement` is recorded only for executing stored procedures.                                                     | `false`       | [Experimental](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/versioning-and-stability.md) |
| `OTEL_DOTNET_AUTO_GRAPHQL_SET_DOCUMENT`                                           | Whether the GraphQL instrumentation can pass raw queries through the `graphql.document` attribute. Queries might contain sensitive information.                                                                                                                                                      | `false`       | [Experimental](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/versioning-and-stability.md) |
| `OTEL_DOTNET_AUTO_ORACLEMDA_SET_DBSTATEMENT_FOR_TEXT`                             | Whether the Oracle Client instrumentation can pass SQL statements through the `db.statement` attribute. Queries might contain sensitive information. If set to `false`, `db.statement` is recorded only for executing stored procedures.                                                             | `false`       | [Experimental](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/versioning-and-stability.md) |
| `OTEL_DOTNET_AUTO_SQLCLIENT_SET_DBSTATEMENT_FOR_TEXT`                             | Whether the SQL Client instrumentation can pass SQL statements through the `db.statement` attribute. Queries might contain sensitive information. If set to `false`, `db.statement` is recorded only for executing stored procedures. **Not supported on .NET Framework for System.Data.SqlClient.** | `false`       | [Experimental](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/versioning-and-stability.md) |
| `OTEL_DOTNET_AUTO_TRACES_ASPNET_INSTRUMENTATION_CAPTURE_REQUEST_HEADERS`          | A comma-separated list of HTTP header names. ASP.NET instrumentations will capture HTTP request header values for all configured header names.                                                                                                                                                       |               | [Experimental](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/versioning-and-stability.md) |
| `OTEL_DOTNET_AUTO_TRACES_ASPNET_INSTRUMENTATION_CAPTURE_RESPONSE_HEADERS`         | A comma-separated list of HTTP header names. ASP.NET instrumentations will capture HTTP response header values for all configured header names. **Not supported on IIS Classic mode.**                                                                                                               |               | [Experimental](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/versioning-and-stability.md) |
| `OTEL_DOTNET_AUTO_TRACES_ASPNETCORE_INSTRUMENTATION_CAPTURE_REQUEST_HEADERS`      | A comma-separated list of HTTP header names. ASP.NET Core instrumentations will capture HTTP request header values for all configured header names.                                                                                                                                                  |               | [Experimental](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/versioning-and-stability.md) |
| `OTEL_DOTNET_AUTO_TRACES_ASPNETCORE_INSTRUMENTATION_CAPTURE_RESPONSE_HEADERS`     | A comma-separated list of HTTP header names. ASP.NET Core instrumentations will capture HTTP response header values for all configured header names.                                                                                                                                                 |               | [Experimental](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/versioning-and-stability.md) |
| `OTEL_DOTNET_AUTO_TRACES_GRPCNETCLIENT_INSTRUMENTATION_CAPTURE_REQUEST_METADATA`  | A comma-separated list of gRPC metadata names. Grpc.Net.Client instrumentations will capture gRPC request metadata values for all configured metadata names.                                                                                                                                         |               | [Experimental](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/versioning-and-stability.md) |
| `OTEL_DOTNET_AUTO_TRACES_GRPCNETCLIENT_INSTRUMENTATION_CAPTURE_RESPONSE_METADATA` | A comma-separated list of gRPC metadata names. Grpc.Net.Client instrumentations will capture gRPC response metadata values for all configured metadata names.                                                                                                                                        |               | [Experimental](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/versioning-and-stability.md) |
| `OTEL_DOTNET_AUTO_TRACES_HTTP_INSTRUMENTATION_CAPTURE_REQUEST_HEADERS`            | A comma-separated list of HTTP header names. HTTP Client instrumentations will capture HTTP request header values for all configured header names.                                                                                                                                                   |               | [Experimental](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/versioning-and-stability.md) |
| `OTEL_DOTNET_AUTO_TRACES_HTTP_INSTRUMENTATION_CAPTURE_RESPONSE_HEADERS`           | A comma-separated list of HTTP header names. HTTP Client instrumentations will capture HTTP response header values for all configured header names.                                                                                                                                                  |               | [Experimental](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/versioning-and-stability.md) |
| `OTEL_DOTNET_EXPERIMENTAL_ASPNETCORE_DISABLE_URL_QUERY_REDACTION`                 | Whether the ASP.NET Core instrumentation turns off redaction of the `url.query` attribute value.                                                                                                                                                                                                     | `false`       | [Experimental](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/versioning-and-stability.md) |
| `OTEL_DOTNET_EXPERIMENTAL_HTTPCLIENT_DISABLE_URL_QUERY_REDACTION`                 | Whether the HTTP client instrumentation turns off redaction of the `url.full` attribute value.                                                                                                                                                                                                       | `false`       | [Experimental](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/versioning-and-stability.md) |
| `OTEL_DOTNET_EXPERIMENTAL_ASPNET_DISABLE_URL_QUERY_REDACTION`                     | Whether the ASP.NET instrumentation turns off redaction of the `url.query` attribute value.                                                                                                                                                                                                          | `false`       | [Experimental](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/versioning-and-stability.md) |

## 更新命令筆記

**建議在 wsl 內執行以下命令，因為 linux 的命令操作比較方便**
**不過可能會需要額外安裝 unzip，因為 wsl 的 ubuntu 可能原生未包含此套件**

```shell
cd src/otel.dotnet.AutoInstrumentation.Release

wget https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/releases/download/v1.11.0/opentelemetry-dotnet-instrumentation-linux-glibc-x64.zip
unzip opentelemetry-dotnet-instrumentation-linux-glibc-x64.zip -d opentelemetry-dotnet-instrumentation-linux-glibc
tar -czvf opentelemetry-dotnet-instrumentation-linux-glibc.tar.gz opentelemetry-dotnet-instrumentation-linux-glibc/

wget https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/releases/download/v1.11.0/opentelemetry-dotnet-instrumentation-linux-musl-x64.zip
unzip opentelemetry-dotnet-instrumentation-linux-musl-x64.zip -d opentelemetry-dotnet-instrumentation-linux-musl
tar -czvf opentelemetry-dotnet-instrumentation-linux-musl.tar.gz opentelemetry-dotnet-instrumentation-linux-musl/
```

## OpenTelemetry dotnet instrumentation plugin memo

OpenTelemetry .NET Automatic Instrumentation 有提供掛載外掛，可以修改設定或是覆寫 options。

實作前請先閱讀官方文件：<a href="https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/blob/v1.11.0/docs/plugins.md" target="_blank">Plugins - GitHub</a>

### 開發須知

1. 必須是非靜態、非抽象的類別
2. 必須要有 `public void initializing()` 方法
3. Plugin 內參考的 nuget 套件版本必須和 OpenTelemetry .NET Automatic Instrumentation 用的一樣
4. plugin 開發時需要考慮使用的 OpenTelemetry .NET Automatic Instrumentation 版本，避免 breaking change
5. 替套件寫單元測試

### 相依版本

copy from [opentelemetry-dotnet-instrumentation 1.11.0 plugins doc](https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/blob/v1.11.0/docs/plugins.md)

## Supported Options

### Tracing

| Options type                                                                              | NuGet package                                     | NuGet version |
|-------------------------------------------------------------------------------------------|---------------------------------------------------|---------------|
| OpenTelemetry.Exporter.ConsoleExporterOptions                                             | OpenTelemetry.Exporter.Console                    | 1.11.2        |
| OpenTelemetry.Exporter.ZipkinExporterOptions                                              | OpenTelemetry.Exporter.Zipkin                     | 1.11.2        |
| OpenTelemetry.Exporter.OtlpExporterOptions                                                | OpenTelemetry.Exporter.OpenTelemetryProtocol      | 1.11.2        |
| OpenTelemetry.Instrumentation.AspNet.AspNetTraceInstrumentationOptions                    | OpenTelemetry.Instrumentation.AspNet              | 1.11.0-beta.2 |
| OpenTelemetry.Instrumentation.AspNetCore.AspNetCoreTraceInstrumentationOptions            | OpenTelemetry.Instrumentation.AspNetCore          | 1.11.1        |
| OpenTelemetry.Instrumentation.EntityFrameworkCore.EntityFrameworkInstrumentationOptions   | OpenTelemetry.Instrumentation.EntityFrameworkCore | 1.11.0-beta.2 |
| OpenTelemetry.Instrumentation.GrpcNetClient.GrpcClientTraceInstrumentationOptions         | OpenTelemetry.Instrumentation.GrpcNetClient       | 1.11.0-beta.2 |
| OpenTelemetry.Instrumentation.Http.HttpClientTraceInstrumentationOptions                  | OpenTelemetry.Instrumentation.Http                | 1.11.1        |
| OpenTelemetry.Instrumentation.Quartz.QuartzInstrumentationOptions                         | OpenTelemetry.Instrumentation.Quartz              | 1.11.0-beta.2 |
| OpenTelemetry.Instrumentation.SqlClient.SqlClientTraceInstrumentationOptions              | OpenTelemetry.Instrumentation.SqlClient           | 1.11.0-beta.2 |
| OpenTelemetry.Instrumentation.StackExchangeRedis.StackExchangeRedisInstrumentationOptions | OpenTelemetry.Instrumentation.StackExchangeRedis  | 1.11.0-beta.2 |
| OpenTelemetry.Instrumentation.Wcf.WcfInstrumentationOptions                               | OpenTelemetry.Instrumentation.Wcf                 | 1.11.0-beta.2 |

### Metrics

| Options type                                                             | NuGet package                                  | NuGet version |
|--------------------------------------------------------------------------|------------------------------------------------|---------------|
| OpenTelemetry.Metrics.MetricReaderOptions                                | OpenTelemetry                                  | 1.11.2        |
| OpenTelemetry.Exporter.ConsoleExporterOptions                            | OpenTelemetry.Exporter.Console                 | 1.11.2        |
| OpenTelemetry.Exporter.PrometheusExporterOptions                         | OpenTelemetry.Exporter.Prometheus.HttpListener | 1.11.0-beta.2 |
| OpenTelemetry.Exporter.OtlpExporterOptions                               | OpenTelemetry.Exporter.OpenTelemetryProtocol   | 1.11.2        |
| OpenTelemetry.Instrumentation.AspNet.AspNetMetricsInstrumentationOptions | OpenTelemetry.Instrumentation.AspNet           | 1.11.0-beta.2 |
| OpenTelemetry.Instrumentation.Runtime.RuntimeInstrumentationOptions      | OpenTelemetry.Instrumentation.Runtime          | 1.11.1        |

### Logs

| Options type                                  | NuGet package                                | NuGet version |
|-----------------------------------------------|----------------------------------------------|---------------|
| OpenTelemetry.Logs.OpenTelemetryLoggerOptions | OpenTelemetry                                | 1.11.2        |
| OpenTelemetry.Exporter.ConsoleExporterOptions | OpenTelemetry.Exporter.Console               | 1.11.2        |
| OpenTelemetry.Exporter.OtlpExporterOptions    | OpenTelemetry.Exporter.OpenTelemetryProtocol | 1.11.2        |

## 參考資料

<a href="https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/blob/main/docs/plugins.md" target="_blank">官方文件</a>

<a href="https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/blob/main/docs/plugins.md" target="_blank">Plugins - GitHub</a>

<a href="https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/blob/main/test/test-applications/integrations/TestApplication.Plugins/Plugin.cs" target="_blank">TestApplication.Plugins/Plugin.cs - GitHub</a>

<a href="https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/blob/main/test/IntegrationTests/PluginsTests.cs" target="_blank">PluginsTests - GitHub</a>

<a href="https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/blob/main/docs/config.md#additional-settings" target="_blank">Config additional settings - GitHub</a>

<a href="https://learn.microsoft.com/en-us/dotnet/api/system.type.assemblyqualifiedname?view=net-6.0#examples" target="_blank">Assembly Qualified Name - Microsoft</a>
