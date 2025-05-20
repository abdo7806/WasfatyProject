
---

# 🏥 Wasfaty | منصة وصفتي الطبية الإلكترونية

An electronic prescription management system built with ASP.NET Core Web API, enabling doctors to issue prescriptions, patients to view their medical records, and pharmacists to dispense medications securely.
النظام يدير الوصفات الطبية إلكترونيًا، مع صلاحيات متعددة: مسؤول، طبيب، صيدلي، ومريض.

---

## 🔧 Technologies Used | التقنيات المستخدمة

### 🖥 Back-End:

* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* JWT Authentication
* Clean Architecture
* Swagger for API Documentation

---

## 👥 User Roles | صلاحيات المستخدمين

### 🛡 Admin | المسؤول:

* Manage doctors, pharmacists, pharmacies, medical centers, medications, users, and patients (CRUD on all)
* إدارة الأطباء، الصيادلة، الصيدليات، المراكز الطبية، الأدوية، المستخدمين، والمرضى (إنشاء، تعديل، حذف، عرض)

### 👨‍⚕️ Doctor | الطبيب:

* Add prescriptions
* Manage assigned patients
* View own prescriptions
* إضافة وصفات طبية، إدارة المرضى التابعين له، عرض وصفاته

### 💊 Pharmacist | الصيدلي:

* Dispense prescriptions
* صرف الوصفات الطبية

### 👤 Patient | المريض:

* View personal medical record and prescriptions
* عرض السجل الطبي والوصفات الخاصة به

---

## 📁 Project Structure | هيكل المشروع

```
Wasfaty.API/
├── Controllers/           # API controllers  
├── Domain/                # Domain entities  
├── Application/           # Business logic and services  
├── Infrastructure/        # Data access and DB context  
├── Program.cs             # Entry point  
└── appsettings.json       # Configuration (DB connection, JWT, etc.)
```

---

## 🚀 How to Run | كيفية التشغيل

1. Import the SQL Server database if not already done
2. Set your connection string in `appsettings.json`
3. Run the project via Visual Studio or use `dotnet run`
4. Access Swagger UI at `https://localhost:5001/swagger/index.html` to test APIs

---

## 🌟 Features | المميزات

* Secure login and registration (default role: Patient)
* Role-based access control with JWT
* Full CRUD operations for Admin role
* Swagger integrated for API testing and documentation
* Separate Front-End repository linked (under development)

---

## 📸 Swagger Screenshots | لقطات من واجهة Swagger

*(قم بإضافة صور هنا مع روابط مباشرة من مستودع الصور الخاص بك)*

---

## 📡 API Endpoints | نقاط نهاية الـ API

### Authentication | المصادقة

* `POST /api/auth/login` – Login
* `POST /api/auth/register` – Register (role = Patient by default)

### Users Management | إدارة المستخدمين (Admin only)

* CRUD على المستخدمين والأدوار

### Prescriptions | الوصفات الطبية

* CRUD للوصفات حسب الصلاحيات

*(تفصيل باقي الـ Endpoints حسب الحاجة)*

---

## 👨‍💻 Developer | المطور

عبدالسلام الضهابي | Abdulsalam Dhahabi

* GitHub: [github.com/abdo7806](https://github.com/abdo7806)
* Email: [balzhaby26@gmail.com](mailto:balzhaby26@gmail.com)
* LinkedIn: [linkedin.com/in/abdulsalam-al-dhahabi-218887312](https://linkedin.com/in/abdulsalam-al-dhahabi-218887312)

---

## 🤝 Contributions | المساهمات

المساهمات مرحب بها! يمكنك عمل Fork وفتح Issues أو Pull Requests.
Contributions are welcome! Feel free to fork, open issues, or submit pull requests.

---

## 📃 License | الترخيص

Open source for learning and personal use only.
المشروع مفتوح المصدر لأغراض التعلم والاستخدام الشخصي فقط.

---

