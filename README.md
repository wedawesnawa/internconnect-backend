# Internconnect Backend

Backend REST API untuk aplikasi **Internconnect**, sebuah platform yang mendukung proses monitoring kegiatan magang, pengelolaan logbook, komunikasi antara mahasiswa dan pembimbing, pengajuan monitoring dan evaluasi (Monev), berbagi logbook, serta integrasi meeting online.

Backend dibangun menggunakan **ASP.NET Core Web API** dengan **Microsoft SQL Server** sebagai database utama, **MinIO** sebagai object storage untuk file dan gambar, **JWT (JSON Web Token)** untuk autentikasi dan otorisasi, serta **Whereby API** untuk mendukung pembuatan online meeting.

---

# Technology Stack

Project ini menggunakan teknologi berikut:

| Technology            | Description                          |
| --------------------- | ------------------------------------ |
| ASP.NET Core          | Framework backend dan REST API       |
| .NET 8                | Runtime aplikasi                     |
| Microsoft SQL Server  | Database utama                       |
| Entity Framework Core | ORM dan database migration           |
| MinIO                 | Object storage untuk gambar dan file |
| JWT                   | Authentication dan authorization     |
| Whereby API           | Pembuatan online meeting             |
| Docker                | Menjalankan service dalam container  |
| Swagger / OpenAPI     | Dokumentasi dan testing API          |

---

# Features

Beberapa fitur utama yang tersedia pada backend:

* User registration dan login
* Authentication menggunakan JWT
* Role-based authorization
* Manajemen user dan role
* Manajemen profile user
* Upload profile picture
* Upload file dokumen
* Manajemen logbook
* Manajemen aktivitas logbook
* Verifikasi aktivitas logbook
* Sharing logbook dengan user lain
* Monitoring dan evaluasi (Monev)
* Upload dan penyimpanan gambar menggunakan MinIO
* Pembuatan online meeting menggunakan Whereby API
* Dokumentasi API menggunakan Swagger

---

# Architecture

```text
Client / Frontend
        │
        ▼
ASP.NET Core Web API
        │
        ├──────────────► JWT Authentication
        │
        ├──────────────► Microsoft SQL Server
        │                     │
        │                     ▼
        │                 Database
        │
        ├──────────────► MinIO
        │                     │
        │                     ▼
        │              Image / File Storage
        │
        └──────────────► Whereby API
                              │
                              ▼
                        Online Meeting
```

---

# Prerequisites

Pastikan software berikut sudah terinstall:

* Git
* Docker Desktop
* .NET 8 SDK
* Visual Studio 2022 atau Visual Studio Code
* SQL Server Management Studio (Opsional)

Periksa versi .NET:

```powershell
dotnet --version
```

Pastikan menggunakan versi yang mendukung:

```text
.NET 8
```

Periksa Docker:

```powershell
docker --version
docker compose version
```

---

# Installation

## 1. Clone Repository

Clone repository:

```powershell
git clone https://github.com/wedawesnawa/internconnect-backend.git
```

Masuk ke folder project:

```powershell
cd internconnect-backend
```

Kemudian masuk ke folder backend:

```powershell
cd InternconnectBackend
```

---

# Docker Setup

Project menggunakan Docker untuk menjalankan service pendukung seperti:

* Microsoft SQL Server
* MinIO

Pastikan Docker Desktop sudah berjalan sebelum melanjutkan.

---

## 2. Microsoft SQL Server dengan Docker

Buat atau gunakan file:

```text
docker-compose.yml
```

Contoh konfigurasi SQL Server:

```yaml
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: internconnect-sqlserver
    restart: unless-stopped

    environment:
      ACCEPT_EULA: "Y"
      MSSQL_SA_PASSWORD: "YourStrongPassword123!"

    ports:
      - "1433:1433"

    volumes:
      - sqlserver_data:/var/opt/mssql

volumes:
  sqlserver_data:
```

Jalankan container:

```powershell
docker compose up -d
```

Periksa container:

```powershell
docker ps
```

SQL Server akan tersedia pada:

