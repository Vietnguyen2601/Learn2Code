# Learn2Code

Nền tảng học lập trình trực tuyến - Backend API được xây dựng với .NET 8 và Clean Architecture.

## 🏗️ Kiến trúc Project

Project được thiết kế theo **Clean Architecture** với 4 layers:

```
BE/
├── Learn2Code.API/              # Presentation Layer
│   ├── Controllers/             # API Controllers
│   ├── Middlewares/             # Custom Middlewares
│   └── Program.cs               # Entry point
│
├── Learn2Code.Application/      # Application Layer (Services)
│   ├── DTOs/                    # Data Transfer Objects
│   ├── Interfaces/              # Service Interfaces
│   ├── Services/                # Business Logic
│   ├── Validators/              # FluentValidation
│   └── Mapper/                  # AutoMapper Profiles
│
├── Learn2Code.Infrastructure/   # Infrastructure Layer (Repositories)
│   ├── Data/Context/            # DbContext
│   ├── Repositories/            # Repository Implementations
│   └── Persistence/             # Database Configurations
│
└── Learn2Code.Domain/           # Domain Layer (Common)
    └── Entities/                # Domain Entities
```

### Layer Dependencies

```
API → Application → Infrastructure → Domain
         ↓              ↓
       Domain        Domain
```

## 🚀 Cài đặt và Chạy

### Yêu cầu

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) (optional)
- [PostgreSQL](https://www.postgresql.org/) (nếu chạy local)

### Chạy Local

```bash
# Clone repository
git clone https://github.com/Vietnguyen2601/Learn2Code.git
cd Learn2Code/BE

# Restore packages
dotnet restore

# Chạy ứng dụng
dotnet run --project Learn2Code.API
```

Truy cập Swagger UI: `https://localhost:5001/swagger`

### Chạy với Docker

#### Build Docker Image

```bash
cd BE
docker build -f Learn2Code.API/Dockerfile -t learn2code-api .
```

#### Chạy Container

```bash
docker run -d \
  --name learn2code-api \
  -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Port=5432;Database=learn2code_db;Username=learn2code;Password=Learn2Code@2024" \
  learn2code-api
```

#### Docker Compose (Recommended)

Tạo file `docker-compose.yml` trong thư mục `BE/`:

```yaml
version: "3.8"

services:
  api:
    build:
      context: .
      dockerfile: Learn2Code.API/Dockerfile
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=learn2code_db;Username=learn2code;Password=Learn2Code@2024
    depends_on:
      - postgres
    networks:
      - learn2code-network

  postgres:
    image: postgres:16-alpine
    environment:
      POSTGRES_DB: learn2code_db
      POSTGRES_USER: learn2code
      POSTGRES_PASSWORD: Learn2Code@2024
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data
    networks:
      - learn2code-network

volumes:
  postgres_data:

networks:
  learn2code-network:
    driver: bridge
```

Chạy với Docker Compose:

```bash
cd BE
docker-compose up -d

# Xem logs
docker-compose logs -f api

# Dừng services
docker-compose down
```

## 📡 API Endpoints

> _Đang trong quá trình phát triển. Các endpoints sẽ được cập nhật khi hoàn thành._

### Planned Endpoints

| Method             | Endpoint                      | Mô tả                       |
| ------------------ | ----------------------------- | --------------------------- |
| **Authentication** |                               |                             |
| POST               | `/api/auth/register`          | Đăng ký tài khoản           |
| POST               | `/api/auth/login`             | Đăng nhập                   |
| POST               | `/api/auth/refresh-token`     | Làm mới token               |
| **Users**          |                               |                             |
| GET                | `/api/users/me`               | Lấy thông tin user hiện tại |
| PUT                | `/api/users/me`               | Cập nhật profile            |
| **Courses**        |                               |                             |
| GET                | `/api/courses`                | Lấy danh sách khóa học      |
| GET                | `/api/courses/{id}`           | Lấy chi tiết khóa học       |
| POST               | `/api/courses`                | Tạo khóa học (Admin)        |
| PUT                | `/api/courses/{id}`           | Cập nhật khóa học (Admin)   |
| DELETE             | `/api/courses/{id}`           | Xóa khóa học (Admin)        |
| **Lessons**        |                               |                             |
| GET                | `/api/courses/{id}/lessons`   | Lấy lessons của khóa học    |
| POST               | `/api/courses/{id}/lessons`   | Thêm lesson (Admin)         |
| **Enrollments**    |                               |                             |
| POST               | `/api/enrollments`            | Đăng ký khóa học            |
| GET                | `/api/enrollments/my-courses` | Khóa học đã đăng ký         |

## 🛠️ Công nghệ sử dụng

- **.NET 8** - Framework chính
- **Entity Framework Core 8** - ORM
- **PostgreSQL** - Database
- **Swagger/OpenAPI** - API Documentation
- **FluentValidation** - Validation
- **AutoMapper** - Object Mapping

## 📝 Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=learn2code_db;Username=learn2code;Password=Learn2Code@2024"
  },
  "Jwt": {
    "SecretKey": "your-secret-key-here",
    "Issuer": "Learn2Code",
    "Audience": "Learn2CodeUsers",
    "ExpiryInMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## 📂 Branch Strategy

- `main` - Production
- `develop` - Development
- `feature/*` - Feature branches
- `hotfix/*` - Hotfix branches

## 🤝 Contributing

1. Fork repository
2. Tạo branch mới: `git checkout -b feature/amazing-feature`
3. Commit changes: `git commit -m 'Add amazing feature'`
4. Push to branch: `git push origin feature/amazing-feature`
5. Tạo Pull Request

## 📄 License

MIT License - xem file [LICENSE](LICENSE) để biết thêm chi tiết.
