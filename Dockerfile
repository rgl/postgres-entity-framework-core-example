FROM mcr.microsoft.com/dotnet/sdk:6.0 AS builder
WORKDIR /build
COPY .config/dotnet-tools.json .config/
RUN dotnet tool restore
COPY *.csproj ./
RUN dotnet restore
COPY *.cs Migrations ./
RUN dotnet build \
        --verbosity normal \
        --configuration Release \
        --no-restore
RUN dotnet publish \
        --verbosity normal \
        --configuration Release \
        --no-build
RUN dotnet ef migrations bundle \
        --verbose \
        --configuration Release \
        --output bin/example-migrate-database \
        --no-build

FROM mcr.microsoft.com/dotnet/runtime:6.0-bullseye-slim
COPY --from=builder /build/bin/Release/net6.0/publish /usr/local/bin/
COPY --from=builder /build/bin/example-migrate-database /usr/local/bin/
COPY star-trek-scraper/data.json /star-trek-scraper/data.json
ENV DOTNET_BUNDLE_EXTRACT_BASE_DIR=/tmp
# NB 65534:65534 is the uid:gid of the nobody:nogroup user:group.
# NB we use a numeric uid:gid to easy the use in kubernetes securityContext.
#    k8s will only be able to infer the runAsUser and runAsGroup values when
#    the USER intruction has a numeric uid:gid. otherwise it will fail with:
#       kubelet Error: container has runAsNonRoot and image has non-numeric
#       user (nobody), cannot verify user is non-root
USER 65534:65534
ENTRYPOINT ["Example"]
