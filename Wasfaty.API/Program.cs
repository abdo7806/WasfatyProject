
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi.Models;
//using System.Text;
//using Wasfaty.API.Authorization;
//using Wasfaty.API.Authorization.DispenseRecordAuthorization.Handlers;
//using Wasfaty.API.Authorization.DispenseRecordAuthorization.Requirements;
//using Wasfaty.API.Authorization.DoctorAuthorization.Handlers;
//using Wasfaty.API.Authorization.DoctorAuthorization.Requirements;
//using Wasfaty.API.Authorization.PatientAuthorization.Handlers;
//using Wasfaty.API.Authorization.PatientAuthorization.Requirements;
//using Wasfaty.API.Authorization.PharmacistAuthorization.Handlers;
//using Wasfaty.API.Authorization.PharmacistAuthorization.Requirements;
//using Wasfaty.API.Authorization.PrescriptionAuthorization.Handlers;
//using Wasfaty.API.Authorization.PrescriptionAuthorization.Requirements;
//using Wasfaty.API.Authorization.UserAuthorization.Handlers;
//using Wasfaty.API.Authorization.UserAuthorization.Requirements;
//using Wasfaty.Application.Constants;
//using Wasfaty.Application.Interfaces.IRepositories;
//using Wasfaty.Application.Interfaces.IServices;
//using Wasfaty.Application.Services;
//using Wasfaty.Infrastructure.Data;
//using Wasfaty.Infrastructure.Repositories;
//using Wasfaty.Infrastructure.Seeders;
//using Wasfaty.Infrastructure.Services;
//using Wasfaty.Infrastructure.Services.EmailServices;

//var builder = WebApplication.CreateBuilder(args);
//// إضافة CORS للسماح بالوصول العام
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll",
//        policy =>
//        {
//            policy.AllowAnyOrigin()  // السماح بأي مصدر
//                  .AllowAnyMethod()  // السماح بأي طريقة (GET, POST, PUT, DELETE)
//                  .AllowAnyHeader(); // السماح بأي رأس (headers)
//        });
//});



//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowFrontend",
//        policy =>
//        {
//            policy.WithOrigins(
//                    "https://yourfrontend.com",
//                    "https://admin.yourfrontend.com"
//                  )
//                  .AllowAnyMethod()
//                  .AllowAnyHeader()
//                  .AllowCredentials(); // إذا تستخدم JWT في الكوكيز
//        });
//});

//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("AdminRole", policy => policy.RequireRole(Roles.Admin));
//    options.AddPolicy("DoctorRole", policy => policy.RequireRole(Roles.Doctor));
//    options.AddPolicy("PatientRole", policy => policy.RequireRole(Roles.Patient));
//    options.AddPolicy("PharmacistRole", policy => policy.RequireRole(Roles.Pharmacist));

//    options.AddPolicy("AdminOrDoctorRole", policy =>
//       policy.RequireAssertion(context =>
//           context.User.IsInRole(Roles.Admin) || context.User.IsInRole(Roles.Doctor)));

//    options.AddPolicy("AdminOrPharmacistRole", policy =>
//       policy.RequireAssertion(context =>
//           context.User.IsInRole(Roles.Admin) 
//           || context.User.IsInRole(Roles.Pharmacist)));

//});




//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("CanAccessPrescription", policy =>
//    policy.Requirements.Add(new CanAccessPrescriptionRequirement()));

//    options.AddPolicy("CanEditPrescription", policy =>
//        policy.Requirements.Add(new CanEditPrescriptionRequirement()));

//    options.AddPolicy("CanDispensePrescription", policy =>
//        policy.Requirements.Add(new CanDispensePrescriptionRequirement()));

//    // user

//    options.AddPolicy("CanAccessUser", policy =>
//    policy.Requirements.Add(new CanAccessUserRequirement()));

//    options.AddPolicy("CanEditUser", policy =>
//        policy.Requirements.Add(new CanEditUserRequirement()));

//    // Patient
//    options.AddPolicy("CanAccessPatient", policy =>
//    policy.Requirements.Add(new CanAccessPatientRequirement()));

