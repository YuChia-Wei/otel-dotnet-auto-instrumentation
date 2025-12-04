ARG dotnetVersion=10.0
ARG BaseImageTag=${dotnetVersion}

# runtime base image
FROM mcr.microsoft.com/dotnet/aspnet:${BaseImageTag} AS base

# Build Plugin
# FROM mcr.microsoft.com/dotnet/sdk:${dotnetVersion} AS build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG dotnetVersion=10.0
WORKDIR /src
COPY ["OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins/OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins.csproj", "OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins/"]
RUN dotnet restore "OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins/OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins.csproj"
COPY . .
WORKDIR "/src/OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins"
RUN dotnet build "OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins.csproj" -c Release -o /app/build -f net${dotnetVersion}

FROM build AS publish
ARG dotnetVersion=10.0
RUN dotnet publish "OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins.csproj" -c Release -o /app/publish -f net${dotnetVersion}

# 使用功能比較完善的建置容器來處理 open telemetry dotnet auto instrumentation 套件
# 這邊將官方發布的 zip 檔案重新打包成 tar.gz 檔案，可避免在 docker build 時還要等待 unzip 工具的安裝
# 未來應持續關注 otel version，若有新版本應更新新版本
FROM build AS otel
COPY otel.dotnet.AutoInstrumentation.Release/opentelemetry-dotnet-instrumentation-linux-glibc.tar.gz otel-dotnet-instrumentation.tar.gz
RUN tar -xzvf otel-dotnet-instrumentation.tar.gz && mv opentelemetry-dotnet-instrumentation-linux-glibc /otel-dotnet-auto

FROM base AS final
ENV OTEL_DOTNET_AUTO_PLUGINS="OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins.OptionsPlugin, OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
COPY --from=otel /otel-dotnet-auto /otel-dotnet-auto
COPY --from=publish /app/publish /otel-dotnet-auto/net