# TripMate API

A comprehensive travel management API built with .NET 9.0, providing services for hotels, restaurants, attractions, and reviews.

## 🚀 Features

- **Authentication & Authorization**: Secure identity management
- **Hotels Management**: CRUD operations for hotel information
- **Restaurants Management**: Restaurant listings and details
- **Attractions Management**: Tourist attractions and activities
- **Reviews System**: User reviews and ratings
- **Caching**: Optimized performance with caching layer
- **Image Management**: Upload and storage for location images
- **Logging**: Comprehensive logging with Serilog

## 🛠️ Technology Stack

- **.NET 9.0**: Latest framework version
- **ASP.NET Core Web API**: RESTful API development
- **Entity Framework Core**: ORM for database operations
- **Serilog**: Structured logging
- **FluentValidation**: Input validation
- **Swagger/OpenAPI**: API documentation

## 📁 Project Structure

```
TripMate/
├── src/
│   ├── Tripmate.API/          # API layer with controllers and middleware
│   ├── Tripmate.Application/  # Business logic and services
│   ├── Tripmate.Domain/       # Domain entities and interfaces
│   └── Tripmate.Infrastructure/ # Data access and external services
├── test/
│   └── Tripmate.ApplicationTests/ # Unit and integration tests
└── .github/
    └── workflows/             # CI/CD pipelines
```

## 🔧 Prerequisites

- .NET 9.0 SDK or later
- SQL Server (or your configured database)
- Visual Studio 2022 or VS Code

## 🚀 Getting Started

### Local Development

1. **Clone the repository**
   ```bash
   git clone https://github.com/hishamabdalla/TripMate.git
   cd TripMate
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore Tripmate.sln
   ```

3. **Update database connection string**
   - Update `appsettings.Development.json` in `src/Tripmate.API/`
   - Configure your database connection string

4. **Apply migrations**
   ```bash
   dotnet ef database update --project src/Tripmate.Infrastructure --startup-project src/Tripmate.API
   ```

5. **Run the application**
   ```bash
   dotnet run --project src/Tripmate.API
   ```

6. **Access Swagger UI**
   - Navigate to `https://localhost:5001/swagger` (or your configured port)

### Running Tests

```bash
# Run all tests
dotnet test Tripmate.sln

# Run tests with coverage
dotnet test Tripmate.sln --collect:"XPlat Code Coverage"
```

## 📦 Build and Publish

### Build for Release
```bash
dotnet build Tripmate.sln --configuration Release
```

### Publish Application
```bash
dotnet publish src/Tripmate.API/Tripmate.API.csproj --configuration Release --output ./publish
```

## 🔄 CI/CD Pipeline

This project uses GitHub Actions for continuous integration and deployment to MonsterASP.NET.

### GitHub Secrets Configuration

To enable deployment, configure the following secrets in your GitHub repository:

1. **FTP_SERVER**: Your MonsterASP.NET FTP server address
   - Example: `ftp.monsterasp.net`

2. **FTP_USERNAME**: Your FTP username
   - Example: `your-username`

3. **FTP_PASSWORD**: Your FTP password
   - Your MonsterASP.NET account password

4. **FTP_SERVER_DIR**: The directory path on the server
   - Example: `/httpdocs/` or `/public_html/`

5. **PRODUCTION_URL**: Your production website URL
   - Example: `https://yourdomain.com`

### Setting up GitHub Secrets

1. Go to your GitHub repository
2. Navigate to **Settings** → **Secrets and variables** → **Actions**
3. Click **New repository secret**
4. Add each secret with its corresponding value

### Workflows

- **deploy.yml**: Builds, tests, and deploys to production on push to master
- **pr-check.yml**: Runs tests and code quality checks on pull requests

### Deployment Process

1. **Push to master branch** triggers the deployment
2. **Build and Test**: Compiles the solution and runs all tests
3. **Publish**: Creates production-ready artifacts
4. **Deploy**: Uploads files to MonsterASP.NET via FTP

## 📝 API Documentation

Once the application is running, visit `/swagger` to view the interactive API documentation.

### Main Endpoints

- `/api/account` - Authentication and user management
- `/api/hotels` - Hotel operations
- `/api/restaurants` - Restaurant operations
- `/api/attractions` - Attractions management
- `/api/reviews` - Review system
- `/api/countries` - Countries data
- `/api/regions` - Regions data

## 🧪 Testing

The project includes comprehensive unit and integration tests in the `Tripmate.ApplicationTests` project.

Run tests:
```bash
dotnet test test/Tripmate.ApplicationTests/Tripmate.ApplicationTests.csproj
```

## 📊 Code Coverage

Code coverage reports are automatically generated during CI/CD pipeline execution and can be viewed in the GitHub Actions artifacts.

## 🔒 Security

- Ensure all sensitive configuration is stored in environment variables or secrets
- Never commit `appsettings.Production.json` or other files containing sensitive data
- Use HTTPS in production
- Implement proper authentication and authorization

## 📄 License

[Your License Here]

## 👥 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📞 Support

For issues and questions, please open an issue in the GitHub repository.

## 🙏 Acknowledgments

- .NET Team for the excellent framework
- All contributors to this project

---

**Built with ❤️ using .NET 9.0**