```text
Server: localhost
Port: 1433
```

Contoh connection string:

```text
Server=localhost,1433;
Database=InternconnectDb;
User Id=sa;
Password=YourStrongPassword123!;
TrustServerCertificate=True;
```

---

# MinIO dengan Docker

MinIO digunakan sebagai **object storage** untuk menyimpan:

* Foto profile
* Gambar logbook
* File dokumen

Tambahkan konfigurasi MinIO pada:

```text
docker-compose.yml
```

Contoh:

```yaml
services:
  minio:
    image: minio/minio
    container_name: internconnect-minio
    restart: unless-stopped

    environment:
      MINIO_ROOT_USER: minioadmin
      MINIO_ROOT_PASSWORD: minioadmin123

    ports:
      - "9000:9000"
      - "9001:9001"

    volumes:
      - minio_data:/data

    command: server /data --console-address ":9001"

volumes:
  minio_data:
```

Jalankan:

```powershell
docker compose up -d
```

MinIO API tersedia pada:

```text
http://localhost:9000
```

MinIO Console tersedia pada:

```text
http://localhost:9001
```

Login menggunakan:

```text
Username: minioadmin
Password: minioadmin123
```

> Untuk production, jangan menggunakan credential default.

---

# Contoh Docker Compose Lengkap

Contoh file:

```yaml
services:

  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: internconnect-sqlserver

    environment:
      ACCEPT_EULA: "Y"
      MSSQL_SA_PASSWORD: "YourStrongPassword123!"

    ports:
      - "1433:1433"

    volumes:
      - sqlserver_data:/var/opt/mssql


  minio:
    image: minio/minio
    container_name: internconnect-minio

    environment:
      MINIO_ROOT_USER: minioadmin
      MINIO_ROOT_PASSWORD: minioadmin123

    ports:
      - "9000:9000"
      - "9001:9001"

    volumes:
      - minio_data:/data

    command: server /data --console-address ":9001"


volumes:
  sqlserver_data:
  minio_data:
```

Jalankan seluruh service:

```powershell
docker compose up -d
```

Menghentikan service:

```powershell
docker compose down
```

Melihat log:

```powershell
docker compose logs
```

---

# Konfigurasi Environment

Buat file:

```text
appsettings.json
```

Jika tersedia file example:

```powershell
Copy-Item appsettings.example.json appsettings.json
```

Contoh konfigurasi:

```json
{
  "ConnectionStrings": {
    "InternconnectConnectionString": "Server=localhost,1433;Database=InternconnectDb;User Id=sa;Password=YourStrongPassword123!;TrustServerCertificate=True"
  },

  "Jwt": {
    "Key": "YOUR_SECRET_KEY_CHANGE_THIS",
    "Issuer": "InternconnectBackend",
    "Audience": "InternconnectFrontend"
  },

  "Minio": {
    "Endpoint": "localhost:9000",
    "AccessKey": "minioadmin",
    "SecretKey": "minioadmin123",
    "BucketName": "internconnect",
    "Secure": false
  },

  "Whereby": {
    "ApiKey": "YOUR_WHEREBY_API_KEY"
  }
}
```

> Jangan melakukan commit terhadap `appsettings.json` apabila file tersebut berisi password, JWT secret, API key, atau credential production.

---

# JWT Authentication

Project menggunakan **JSON Web Token (JWT)** untuk melakukan authentication dan authorization.

Alur autentikasi:

```text
User Login
    │
    ▼
POST /api/Account/login
    │
    ▼
Backend memverifikasi username dan password
    │
    ▼
JWT Token dibuat
    │
    ▼
Token dikirim ke client
    │
    ▼
Client mengirim token pada request berikutnya
    │
    ▼
Authorization berhasil
```

Contoh request header:

```http
Authorization: Bearer YOUR_JWT_TOKEN
```

JWT digunakan untuk mengidentifikasi user yang sedang login dan menentukan hak akses berdasarkan role.

Contoh role yang digunakan dalam sistem:

* User
* Supervisor
* Mentor
* Admin

---

# 👤 Account API

## Register

