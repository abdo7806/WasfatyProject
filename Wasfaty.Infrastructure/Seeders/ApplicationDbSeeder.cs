using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasfaty.Application.DTOs.Users;
using Wasfaty.Application.Interfaces;

namespace Wasfaty.Infrastructure.Seeders
{
    public static class ApplicationDbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            var dbContext = services.GetRequiredService<ApplicationDbContext>();

            try
            {
                // إنشاء الأدوار من الـ Enum
                await SeedRolesAsync(dbContext);

                // إنشاء المستخدم الأدمن
                await SeedAdminUserAsync(dbContext);

                await dbContext.SaveChangesAsync();

                Console.WriteLine("✅ Database seeding completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ An error occurred while seeding the database: {ex.Message}");
                throw;
            }
        }

        private static async Task SeedRolesAsync(ApplicationDbContext dbContext)
        {
            // جلب جميع قيم الـ Enum
            var roleValues = Enum.GetValues(typeof(UserRoleEnum));

            foreach (UserRoleEnum roleValue in roleValues)
            {
                var roleName = roleValue.ToString();

                // التحقق إذا كان الدور موجود بالفعل
                var roleExists = await dbContext.Roles
                    .AnyAsync(r => r.Name == roleName && r.Id == (int)roleValue);

                if (!roleExists)
                {
                    // إنشاء دور جديد
                    var role = new Role
                    {
                        Name = roleName,
                    };

                    await dbContext.Roles.AddAsync(role);
                    Console.WriteLine($"✅ Role '{roleName}' created successfully.");
                }
                else
                {
                    Console.WriteLine($"ℹ️ Role '{roleName}' already exists.");
                }
            }
        }

        private static async Task SeedAdminUserAsync(ApplicationDbContext dbContext)
        {
            const string adminEmail = "admin@wasfaty.com";
            const string adminPassword = "Admin@123456";

            // التحقق إذا كان المستخدم موجود
            var existingUser = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == adminEmail);

            if (existingUser == null)
            {
                // إنشاء المستخدم الأدمن
                var adminUser = new User
                {
                    Email = adminEmail,
                    FullName = "Administrator",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                    RoleId = (int)UserRoleEnum.Admin,
                    CreatedAt = DateTime.UtcNow,
                };

                await dbContext.Users.AddAsync(adminUser);
                Console.WriteLine("Admin user created successfully.");
            }
            else
            {
                Console.WriteLine("Admin user already exists.");
            }
        }
    }
}