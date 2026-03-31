
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Wasfaty.Application.Constants;
using Wasfaty.Application.Interfaces;
using Wasfaty.Application.Interfaces.Repositories;
using Wasfaty.Infrastructure.Data;
using Wasfaty.Infrastructure.Repositories;
using Wasfaty.Infrastructure.Repositories.Interfaces;
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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminRole", policy => policy.RequireRole(Roles.Admin));
    options.AddPolicy("DoctorRole", policy => policy.RequireRole(Roles.Doctor));
    options.AddPolicy("PatientRole", policy => policy.RequireRole(Roles.Patient));
    options.AddPolicy("PharmacistRole", policy => policy.RequireRole(Roles.Pharmacist));

    options.AddPolicy("AdminOrDoctorRole", policy =>
       policy.RequireAssertion(context =>
           context.User.IsInRole(Roles.Admin) || context.User.IsInRole(Roles.Doctor)));

});

// ≈÷«›… «·Œœ„« 
builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


// ≈⁄œ«œ Œœ„«  «·„’«œﬁ… »«” Œœ«„ JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
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


builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();


/*builder.Services.Configure<SendGridSettings>(builder.Configuration.GetSection("SendGrid"));
builder.Services.AddTransient<ISendGridEmailService, SendGridEmailService>();
*/
builder.Services.Configure<SendGridSettings>(builder.Configuration.GetSection("SendGrid"));
builder.Services.AddTransient<IEmailService, SendGridEmailService>();
// ≈÷«›… Œœ„«  Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//    db.Database.Migrate();

//    await ApplicationDbSeeder.SeedAsync(scope.ServiceProvider);

//}

//using (var scope = app.Services.CreateScope())
//{
//    try
//    {
//        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

//        //  ÿ»Ìﬁ «·‹ Migrations
//        await db.Database.MigrateAsync();

//        //  ‘€Ì· «·‹ Seeder
//        await ApplicationDbSeeder.SeedAsync(scope.ServiceProvider);

//        Console.WriteLine("Database migration and seeding completed successfully.");
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"An error occurred while migrating/seeding the database: {ex.Message}");
//        throw;
//    }
//}


//using (var scope = app.Services.CreateScope())
//{
//    try
//    {
//        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

//        // ??  Õﬁﬁ Â· ›ÌÂ Migrations ÃœÌœ… ›ﬁÿ
//        if (db.Database.GetPendingMigrations().Any())
//        {
//            await db.Database.MigrateAsync();
//        }

//        // ?? ‘€· Seeder (Ì›÷·  Œ·ÌÂ –ﬂÌ)
//        await ApplicationDbSeeder.SeedAsync(scope.ServiceProvider);

//        Console.WriteLine("Database migration and seeding completed successfully.");
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"An error occurred while migrating/seeding the database: {ex.Message}");
//        // ? ·«  ⁄„· throw Â‰«
//    }
//}

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
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty; // ·⁄—÷ «·Ê«ÃÂ… ⁄·Ï «·Ã–—
    });
    app.MapOpenApi(); //  √ﬂœ „‰ √‰ﬂ  ﬁÊ„ » ‘€Ì· MapOpenApi Â‰«

}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();  //  √ﬂœ „‰ √‰ Swagger Ì⁄„· »‘ﬂ· ’ÕÌÕ

    app.MapOpenApi();
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