Mendaftarkan user baru.

```http
POST /api/Account/register
```

Request:

```json
{
  "username": "string",
  "email": "string",
  "password": "string"
}
```

---

## Login

Melakukan autentikasi user.

```http
POST /api/Account/login
```

Request:

```json
{
  "username": "string",
  "password": "string"
}
```

Setelah login berhasil, backend melakukan autentikasi user dan menghasilkan token JWT yang digunakan untuk mengakses endpoint yang membutuhkan authorization.

---

## Get Current User

Mendapatkan informasi user yang sedang login berdasarkan identitas yang terdapat pada JWT token.

```http
GET /api/Account/me
```

Endpoint ini biasanya digunakan oleh frontend untuk:

* Mengecek status login user
* Mendapatkan identitas user yang sedang login
* Mengambil username atau claim dari JWT
* Menentukan role user
* Menampilkan data user pada aplikasi

Request membutuhkan JWT:

```http
Authorization: Bearer YOUR_JWT_TOKEN
```

---

## Logout

Melakukan proses logout user.

```http
POST /api/Account/logout
```

Pada arsitektur JWT, mekanisme logout dapat berupa penghapusan token dari client atau mekanisme tambahan seperti token revocation, tergantung implementasi backend.

---

## Assign Role

Mengubah atau memberikan role kepada user.

```http
POST /api/Account/assign-role
```

Request:

```json
{
  "username": "string",
  "role": "string"
}
```

Endpoint ini hanya dapat digunakan oleh user dengan role yang memiliki hak akses untuk melakukan manajemen role.

---

# Logbook API

## Create Logbook

Membuat logbook baru.

```http
POST /api/Logbook/create
```

Content-Type:

```text
multipart/form-data
```

| Field     | Type     | Required |
| --------- | -------- | -------- |
| Content   | string   | Yes      |
| DateStart | datetime | Yes      |
| DateEnd   | datetime | Yes      |
| Status    | string   | Yes      |
| Deskripsi | string   | Yes      |
| Image     | binary   | No       |

Gambar yang diupload dapat disimpan menggunakan MinIO.

---

## Update Logbook

```http
PUT /api/Logbook/update/{kodeLogbook}
```

Digunakan untuk memperbarui data logbook berdasarkan kode logbook.

---

## Delete Logbook

```http
DELETE /api/Logbook/delete/{kodeLogbook}
```

Menghapus logbook berdasarkan kode logbook.

---

## Get All Logbooks

```http
GET /api/Logbook/all
```

Mengambil seluruh logbook yang tersedia sesuai dengan hak akses user.

---

## Get My Logbooks

```http
GET /api/Logbook/my-logbooks
```

Mengambil seluruh logbook milik user yang sedang login.

Identitas user diperoleh dari JWT token.

---

## Get Logbook Detail

```http
GET /api/Logbook/{kodeLogbook}
```

Mengambil detail satu logbook berdasarkan `kodeLogbook`.

Endpoint ini dapat digunakan ketika user yang memiliki izin ingin melihat logbook yang telah dibagikan.

---

## Get Logbook Image URL

```http
GET /api/Logbook/image-url/{kodeLogbook}
```

Mengambil URL gambar yang telah diupload pada logbook.

---

# Detail Logbook API

## Create Activity

```http
POST /api/DetailLogbook/{kodeLogbook}/create
```

Digunakan untuk membuat aktivitas pada logbook.

---

## Get Activity Detail

```http
GET /api/DetailLogbook/{id}
```

Mengambil detail satu aktivitas berdasarkan ID.

---

## Get All Activities

```http
GET /api/DetailLogbook/{kodeLogbook}/all
```

Mengambil seluruh aktivitas dari satu logbook.

---

## Update Activity

```http
PUT /api/DetailLogbook/{id}/update
```

Memperbarui aktivitas logbook.

---

## Delete Activity

```http
DELETE /api/DetailLogbook/{id}/delete
```

Menghapus aktivitas logbook.

---

## Verify Activity

```http
PUT /api/DetailLogbook/{id}/verif
```

