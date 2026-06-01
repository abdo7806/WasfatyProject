using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasfaty.Application.Settings
{
    public class CookieSettings
    {
        public bool HttpOnly { get; set; } = true;
        public bool Secure { get; set; } = true;
        public string SameSite { get; set; } = "Strict";
        public int RefreshTokenExpirationDays { get; set; } = 7;
    }
}