//    options.AddPolicy("CanEditPatient", policy =>
//        policy.Requirements.Add(new CanEditPatientRequirement()));

//    // Doctor
//    options.AddPolicy("CanAccessDoctor", policy =>
//    policy.Requirements.Add(new CanAccessDoctorRequirement()));

//    options.AddPolicy("CanEditDoctor", policy =>
//        policy.Requirements.Add(new CanEditDoctorRequirement()));

//    // Pharmacist
//    options.AddPolicy("CanAccessPharmacist", policy =>
//    policy.Requirements.Add(new CanAccessPharmacistRequirement()));

//    options.AddPolicy("CanEditPharmacist", policy =>
//        policy.Requirements.Add(new CanEditPharmacistRequirement()));

//    // DispenseRecord
//    options.AddPolicy("CanAccessDispenseRecord",
//    policy => policy.Requirements.Add(new CanAccessDispenseRecordRequirement()));

//    options.AddPolicy("CanEditDispenseRecord",
//        policy => policy.Requirements.Add(new CanEditDispenseRecordRequirement()));
//});

//builder.Services.AddScoped<IAuthorizationHandler, CanAccessPrescriptionHandler>();
//builder.Services.AddScoped<IAuthorizationHandler, CanEditPrescriptionHandler>();
//builder.Services.AddScoped<IAuthorizationHandler, CanDispensePrescriptionHandler>();


//builder.Services.AddScoped<IAuthorizationHandler, CanAccessUserHandler>();
//builder.Services.AddScoped<IAuthorizationHandler, CanEditUserHandler>();

//builder.Services.AddScoped<IAuthorizationHandler, CanAccessPatientHandler>();
//builder.Services.AddScoped<IAuthorizationHandler, CanEditPatientHandler>();

//builder.Services.AddScoped<IAuthorizationHandler, CanAccessDoctorHandler>();
//builder.Services.AddScoped<IAuthorizationHandler, CanEditDoctorHandler>();

//builder.Services.AddScoped<IAuthorizationHandler, CanAccessPharmacistHandler>();
//builder.Services.AddScoped<IAuthorizationHandler, CanEditPharmacistHandler>();

//builder.Services.AddScoped<IAuthorizationHandler, CanAccessDispenseRecordHandler>();
//builder.Services.AddScoped<IAuthorizationHandler, CanEditDispenseRecordHandler>();

//// إضافة الخدمات
//builder.Services.AddControllers();
//// Add services to the container.
//// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();


//// إعداد خدمات المصادقة باستخدام JWT
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {

//        options.RequireHttpsMetadata = true;

//        // إعداد معلمات التحقق من صحة التوكن
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            // تحقق مما إذا كان المُصدر للتوكن صحيحًا
//            ValidateIssuer = true,
//            // تحقق مما إذا كان الجمهور المستهدف للتوكن صحيحًا
//            ValidateAudience = true,
//            // تحقق مما إذا كانت صلاحية التوكن قد انتهت
//            ValidateLifetime = true,
//            // تحقق مما إذا كان مفتاح التوقيع صحيحًا
//            ValidateIssuerSigningKey = true,
//            // الجهة المصدرة للتوكن، يتم الحصول عليها من ملف الإعدادات
//            ValidIssuer = builder.Configuration["Jwt:Issuer"],
//            // الجمهور المستهدف الذي يستخدم التوكن، يتم الحصول عليه من ملف الإعدادات
//            ValidAudience = builder.Configuration["Jwt:Audience"],
//            // المفتاح السري الذي يستخدم لتوقيع التوكن، يتم تحويله إلى مصفوفة بايت
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),

//            ClockSkew = TimeSpan.Zero
//        };
//    });


//// إضافة DbContext الخاص بك مع إعداد الاتصال
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

////// إضافة DbContext الخاص بك مع إعداد الاتصال
////builder.Services.AddDbContext<WasfatyDbContext>(options =>
////    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddScoped<IAuthRepository, AuthRepository>();
//builder.Services.AddScoped<IAuthService, AuthService>();

//builder.Services.AddScoped<IUserRepository, UserRepository>();
//builder.Services.AddScoped<IUserService, UserService>();

