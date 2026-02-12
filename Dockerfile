# 建置階段
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY . .

# 👉 正確進到專案資料夾（沒有 src）
WORKDIR /app/cakeTodoList

RUN dotnet restore
RUN dotnet publish -c Release -o /app/out

# 執行階段
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 8080
ENTRYPOINT ["dotnet", "cakeTodoList.dll"]
