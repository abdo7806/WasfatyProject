
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
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
using Wasfaty.API.Configurations;
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
using Serilog;
using Serilog.Events;
using Wasfaty.API.Middlewares;

// Bootstrap Logger
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

// إضافة Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName));


// =============  إعدادات CORS الصحيحة =============
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
            "http://localhost:57223",
            "https://localhost:57223",
            "https://localhost:4443"

                 //"null"  //  أضف هذا للملفات المحلية (HTML)

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
    options.MinimumSameSitePolicy = SameSiteMode.Lax;     //  غير إلى Lax
    //options.Secure = CookieSecurePolicy.None;             //  غير إلى None (للتطوير)
    options.Secure = CookieSecurePolicy.Always;
    options.HttpOnly = HttpOnlyPolicy.Always;
});

// ============= 3. باقي خدماتك =============
builder.Services.AddAuthorization(options =>
{
    // Role-based policies
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


    // Ownership policies

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

// تسجيل RateLimitingSettings ليتم استخدامه في الـ OnRejected
builder.Services.Configure<RateLimitingSettings>(builder.Configuration.GetSection("RateLimiting"));
builder.Services.AddSingleton(resolver =>
    resolver.GetRequiredService<IOptions<RateLimitingSettings>>().Value);

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

// ============= 5. Rate Limiting (ممتاز) =============

// تحميل الإعدادات من appsettings.json
var rateLimitingSettings = builder.Configuration.GetSection("RateLimiting").Get<RateLimitingSettings>()
    ?? new RateLimitingSettings(); // قيم افتراضية إذا لم توجد الإعدادات


builder.Services.AddRateLimiter(options =>
{

    // ---------------------- 1. سياسة عامة لجميع الـ endpoints ----------------------
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
        httpContext =>
        {
            var key = httpContext.User.Identity?.IsAuthenticated == true
               ? httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty
               : httpContext.Connection.RemoteIpAddress?.ToString()
                 ?? "anonymous";

            return RateLimitPartition.GetFixedWindowLimiter(
                     partitionKey: key,
                     factory: _ => new FixedWindowRateLimiterOptions
                     {
                         PermitLimit = rateLimitingSettings.Global.PermitLimit,
                         Window = TimeSpan.FromMinutes(rateLimitingSettings.Global.WindowMinutes),
                         QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                         QueueLimit = rateLimitingSettings.Global.QueueLimit
                     }) ;
        });

    // ---------------------- 2. سياسة شديدة لـ Auth ----------------------
    options.AddPolicy("StrictAuthPolicy", httpContext =>
    {
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = rateLimitingSettings.StrictAuth.PermitLimit,
            Window = TimeSpan.FromMinutes(rateLimitingSettings.StrictAuth.WindowMinutes),
            QueueLimit = rateLimitingSettings.StrictAuth.QueueLimit// ? لا نسمح بالانتظار لطلبات Auth الفاشلة
        });
    });

    // ---------------------- 3. سياسة متوسطة للعمليات (Create/Update/Delete) ----------------------
    options.AddPolicy("WriteOperationsPolicy", httpContext =>
    {
        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
        return RateLimitPartition.GetFixedWindowLimiter(userId, _ => new FixedWindowRateLimiterOptions
        {
            //PermitLimit = 50,
            //Window = TimeSpan.FromMinutes(1)


            PermitLimit = rateLimitingSettings.WriteOperations.PermitLimit,
            Window = TimeSpan.FromMinutes(rateLimitingSettings.WriteOperations.WindowMinutes),
            QueueLimit = rateLimitingSettings.WriteOperations.QueueLimit
        });
    });

    // ---------------------- 4. سياسة للـ Polling (مثل /New/{id}) ----------------------
    options.AddPolicy("PollingPolicy", httpContext =>
    {
        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
        return RateLimitPartition.GetSlidingWindowLimiter(userId, _ => new SlidingWindowRateLimiterOptions
        {
            //PermitLimit = 15,
            //Window = TimeSpan.FromMinutes(1),
            //SegmentsPerWindow = 4  // كل 15 ثانية

            PermitLimit = rateLimitingSettings.Polling.PermitLimit,
            Window = TimeSpan.FromMinutes(rateLimitingSettings.Polling.WindowMinutes),
            SegmentsPerWindow = rateLimitingSettings.Polling.SegmentsPerWindow,// تقسيم النافذة إلى 4 أجزاء (كل 15 ثانية)
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0

        });
    });

    // ---------------------- 5. سياسة للـ Dashboard (أقل قليلاً) ----------------------
    options.AddPolicy("DashboardPolicy", httpContext =>
    {
        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
        return RateLimitPartition.GetFixedWindowLimiter(userId, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = rateLimitingSettings.Dashboard.PermitLimit,
            Window = TimeSpan.FromMinutes(rateLimitingSettings.Dashboard.WindowMinutes),
            QueueLimit = rateLimitingSettings.Dashboard.QueueLimit
        });
    });

    // ---------------------- 6. سياسة لـ GetMultipleByIds ----------------------
    options.AddPolicy("MultipleGetPolicy", httpContext =>
    {
        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
        return RateLimitPartition.GetConcurrencyLimiter(userId, _ => new ConcurrencyLimiterOptions
        {

            PermitLimit = rateLimitingSettings.MultipleGet.PermitLimit,// ? 5 طلبات فقط تتزامن في نفس الوقت
            QueueLimit = rateLimitingSettings.MultipleGet.QueueLimit
        });
    });


    // ---------------------- 7. تنسيق رسالة الرفض مع Logging و Headers ----------------------
    options.OnRejected = async (context, token) =>
    {
        // إعداد الـ Response الأساسية
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";

        // ✅ Logging - تسجيل محاولة التجاوز
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();

        var userId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
        var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var path = context.HttpContext.Request.Path;
        var method = context.HttpContext.Request.Method;

        logger.LogWarning(
            "🚨 Rate limit exceeded | UserId: {UserId} | IP: {IP} | {Method} {Path}",
            userId, ip, method, path);

        // ✅ استخراج RetryAfter من Lease Metadata
        double retryAfterSeconds = 60; // قيمة افتراضية

        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfter))
        {
            retryAfterSeconds = Math.Ceiling(retryAfter.TotalSeconds);
        }

        // ✅ إضافة Headers للـ Frontend
        var resetTime = DateTimeOffset.UtcNow.AddSeconds(retryAfterSeconds);

        context.HttpContext.Response.Headers.Append("X-RateLimit-Remaining", "0");
        context.HttpContext.Response.Headers.Append("X-RateLimit-Reset",
            resetTime.ToUnixTimeSeconds().ToString());
        context.HttpContext.Response.Headers.Append("Retry-After",
            retryAfterSeconds.ToString(NumberFormatInfo.InvariantInfo));

        // ✅ جلب الإعدادات من DI Container
        var settings = context.HttpContext.RequestServices.GetRequiredService<RateLimitingSettings>();

        // ✅ تحديد الـ Limit حسب الـ Path
        var currentPath = context.HttpContext.Request.Path.ToString();
        int limit;

        if (currentPath.Contains("dashboard", StringComparison.OrdinalIgnoreCase))
        {
            limit = settings.Dashboard.PermitLimit;
        }
        else if (currentPath.Contains("Auth", StringComparison.OrdinalIgnoreCase))
        {
            limit = settings.StrictAuth.PermitLimit;
        }
        else if (currentPath.Contains("Polling", StringComparison.OrdinalIgnoreCase))
        {
            limit = settings.Polling.PermitLimit;
        }
        else if (currentPath.Contains("Write", StringComparison.OrdinalIgnoreCase))
        {
            limit = settings.WriteOperations.PermitLimit;
        }
        else
        {
            limit = settings.Global.PermitLimit;
        }

        context.HttpContext.Response.Headers.Append("X-RateLimit-Limit", limit.ToString());

        // ✅ إرسال الـ Response Body
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            status = 429,
            title = "Too Many Requests",
            detail = $"لقد تجاوزت الحد المسموح من الطلبات. يرجى المحاولة بعد {retryAfterSeconds} ثانية.",
            retryAfter = retryAfterSeconds
        }, token);
    };
});

// ============= 6. Database & Repositories =============
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
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IAuditService, AuditService>();

builder.Services.AddHttpContextAccessor();


// Email Services
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.Configure<SendGridSettings>(builder.Configuration.GetSection("SendGrid"));
builder.Services.AddTransient<IEmailService, SendGridEmailService>();

// ============= 7. Swagger =============
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


// ============= 8. Database Migration =============
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
// إضافة الـ Middlewares (الترتيب مهم جداً!)
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();

// ============= 9. Middleware Pipeline (الترتيب مهم جداً!) =============

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
app.UseRateLimiter();  
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

// إضافة Request Logging
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    options.GetLevel = (httpContext, elapsed, ex) =>
    {
        if (ex != null) return LogEventLevel.Error;
        if (httpContext.Response.StatusCode >= 500) return LogEventLevel.Error;
        if (httpContext.Response.StatusCode >= 400) return LogEventLevel.Warning;
        return LogEventLevel.Information;
    };
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

