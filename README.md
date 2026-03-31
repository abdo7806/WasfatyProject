
---

# 🏥 Wasfaty | Electronic Medical Prescription Platform
نظام إلكتروني شامل لإدارة الوصفات الطبية باستخدام ASP.NET Core
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
Wasfaty.API/ # واجهة برمجة التطبيقات (API Layer)
Wasfaty.Application/ # المنطق التجاري (Use Cases, Interfaces)
Wasfaty.Domain/ # الكيانات (Entities) والـDomain Rules
Wasfaty.Infrastructure/ # الوصول لقاعدة البيانات وتنفيذ الواجهات
Wasfaty.Tests/ # اختبارات (اختياري)
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
* Modular architecture with separated Back-End and Front-End repositories
* بنية معيارية بفصل الواجهة الخلفية والواجهة الأمامية في مستودعين منفصلين
---

## 🔗 Front-End Repository | مستودع الواجهة الأمامية

تم بناء الواجهة الأمامية بشكل منفصل:

➡️ [Wasfaty Front-End GitHub Repo](https://github.com/abdo7806/WasfatyProject_front-end.git)

---
## 📸 Swagger API Screenshots | لقطات شاشة لـ Swagger

![Auth Endpoints](https://github.com/abdo7806/WasfatyProject/blob/master/Screenshot/AuthEndpoints.png?raw=true)  
![Doctor Endpoints](https://github.com/abdo7806/WasfatyProject/blob/master/Screenshot/DoctorEndpoints.png?raw=true)  
![Patient Endpoints](https://github.com/abdo7806/WasfatyProject/blob/master/Screenshot/PatientEndpoints.png?raw=true)  
![MedicalCenter Endpoints](https://github.com/abdo7806/WasfatyProject/blob/master/Screenshot/MedicalCenterEndpoints.png?raw=true)  
![Pharmacist Endpoints](https://github.com/abdo7806/WasfatyProject/blob/master/Screenshot/PharmacistEndpoints.png?raw=true)  
![Pharmacy Endpoints](https://github.com/abdo7806/WasfatyProject/blob/master/Screenshot/PharmacyEndpoints.png?raw=true)  
![Prescription Endpoints](https://github.com/abdo7806/WasfatyProject/blob/master/Screenshot/PrescriptionEndpoints.png?raw=true)  
![PrescriptionItem Endpoints](https://github.com/abdo7806/WasfatyProject/blob/master/Screenshot/PrescriptionItemEndpoints.png?raw=true)  
![Medication Endpoints](https://github.com/abdo7806/WasfatyProject/blob/master/Screenshot/MedicationEndpoint.png?raw=true)  
![Dispense Records Endpoints](https://github.com/abdo7806/WasfatyProject/blob/master/Screenshot/DispenseRecordEndpoint.png?raw=true)
![User Endpoints](https://github.com/abdo7806/WasfatyProject/blob/master/Screenshot/UserEndpoints.png?raw=true)

---

## 📡 API Endpoints | نقاط نهاية الـ API

### Authentication | المصادقة

* `POST /api/auth/login` – Login
* `POST /api/auth/register` – Register (role = Patient by default)

### Users Management | إدارة المستخدمين (Admin only)

* CRUD على المستخدمين والأدوار

### Prescriptions | الوصفات الطبية

* CRUD للوصفات حسب الصلاحيات


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

