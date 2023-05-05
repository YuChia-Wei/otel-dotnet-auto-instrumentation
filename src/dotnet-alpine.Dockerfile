# 目前 Plugin 專案使用 .net 7 撰寫，所以這邊僅支援 .net 7，如果要打包 .net 6 的話需要另外調整
ARG dotnetVersion=7.0-alpine

FROM mcr.microsoft.com/dotnet/aspnet:$dotnetVersion AS base
# 如果有 SSL 版本問題可以使用這個，但是需評估是否要將此段隱藏在 base image
#RUN sed -i '1i openssl_conf = default_conf' /etc/ssl/openssl.cnf && echo -e "\n[ default_conf ]\nssl_conf = ssl_sect\n[ssl_sect]\nsystem_default = system_default_sect\n[system_default_sect]\nMinProtocol = TLSv1\nCipherString = DEFAULT:@SECLEVEL=1" >> /etc/ssl/openssl.cnf

# 處理 open telemetry dotnet auto instrumentation 套件
# 這邊將官方發布的 zip 檔案重新打包成 tar.gz 檔案，可避免在 docker build 時還要等待 unzip 工具的安裝
# 未來應持續關注 otel version，若有新版本應更新新版本
FROM base AS otel
COPY OpenTelemetry.dotnet.AutoInstrumentation.Release/linux-musl-0.7.0.tar.gz otel-dotnet-instrumentation.tar.gz
RUN tar -xzvf otel-dotnet-instrumentation.tar.gz && mv linux-musl-0.7.0 otel-dotnet-auto
#複製必要資料到 0.5.0 版時，此檔案的原始位置，這樣就不用調整舊版部署檔的參數路徑，而新的部署檔要使用新版路徑也可以
RUN cp /otel-dotnet-auto/linux-musl-x64/OpenTelemetry.AutoInstrumentation.Native.so /otel-dotnet-auto/OpenTelemetry.AutoInstrumentation.Native.so

# Build Plugin
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

FROM otel AS final
ENV OTEL_DOTNET_AUTO_PLUGINS="OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins.OptionsPlugin, OpenTelemetry.AutoInstrumentation.AspNetCore.Plugins, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
COPY --from=publish /app/publish /otel-dotnet-auto/net