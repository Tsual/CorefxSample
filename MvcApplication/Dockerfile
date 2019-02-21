FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["MvcApplication/MvcApplication.csproj", "MvcApplication/"]
RUN dotnet restore "MvcApplication/MvcApplication.csproj"
COPY . .
WORKDIR "/src/MvcApplication"
RUN dotnet build "MvcApplication.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "MvcApplication.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MvcApplication.dll"]