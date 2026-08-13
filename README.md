# Internconnect Backend

Backend API for the **Internconnect** application.

## API Documentation

The following API documentation is based on the **InternconnectBackend** Swagger UI.

### Base URL

```text
http://localhost:5244
```

---

# API Endpoints

## 1. Account

Endpoints related to authentication and account management.

### 1.1 Register

**POST** `/api/Account/register`

#### Request Body

```json
{
  "username": "string",
  "email": "string",
  "password": "string"
}
```

#### Response

**200 — Success**

---

### 1.2 Login

**POST** `/api/Account/login`

#### Request Body

```json
{
  "username": "string",
  "password": "string"
}
```

#### Response

**200 — Success**

---

### 1.3 Assign Role

**POST** `/api/Account/assign-role`

#### Request Body

```json
{
  "username": "string",
  "role": "string"
}
```

#### Response

**200 — Success**

---

# 2. Admin

Endpoints for retrieving admin data.

### 2.1 Get Admin

**GET** `/api/Admin`

#### Parameters

No parameters required.

#### Response

**200 — Success**

---

# 3. Detail Logbook

Endpoints for managing logbook details.

## 3.1 Create Detail Logbook

**POST** `/api/DetailLogbook/{kodeLogbook}/create`

#### Path Parameters

| Parameter     | Type          | Required | Description  |
| ------------- | ------------- | -------- | ------------ |
| `kodeLogbook` | string (UUID) | Yes      | Kode logbook |

#### Request Body

```json
{
  "date": "2026-08-13T08:33:28.952Z",
  "deskripsi": "string",
  "kendala": "string",
  "statusAttend": "string",
  "timeStart": {
    "ticks": 0
  },
  "timeEnd": {
    "ticks": 0
  },
  "status": "string"
}
```

#### Response

**200 — Success**

---

## 3.2 Get Detail Logbook by ID

**GET** `/api/DetailLogbook/{id}`

#### Path Parameters

| Parameter | Type            | Required |
| --------- | --------------- | -------- |
| `id`      | integer (int32) | Yes      |

#### Response

**200 — Success**

---

## 3.3 Get All Detail Logbook

**GET** `/api/DetailLogbook/{kodeLogbook}/all`

#### Path Parameters

| Parameter     | Type          | Required |
| ------------- | ------------- | -------- |
| `kodeLogbook` | string (UUID) | Yes      |

#### Response

**200 — Success**

---

## 3.4 Update Detail Logbook

**PUT** `/api/DetailLogbook/{id}/update`

#### Path Parameters

| Parameter | Type            | Required |
| --------- | --------------- | -------- |
| `id`      | integer (int32) | Yes      |

#### Request Body

```json
{
  "date": "2026-08-13T08:33:28.958Z",
  "deskripsi": "string",
  "kendala": "string",
  "statusAttend": "string",
  "timeStart": {
    "ticks": 0
  },
  "timeEnd": {
    "ticks": 0
  },
  "status": "string"
}
```

#### Response

**200 — Success**

---

## 3.5 Delete Detail Logbook

**DELETE** `/api/DetailLogbook/{id}/delete`

#### Path Parameters

| Parameter | Type            | Required |
| --------- | --------------- | -------- |
| `id`      | integer (int32) | Yes      |

#### Response

**200 — Success**

---

## 3.6 Verify Detail Logbook

**PUT** `/api/DetailLogbook/{id}/verif`

#### Path Parameters

| Parameter | Type            | Required |
| --------- | --------------- | -------- |
| `id`      | integer (int32) | Yes      |

#### Request Body

```json
{
  "status": "string"
}
```

#### Response

**200 — Success**

---

# 4. Dosen

Endpoints for retrieving lecturer data and its relationships.

### 4.1 Get All Dosen

**GET** `/api/Dosen`

#### Parameters

No parameters required.

#### Response

**200 — Success**

---

### 4.2 Get Dosen Relation User

**GET** `/api/Dosen/relation-user`

#### Parameters

No parameters required.

#### Response

**200 — Success**

---

### 4.3 Get Dosen Relation

**GET** `/api/Dosen/relation`

#### Parameters

No parameters required.

#### Response

**200 — Success**

---

# 5. Pembimbing

Endpoints for retrieving supervisor data.

### 5.1 Get Pembimbing

**GET** `/api/Pembimbing`

#### Parameters

No parameters required.

#### Response

**200 — Success**

---

# 6. Logbook

