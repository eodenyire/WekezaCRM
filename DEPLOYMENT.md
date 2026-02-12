# Wekeza CRM - Deployment Guide

## Prerequisites

### Required Software
- .NET 8 SDK or later
- SQL Server 2019+ (or Azure SQL Database)
- Visual Studio 2022 / VS Code (optional for development)

### Optional
- Docker (for containerized deployment)
- Azure CLI (for Azure deployment)

## Local Development Setup

### 1. Clone the Repository
```bash
git clone https://github.com/eodenyire/WekezaCRM.git
cd WekezaCRM
```

### 2. Configure Database Connection

Edit `src/API/WekezaCRM.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=WekezaCRM;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=true"
  }
}
```

For Azure SQL:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:YOUR_SERVER.database.windows.net,1433;Initial Catalog=WekezaCRM;Persist Security Info=False;User ID=YOUR_USER;Password=YOUR_PASSWORD;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
```

### 3. Apply Database Migrations

```bash
cd src/Infrastructure/WekezaCRM.Infrastructure
dotnet ef database update --startup-project ../../API/WekezaCRM.API
```

### 4. Run the Application

```bash
cd ../../API/WekezaCRM.API
dotnet run
```

The API will be available at:
- HTTPS: https://localhost:5001
- HTTP: http://localhost:5000
- Swagger: https://localhost:5001/swagger

## Production Deployment

### Option 1: Windows Server / IIS

#### 1. Publish the Application

```bash
cd src/API/WekezaCRM.API
dotnet publish -c Release -o ./publish
```

#### 2. Configure IIS

1. Install .NET 8 Hosting Bundle from Microsoft
2. Create a new Application Pool (.NET CLR Version: No Managed Code)
3. Create a new Website pointing to the publish folder
4. Set the Application Pool for the website

#### 3. Configure Connection String

Edit `publish/appsettings.json` with production database credentials.

#### 4. Set Environment Variables

In IIS, set the `ASPNETCORE_ENVIRONMENT` to `Production`.

### Option 2: Linux / Systemd

#### 1. Publish the Application

```bash
dotnet publish -c Release -o /var/www/wekeza-crm
```

#### 2. Create Systemd Service

Create `/etc/systemd/system/wekeza-crm.service`:

```ini
[Unit]
Description=Wekeza CRM API
After=network.target

[Service]
WorkingDirectory=/var/www/wekeza-crm
ExecStart=/usr/bin/dotnet /var/www/wekeza-crm/WekezaCRM.API.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=wekeza-crm
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

#### 3. Enable and Start Service

```bash
sudo systemctl enable wekeza-crm.service
sudo systemctl start wekeza-crm.service
sudo systemctl status wekeza-crm.service
```

#### 4. Configure Nginx as Reverse Proxy

Create `/etc/nginx/sites-available/wekeza-crm`:

```nginx
server {
    listen 80;
    server_name api.wekeza.com;
    
    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

Enable the site:
```bash
sudo ln -s /etc/nginx/sites-available/wekeza-crm /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx
```

### Option 3: Docker

#### 1. Create Dockerfile

Create `Dockerfile` in the root directory:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/API/WekezaCRM.API/WekezaCRM.API.csproj", "API/WekezaCRM.API/"]
COPY ["src/Core/WekezaCRM.Application/WekezaCRM.Application.csproj", "Core/WekezaCRM.Application/"]
COPY ["src/Core/WekezaCRM.Domain/WekezaCRM.Domain.csproj", "Core/WekezaCRM.Domain/"]
COPY ["src/Infrastructure/WekezaCRM.Infrastructure/WekezaCRM.Infrastructure.csproj", "Infrastructure/WekezaCRM.Infrastructure/"]
RUN dotnet restore "API/WekezaCRM.API/WekezaCRM.API.csproj"
COPY . .
WORKDIR "/src/API/WekezaCRM.API"
RUN dotnet build "WekezaCRM.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WekezaCRM.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WekezaCRM.API.dll"]
```

#### 2. Create docker-compose.yml

