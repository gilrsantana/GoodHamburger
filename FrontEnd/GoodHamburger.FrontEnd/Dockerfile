FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["FrontEnd/GoodHamburger.FrontEnd/GoodHamburger.FrontEnd.csproj", "FrontEnd/GoodHamburger.FrontEnd/"]
RUN dotnet restore "FrontEnd/GoodHamburger.FrontEnd/GoodHamburger.FrontEnd.csproj"

COPY . .

WORKDIR "/src/FrontEnd/GoodHamburger.FrontEnd"
RUN dotnet build "GoodHamburger.FrontEnd.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GoodHamburger.FrontEnd.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM nginx:alpine AS final
ENTRYPOINT []
COPY --from=publish /app/publish/wwwroot /usr/share/nginx/html
COPY FrontEnd/GoodHamburger.FrontEnd/nginx.conf /etc/nginx/nginx.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