Endpoints for creating, updating, deleting, and retrieving logbook data.

## 6.1 Create Logbook

**POST** `/api/Logbook/create`

### Content-Type

```text
multipart/form-data
```

### Request Body

| Field       | Type               | Required |
| ----------- | ------------------ | -------- |
| `Content`   | string             | Yes      |
| `DateStart` | string (date-time) | Yes      |
| `DateEnd`   | string (date-time) | Yes      |
| `Status`    | string             | Yes      |
| `Deskripsi` | string             | Yes      |
| `Image`     | string (binary)    | No       |

#### Response

**200 — Success**

---

## 6.2 Update Logbook

**PUT** `/api/Logbook/update/{kodeLogbook}`

### Path Parameters

| Parameter     | Type          | Required |
| ------------- | ------------- | -------- |
| `kodeLogbook` | string (UUID) | Yes      |

### Content-Type

```text
multipart/form-data
```

### Request Body

| Field       | Type               | Required |
| ----------- | ------------------ | -------- |
| `Content`   | string             | Yes      |
| `DateStart` | string (date-time) | Yes      |
| `DateEnd`   | string (date-time) | Yes      |
| `Status`    | string             | Yes      |
| `Deskripsi` | string             | Yes      |
| `Image`     | string (binary)    | No       |

#### Response

**200 — Success**

---

## 6.3 Delete Logbook

**DELETE** `/api/Logbook/delete/{kodeLogbook}`

### Path Parameters

| Parameter     | Type          | Required |
| ------------- | ------------- | -------- |
| `kodeLogbook` | string (UUID) | Yes      |

#### Response

**200 — Success**

---

## 6.4 Get All Logbooks

**GET** `/api/Logbook/all`

#### Parameters

No parameters required.

#### Response

**200 — Success**

---

## 6.5 Get My Logbooks

**GET** `/api/Logbook/my-logbooks`

#### Parameters

No parameters required.

#### Response

**200 — Success**

---

## 6.6 Get Logbook by Kode Logbook

**GET** `/api/Logbook/{kodeLogbook}`

### Path Parameters

| Parameter     | Type          | Required |
| ------------- | ------------- | -------- |
| `kodeLogbook` | string (UUID) | Yes      |

#### Response

**200 — Success**

---

## 6.7 Get Logbook by User

**GET** `/api/Logbook/by-user/{username}`

### Path Parameters

| Parameter  | Type   | Required |
| ---------- | ------ | -------- |
| `username` | string | Yes      |

#### Response

**200 — Success**

---

# 7. Money

The **Money** category is available in the Swagger UI, but its endpoints are not displayed or exposed in the provided documentation.

---

# 8. Shared

Endpoints for managing logbook sharing data.

## 8.1 Create Shared

**POST** `/api/Shared/{kodeLogbook}/create`

### Path Parameters

| Parameter     | Type          | Required |
| ------------- | ------------- | -------- |
| `kodeLogbook` | string (UUID) | Yes      |

### Request Body

```json
{
  "idShared": 0,
  "sharedWith": "string",
  "permission": "string"
}
```

#### Response

**200 — Success**

---

## 8.2 Update Shared

**PUT** `/api/Shared/{kodeLogbook}/update/{id}`

### Path Parameters

| Parameter     | Type            | Required |
| ------------- | --------------- | -------- |
| `kodeLogbook` | string (UUID)   | Yes      |
| `id`          | integer (int32) | Yes      |

### Request Body

```json
{
  "idShared": 0,
  "sharedWith": "string",
  "permission": "string"
}
```

#### Response

**200 — Success**

---

## 8.3 Delete Shared

**DELETE** `/api/Shared/{kodeLogbook}/delete/{id}`

### Path Parameters

| Parameter     | Type            | Required |
| ------------- | --------------- | -------- |
| `kodeLogbook` | string (UUID)   | Yes      |
| `id`          | integer (int32) | Yes      |

#### Response

**200 — Success**

---

## 8.4 Get All Shared

**GET** `/api/Shared/{kodeLogbook}/all`

### Path Parameters

| Parameter     | Type          | Required |
| ------------- | ------------- | -------- |
| `kodeLogbook` | string (UUID) | Yes      |

#### Response

**200 — Success**

---

# Endpoint Summary

