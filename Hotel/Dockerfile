FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Hotel/Hotel.csproj", "Hotel/"]
RUN dotnet restore "Hotel/Hotel.csproj"
COPY . .
WORKDIR "/src/Hotel"
RUN dotnet build "Hotel.csproj" -c Development -o /app/build

FROM build AS publish
RUN dotnet publish "Hotel.csproj" -c Development -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Hotel.dll"]
# docker run -d -p 8080:8080 --name hotel-app hotel
