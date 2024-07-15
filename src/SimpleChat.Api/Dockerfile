FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5008

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/SimpleChat.Library/SimpleChat.Library.csproj", "SimpleChat.Library/"]
RUN dotnet restore "SimpleChat.Library/SimpleChat.Library.csproj"

COPY ["src/SimpleChat.Api/SimpleChat.Api.csproj", "SimpleChat.Api/"]
RUN dotnet restore "SimpleChat.Api/SimpleChat.Api.csproj"

COPY . .

WORKDIR "/src/src/SimpleChat.Api"
RUN dotnet build "SimpleChat.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build -r linux-x64 --self-contained

FROM build AS publish
RUN dotnet publish "SimpleChat.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "SimpleChat.Api.dll"]