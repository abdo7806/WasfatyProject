using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wasfaty.Application.Interfaces;

namespace Wasfaty.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {



        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequest request)
        {
            await _emailService.SendEmailAsync(request.ToEmail, request.Subject, request.HtmlBody);
            return Ok("Email sent successfully!");
        }
    }

    public class EmailRequest
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string HtmlBody { get; set; }

    }
}
