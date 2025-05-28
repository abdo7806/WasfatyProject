using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasfaty.Application.Interfaces
{
    public interface ISendGridEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
    }

}
