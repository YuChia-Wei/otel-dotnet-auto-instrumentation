ARG dotnetVersion=7.0-alpine

FROM mcr.microsoft.com/dotnet/aspnet:$dotnetVersion AS base
# 如果有 SSL 版本問題可以使用這個，但是需評估是否要將此段隱藏在 base image
#RUN sed -i '1i openssl_conf = default_conf' /etc/ssl/openssl.cnf && echo -e "\n[ default_conf ]\nssl_conf = ssl_sect\n[ssl_sect]\nsystem_default = system_default_sect\n[system_default_sect]\nMinProtocol = TLSv1\nCipherString = DEFAULT:@SECLEVEL=1" >> /etc/ssl/openssl.cnf

#複製 k8s 自動追蹤套件，此套件只是去解壓縮線上已編譯好的版本，所以使用的 .net image 版本不影響結果
#未來應持續關注 otel version，若有新版本應更新新版本
FROM mcr.microsoft.com/dotnet/aspnet:7.0-bullseye-slim AS otel
RUN apt-get update && apt-get install unzip -y
#ARG ARCHIVE=opentelemetry-dotnet-instrumentation-linux-glibc.zip
#ARG OTEL_VERSION=0.6.0
#ADD https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/releases/download/v$OTEL_VERSION/$ARCHIVE otel-dotnet-instrumentation.zip
#從下載回來的備份來解壓縮，要更快的話可能要把成品直接解壓縮放著，或是不要用 zip 改用 linux 裡面原生就有的壓縮格式
COPY OpenTelemetry.dotnet.AutoInstrumentation.Release/linux-musl-0.6.0.zip otel-dotnet-instrumentation.zip
RUN unzip -q otel-dotnet-instrumentation.zip -d /otel-dotnet-auto
#複製必要資料到 0.5.0 版時，此檔案的原始位置，這樣就不用調整舊版部署檔的參數路徑，而新的部署檔要使用新版路徑也可以
RUN cp /otel-dotnet-auto/linux-musl-x64/OpenTelemetry.AutoInstrumentation.Native.so /otel-dotnet-auto/OpenTelemetry.AutoInstrumentation.Native.so

# Build Plugin
# 其實可以統一使用 alpine 版本或是 bullseye-slim 版本跑這一段，但避免未來建置出的 dll 會區分 OS 版本，還是使用對應輸出的 OS 來建置
FROM mcr.microsoft.com/dotnet/sdk:$dotnetVersion AS build
# 如果有 SSL 版本問題可以使用這個
#RUN sed -i 's/openssl_conf = openssl_init/#openssl_conf = openssl_init/' /etc/ssl/openssl.cnf
#RUN sed -i '1i openssl_conf = default_conf' /etc/ssl/openssl.cnf && echo -e "\n[ default_conf ]\nssl_conf = ssl_sect\n[ssl_sect]\nsystem_default = system_default_sect\n[system_default_sect]\nMinProtocol = TLSv1\nCipherString = DEFAULT:@SECLEVEL=1" >> /etc/ssl/openssl.cnf
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