#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5004

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["EtuStackOverflow/EtuStackOverflow.csproj", "EtuStackOverflow/"]
COPY ["AskForEtu.Repository/AskForEtu.Repository.csproj", "AskForEtu.Repository/"]
COPY ["AskForEtu.Core/AskForEtu.Core.csproj", "AskForEtu.Core/"]
COPY ["EmailSenderService/EmailSenderService.csproj", "EmailSenderService/"]
RUN dotnet restore "./EtuStackOverflow/EtuStackOverflow.csproj"
COPY . .
WORKDIR "/src/EtuStackOverflow"
RUN dotnet build "./EtuStackOverflow.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EtuStackOverflow.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EtuStackOverflow.dll"]