FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
# COPY *.sln ./
COPY *.csproj ./
RUN dotnet restore RabbitMq.Tutorial.Store.csproj

# Copy everything else and build
COPY . ./
RUN dotnet publish RabbitMq.Tutorial.Store.csproj -c Release -o ../out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0-focal
WORKDIR /app
COPY --from=build-env /out .
ENTRYPOINT ["dotnet", "RabbitMq.Tutorial.Store.dll"]