```yaml
version: '3.8'

services:
  api:
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - "8080:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=db;Database=WekezaCRM;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true
    depends_on:
      - db
    networks:
      - wekeza-network

  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong@Passw0rd
    ports:
      - "1433:1433"
    volumes:
      - sqldata:/var/opt/mssql
    networks:
      - wekeza-network

volumes:
  sqldata:

networks:
  wekeza-network:
    driver: bridge
```

#### 3. Build and Run

```bash
docker-compose up -d
```

#### 4. Apply Migrations

```bash
docker-compose exec api dotnet ef database update
```

### Option 4: Azure App Service

#### 1. Create Azure Resources

```bash
# Login to Azure
az login

# Create resource group
az group create --name wekeza-crm-rg --location eastus

# Create SQL Server
az sql server create \
  --name wekeza-sql-server \
  --resource-group wekeza-crm-rg \
  --location eastus \
  --admin-user sqladmin \
  --admin-password YourStrong@Password

# Create SQL Database
az sql db create \
  --resource-group wekeza-crm-rg \
  --server wekeza-sql-server \
  --name WekezaCRM \
  --service-objective S0

# Create App Service Plan
az appservice plan create \
  --name wekeza-crm-plan \
  --resource-group wekeza-crm-rg \
  --sku B1 \
  --is-linux

# Create Web App
az webapp create \
  --resource-group wekeza-crm-rg \
  --plan wekeza-crm-plan \
  --name wekeza-crm-api \
  --runtime "DOTNETCORE:8.0"
```

#### 2. Configure Connection String

```bash
az webapp config connection-string set \
  --name wekeza-crm-api \
  --resource-group wekeza-crm-rg \
  --connection-string-type SQLAzure \
  --settings DefaultConnection="Server=tcp:wekeza-sql-server.database.windows.net,1433;Initial Catalog=WekezaCRM;Persist Security Info=False;User ID=sqladmin;Password=YourStrong@Password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
```

#### 3. Deploy Application

```bash
cd src/API/WekezaCRM.API
dotnet publish -c Release -o ./publish
cd publish
zip -r ../deploy.zip *
cd ..
az webapp deployment source config-zip \
  --resource-group wekeza-crm-rg \
  --name wekeza-crm-api \
  --src deploy.zip
```

## Security Considerations

### 1. JWT Secret Key

**Production**: Generate a secure random key:

```bash
# Linux/Mac
openssl rand -base64 32
```

Update `appsettings.Production.json`:
```json
{
  "JwtSettings": {
    "SecretKey": "YOUR_SECURE_RANDOM_KEY_HERE"
  }
}
```

### 2. CORS Configuration

For production, restrict CORS to specific domains:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins("https://crm.wekeza.com", "https://app.wekeza.com")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Use in production
app.UseCors("Production");
```

### 3. HTTPS

Always use HTTPS in production. Consider:
- Let's Encrypt for free SSL certificates
- Azure App Service provides free SSL
- Cloudflare for additional DDoS protection

### 4. Database Security

- Use strong passwords
- Enable SQL Server firewall rules
- Use Azure Key Vault for storing secrets
- Enable SQL Server auditing
- Regular backups

## Monitoring and Logging

### Application Insights (Azure)

Add to `Program.cs`:
```csharp
builder.Services.AddApplicationInsightsTelemetry();
```

### Health Checks

The API includes a health endpoint at `/api/health`.

### Logging

Configure logging in `appsettings.Production.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## Maintenance

### Database Backups

Schedule regular SQL Server backups:
- Daily full backups
- Hourly transaction log backups
- Test restore procedures monthly

### Updates

```bash
# Pull latest changes
git pull origin main

# Rebuild and redeploy
dotnet publish -c Release
# Copy to production server
# Restart service
```

## Troubleshooting

### Connection Issues

Check:
1. SQL Server is running
2. Firewall allows connections
3. Connection string is correct
4. Database exists

### Migration Issues

```bash
# Check migration status
dotnet ef migrations list --startup-project ../../API/WekezaCRM.API

# Apply specific migration
dotnet ef database update MigrationName --startup-project ../../API/WekezaCRM.API
```

### Permission Issues

Ensure the application user has:
- db_datareader
- db_datawriter
- Execute permissions on stored procedures (if any)

## Support

For deployment support:
- Email: dev@wekeza.com
- Documentation: See README.md
- Issues: https://github.com/eodenyire/WekezaCRM/issues
