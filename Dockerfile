#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Userservice/Zhaoxi.MSACommerce.UserMicroservice.csproj", "Userservice/"]
#COPY ["Zhaoxi.MSACommerce.Service/Zhaoxi.MSACommerce.Service.csproj", "Zhaoxi.MSACommerce.Service/"]
#COPY ["Zhaoxi.MSACommerce.Model/Zhaoxi.MSACommerce.Model.csproj", "Zhaoxi.MSACommerce.Model/"]
#COPY ["Zhaoxi.AgileFramework.Core/Zhaoxi.AgileFramework.Core.csproj", "Zhaoxi.AgileFramework.Core/"]
#COPY ["Zhaoxi.AgileFramework.Common/Zhaoxi.AgileFramework.Common.csproj", "Zhaoxi.AgileFramework.Common/"]
#COPY ["Zhaoxi.MSACommerce.Interface/Zhaoxi.MSACommerce.Interface.csproj", "Zhaoxi.MSACommerce.Interface/"]
#COPY ["Zhaoxi.AgileFramework.WebCore/Zhaoxi.AgileFramework.WebCore.csproj", "Zhaoxi.AgileFramework.WebCore/"]
RUN dotnet restore "Userservice/Zhaoxi.MSACommerce.UserMicroservice.csproj"
COPY . .
WORKDIR "/src/Userservice"
RUN dotnet build "Zhaoxi.MSACommerce.UserMicroservice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Zhaoxi.MSACommerce.UserMicroservice.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Zhaoxi.MSACommerce.UserMicroservice.dll"]