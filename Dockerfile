FROM mcr.microsoft.com/dotnet/sdk:6.0 AS builder
WORKDIR /build
COPY .config/dotnet-tools.json .config/
RUN dotnet tool restore
COPY *.csproj ./
RUN dotnet restore
COPY *.cs Migrations ./
RUN dotnet publish -c Release -o out/app
RUN dotnet ef migrations bundle -o out/example-migrate-database

FROM mcr.microsoft.com/dotnet/runtime:6.0-bullseye-slim
COPY --from=builder /build/out/app /usr/local/bin/
COPY --from=builder /build/out/example-migrate-database /usr/local/bin/
COPY star-trek-scraper/data.json /star-trek-scraper/data.json
# TODO USER nobody.
ENTRYPOINT ["Example"]
