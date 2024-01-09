# syntax=docker/dockerfile:1
# Create a stage for building the application.
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
ARG TARGETARCH
WORKDIR /source

COPY ./CounterClient /source/CounterClient
COPY ./BrokerLib /source/BrokerLib

WORKDIR /source/CounterClient

# Build the application.
# Leverage a cache mount to /root/.nuget/packages so that subsequent builds don't have to re-download packages.
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish --runtime linux-musl-x64 --self-contained false -o /app

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS final
WORKDIR /app

# Copy everything needed to run the app from the "build" stage.
COPY --from=build /app .

ARG UID=10001
RUN adduser \
    --disabled-password \
    --gecos "" \
    --home "/nonexistent" \
    --shell "/sbin/nologin" \
    --no-create-home \
    --uid "${UID}" \
    appuser
USER appuser

ENTRYPOINT ["dotnet", "CounterClient.dll"]
