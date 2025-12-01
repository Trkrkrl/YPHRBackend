# 📌 Personel Bilgi Sistemi – Backend (.NET 8 Web API)

Bu proje, bir teknik değerlendirme kapsamında geliştirilmiş basit bir **Personel Bilgi Yönetim Sistemi** arka ucu (backend) uygulamasıdır.

- **Backend:** ASP.NET Core 8 Web API  
- **ORM:** Entity Framework Core  
- **Database:** SQLite (Code-First + ilk çalıştırmada otomatik migration)  

> Not: Frontend (React tabanlı) ayrı bir proje olarak geliştirilecektir ve bu repoda yer almamaktadır.


## 🚀 Gereksinimler

Projeyi çalıştırmak için aşağıdakilere ihtiyacınız vardır:

- .NET 8 SDK
- Aşağıdakilerden biri:
  - Visual Studio 2022 veya Visual Studio Code + C# Dev Kit
- Windows, macOS veya Linux

---

## 📂 Proje Yapısı

- `Core`
- `Entities`
- `DataAccess`
- `Business`
- `WebAPI`

## ▶️ Backend Çalıştırma

### 1) WebAPI'yi başlangıç projesi olarak seçin

### 2) WebAPI'yi çalıştırın.

### 3) Swagger  localhost:5001 portunda açılacaktır.

### 4) Eğer ilk çalıştırmada SSL sorarsa → Evet seçin.


# **📦 4 —  (Database & Endpointler)**  

## 🗄️ Veritabanı

Varsayılan: SQLite  
Otomatik `dbContext.Database.Migrate()` ile migration uygulanır.

---

## 📡 API Endpointleri

### Department:
- GET /api/Department
- GET /api/Department/{id}
- POST /api/Department
- PUT /api/Department
- DELETE /api/Department/{id}

### Title:
- GET /api/Title
- GET /api/Title/{id}
- POST /api/Title
- PUT /api/Title
- DELETE /api/Title

### Employee:
- GET /api/Employee
- GET /api/Employee/{id}
- POST /api/Employee
- POST /api/Employee/uploadImage/{employeeId}
- PUT /api/Employee
- DELETE /api/Employee/{id}

### Auth 
- POST /api/auth/register
- POST /api/auth/loginwithmail


## 5 - Testler

Temel fonksiyonlar için unit testler hazırlanmıştır.

Visual studio'da Tests>RunAllTest ile mevcut testler çalıştırılabilir.






