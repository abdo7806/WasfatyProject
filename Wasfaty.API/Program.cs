
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
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
using Wasfaty.Infrastructure.Data;
using Wasfaty.Infrastructure.Repositories;
using Wasfaty.Infrastructure.Seeders;
using Wasfaty.Infrastructure.Services;
using Wasfaty.Infrastructure.Services.EmailServices;

var builder = WebApplication.CreateBuilder(args);
// ≈÷«›… CORS ··”„«Õ »«·Ê’Ê· «·⁄«„
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()  // «·”„«Õ »√Ì „’œ—
                  .AllowAnyMethod()  // «·”„«Õ »√Ì ÿ—Ìﬁ… (GET, POST, PUT, DELETE)
                  .AllowAnyHeader(); // «·”„«Õ »√Ì —√” (headers)
        });
});



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins(
                    "https://yourfrontend.com",
                    "https://admin.yourfrontend.com"
                  )
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials(); // ≈–«  ” Œœ„ JWT ›Ì «·ﬂÊﬂÌ“
        });
});

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
           context.User.IsInRole(Roles.Admin) 
           || context.User.IsInRole(Roles.Pharmacist)));

});




builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanAccessPrescription", policy =>
    policy.Requirements.Add(new CanAccessPrescriptionRequirement()));

    options.AddPolicy("CanEditPrescription", policy =>
        policy.Requirements.Add(new CanEditPrescriptionRequirement()));

    options.AddPolicy("CanDispensePrescription", policy =>
        policy.Requirements.Add(new CanDispensePrescriptionRequirement()));

    // user

    options.AddPolicy("CanAccessUser", policy =>
    policy.Requirements.Add(new CanAccessUserRequirement()));

    options.AddPolicy("CanEditUser", policy =>
        policy.Requirements.Add(new CanEditUserRequirement()));

    // Patient
    options.AddPolicy("CanAccessPatient", policy =>
    policy.Requirements.Add(new CanAccessPatientRequirement()));

    options.AddPolicy("CanEditPatient", policy =>
        policy.Requirements.Add(new CanEditPatientRequirement()));

    // Doctor
    options.AddPolicy("CanAccessDoctor", policy =>
    policy.Requirements.Add(new CanAccessDoctorRequirement()));

    options.AddPolicy("CanEditDoctor", policy =>
        policy.Requirements.Add(new CanEditDoctorRequirement()));

    // Pharmacist
    options.AddPolicy("CanAccessPharmacist", policy =>
    policy.Requirements.Add(new CanAccessPharmacistRequirement()));

    options.AddPolicy("CanEditPharmacist", policy =>
        policy.Requirements.Add(new CanEditPharmacistRequirement()));

    // DispenseRecord
    options.AddPolicy("CanAccessDispenseRecord",
    policy => policy.Requirements.Add(new CanAccessDispenseRecordRequirement()));

    options.AddPolicy("CanEditDispenseRecord",
        policy => policy.Requirements.Add(new CanEditDispenseRecordRequirement()));
});

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

// ≈÷«›… «·Œœ„« 
builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


// ≈⁄œ«œ Œœ„«  «·„’«œﬁ… »«” Œœ«„ JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        
        options.RequireHttpsMetadata = true;

        // ≈⁄œ«œ „⁄·„«  «· Õﬁﬁ „‰ ’Õ… «· Êﬂ‰
        options.TokenValidationParameters = new TokenValidationParameters
        {
            //  Õﬁﬁ „„« ≈–« ﬂ«‰ «·„ı’œ— ·· Êﬂ‰ ’ÕÌÕ«
            ValidateIssuer = true,
            //  Õﬁﬁ „„« ≈–« ﬂ«‰ «·Ã„ÂÊ— «·„” Âœ› ·· Êﬂ‰ ’ÕÌÕ«
            ValidateAudience = true,
            //  Õﬁﬁ „„« ≈–« ﬂ«‰  ’·«ÕÌ… «· Êﬂ‰ ﬁœ «‰ Â 
            ValidateLifetime = true,
            //  Õﬁﬁ „„« ≈–« ﬂ«‰ „› «Õ «· ÊﬁÌ⁄ ’ÕÌÕ«
            ValidateIssuerSigningKey = true,
            // «·ÃÂ… «·„’œ—… ·· Êﬂ‰° Ì „ «·Õ’Ê· ⁄·ÌÂ« „‰ „·› «·≈⁄œ«œ« 
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            // «·Ã„ÂÊ— «·„” Âœ› «·–Ì Ì” Œœ„ «· Êﬂ‰° Ì „ «·Õ’Ê· ⁄·ÌÂ „‰ „·› «·≈⁄œ«œ« 
            ValidAudience = builder.Configuration["Jwt:Audience"],
            // «·„› «Õ «·”—Ì «·–Ì Ì” Œœ„ · ÊﬁÌ⁄ «· Êﬂ‰° Ì „  ÕÊÌ·Â ≈·Ï „’›Ê›… »«Ì 
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),

            ClockSkew = TimeSpan.Zero
        };
    });


// ≈÷«›… DbContext «·Œ«’ »ﬂ „⁄ ≈⁄œ«œ «·« ’«·
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//// ≈÷«›… DbContext «·Œ«’ »ﬂ „⁄ ≈⁄œ«œ «·« ’«·
//builder.Services.AddDbContext<WasfatyDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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


builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();


builder.Services.Configure<SendGridSettings>(builder.Configuration.GetSection("SendGrid"));
builder.Services.AddTransient<IEmailService, SendGridEmailService>();

// ≈÷«›… Œœ„«  Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    //  ⁄—Ì› Security Scheme
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT Token like this: Bearer {your token}"
    });

    //  ÿ»ÌﬁÂ ⁄·Ï ﬂ· «·‹ APIs
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

//  „ﬂÌ‰ Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();  //  √ﬂœ „‰ √‰ Swagger Ì⁄„· »‘ﬂ· ’ÕÌÕ

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty; // ·⁄—÷ «·Ê«ÃÂ… ⁄·Ï «·Ã–—
    });
    app.MapOpenApi(); //  √ﬂœ „‰ √‰ﬂ  ﬁÊ„ » ‘€Ì· MapOpenApi Â‰«


}



if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
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


// Â‰«°  √ﬂœ „‰ √‰ﬂ  ” Œœ„ CORS √Ê·« ﬁ»· «·„’«œﬁ… Ê«·‹ Authorization
app.UseCors("AllowAll");  //  ÿ»Ìﬁ ”Ì«”… CORS

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}