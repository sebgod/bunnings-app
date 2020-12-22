FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["App.csproj", "./"]
RUN dotnet restore "App.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet tool install dotnet-ef --tool-path .
RUN ./dotnet-ef database update --configuration Release -v --context App.Data.ApplicationDbContext
RUN dotnet publish "App.csproj" -c Release -o /app/publish
RUN mv ./db /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "App.dll"]
