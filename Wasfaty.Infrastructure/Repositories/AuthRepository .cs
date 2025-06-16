using Microsoft.EntityFrameworkCore;
using Wasfaty.Application.DTOs.Patients;
using Wasfaty.Infrastructure.Data;

public class AuthRepository  : IAuthRepository
{
    private readonly ApplicationDbContext _context;

    public AuthRepository (ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)// ارجاع المستخدم حسب الايمال
    {
        return await _context.Users.Include(mc => mc.Role).FirstOrDefaultAsync(u => u.Email == email);
    }

public async Task<User> CreateAsync(User user)
{
    using (var transaction = await _context.Database.BeginTransactionAsync())
    {
        try
        {
            // أضف المستخدم واحفظ التغييرات للحصول على الـ Id
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // أنشئ المريض بعد التأكد من وجود user.Id
            Patient patient = new Patient
            {
                UserId = user.Id
            };
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return user;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine(ex.Message);
            return null;
        }
    }
}

    public async Task<bool> ChangeUserPassword(User user)
    {

        try
        {
            _context.Users.Update(user);

            await _context.SaveChangesAsync();
            return true;


        }
        catch (Exception ex)
        {
            return false;
        }



    }


}
