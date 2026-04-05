<div align="center">

# 🏥 Wasfaty API | وصفتي API
### Electronic Medical Prescription Platform (Back-End)
### المنصة الإلكترونية لإدارة الوصفات الطبية (الواجهة الخلفية)

[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com)
[![Entity Framework](https://img.shields.io/badge/Entity_Framework_Core-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://docs.microsoft.com/en-us/ef/)
[![SQL Server](https://img.shields.io/badge/SQL_Server-2019-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)](https://www.microsoft.com/en-us/sql-server)
[![JWT](https://img.shields.io/badge/JWT-Authentication-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white)](https://jwt.io)
[![Docker](https://img.shields.io/badge/Docker-Containerized-2496ED?style=for-the-badge&logo=docker&logoColor=white)](https://docker.com)
[![Swagger](https://img.shields.io/badge/Swagger-API_Docs-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)](https://swagger.io)

</div>

---

## 📖 Overview | نظرة عامة

| English | العربية |
| :--- | :--- |
| **Wasfaty API** is the back-end service for the electronic prescription management system. Built with **ASP.NET Core Web API** following **Clean Architecture** principles, it provides secure RESTful APIs for Web and Mobile clients. | **واجهة Wasfaty الخلفية** هي الخدمة الخلفية لنظام إدارة الوصفات الطبية الإلكتروني. تم بناؤها باستخدام **ASP.NET Core Web API** باتباع مبادئ **Clean Architecture**، وتوفر واجهات برمجية آمنة لتطبيقات الويب والموبايل. |

---

## ✨ Key Features | المميزات الرئيسية

| Feature | الميزة |
| :--- | :--- |
| 🔐 **JWT Authentication** | 🔐 **مصادقة JWT** - حماية جميع نقاط النهاية |
| 👥 **Role-Based Access Control** | 👥 **صلاحيات متعددة** - Admin, Doctor, Pharmacist, Patient |
| 🗄 **Full CRUD Operations** | 🗄 **عمليات CRUD كاملة** - لجميع الكيانات |
| 🧱 **Clean Architecture** | 🧱 **نظافة البنية** - فصل كامل بين الطبقات |
| 🐳 **Docker Support** | 🐳 **دعم Docker** - تشغيل سهل بضغطة زر |
| 📄 **Swagger Documentation** | 📄 **توثيق Swagger** - اختبار الـ API بسهولة |
| ⚡ **Auto Migration & Seeding** | ⚡ **ترحيل تلقائي** - إنشاء قاعدة البيانات تلقائياً |

---

## 🛠 Tech Stack | التقنيات المستخدمة

| Layer | Technology |
| :--- | :--- |
| **Framework** | ASP.NET Core 8.0 Web API |
| **ORM** | Entity Framework Core 8.0 |
| **Database** | SQL Server (Dockerized) |
| **Architecture** | Clean Architecture (Domain, Application, Infrastructure, API) |
| **Authentication** | JWT (JSON Web Tokens) |
| **API Documentation** | Swagger / OpenAPI |
| **Containerization** | Docker & Docker Compose |

---

## 👥 User Roles | صلاحيات المستخدمين

| Role | الدور | Permissions | الصلاحيات |
| :--- | :--- | :--- | :--- |
| 🛡 **Admin** | **مدير** | Full system control (CRUD on all entities) | تحكم كامل (إدارة جميع الكيانات) |
| 👨‍⚕️ **Doctor** | **طبيب** | Create prescriptions, manage patients | إنشاء وصفات، إدارة المرضى |
| 💊 **Pharmacist** | **صيدلي** | Dispense prescriptions | صرف الوصفات الطبية |
| 👤 **Patient** | **مريض** | View prescriptions and medical history | عرض الوصفات والسجل الطبي |

---

## 📁 Project Structure | هيكل المشروع

```text
Wasfaty/
│
├── Wasfaty.Domain/                 # 1. Domain Layer | طبقة النطاق
│   ├── Entities/                   # Core business entities (Prescription, Patient, Doctor...)
│   │   ├── Prescription.cs
│   │   ├── Patient.cs
│   │   ├── Doctor.cs
│   │   └── ...
│   ├── Enums/                      # Enumerations (UserRole, PrescriptionStatus...)
│   └── Interfaces/                 # Domain interfaces (IRepository base)
│
├── Wasfaty.Application/            # 2. Application Layer | طبقة التطبيق
│   ├── Services/                   # Business logic & Use Cases (حالات الاستخدام)
│   │   ├── AuthService.cs          # Register, Login, Change Password
│   │   ├── PrescriptionService.cs  # Create, Update, Delete, Get prescriptions
│   │   ├── PatientService.cs       # Patient management business logic
│   │   ├── DoctorService.cs        # Doctor management business logic
│   │   ├── PharmacistService.cs    # Pharmacist management business logic
│   │   ├── MedicationService.cs    # Medication CRUD operations
│   │   ├── MedicalCenterService.cs # Medical center management
│   │   ├── PharmacyService.cs      # Pharmacy management
│   │   ├── DispenseRecordService.cs# Dispense record logic
│   │   └── UserService.cs          # User management
│   │
│   ├── DTOs/                       # Data Transfer Objects
│   │   ├── AuthDTOs.cs
│   │   ├── PrescriptionDTOs.cs
│   │   └── ...
│   │
│   ├── Interfaces/                 # Application interfaces (injected later)
│   │   ├── IAuthService.cs
│   │   ├── IPrescriptionService.cs
│   │   ├── IPatientService.cs
│   │   └── ...
│   │
│   └── Common/                     # Shared utilities (Mappings, Validations)
│
├── Wasfaty.Infrastructure/         # 3. Infrastructure Layer | طبقة البنية التحتية
│   ├── Data/                       # Database Context
│   │   └── ApplicationDbContext.cs
│   │
│   ├── Migrations/                 # EF Core Migrations
│   │
│   ├── Repositories/               # Implementation of domain interfaces
│   │   ├── GenericRepository.cs
│   │   ├── PrescriptionRepository.cs
│   │   └── ...
│   │
│   ├── Services/                   # External services implementation
│   │   ├── EmailService.cs         # Sending emails (SendGrid/SMTP)
│   │   ├── FileService.cs          # File upload/handling
│   │   └── PdfService.cs           # PDF generation
│   │
│   └── Configurations/             # Entity configurations (Fluent API)
│
├── Wasfaty.API/                    # 4. API Layer | طبقة تقديم الخدمات
│   ├── Controllers/                # API Endpoints
│   │   ├── AuthController.cs
│   │   ├── PrescriptionController.cs
│   │   ├── PatientController.cs
│   │   ├── DoctorController.cs
│   │   ├── PharmacistController.cs
│   │   ├── MedicationController.cs
│   │   ├── MedicalCenterController.cs
│   │   ├── PharmacyController.cs
│   │   ├── DispenseRecordController.cs
│   │   ├── PrescriptionItemController.cs
│   │   └── UserController.cs
│   │
│   ├── Middleware/                 # Custom middleware (Error handling, Logging)
│   ├── Extensions/                 # Service extensions (Dependency Injection)
│   ├── Program.cs                  # Entry point & configuration
│   └── appsettings.json            # Configuration settings
│
├── Wasfaty.Tests/                  # (Optional) Unit & Integration Tests
│
├── docker-compose.yml              # Docker orchestration
├── Dockerfile                      # Container configuration
└── Wasfaty.sln                     # Solution file
```

---

## 🚀 How to Run | كيفية التشغيل

### Option 1: Docker (Recommended) | الخيار الأول: Docker (موصى به)

```bash
# 1. Clone the repository
git clone https://github.com/abdo7806/Wasti-Mobile-Project.git
cd Wasti-Mobile-Project

# 2. Run with Docker Compose
docker-compose up --build

# 3. Access the API
# API: http://localhost:5000
# Swagger UI: http://localhost:5000/swagger
```

### Option 2: Manual (.NET CLI) | الخيار الثاني: يدوياً

```bash
# 1. Clone the repository
git clone https://github.com/abdo7806/Wasti-Mobile-Project.git
cd Wasti-Mobile-Project/Wasfaty.API

# 2. Update connection string in appsettings.json

# 3. Run migrations
dotnet ef database update

# 4. Run the API
dotnet run

# 5. Access Swagger
# https://localhost:5001/swagger
```

---

## ⚙️ Environment Configuration | إعدادات البيئة

### Docker (.env file)

```env
DB_PASSWORD=YourStrong!Pass123
JWT_SECRET=your-super-secret-jwt-key-here
JWT_ISSUER=WasfatyAPI
JWT_AUDIENCE=WasfatyClients
```

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=WasfatyDb;User Id=sa;Password=YourStrong!Pass123;TrustServerCertificate=True;"
  },
  "JWT": {
    "Secret": "your-super-secret-jwt-key-here",
    "Issuer": "WasfatyAPI",
    "Audience": "WasfatyClients",
    "ExpiryInMinutes": 60
  }
}
```

---

## 🗄️ Database | قاعدة البيانات

- SQL Server runs inside Docker container
- EF Core Migrations applied automatically on startup
- Database seeded with:
  - Default Roles (Admin, Doctor, Pharmacist, Patient)
  - Default Admin User
    - Email: `admin@wasfaty.com`
    - Password: `Admin@123`

---

## 📡 API Endpoints | نقاط نهاية الـ API

توضح الصور التالية كيف أن النظام يحتوي على توثيق آلي عبر Swagger، حيث يتم عرض جميع Endpoints، وأمثلة على الطلبات والاستجابات المتوقعة.

The following images show that the system includes automatic documentation via Swagger, displaying all endpoints with examples of expected requests and responses.

---

### 1️⃣ Authentication Controller | قسم المصادقة

**Controller:** `AuthController`

| Endpoint | Method | Description | الوصف |
|----------|--------|-------------|------|
| `/api/Auth/register` | POST | Register new user | تسجيل مستخدم جديد |
| `/api/Auth/login` | POST | User login | تسجيل الدخول |
| `/api/Auth/change-password` | POST | Change password | تغيير كلمة المرور |

![Auth Endpoints](https://github.com/abdo7806/Wasti-Mobile-Project/blob/master/Screenshot/AuthEndpoints.png?raw=true)

---

### 2️⃣ Prescription Controller | قسم الوصفات الطبية

**Controller:** `PrescriptionController`

| Endpoint | Method | Description | الوصف |
|----------|--------|-------------|------|
| `/api/Prescription/All` | GET | Get all prescriptions | جلب جميع الوصفات الطبية |
| `/api/Prescription/{id}` | GET | Get prescription by ID | جلب تفاصيل وصفة طبية بناءً على الـ id |
| `/api/Prescription/{id}` | PUT | Update prescription | تحديث معلومات وصفة طبية موجودة |
| `/api/Prescription/{id}` | DELETE | Delete prescription | حذف وصفة طبية |
| `/api/Prescription/CreatePrescription` | POST | Create new prescription | إنشاء وصفة طبية جديدة |
| `/api/Prescription/MarkAsDispensed/{id}` | PUT | Mark prescription as dispensed | تحديث حالة الوصفة إلى "تم صرفها" |
| `/api/Prescription/GetByDoctorId/{doctorId}` | GET | Get prescriptions by doctor | جلب جميع الوصفات المرتبطة بطبيب معين |
| `/api/Prescription/GetByPatientId/{patientId}` | GET | Get prescriptions by patient | جلب جميع الوصفات المرتبطة بمريض معين |
| `/api/Prescription/dashboard` | GET | Get prescription dashboard stats | جلب بيانات لوحة التحكم (إحصائيات) |
| `/api/Prescription/Pending` | GET | Get pending prescriptions | جلب الوصفات التي حالتها "قيد الانتظار" |

![Prescription Endpoints](https://github.com/abdo7806/Wasti-Mobile-Project/blob/master/Screenshot/PrescriptionEndpoints.png?raw=true)

---

### 3️⃣ DispenseRecord Controller | قسم سجلات صرف الوصفات

**Controller:** `DispenseRecordController`

| Endpoint | Method | Description | الوصف |
|----------|--------|-------------|------|
| `/api/DispenseRecord/All` | GET | Get all dispense records | جلب جميع سجلات الصرف |
| `/api/DispenseRecord/{id}` | GET | Get dispense record by ID | جلب تفاصيل سجل الصرف بناءً على الـ id |
| `/api/DispenseRecord/{id}` | PUT | Update dispense record | تحديث سجل الصرف |
| `/api/DispenseRecord/{id}` | DELETE | Delete dispense record | حذف سجل الصرف |
| `/api/DispenseRecord/CreateDispenseRecord` | POST | Create new dispense record | إنشاء سجل صرف جديد |
| `/api/DispenseRecord/GetAllDispenseRecord/{pharmacyId}` | GET | Get dispense records by pharmacy | جلب جميع سجلات الصرف المرتبطة بصيدلية معينة |

![DispenseRecord Endpoints](https://github.com/abdo7806/Wasti-Mobile-Project/blob/master/Screenshot/DispenseRecordEndpoint.png?raw=true)

---

### 4️⃣ Doctor Controller | قسم الأطباء

**Controller:** `DoctorController`

| Endpoint | Method | Description | الوصف |
|----------|--------|-------------|------|
| `/api/Doctor/all` | GET | Get all doctors | جلب قائمة جميع الأطباء |
| `/api/Doctor/{id}` | GET | Get doctor by ID | جلب تفاصيل طبيب معين |
| `/api/Doctor/{id}` | PUT | Update doctor | تحديث معلومات طبيب موجود |
| `/api/Doctor/{id}` | DELETE | Delete doctor | حذف طبيب |
| `/api/Doctor/CreateDoctor` | POST | Create new doctor | إنشاء طبيب جديد |
| `/api/Doctor/GetDoctorByUserId/{userId}` | GET | Get doctor by User ID | جلب معلومات الطبيب المرتبطة بـ userId |
| `/api/Doctor/dashboard/{doctorId}` | GET | Get doctor dashboard stats | جلب بيانات لوحة تحكم الطبيب (إحصائيات) |

![Doctor Endpoints](https://github.com/abdo7806/Wasti-Mobile-Project/blob/master/Screenshot/DoctorEndpoints.png?raw=true)

---

### 5️⃣ Patient Controller | قسم المريض

**Controller:** `PatientController`

| Endpoint | Method | Description | الوصف |
|----------|--------|-------------|------|
| `/api/Patient/All` | GET | Get all patients | جلب قائمة جميع المرضى |
| `/api/Patient/{id}` | GET | Get patient by ID | جلب تفاصيل مريض معين |
| `/api/Patient/{id}` | PUT | Update patient | تحديث معلومات مريض موجود |
| `/api/Patient/{id}` | DELETE | Delete patient | حذف مريض |
| `/api/Patient` | POST | Create new patient | إنشاء مريض جديد |
| `/api/Patient/SearchPatients/{term}` | GET | Search patients by term | البحث عن مرضى بناءً على مصطلح معين |
| `/api/Patient/GetPatientByUserId/{userId}` | GET | Get patient by User ID | جلب معلومات المريض المرتبطة بـ userId |
| `/api/Patient/dashboard/{patientId}` | GET | Get patient dashboard stats | جلب بيانات لوحة تحكم المريض (إحصائيات) |

![Patient Endpoints](https://github.com/abdo7806/Wasti-Mobile-Project/blob/master/Screenshot/PatientEndpoints.png?raw=true)

---

### 6️⃣ Pharmacist Controller | قسم الصيدلي

**Controller:** `PharmacistController`

| Endpoint | Method | Description | الوصف |
|----------|--------|-------------|------|
| `/api/Pharmacist/all` | GET | Get all pharmacists | جلب قائمة جميع الصيادلة |
| `/api/Pharmacist/{id}` | GET | Get pharmacist by ID | جلب تفاصيل صيدلي معين |
| `/api/Pharmacist/{id}` | PUT | Update pharmacist | تحديث معلومات صيدلي موجود |
| `/api/Pharmacist/{id}` | DELETE | Delete pharmacist | حذف صيدلي |
| `/api/Pharmacist/CreatePharmacist` | POST | Create new pharmacist | إنشاء صيدلي جديد |
| `/api/Pharmacist/GetByPharmacyIdAsync/{pharmacyId}` | GET | Get pharmacists by pharmacy | جلب الصيادلة المرتبطين بصيدلية معينة |
| `/api/Pharmacist/GetPharmacistByUserId/{userId}` | GET | Get pharmacist by User ID | جلب معلومات الصيدلي المرتبطة بحساب المستخدم |
| `/api/Pharmacist/stats/{pharmacistId}` | GET | Get pharmacist statistics | جلب إحصائيات الصيدلي (عدد الوصفات المصرفة) |

![Pharmacist Endpoints](https://github.com/abdo7806/Wasti-Mobile-Project/blob/master/Screenshot/PharmacistEndpoints.png?raw=true)

---

### 7️⃣ MedicalCenter Controller | قسم المراكز الطبية

**Controller:** `MedicalCenterController`

| Endpoint | Method | Description | الوصف |
|----------|--------|-------------|------|
| `/api/MedicalCenter/all` | GET | Get all medical centers | جلب قائمة جميع المراكز الطبية |
| `/api/MedicalCenter/{id}` | GET | Get medical center by ID | جلب تفاصيل مركز طبي معين |
| `/api/MedicalCenter/{id}` | PUT | Update medical center | تحديث معلومات مركز طبي موجود |
| `/api/MedicalCenter/{id}` | DELETE | Delete medical center | حذف مركز طبي |
| `/api/MedicalCenter/CreateMedicalCenter` | POST | Create new medical center | إنشاء مركز طبي جديد |

![MedicalCenter Endpoints](https://github.com/abdo7806/Wasti-Mobile-Project/blob/master/Screenshot/MedicalCenterEndpoints.png?raw=true)

---

### 8️⃣ Pharmacy Controller | قسم الصيدليات

**Controller:** `PharmacyController`

| Endpoint | Method | Description | الوصف |
|----------|--------|-------------|------|
| `/api/Pharmacy/All` | GET | Get all pharmacies | جلب قائمة جميع الصيدليات |
| `/api/Pharmacy/{id}` | GET | Get pharmacy by ID | جلب تفاصيل صيدلية معينة |
| `/api/Pharmacy/{id}` | PUT | Update pharmacy | تحديث معلومات صيدلية موجودة |
| `/api/Pharmacy/{id}` | DELETE | Delete pharmacy | حذف صيدلية |
| `/api/Pharmacy/CreatePharmacy` | POST | Create new pharmacy | إنشاء صيدلية جديدة |

![Pharmacy Endpoints](https://github.com/abdo7806/Wasti-Mobile-Project/blob/master/Screenshot/PharmacyEndpoints.png?raw=true)

---

### 9️⃣ Medication Controller | قسم الأدوية

**Controller:** `MedicationController`

| Endpoint | Method | Description | الوصف |
|----------|--------|-------------|------|
| `/api/Medication/All` | GET | Get all medications | جلب قائمة جميع الأدوية |
| `/api/Medication/{id}` | GET | Get medication by ID | جلب تفاصيل دواء معين |
| `/api/Medication/{id}` | PUT | Update medication | تحديث معلومات دواء موجود |
| `/api/Medication/{id}` | DELETE | Delete medication | حذف دواء |
| `/api/Medication/CreateMedication` | POST | Create new medication | إنشاء دواء جديد |
| `/api/Medication/GetMultipleByIds?ids=1,2,3` | GET | Get multiple medications by IDs | جلب عدة أدوية بناءً على قائمة من المعرفات |

![Medication Endpoints](https://github.com/abdo7806/Wasti-Mobile-Project/blob/master/Screenshot/MedicationEndpoint.png?raw=true)

---

### 🔟 PrescriptionItem Controller | قسم عناصر الوصفات الطبية

**Controller:** `PrescriptionItemController`

| Endpoint | Method | Description | الوصف |
|----------|--------|-------------|------|
| `/api/PrescriptionItem/All` | GET | Get all prescription items | جلب جميع عناصر الوصفات الطبية |
| `/api/PrescriptionItem/{id}` | GET | Get prescription item by ID | جلب عنصر وصفة طبية محدد |
| `/api/PrescriptionItem/{id}` | PUT | Update prescription item | تحديث عنصر وصفة طبية موجود |
| `/api/PrescriptionItem/{id}` | DELETE | Delete prescription item | حذف عنصر وصفة طبية |
| `/api/PrescriptionItem/prescription/{prescriptionId}` | GET | Get items by prescription ID | جلب جميع عناصر الوصفة الطبية المرتبطة بـ prescriptionId |
| `/api/PrescriptionItem/CreatePrescriptionItem` | POST | Create new prescription item | إنشاء عنصر وصفة طبية جديد |

![PrescriptionItem Endpoints](https://github.com/abdo7806/Wasti-Mobile-Project/blob/master/Screenshot/PrescriptionItemEndpoints.png?raw=true)

---

### 1️⃣1️⃣ User Controller | قسم المستخدمين

**Controller:** `UserController`

| Endpoint | Method | Description | الوصف |
|----------|--------|-------------|------|
| `/api/User/All` | GET | Get all users | جلب قائمة جميع المستخدمين |
| `/api/User/{id}` | GET | Get user by ID | جلب تفاصيل مستخدم معين |
| `/api/User/{id}` | PUT | Update user | تحديث بيانات مستخدم موجود |
| `/api/User/{id}` | DELETE | Delete user | حذف مستخدم |
| `/api/User` | POST | Create new user | إنشاء مستخدم جديد |

![User Endpoints](https://github.com/abdo7806/Wasti-Mobile-Project/blob/master/Screenshot/UserEndpoints.png?raw=true)

---

## 📊 Summary | ملخص الـ Endpoints

| Controller | عدد الـ Endpoints |
|------------|-------------------|
| Auth | 3 |
| Prescription | 10 |
| DispenseRecord | 6 |
| Doctor | 7 |
| Patient | 8 |
| Pharmacist | 8 |
| MedicalCenter | 5 |
| Pharmacy | 5 |
| Medication | 6 |
| PrescriptionItem | 6 |
| User | 5 |
| **Total** | **69 Endpoints** |

---


## 📸 Swagger API Screenshots | لقطات شاشة Swagger

| Auth Endpoints | Doctor Endpoints | Patient Endpoints |
| :---: | :---: | :---: |
| ![Auth](https://github.com/abdo7806/Wasti-Mobile-Project/blob/master/Screenshot/AuthEndpoints.png?raw=true) | ![Doctor](https://github.com/abdo7806/Wasti-Mobile-Project/blob/master/Screenshot/DoctorEndpoints.png?raw=true) | ![Patient](https://github.com/abdo7806/Wasti-Mobile-Project/blob/master/Screenshot/PatientEndpoints.png?raw=true) |

| Medical Centers | Pharmacist Endpoints | Pharmacy Endpoints |
| :---: | :---: | :---: |
| ![MedicalCenter](https://github.com/abdo7806/Wasti-Mobile-Project/blob/master/Screenshot/MedicalCenterEndpoints.png?raw=true) | ![Pharmacist](https://github.com/abdo7806/Wasti-Mobile-Project/blob/master/Screenshot/PharmacistEndpoints.png?raw=true) | ![Pharmacy](https://github.com/abdo7806/Wasti-Mobile-Project/blob/master/Screenshot/PharmacyEndpoints.png?raw=true) |

| Prescription Endpoints | PrescriptionItem Endpoints | Medication Endpoints |
| :---: | :---: | :---: |
| ![Prescription](https://github.com/abdo7806/Wasti-Mobile-Project/blob/master/Screenshot/PrescriptionEndpoints.png?raw=true) | ![PrescriptionItem](https://github.com/abdo7806/Wasti-Mobile-Project/blob/master/Screenshot/PrescriptionItemEndpoints.png?raw=true) | ![Medication](https://github.com/abdo7806/Wasti-Mobile-Project/blob/master/Screenshot/MedicationEndpoint.png?raw=true) |

| Dispense Records | User Endpoints |
| :---: | :---: |
| ![DispenseRecord](https://github.com/abdo7806/Wasti-Mobile-Project/blob/master/Screenshot/DispenseRecordEndpoint.png?raw=true) | ![User](https://github.com/abdo7806/Wasti-Mobile-Project/blob/master/Screenshot/UserEndpoints.png?raw=true) |

---

## 🔗 Connected Repositories | المشاريع المرتبطة

| Project | Technology | Repository |
| :--- | :--- | :--- |
| **💻 Web Front-End** | HTML5, CSS3, JavaScript, Bootstrap | [github.com/abdo7806/Wasfaty-FrontEnd](https://github.com/abdo7806/WasfatyProject_front-end.git) |
| **📱 Mobile App** | Flutter (Patient Only) | [github.com/abdo7806/Wasfaty-Mobile](https://github.com/abdo7806/Wasti-Mobile-Project.git) |
| **⚙️ Back-End API** | ASP.NET Core Web API | هذا المستودع |

---

## ⚠️ Security Notes | ملاحظات أمنية

| مهمة | الشرح |
| :--- | :--- |
| ❌ **Never commit secrets** | لا ترفع كلمات المرور أو المفاتيح السرية إلى GitHub |
| ✅ **Use Environment Variables** | استخدم متغيرات البيئة أو User Secrets في التطوير |
| ✅ **Use .env with Docker** | استخدم ملف `.env` مع Docker للبيانات الحساسة |
| ✅ **HTTPS in Production** | استخدم HTTPS في بيئة الإنتاج |

---

## 👨‍💻 Developer | المطور

<table align="center">
  <tr>
    <td align="center" width="150">
      <img src="https://github.com/abdo7806.png" width="120" style="border-radius: 50%; border: 2px solid #512BD4;" alt="Abdulsalam AL-Dhahabi"/>
      <br />
      <b>Abdulsalam AL-Dhahabi</b>
     </td>
     <td align="center">
      <p><b>Software Engineer / Full-Stack Developer</b></p>
      <p>Passionate about building scalable backend solutions and clean APIs.<br />مطور شغوف ببناء حلول خلفية قابلة للتوسع وواجهات برمجية نظيفة.</p>
      <p>
        <a href="mailto:balzhaby26@gmail.com"><img src="https://img.shields.io/badge/Email-D14836?style=flat&logo=gmail&logoColor=white" /></a>
        <a href="https://linkedin.com/in/abdulsalam-al-dhahabi-218887312"><img src="https://img.shields.io/badge/LinkedIn-0077B5?style=flat&logo=linkedin&logoColor=white" /></a>
        <a href="https://github.com/abdo7806"><img src="https://img.shields.io/badge/GitHub-181717?style=flat&logo=github&logoColor=white" /></a>
      </p>
     </td>
   </tr>
</table>

---

## 🤝 Contributions | المساهمات

Contributions are welcome! Feel free to:

1. Fork the project
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

المساهمات مرحب بها! يمكنك عمل Fork وفتح Pull Request.

---

## 📃 License | الترخيص

```
Open source for learning and personal use only.
المشروع مفتوح المصدر لأغراض التعلم والاستخدام الشخصي فقط.
```

---

<div align="center">

### ⭐ Don't forget to star the repo! | لا تنسى وضع نجمة للمستودع ⭐

**Built with ❤️ using ASP.NET Core**

</div>
```

---
