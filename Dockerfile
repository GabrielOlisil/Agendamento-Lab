FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /App

# Copy csproj and restore as distinct layers
COPY Agendamentos/*.csproj ./Agendamentos/
COPY Agendamentos/packages.lock.json ./Agendamentos/
COPY Models/*.csproj ./Models/
# Restore as distinct layers
RUN dotnet restore Agendamentos/Agendamentos.csproj

COPY . ./
# Build and publish a release
RUN dotnet publish Agendamentos/Agendamentos.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /App
COPY --from=build /App/out ./
ENTRYPOINT ["dotnet", "Agendamentos.dll"]