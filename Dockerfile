FROM mcr.microsoft.com/dotnet/sdk:6.0 AS builder
WORKDIR /build
COPY .config/dotnet-tools.json .config/
RUN dotnet tool restore
COPY *.csproj ./
RUN dotnet restore
COPY *.cs Migrations ./
RUN dotnet build \
        --configuration Release \
        --no-restore
RUN dotnet publish \
        --configuration Release \
        --no-build
RUN dotnet ef migrations bundle \
        --configuration Release \
        --output bin/example-migrate-database \
        --no-build

FROM mcr.microsoft.com/dotnet/runtime:6.0-bullseye-slim
COPY --from=builder /build/bin/Release/net6.0/publish /usr/local/bin/
COPY --from=builder /build/bin/example-migrate-database /usr/local/bin/
COPY star-trek-scraper/data.json /star-trek-scraper/data.json
# TODO USER nobody.
ENTRYPOINT ["Example"]
