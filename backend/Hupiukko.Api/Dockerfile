# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ./*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Install locales for fi_FI and en_US
RUN apt-get update \
    && apt-get install -y locales \
    && locale-gen fi_FI.UTF-8 en_US.UTF-8 \
    && update-locale

EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
# Disable the invariant mode (set in base image)
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

ENTRYPOINT ["dotnet", "Hupiukko.Api.dll"] 