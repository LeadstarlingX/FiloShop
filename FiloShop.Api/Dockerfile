# ------- 1. Runtime layer (smallest) -------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080                   
ENV ASPNETCORE_HTTP_PORTS=8080

# ------- 2. Build / publish layer -------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release

# copy sln + all csproj files first → best restore cache
WORKDIR /src
COPY *.sln ./
COPY */*.csproj ./
RUN for f in *.csproj; do mkdir -p ${f%.*}/ && mv $f ${f%.*}/; done
RUN dotnet restore

# copy everything else & build
COPY . .
RUN dotnet publish FiloShop.Api/FiloShop.Api.csproj \
       -c $BUILD_CONFIGURATION \
       -o /app/publish \
#       --no-restore \
       /p:UseAppHost=false

# ------- 3. Final stage -------
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FiloShop.Api.dll"]