Digunakan oleh role seperti Supervisor atau Mentor untuk mengubah status atau melakukan verifikasi terhadap aktivitas user.

---

# Shared Logbook API

Fitur ini memungkinkan user membagikan logbook kepada user lain.

## Create Shared Logbook

```http
POST /api/Shared/{kodeLogbook}/create
```

Request:

```json
{
  "idShared": 0,
  "sharedWith": "string",
  "permission": "string"
}
```

---

## Update Shared Logbook

```http
PUT /api/Shared/{kodeLogbook}/update/{id}
```

Digunakan untuk memperbarui informasi sharing atau permission.

---

## Delete Shared Access

```http
DELETE /api/Shared/{kodeLogbook}/delete/{id}
```

Menghapus akses user terhadap logbook yang sebelumnya dibagikan.

---

## Get Shared Users

```http
GET /api/Shared/{kodeLogbook}/all
```

Mengambil daftar user yang memiliki akses terhadap logbook tertentu.

---

# Monitoring dan Evaluasi (Monev)

## Ajukan Monev

```http
POST /api/Monev/ajukan-monev
```

Digunakan oleh user untuk mengajukan proses monitoring dan evaluasi.

---

## Get Monev

```http
GET /api/Monev/{kodeLogbook}
```

Mengambil data monitoring dan evaluasi berdasarkan logbook.

---

# User API

## Update Role

```http
PUT /api/User/update-role
```

Digunakan ketika user mengajukan perubahan role, misalnya dari role default menjadi:

* Mentor
* Supervisor

Endpoint dapat menerima file pendukung seperti surat tugas.

---

## Get User by Role

```http
GET /api/User/by-role?role={role}
```

Digunakan untuk mendapatkan user berdasarkan role.

Contoh:

```text
/api/User/by-role?role=Supervisor
```

---

# User Detail API

## Get User Detail

```http
GET /api/UserDetail
```

Mengambil informasi detail user yang sedang login.

---

## Create User Detail

```http
POST /api/UserDetail
```

Membuat informasi detail user.

---

## Update User Detail

```http
PUT /api/UserDetail
```

Memperbarui informasi user.

---

## Upload Profile Picture

```http
PUT /api/UserDetail/upload-profile-picture
```

Digunakan untuk mengubah foto profile atau avatar user.

File dapat disimpan menggunakan MinIO.

---

## Get User Detail by Username

```http
GET /api/UserDetail/{username}
```

Mengambil informasi detail user berdasarkan username.

Endpoint ini dapat digunakan pada:

* Daftar user
* Informasi pembimbing
* Informasi mentor
* Informasi user yang menerima atau membagikan logbook

---

## Get Profile Picture URL

```http
GET /api/UserDetail/profile-picture-url
```

Mengambil URL foto profile user yang sedang login.

---

## Download File

```http
GET /api/UserDetail/download-file?filePath={filePath}
```

Digunakan untuk mengunduh file yang sebelumnya diupload, misalnya dokumen atau surat tugas.

---

# Online Meeting dengan Whereby API

Internconnect dapat menggunakan **Whereby API** untuk membuat online meeting.

Alur sederhana:

```text
User memilih jadwal meeting
        │
        ▼
Frontend mengirim request ke Backend
        │
        ▼
ASP.NET Core Backend
        │
        ▼
Whereby API
        │
        ▼
Meeting Room dibuat
        │
        ▼
Meeting URL dikembalikan ke Frontend
        │
        ▼
User bergabung ke Online Meeting
```

API key Whereby sebaiknya disimpan di environment configuration:

```json
{
  "Whereby": {
    "ApiKey": "YOUR_WHEREBY_API_KEY"
  }
}
```

> API key tidak boleh disimpan secara langsung pada frontend karena dapat diketahui oleh pengguna.

Backend bertugas sebagai perantara antara frontend dan Whereby API agar API key tetap aman.

Contoh struktur service:

```text
Controllers/
    MeetingController.cs

Services/
    WherebyService.cs

Models/
    MeetingRequest.cs
    MeetingResponse.cs
```

---

# Database Migration

