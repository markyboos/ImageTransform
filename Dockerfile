FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /webapp

# Copy csproj and restore as distinct layers
COPY /ImageFetcher/*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY ./ImageFetcher/ ./
RUN dotnet publish -c release -o output

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /webapp
COPY --from=build /webapp/output .
COPY ./resources/stocky.ttf . 
ENTRYPOINT ["dotnet", "ImageFetcher.dll"]