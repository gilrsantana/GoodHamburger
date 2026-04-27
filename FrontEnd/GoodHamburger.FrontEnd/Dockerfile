# Use the .NET 10 SDK as the base image for building
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["FrontEnd/GoodHamburger.FrontEnd/GoodHamburger.FrontEnd.csproj", "FrontEnd/GoodHamburger.FrontEnd/"]
RUN dotnet restore "FrontEnd/GoodHamburger.FrontEnd/GoodHamburger.FrontEnd.csproj"

# Copy all source files
COPY . .

# Build the application
WORKDIR "/src/FrontEnd/GoodHamburger.FrontEnd"
RUN dotnet build "GoodHamburger.FrontEnd.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "GoodHamburger.FrontEnd.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use a lightweight web server to serve the static files
FROM nginx:alpine AS final

# Copy the published Blazor WebAssembly output to nginx
COPY --from=publish /app/publish/wwwroot /usr/share/nginx/html

# Copy custom nginx configuration
COPY FrontEnd/GoodHamburger.FrontEnd/nginx.conf /etc/nginx/nginx.conf

# Expose port 80
EXPOSE 80

# Start nginx
CMD ["nginx", "-g", "daemon off;"]
