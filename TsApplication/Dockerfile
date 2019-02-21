FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["TsApplication/TsApplication.csproj", "TsApplication/"]
RUN dotnet restore "TsApplication/TsApplication.csproj"
COPY . .
WORKDIR "/src/TsApplication"
RUN dotnet build "TsApplication.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "TsApplication.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TsApplication.dll"]