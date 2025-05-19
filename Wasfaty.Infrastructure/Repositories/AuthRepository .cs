using Microsoft.EntityFrameworkCore;

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

    public async Task<User> CreateAsync(User user)// انشاء مستخدم
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
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
