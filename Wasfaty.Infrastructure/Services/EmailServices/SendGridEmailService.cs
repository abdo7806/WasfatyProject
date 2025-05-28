using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Options;
using Wasfaty.Application.Interfaces;
using Wasfaty.Infrastructure.Services.EmailServices;

public class SendGridEmailService : IEmailService
{
    private readonly SendGridSettings _sendGridSettings;

    public SendGridEmailService(IOptions<SendGridSettings> sendGridSettings)
    {
        _sendGridSettings = sendGridSettings.Value;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
    {
        var client = new SendGridClient(_sendGridSettings.ApiKey);
        var from = new EmailAddress(_sendGridSettings.FromEmail, _sendGridSettings.FromName);
        var to = new EmailAddress(toEmail);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent: null, htmlContent);
        var response = await client.SendEmailAsync(msg);

        // تحقق من نجاح الإرسال (اختياري)
        if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
        {
            throw new Exception($"Failed to send email: {await response.Body.ReadAsStringAsync()}");
        }
    }
}