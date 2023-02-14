# OpenTelemetry .NET Automatic Instrumentation Image

OpenTelemetry .NET Automatic Instrumentation source: [opentelemetry-dotnet-instrumentation](https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation)

**Note: This repo is not yet complete!!!!!**

## 簡介

AspNetCore 基底映像檔，內部預先安裝 [OpenTelemetry .NET Automatic Instrumentation](https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation) 並設定好必要的環境參數與所需的自訂 plugin。
使用此 image base 打包的容器預設即支援使用 open telemetry 協定輸出網路路由追蹤資料到外部服務。

## os type memo

雖然 aspnet:7.0-alpine 在 opentelemetry-dotnet-instrumentation 的安裝用 SH 檔判斷下會使用 linux-musl 的版本，但是我實務使用時會出現執行錯誤的問題，但是使用 linux-glibc 版本就正常，所以我自己是統一使用 glibc 版本來發布 image

| OS                     | Type        |
|------------------------|-------------|
| alpine                 | linux-musl  |
| debian (bullseye-slim) | linux-glibc |
| ubuntu (jammy)         | linux-glibc |

## 執行時需要的環境參數

```
CORECLR_ENABLE_PROFILING: '1'
CORECLR_PROFILER: '{918728DD-259F-4A6A-AC2B-B85E1B658318}'
CORECLR_PROFILER_PATH: /otel-dotnet-auto/OpenTelemetry.AutoInstrumentation.Native.so
DOTNET_ADDITIONAL_DEPS: /otel-dotnet-auto/AdditionalDeps
DOTNET_SHARED_STORE: /otel-dotnet-auto/store
DOTNET_STARTUP_HOOKS: /otel-dotnet-auto/net/OpenTelemetry.AutoInstrumentation.StartupHook.dll
OTEL_DOTNET_AUTO_HOME: /otel-dotnet-auto
OTEL_DOTNET_AUTO_INTEGRATIONS_FILE: /otel-dotnet-auto/integrations.json
OTEL_DOTNET_AUTO_METRICS_ADDITIONAL_SOURCES: sample-api
OTEL_DOTNET_AUTO_TRACES_ADDITIONAL_SOURCES: sample-api
OTEL_EXPORTER_OTLP_ENDPOINT: 'http://otel.observability.svc.cluster.local:4317'
OTEL_EXPORTER_OTLP_PROTOCOL: grpc
OTEL_RESOURCE_ATTRIBUTES: "service.version=docker-image-name:imagetag, service.namespace=service-namespace"
OTEL_SERVICE_NAME: sample-api
```

## 參考資料

<a href="https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/blob/main/docs/plugins.md" target="_blank">官方文件</a>

<a href="https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/blob/main/docs/plugins.md" target="_blank">Plugins - GitHub</a>

<a href="https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/blob/main/test/test-applications/integrations/TestApplication.Plugins/Plugin.cs" target="_blank">TestApplication.Plugins/Plugin.cs - GitHub</a>

<a href="https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/blob/main/test/IntegrationTests/PluginsTests.cs" target="_blank">PluginsTests - GitHub</a>

<a href="https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/blob/main/docs/config.md#additional-settings" target="_blank">Config additional settings - GitHub</a>

<a href="https://learn.microsoft.com/en-us/dotnet/api/system.type.assemblyqualifiedname?view=net-6.0#examples" target="_blank">Assembly Qualified Name - Microsoft</a>