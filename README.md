# OpenTelemetry .NET Automatic Instrumentation Image

OpenTelemetry .NET Automatic Instrumentation
source: [opentelemetry-dotnet-instrumentation](https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation)

**Note: This repo is not yet complete!!!!!**

## 簡介

AspNetCore
基底映像檔，內部預先安裝 [OpenTelemetry .NET Automatic Instrumentation](https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation)
並設定好必要的環境參數與所需的自訂 plugin。
使用此 image base 打包的容器預設即支援使用 open telemetry 協定輸出網路路由追蹤資料到外部服務。

## 執行時需要的環境參數

- [官方說明文件](https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/blob/v0.7.0/docs/README.md)
- [官方設定參數說明](https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/blob/v0.7.0/docs/config.md)

### Open Telemetry 執行參數

- CORECLR_PROFILER_PATH 有關的環境參數的部分，本專案在建置時有複製一份檔案回到舊版位置，所以部署檔可改可不改
- 目前版本在 .net 7 有 Log 重複送出的問題，所以建議 `CORECLR_ENABLE_PROFILING` 參數設定為 0 來解決
- 目前 opentelemetry dotnet instrumentation 並不直接支援 serilog，如果應用服務使用 serilog 作為主要的 Log 輸出工具，在採用此 base image 時會無法正常輸出 log，需要在 UseSerilog 時額外設定 writeToProviders = true。
  - **注意！這仍會被 .net7 重複 log 的問題影響，而且關掉 CLR Profiler 也無法解決！**
  - 暫時性的最佳解是放棄 Serilog 改回原生的 Log 工具

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