//builder.Services.AddScoped<IPatientRepository, PatientRepository>();
//builder.Services.AddScoped<IPatientService, PatientService>();

//builder.Services.AddScoped<IMedicalCenterRepository, MedicalCenterRepository>();
//builder.Services.AddScoped<IMedicalCenterService, MedicalCenterService>();

//builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
//builder.Services.AddScoped<IDoctorService, DoctorService>();


//builder.Services.AddScoped<IPharmacyRepository, PharmacyRepository>();
//builder.Services.AddScoped<IPharmacyService, PharmacyService>();


//builder.Services.AddScoped<IPharmacistRepository, PharmacistRepository>();
//builder.Services.AddScoped<IPharmacistService, PharmacistService>();

//builder.Services.AddScoped<IMedicationRepository, MedicationRepository>();
//builder.Services.AddScoped<IMedicationService, MedicationService>();

//builder.Services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
//builder.Services.AddScoped<IPrescriptionService, PrescriptionService>();


//builder.Services.AddScoped<IPrescriptionItemRepository, PrescriptionItemRepository>();
//builder.Services.AddScoped<IPrescriptionItemService, PrescriptionItemService>();


//builder.Services.AddScoped<IDispenseRecordRepository, DispenseRecordRepository>();
//builder.Services.AddScoped<IDispenseRecordService, DispenseRecordService>();


//builder.Services.AddScoped<IAdminRepository, AdminRepository>();
//builder.Services.AddScoped<IAdminService, AdminService>();

//builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

////builder.Services.AddScoped<ITokenService, TokenService>();
//builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();


//builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
//builder.Services.AddTransient<IEmailService, EmailService>();


//builder.Services.Configure<SendGridSettings>(builder.Configuration.GetSection("SendGrid"));
//builder.Services.AddTransient<IEmailService, SendGridEmailService>();

//// إضافة خدمات Swagger
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(options =>
//{
//    // تعريف Security Scheme
//    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Name = "Authorization",
//        Type = SecuritySchemeType.Http,
//        Scheme = "Bearer",
//        BearerFormat = "JWT",
//        In = ParameterLocation.Header,
//        Description = "Enter JWT Token like this: Bearer {your token}"
//    });

//    // تطبيقه على كل الـ APIs
//    options.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            Array.Empty<string>()
//        }
//    });
//});

//var app = builder.Build();


//using (var scope = app.Services.CreateScope())
//{
//    try
//    {
//        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

//        if ((await db.Database.GetPendingMigrationsAsync()).Any())
//        {
//            await db.Database.MigrateAsync();
//        }

//        await ApplicationDbSeeder.SeedAsync(scope.ServiceProvider);

//        Console.WriteLine("Database migration and seeding completed successfully.");
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"Migration error: {ex.Message}");
//    }
//}

//// تمكين Swagger
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();  // تأكد من أن Swagger يعمل بشكل صحيح

//    app.UseSwaggerUI(c =>
//    {
//        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
//        c.RoutePrefix = string.Empty; // لعرض الواجهة على الجذر
//    });
//    app.MapOpenApi(); // تأكد من أنك تقوم بتشغيل MapOpenApi هنا


//}



//if (!app.Environment.IsDevelopment())
//{
//    app.UseHsts();
//}

//app.UseHttpsRedirection();

//var summaries = new[]
//{
//    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
//};

//app.MapGet("/weatherforecast", () =>
//{
//    var forecast =  Enumerable.Range(1, 5).Select(index =>
//        new WeatherForecast
//        (
//            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//            Random.Shared.Next(-20, 55),
//            summaries[Random.Shared.Next(summaries.Length)]
//        ))
//        .ToArray();
//    return forecast;
//})
//.WithName("GetWeatherForecast");


//// هنا، تأكد من أنك تستخدم CORS أولاً قبل المصادقة والـ Authorization
//app.UseCors("AllowAll");  // تطبيق سياسة CORS

//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllers();
//app.Run();

//record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
//{
//    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
//}