| No. | Method | Endpoint                                  |
| --: | ------ | ----------------------------------------- |
|   1 | POST   | `/api/Account/register`                   |
|   2 | POST   | `/api/Account/login`                      |
|   3 | POST   | `/api/Account/assign-role`                |
|   4 | GET    | `/api/Admin`                              |
|   5 | POST   | `/api/DetailLogbook/{kodeLogbook}/create` |
|   6 | GET    | `/api/DetailLogbook/{id}`                 |
|   7 | GET    | `/api/DetailLogbook/{kodeLogbook}/all`    |
|   8 | PUT    | `/api/DetailLogbook/{id}/update`          |
|   9 | DELETE | `/api/DetailLogbook/{id}/delete`          |
|  10 | PUT    | `/api/DetailLogbook/{id}/verif`           |
|  11 | GET    | `/api/Dosen`                              |
|  12 | GET    | `/api/Dosen/relation-user`                |
|  13 | GET    | `/api/Dosen/relation`                     |
|  14 | GET    | `/api/Pembimbing`                         |
|  15 | POST   | `/api/Logbook/create`                     |
|  16 | PUT    | `/api/Logbook/update/{kodeLogbook}`       |
|  17 | DELETE | `/api/Logbook/delete/{kodeLogbook}`       |
|  18 | GET    | `/api/Logbook/all`                        |
|  19 | GET    | `/api/Logbook/my-logbooks`                |
|  20 | GET    | `/api/Logbook/{kodeLogbook}`              |
|  21 | GET    | `/api/Logbook/by-user/{username}`         |
|  22 | POST   | `/api/Shared/{kodeLogbook}/create`        |
|  23 | PUT    | `/api/Shared/{kodeLogbook}/update/{id}`   |
|  24 | DELETE | `/api/Shared/{kodeLogbook}/delete/{id}`   |
|  25 | GET    | `/api/Shared/{kodeLogbook}/all`           |


# Backend Installation
## 1. Clone repository

```powershell
git clone https://github.com/wedawesnawa/internconnect-backend.git
cd internconnect-backend
```

Then navigate to the project folder:

```powershell
cd InternconnectBackend
```

## 2. Pastikan software yang dibutuhkan

You need to have the following installed:

* **Visual Studio 2022**

  * ASP.NET and web development workload
* **.NET 8 SDK**
* **SQL Server Express**
* **SQL Server Management Studio (SSMS)**
* Git

Check the installed .NET version:

```powershell
dotnet --version
```

The installed version must support **.NET 8**.

## 3. Buat `appsettings.json`

Copy the example configuration file:

```powershell
Copy-Item appsettings.example.json appsettings.json
```

Then edit the file:

```powershell
notepad appsettings.json
```

Configure the connection string according to your SQL Server instance.

Contoh:

```json
{
  "ConnectionStrings": {
    "InternconnectConnectionString": "Server=localhost\\SQLEXPRESS;Database=InternconnectDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

If your SQL Server instance name is different, adjust it accordingly.

For example:

```text
DESKTOP-ABC123\SQLEXPRESS
```

maka:

```text
Server=DESKTOP-ABC123\\SQLEXPRESS;
```

## 4. Buat database

The repository contains the following migration files:

```text
Migrations/
├── 20250220042630_Initial migration.cs
├── 20250225030130_Update monev tb.cs
├── 20250225140200_Update Logbook.cs
├── 20250226114514_Update User Detail.cs
└── InternconnectDbContextModelSnapshot.cs
```

From the project folder, run:

```powershell
dotnet ef database update
```

This will create:

```text
SQL Server
    ↓
InternconnectDb
    ↓
Semua tabel dari migration
```

If `dotnet ef` is not available:

```powershell
dotnet tool install --global dotnet-ef
```

Then run:

```powershell
dotnet ef database update
```

> Because this project uses EF Core migrations, **do not create the tables manually in SSMS**. Let the migrations create the database structure.

## 5. Restore dependency

This is usually performed automatically during the build, but you can run it explicitly:

```powershell
dotnet restore
```

Then run:

```powershell
dotnet build
```

You should get:

```text
Build succeeded.
0 Warning(s)
0 Error(s)
```

## 6. Jalankan backend

```powershell
dotnet run
```

You should see output similar to:

```text
Now listening on: http://localhost:5244
Application started.
Hosting environment: Development
```

The port may vary depending on the project configuration.

## 7. Buka Swagger

If the backend is running on the current port:

```text
http://localhost:5244/swagger
```

There, you can view the available endpoints:

```text
GET
POST
PUT
DELETE
```

and test the API.

