#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 5079
EXPOSE 7079

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DrivingTestExplorer/DrivingTestExplorer.csproj", "DrivingTestExplorer/"]
RUN dotnet restore "DrivingTestExplorer/DrivingTestExplorer.csproj"
COPY . .
WORKDIR "/src/DrivingTestExplorer"
RUN dotnet build "DrivingTestExplorer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DrivingTestExplorer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DrivingTestExplorer.dll"]