using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Wasfaty.API.Authorization;
using Wasfaty.API.Authorization.DispenseRecordAuthorization.Handlers;
using Wasfaty.API.Authorization.DispenseRecordAuthorization.Requirements;
using Wasfaty.API.Authorization.DoctorAuthorization.Handlers;
using Wasfaty.API.Authorization.DoctorAuthorization.Requirements;
using Wasfaty.API.Authorization.PatientAuthorization.Handlers;
using Wasfaty.API.Authorization.PatientAuthorization.Requirements;
using Wasfaty.API.Authorization.PharmacistAuthorization.Handlers;
using Wasfaty.API.Authorization.PharmacistAuthorization.Requirements;
using Wasfaty.API.Authorization.PrescriptionAuthorization.Handlers;
using Wasfaty.API.Authorization.PrescriptionAuthorization.Requirements;
using Wasfaty.API.Authorization.UserAuthorization.Handlers;
using Wasfaty.API.Authorization.UserAuthorization.Requirements;
using Wasfaty.Application.Constants;
using Wasfaty.Application.Interfaces.IRepositories;
using Wasfaty.Application.Interfaces.IServices;
using Wasfaty.Application.Services;
using Wasfaty.Application.Settings;
using Wasfaty.Infrastructure.Data;
using Wasfaty.Infrastructure.Repositories;
using Wasfaty.Infrastructure.Seeders;
using Wasfaty.Infrastructure.Services;
using Wasfaty.Infrastructure.Services.EmailServices;

var builder = WebApplication.CreateBuilder(args);



// ============= ✅ إعدادات CORS الصحيحة =============
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecific", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",
            "http://localhost:4200", 
            "https://localhost:3000",
            "https://localhost:7219",
            "http://127.0.0.1:5500",    
            "https://localhost:5500",
            "https://localhost:4443",    

                 "null"  // ✅ أضف هذا للملفات المحلية (HTML)

            )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
// ============= 2. إعدادات Cookie Policy =============
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => false;  // لا نحتاج موافقة (GDPR)
    options.MinimumSameSitePolicy = SameSiteMode.Lax;     // ✅ غير إلى Lax
    //options.Secure = CookieSecurePolicy.None;             // ✅ غير إلى None (للتطوير)
    options.Secure = CookieSecurePolicy.Always;
    options.HttpOnly = HttpOnlyPolicy.Always;
});

// ============= 3. باقي خدماتك =============
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminRole", policy => policy.RequireRole(Roles.Admin));
    options.AddPolicy("DoctorRole", policy => policy.RequireRole(Roles.Doctor));
    options.AddPolicy("PatientRole", policy => policy.RequireRole(Roles.Patient));
    options.AddPolicy("PharmacistRole", policy => policy.RequireRole(Roles.Pharmacist));

    options.AddPolicy("AdminOrDoctorRole", policy =>
       policy.RequireAssertion(context =>
           context.User.IsInRole(Roles.Admin) || context.User.IsInRole(Roles.Doctor)));

    options.AddPolicy("AdminOrPharmacistRole", policy =>
       policy.RequireAssertion(context =>
           context.User.IsInRole(Roles.Admin) || context.User.IsInRole(Roles.Pharmacist)));
});

// Authorization Handlers...
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanAccessPrescription", policy =>
        policy.Requirements.Add(new CanAccessPrescriptionRequirement()));

    options.AddPolicy("CanEditPrescription", policy =>
        policy.Requirements.Add(new CanEditPrescriptionRequirement()));

    options.AddPolicy("CanDispensePrescription", policy =>
        policy.Requirements.Add(new CanDispensePrescriptionRequirement()));

    options.AddPolicy("CanAccessUser", policy =>
        policy.Requirements.Add(new CanAccessUserRequirement()));

    options.AddPolicy("CanEditUser", policy =>
        policy.Requirements.Add(new CanEditUserRequirement()));

    options.AddPolicy("CanAccessPatient", policy =>
        policy.Requirements.Add(new CanAccessPatientRequirement()));

    options.AddPolicy("CanEditPatient", policy =>
        policy.Requirements.Add(new CanEditPatientRequirement()));

    options.AddPolicy("CanAccessDoctor", policy =>
        policy.Requirements.Add(new CanAccessDoctorRequirement()));

    options.AddPolicy("CanEditDoctor", policy =>
        policy.Requirements.Add(new CanEditDoctorRequirement()));

    options.AddPolicy("CanAccessPharmacist", policy =>
        policy.Requirements.Add(new CanAccessPharmacistRequirement()));

    options.AddPolicy("CanEditPharmacist", policy =>
        policy.Requirements.Add(new CanEditPharmacistRequirement()));

    options.AddPolicy("CanAccessDispenseRecord", policy =>
        policy.Requirements.Add(new CanAccessDispenseRecordRequirement()));

    options.AddPolicy("CanEditDispenseRecord", policy =>
        policy.Requirements.Add(new CanEditDispenseRecordRequirement()));
});

