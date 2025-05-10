using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Wasfaty.Application.DTOs.Auth;
using Wasfaty.Application.DTOs.Users;
using Wasfaty.Application.Interfaces;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IAuthRepository authRepository, IConfiguration configuration)
    {
        _authRepository = authRepository;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _authRepository.GetByEmailAsync(request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            //throw new UnauthorizedAccessException("Invalid credentials");
            return null;
        }

        var token = GenerateJwtToken(user);

        return new LoginResponseDto 
        { 
            Token = token ,
            User = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = (UserRoleEnum)user.RoleId,
            }
        };
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
            RoleId = (int)request.Role,
            CreatedAt = DateTime.UtcNow
        };

        var createdUser = await _authRepository.CreateAsync(user);

        return new UserDto
        {
            Id = createdUser.Id,
            Email = createdUser.Email,
            FullName = createdUser.FullName
        };
    }

    private string GenerateJwtToken(User user)
    {


        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("role", user.Role.Name)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}
