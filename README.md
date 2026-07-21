# Horse Racing Tournament Management System API (PRN232)

Hệ thống quản lý giải đua ngựa RESTful API được xây dựng dựa trên kiến trúc **Clean Architecture / Onion Architecture** (.NET 9 / C#).

---

## 🏗 Architecture (Kiến trúc hệ thống)

Dự án được chia thành các layer chính:
- **`HorseRacing.Domain`**: Chứa các Core Entities, Enums và Exceptions.
- **`HorseRacing.Application`**: Chứa DTOs, Mappings (AutoMapper) và Interfaces.
- **`HorseRacing.Infrastructure`**: Chứa EF Core `AppDbContext`, Database Migrations, Repositories và Services.
- **`HorseRacing.API`**: RESTful API Controllers, Authentication (JWT), Middleware và Swagger configuration.
- **`HorseRacing.Shared`**: Chứa các utilities, constants và helper classes dùng chung.

---

## 🚀 Quick Start (Hướng dẫn chạy dự án)

### 1. Yêu cầu hệ thống
- .NET 9.0 SDK trở lên
- SQL Server (LocalDB hoặc Server instance)

### 2. Cấu hình chuỗi kết nối
Cập nhật chuỗi kết nối trong file `HorseRacing.API/appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=HorseRacingDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 3. Chạy dự án
```bash
dotnet restore PRN232_HorseRacingTournamentManagementSystem.slnx
dotnet build PRN232_HorseRacingTournamentManagementSystem.slnx
dotnet run --project HorseRacing.API
```

---

## 📌 Main API Endpoints

- **Auth**: `POST /api/auth/login`, `POST /api/auth/register`
- **Tournaments**: `GET /api/tournaments`, `POST /api/tournaments`
- **Races**: `GET /api/races`, `POST /api/races`
- **Horses**: `GET /api/horses`, `POST /api/horses`
- **Bets**: `GET /api/bets`, `POST /api/bets`
- **Registrations**: `GET /api/registrations`, `POST /api/registrations`
- **Swagger Documentation**: `http://localhost:<port>/swagger`
