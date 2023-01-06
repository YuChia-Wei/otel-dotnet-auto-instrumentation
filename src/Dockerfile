ARG dotnetImageTag=7.0-bullseye-slim

FROM mcr.microsoft.com/dotnet/aspnet:$dotnetImageTag AS base

#複製 k8s 自動追蹤套件，此套件只是去解壓縮線上已編譯好的版本，所以使用的 .net image 版本不影響結果
#未來應持續關注 otel version，若有新版本應更新新版本
FROM mcr.microsoft.com/dotnet/aspnet:7.0-bullseye-slim AS otel
RUN apt-get update && apt-get install unzip -y
ARG ARCHIVE=opentelemetry-dotnet-instrumentation-linux-glibc.zip
ARG OTEL_VERSION=0.5.0
ADD https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/releases/download/v$OTEL_VERSION/$ARCHIVE otel-dotnet-instrumentation.zip
RUN unzip -q otel-dotnet-instrumentation.zip -d /otel-dotnet-auto

# Build Plugin
# Plugin 專案使用 .net 7 撰寫
FROM mcr.microsoft.com/dotnet/sdk:7.0-bullseye-slim AS build
WORKDIR /src
COPY ["OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins/OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins.csproj", "OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins/"]
RUN dotnet restore "OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins/OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins.csproj"
COPY . .
WORKDIR "/src/OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins"
RUN dotnet build "OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins.csproj" -c Release -o /app/publish

FROM base AS final
# 環境參數設定說明 https://stackoverflow.com/a/33379487
ENV OTEL_DOTNET_AUTO_PLUGINS="OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins.OptionsPlugin, OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
COPY --from=otel /otel-dotnet-auto /otel-dotnet-auto
COPY --from=publish /app/publish /otel-dotnet-auto/net