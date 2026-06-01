using System.Security.Claims;
using System.Text;

using Wasfaty.Application.DTOs.Auth;
using Wasfaty.Application.DTOs.Patients;
using Wasfaty.Application.DTOs.Users;
using Wasfaty.Application.Interfaces.IServices;
using Wasfaty.Application.Interfaces.IRepositories;
using Microsoft.Extensions.Configuration;
using BCrypt.Net;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Wasfaty.Domain.Entities.Accounts;
using System.Security.Cryptography;
using Wasfaty.Application.Settings;
using Microsoft.Extensions.Options;

namespace Wasfaty.Application.Services;
public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepo;

    private readonly IOptions<JwtSettings> _jwtSettings;  
    private readonly IOptions<CookieSettings> _cookieSettings;  
    public AuthService(
        IAuthRepository authRepository, 
        IConfiguration configuration, 
        IUserRepository userRepository, 
        IRefreshTokenRepository refreshTokenRepo,
          IOptions<JwtSettings> jwtSettings,     
        IOptions<CookieSettings> cookieSettings)
    {
        _authRepository = authRepository;
        _configuration = configuration;
        _userRepository = userRepository;
        _refreshTokenRepo = refreshTokenRepo;
        _jwtSettings = jwtSettings;
        _cookieSettings = cookieSettings;
    }

    //  5. تغيير كلمة المرور
    public async Task<bool> ChangeUserPassword(int userId, string currentPassword, string newPassword)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null || !VerifyPassword(user, currentPassword))
            return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        return await _authRepository.ChangeUserPassword(user);
    }

 
    //  1. تسجيل الدخول - ترجع الـ Refresh Token في Tuple
    public async Task<(LoginResponseDto Result, string RefreshToken)> LoginAsync(
        LoginRequestDto request,
        string deviceInfo = null,
        string ipAddress = null)
    {
        var user = await _authRepository.GetByEmailAsync(request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return (null, null);
        }

        var accessToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        var refreshTokenLifetimeDays = _cookieSettings.Value.RefreshTokenExpirationDays;


        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            Expires = DateTime.UtcNow.AddDays(refreshTokenLifetimeDays), //  من الإعدادات
            UserId = user.Id,
            DeviceInfo = deviceInfo ?? "Unknown Device",
            IpAddress = ipAddress ?? "Unknown IP",
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        await _refreshTokenRepo.CreateAsync(refreshTokenEntity);

        var accessTokenLifetimeMinutes = _jwtSettings.Value.AccessTokenLifetimeInMinutes;


        var result = new LoginResponseDto
        {
            AccessToken = accessToken,
            User = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = (UserRoleEnum)user.RoleId,
            },
            ExpiresIn = accessTokenLifetimeMinutes * 60  // تحويل إلى ثواني
        };

        return (result, refreshToken);
    }

    public async Task<UserDto> RegisterAsync(RegisterUserDto request)
    {
        var existingUser = await _authRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)// اسم المستخدم موجود بالفعل
            return null;

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            RoleId = UserRoleEnum.Patient.GetHashCode(), // افتراضياً يتم تعيين دور المريض، يمكنك تعديل هذا حسب الحاجة
            CreatedAt = DateTime.UtcNow
        };

        var createdUser = await _authRepository.CreateAsync(user);

     
      //  var createPatientDto = await 

        return new UserDto
        {
            Id = createdUser.Id,
            Email = createdUser.Email,
            FullName = createdUser.FullName
        };
    }



    // 2. أضف دالة Refresh جديدة كلية
    public async Task<(RefreshTokenResponseDto Result, string NewRefreshToken)> RefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _refreshTokenRepo.GetByTokenAsync(refreshToken);

        if (storedToken == null || !storedToken.IsActive)
            return (null, null);

        var user = await _userRepository.GetByIdWithRoleAsync(storedToken.UserId);
        if (user == null)
            return (null, null);

        //  إبطال الـ RefreshToken القديم (One-time use)
        storedToken.IsRevoked = true;
        storedToken.RevokedAt = DateTime.UtcNow;
        await _refreshTokenRepo.UpdateAsync(storedToken);

        //  إنشاء Access Token جديد
        var newAccessToken = GenerateJwtToken(user);

        //  إنشاء Refresh Token جديد (Rotation)
        var newRefreshToken = GenerateRefreshToken();


        var refreshTokenLifetimeDays = _cookieSettings.Value.RefreshTokenExpirationDays;


        var newRefreshTokenEntity = new RefreshToken
        {
            Token = newRefreshToken,
            Expires = DateTime.UtcNow.AddDays(refreshTokenLifetimeDays), //  من الإعدادات
            UserId = user.Id,
            DeviceInfo = storedToken.DeviceInfo,
            IpAddress = storedToken.IpAddress,
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        await _refreshTokenRepo.CreateAsync(newRefreshTokenEntity);

        var accessTokenLifetimeMinutes = _jwtSettings.Value.AccessTokenLifetimeInMinutes;

        //  إرجاع Access Token فقط (Read refresh token سيتم إعادته في Cookie)
        var result = new RefreshTokenResponseDto
        {
            AccessToken = newAccessToken,
            ExpiresIn = accessTokenLifetimeMinutes * 60
        };

        return (result, newRefreshToken);
    }
    // 3. أضف دالة Logout لسحب التوكن
    public async Task<bool> LogoutAsync(int userId, string refreshToken)
    {
        var token = await _refreshTokenRepo.GetByTokenAsync(refreshToken);
        if (token == null || token.UserId != userId)
            return false;

        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;
        await _refreshTokenRepo.UpdateAsync(token);

        return true;
    }


    public async Task<bool> RevokeAllUserTokensAsync(int userId)
    {
        try
        {
            // استدعاء الـ Repository لسحب كل التوكنات
            await _refreshTokenRepo.RevokeAllUserTokensAsync(userId);
            return true;
        }
        catch (Exception ex)
        {
            // تسجيل الخطأ (اختياري)
            // _logger.LogError(ex, "Error revoking all tokens for user {UserId}", userId);
            return false;
        }
    }
    private string GenerateJwtToken(User user)
    {



        var accessTokenLifetimeMinutes = _jwtSettings.Value.AccessTokenLifetimeInMinutes;


    

        var claims = new[]
{
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.Name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(accessTokenLifetimeMinutes), //  من الإعدادات
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
    private bool VerifyPassword(User user, string password)
    {
        // تحقق من كلمة المرور باستخدام خوارزمية التشفير المناسبة
        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
    }
}