Project menggunakan Entity Framework Core Migration.

Restore dependency:

```powershell
dotnet restore
```

Install EF Tool jika belum tersedia:

```powershell
dotnet tool install --global dotnet-ef
```

Jalankan migration:

```powershell
dotnet ef database update
```

Perintah tersebut akan membuat:

```text
Microsoft SQL Server
        │
        ▼
InternconnectDb
        │
        ▼
Tables
Relationships
Constraints
```

> Jangan membuat tabel secara manual jika project sudah menggunakan Entity Framework Core Migration.

---

# Build Project

Restore dependency:

```powershell
dotnet restore
```

Build project:

```powershell
dotnet build
```

Jika berhasil:

```text
Build succeeded.
0 Warning(s)
0 Error(s)
```

---

# Run Backend

Jalankan backend:

```powershell
dotnet run
```

Contoh output:

```text
Now listening on: http://localhost:5244
Application started.
Hosting environment: Development
```

Port dapat berbeda tergantung konfigurasi project.

---

# Swagger API Documentation

Setelah backend berjalan, buka:

```text
http://localhost:5244/swagger
```

Swagger dapat digunakan untuk:

* Melihat seluruh endpoint
* Melihat request body
* Melihat parameter
* Menguji API
* Melakukan autentikasi menggunakan JWT
* Memeriksa response API

---

# Menggunakan JWT di Swagger

Setelah melakukan login dan mendapatkan JWT token:

1. Buka Swagger.
2. Klik tombol **Authorize**.
3. Masukkan token dengan format:

```text
Bearer YOUR_JWT_TOKEN
```

4. Klik **Authorize**.
5. Endpoint yang membutuhkan authorization dapat diuji.

---

# Suggested Project Structure

```text
InternconnectBackend/
│
├── Controllers/
│   ├── AccountController.cs
│   ├── AdminController.cs
│   ├── LogbookController.cs
│   ├── DetailLogbookController.cs
│   ├── SharedController.cs
│   ├── MonevController.cs
│   ├── UserController.cs
│   └── UserDetailController.cs
│
├── Services/
│   ├── MinioService.cs
│   ├── WherebyService.cs
│   └── ...
│
├── Models/
│
├── DTOs/
│
├── Data/
│   └── ApplicationDbContext.cs
│
├── Migrations/
│
├── Properties/
│
├── appsettings.json
├── appsettings.Development.json
├── Program.cs
└── InternconnectBackend.csproj
```

---

# Development Workflow

Untuk menjalankan project:

## 1. Jalankan Docker Service

```powershell
docker compose up -d
```

## 2. Pastikan Container Berjalan

```powershell
docker ps
```

Pastikan container:

```text
internconnect-sqlserver
internconnect-minio
```

berstatus:

```text
Up
```

## 3. Jalankan Database Migration

```powershell
dotnet ef database update
```

## 4. Jalankan Backend

```powershell
dotnet run
```

## 5. Buka Swagger

```text
http://localhost:5244/swagger
```

---

# Stop Development Environment

Menghentikan backend:

```text
CTRL + C
```

Menghentikan Docker container:

```powershell
docker compose down
```

Jika ingin menghapus container beserta volume:

```powershell
docker compose down -v
```

> Hati-hati menggunakan `-v` karena data SQL Server dan MinIO yang disimpan pada Docker volume dapat terhapus.

---

# Security Notes

Untuk keamanan project:

* Jangan commit `appsettings.json` yang berisi secret.
* Jangan menyimpan JWT secret di repository public.
* Jangan menyimpan Whereby API key di frontend.
* Gunakan password SQL Server yang kuat.
* Ganti credential default MinIO.
* Gunakan environment variables pada production.
* Gunakan HTTPS pada deployment production.
* Batasi akses endpoint berdasarkan role.

---

# License

This project is developed for the **Internconnect** application.

---

# Development

Developed using:

* ASP.NET Core
* .NET 8
* Microsoft SQL Server
* Entity Framework Core
* Docker
* MinIO
* JWT Authentication
* Whereby API
* Swagger / OpenAPI
