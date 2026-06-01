using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Wasfaty.Application.DTOs.Auth;
using Wasfaty.Application.DTOs.Users;
using Wasfaty.Application.Interfaces.IServices;
using Wasfaty.Application.Settings;


[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IOptions<CookieSettings> _cookieSettings;
    public AuthController(IAuthService authService, IOptions<CookieSettings> cookieSettings)
    {
        _authService = authService;
        _cookieSettings = cookieSettings;
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
    public async Task<ActionResult> Register([FromBody] RegisterUserDto request)
    {

        if (request == null || string.IsNullOrEmpty(request.FullName) 
            || string.IsNullOrEmpty(request.Email)|| 
            string.IsNullOrEmpty(request.Password) )
        {
            return BadRequest("Invalid User data.");
        }

        var user = await _authService.RegisterAsync(request);
        if (user == null)
            return BadRequest("اسم المستخدم موجود بالفعل");

        return Ok(user);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequestDto request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest("اسم المستخدم وكلمة المرور مطلوبان.");
        }
        // جلب معلومات الجهاز (اختياري للأمان)
        var deviceInfo = GetDeviceInfo();
        var ipAddress = GetIpAddress();

        var (result, refreshToken) = await _authService.LoginAsync(request, deviceInfo, ipAddress);

        if (result == null)
            return Unauthorized("Invalid email or password");


        // ✅ تخزين Refresh Token في HttpOnly Cookie
        SetRefreshTokenCookie(refreshToken);

        //var token = await _authService.LoginAsync(request);

        //if (token == null)
        //{
        //    return Unauthorized("Invalid email or password");
        //}
        return Ok(new
        {
            Token = result.AccessToken,      // Access Token فقط
            User = result.User,
            ExpiresIn = result.ExpiresIn
        });
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh()
    {
        // ✅ جلب Refresh Token من الـ Cookie (وليس من Body)
        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized(new { message = "No refresh token found" });


        var (result, newRefreshToken) = await _authService.RefreshTokenAsync(refreshToken);



        if (result == null)
            return Unauthorized(new { message = "Invalid or expired refresh token" });

        // ✅ تحديث الـ Cookie بـ Refresh Token الجديد (Token Rotation)
        SetRefreshTokenCookie(newRefreshToken);


        // ✅ تحديث الـ Cookie بـ Refresh Token جديد (سيتم إنشاؤه في الـ Service)
        // ملاحظة: الـ Refresh Token الجديد تم إنشاؤه داخل الـ Service
        // نحتاج لإرجاعه من الـ Service أيضاً

        return Ok(new
        {
            AccessToken = result.AccessToken,
            ExpiresIn = result.ExpiresIn
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        // ✅ جلب Refresh Token من الـ Cookie
        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
            return BadRequest("No active session found");

        var userId = GetUserId();

        var result = await _authService.LogoutAsync(userId, refreshToken);

        if (!result)
            return BadRequest("Logout failed");

        // ✅ مسح الـ Cookie
        Response.Cookies.Delete("refreshToken");

        return Ok(new { message = "Logged out successfully" });
    }

    [HttpPost("revoke-all")]
    [Authorize]
    public async Task<IActionResult> RevokeAllTokens()
    {
        var userId = GetUserId();
        var result = await _authService.RevokeAllUserTokensAsync(userId);

        if (!result)
            return BadRequest("Failed to revoke tokens");

        // ✅ مسح الـ Cookie
        Response.Cookies.Delete("refreshToken");

        return Ok(new { message = "All sessions have been terminated" });
    }



    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordDto model)
    {
        // استخراج UserId من التوكن
        //var userIdFromToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        var userId = GetUserId();

        // التحقق من تطابق الـ ID
        if (userId != model.UserId)
        {
            return Unauthorized("User ID does not match.");
        }

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        //var result = await _authService.ChangeUserPassword(model.UserId, model.CurrentPassword, model.NewPassword);
        var result = await _authService.ChangeUserPassword(userId, model.CurrentPassword, model.NewPassword);
        if (!result)
            return BadRequest("Failed to change password.");

        // ✅ بعد تغيير الباسورد، اسحب كل التوكنات
        await _authService.RevokeAllUserTokensAsync(userId);

        // ✅ مسح الـ Cookie
        Response.Cookies.Delete("refreshToken");

        return Ok("Password changed successfully.");
    }



// ============= دوال مساعدة =============


    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            throw new UnauthorizedAccessException("User ID not found in token");

        return int.Parse(userIdClaim);
    }

    private string GetDeviceInfo()
    {
        var userAgent = Request.Headers["User-Agent"].ToString();
        return string.IsNullOrEmpty(userAgent) ? "Unknown Device" : userAgent[..Math.Min(200, userAgent.Length)];
    }

    private string GetIpAddress()
    {
        // دعم الـ Proxy والـ Load Balancer
        var ip = Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (string.IsNullOrEmpty(ip))
        {
            ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        }

        // إزالة الـ port لو موجود
        if (ip != null && ip.Contains(":"))
            ip = ip.Split(':')[0];

        return ip ?? "Unknown IP";
    }

    //private void SetRefreshTokenCookie(string refreshToken)
    //{
    //    var settings = _cookieSettings.Value;

    //    //  تحويل SameSite من string إلى enum
    //    var sameSiteMode = settings.SameSite?.ToLower() switch
    //    {
    //        "strict" => SameSiteMode.Strict,
    //        "lax" => SameSiteMode.Lax,
    //        "none" => SameSiteMode.None,
    //        _ => SameSiteMode.Lax
    //    };

    //    // ✅ إعدادات أمان متقدمة للـ Cookie
    //    var cookieOptions = new CookieOptions
    //    {
    //        HttpOnly = true,                    // 🔒 منع الوصول من JavaScript (يمنع XSS)
    //        Secure = settings.Secure,                      // 🔒 يرسل فقط عبر HTTPS
    //        SameSite = sameSiteMode,    // 🔒 منع CSRF
    //        Expires = DateTime.UtcNow.AddDays(settings.RefreshTokenExpirationDays),
    //        Path = "/",                         // متاح لكل المسارات
    //        Domain = null,                      // يستخدم الـ domain الحالي تلقائياً
    //        IsEssential = true                  // مهم للتطبيق
    //    };

    //    //  للتطوير على localhost
    //    if (Request.Host.Host == "localhost")
    //    {
    //        cookieOptions.Secure = false;
    //        cookieOptions.SameSite = SameSiteMode.Lax;
    //    }

    //    // ✅ إضافة SameSite=None لـ Cross-domain لو محتاج (بحذر)
    //    // cookieOptions.SameSite = SameSiteMode.None;

    //    Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    //}

    private void SetRefreshTokenCookie(string refreshToken)
    {
        var settings = _cookieSettings.Value;

        // تحويل SameSite من string إلى enum
        var sameSiteMode = settings.SameSite?.ToLower() switch
        {
            "strict" => SameSiteMode.Strict,
            "lax" => SameSiteMode.Lax,
            "none" => SameSiteMode.None,
            _ => SameSiteMode.Lax
        };

        // ✅ إعدادات Cookie (معدلة للتطوير)
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // ✅ مهم جدًا
            SameSite = SameSiteMode.None,  // ✅ None هو المفتاح!
            Expires = DateTime.UtcNow.AddDays(settings.RefreshTokenExpirationDays),
            Path = "/",
            IsEssential = true,
            //Domain = null  // ✅ يسمح بالـ Cookie على أي subdomain


        };

        // ✅ إضافة Domain (اختياري)
        // cookieOptions.Domain = "localhost";  // إذا كنت تريد تحديد الـ Domain

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);

        // ✅ للتأكد
        Console.WriteLine($"Cookie set with SameSite=None, Secure=false");
    }
}
