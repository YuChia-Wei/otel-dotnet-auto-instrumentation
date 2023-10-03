# OpenTelemetry .NET Automatic Instrumentation Image

OpenTelemetry .NET Automatic Instrumentation
source: [opentelemetry-dotnet-instrumentation](https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation)

This repository has currently published the following container images, which can be used if needed:
- ghcr.io/yuchia-wei/otel-dotnet-auto-instrumentation:6.0-alpine
- ghcr.io/yuchia-wei/otel-dotnet-auto-instrumentation:7.0-alpine
- ghcr.io/yuchia-wei/otel-dotnet-auto-instrumentation:6.0-bullseye-slim
- ghcr.io/yuchia-wei/otel-dotnet-auto-instrumentation:7.0-bullseye-slim

## Introduction

此容器基於微軟官方 mcr.microsoft.com/dotnet/aspnet 容器，預先安裝好 [OpenTelemetry .NET Automatic Instrumentation](https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation) 1.0.2 版套件，並另外製作專用 plugin 後重新打包的版本。
使用 plugin 所需的參數已經設定完畢，其他執行時需要設定的環境參數於下一章節中有簡單整理，但我會建議去 [open-telemetry dotnet instrumentation documentation](https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/blob/v0.7.0/docs/README.md) 查閱文件會更好。
這邊預裝的 plugin 功能為

1. 自動排除 a10 / kube-probe 等來源的流量追蹤。此功能主要目的是要忽略 kubernetes 與網路設定的 health check 流量，以減少無用的追蹤資料。

---

> This translation is done using ChatGPT. If you have any questions, feel free to contact me.
 
This container is based on the official Microsoft mcr.microsoft.com/dotnet/aspnet container, with the pre-installed [OpenTelemetry .NET Automatic Instrumentation](https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation) version 0.7.0 package. It has been repackaged after incorporating a custom plugin.

The necessary parameters for using the plugin have been pre-configured. Other environment variables that need to be set during runtime will be briefly summarized in the next chapter, but I would recommend referring to the [open-telemetry dotnet instrumentation documentation](https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/blob/v0.7.0/docs/README.md) for better understanding.

The pre-installed plugin provides the following functionalities:

1. Automatic exclusion of outbound HTTP traffic from URLs starting with 'loki' (Since I have already transitioned to using open-telemetry for logging, this feature is no longer needed. I will re-evaluate if the official configuration parameters include similar functionality and remove this setting if applicable.)
2. Automatic exclusion of traffic from sources like 'a10' and 'kube-probe'. This functionality is primarily aimed at ignoring Kubernetes and network configuration health check traffic to reduce unnecessary tracking data.


## 執行時需要的環境參數

- [官方說明文件](https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/blob/v0.7.0/docs/README.md)
- [官方設定參數說明](https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/blob/v0.7.0/docs/config.md)

### Open Telemetry 執行參數

- CORECLR_PROFILER_PATH 有關的環境參數的部分，本專案在建置時有複製一份檔案回到舊版位置，所以部署檔可改可不改
- --目前版本在 .net 7 有 Log 重複送出的問題，所以建議 `CORECLR_ENABLE_PROFILING` 參數設定為 0 來解決-- 官方說明在 1.0.2 版已修正
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

| Environment variable                        | Value Sample                                                                                                 |
|---------------------------------------------|--------------------------------------------------------------------------------------------------------------|
| OTEL_DOTNET_AUTO_METRICS_ADDITIONAL_SOURCES | sample-api                                                                                                   |
| OTEL_DOTNET_AUTO_TRACES_ADDITIONAL_SOURCES  | sample-api                                                                                                   |
| OTEL_EXPORTER_OTLP_ENDPOINT                 | http://otel.observability.svc.cluster.local:4317                                                             |
| OTEL_EXPORTER_OTLP_PROTOCOL                 | grpc                                                                                                         |
| OTEL_RESOURCE_ATTRIBUTES                    | service.version=docker-image-name:imagetag, service.namespace=service-namespace, deployment.environment=dev  |
| OTEL_SERVICE_NAME                           | sample-api                                                                                                   | 

## 參考資料

<a href="https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/blob/main/docs/plugins.md" target="_blank">
官方文件</a>

<a href="https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/blob/main/docs/plugins.md" target="_blank">
Plugins - GitHub</a>

<a href="https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/blob/main/test/test-applications/integrations/TestApplication.Plugins/Plugin.cs" target="_blank">
TestApplication.Plugins/Plugin.cs - GitHub</a>

<a href="https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/blob/main/test/IntegrationTests/PluginsTests.cs" target="_blank">
PluginsTests - GitHub</a>

<a href="https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/blob/main/docs/config.md#additional-settings" target="_blank">
Config additional settings - GitHub</a>

<a href="https://learn.microsoft.com/en-us/dotnet/api/system.type.assemblyqualifiedname?view=net-6.0#examples" target="_blank">
Assembly Qualified Name - Microsoft</a>