// Register Handlers...
builder.Services.AddScoped<IAuthorizationHandler, CanAccessPrescriptionHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CanEditPrescriptionHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CanDispensePrescriptionHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CanAccessUserHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CanEditUserHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CanAccessPatientHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CanEditPatientHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CanAccessDoctorHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CanEditDoctorHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CanAccessPharmacistHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CanEditPharmacistHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CanAccessDispenseRecordHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CanEditDispenseRecordHandler>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// ============= 4. إعدادات JWT Authentication =============
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;  // ? إجباري في production

        // ? إعداد مهم لجلب التوكن من QueryString لـ SignalR (اختياري)
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ClockSkew = TimeSpan.FromSeconds(30)  // ? 30 ثانية فقط للتسامح
        };
    });

// ============= 5. Database & Repositories =============
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//  تسجيل الإعدادات من appsettings.json
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<CookieSettings>(builder.Configuration.GetSection("CookieSettings"));

// أو تسجيلها كـ Singleton للاستخدام المباشر
builder.Services.AddSingleton(resolver =>
    resolver.GetRequiredService<IOptions<JwtSettings>>().Value);
builder.Services.AddSingleton(resolver =>
    resolver.GetRequiredService<IOptions<CookieSettings>>().Value);

// Register your services...
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IMedicalCenterRepository, MedicalCenterRepository>();
builder.Services.AddScoped<IMedicalCenterService, MedicalCenterService>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IPharmacyRepository, PharmacyRepository>();
builder.Services.AddScoped<IPharmacyService, PharmacyService>();
builder.Services.AddScoped<IPharmacistRepository, PharmacistRepository>();
builder.Services.AddScoped<IPharmacistService, PharmacistService>();
builder.Services.AddScoped<IMedicationRepository, MedicationRepository>();
builder.Services.AddScoped<IMedicationService, MedicationService>();
builder.Services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
builder.Services.AddScoped<IPrescriptionService, PrescriptionService>();
builder.Services.AddScoped<IPrescriptionItemRepository, PrescriptionItemRepository>();
builder.Services.AddScoped<IPrescriptionItemService, PrescriptionItemService>();
builder.Services.AddScoped<IDispenseRecordRepository, DispenseRecordRepository>();
builder.Services.AddScoped<IDispenseRecordService, DispenseRecordService>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

// Email Services
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.Configure<SendGridSettings>(builder.Configuration.GetSection("SendGrid"));
builder.Services.AddTransient<IEmailService, SendGridEmailService>();

// ============= 6. Swagger =============
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT Token like this: Bearer {your token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ============= 7. Database Migration =============
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if ((await db.Database.GetPendingMigrationsAsync()).Any())
        {
            await db.Database.MigrateAsync();
        }

        await ApplicationDbSeeder.SeedAsync(scope.ServiceProvider);
        Console.WriteLine("Database migration and seeding completed successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Migration error: {ex.Message}");
    }
}

// ============= 8. Middleware Pipeline (الترتيب مهم جداً!) =============

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty;
    });
    app.MapOpenApi();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();



//app.UseCors("AllowFrontend");  // ? استخدم السياسة الصحيحة فقط
app.UseCors("AllowSpecific");// الكل


app.UseCookiePolicy();          // ? مهم جداً للـ Cookies
app.UseAuthentication();        // ? التحقق من JWT
app.UseAuthorization();         // ? التحقق من الصلاحيات

app.MapControllers();

// Weather endpoint (optional)
app.MapGet("/weatherforecast", () =>
{
    var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}