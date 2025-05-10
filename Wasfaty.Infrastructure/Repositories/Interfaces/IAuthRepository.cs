public interface IAuthRepository
{
    Task<User?> GetByEmailAsync(string email);// ارجاع المستخدم حسب الايمال
    Task<User> CreateAsync(User user);// انشاء مستخدم
}
