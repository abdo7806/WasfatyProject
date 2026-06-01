using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasfaty.Application.DTOs.Auth
{
    public class RefreshTokenResponseDto
    {
        public string AccessToken { get; set; }  // لا يوجد RefreshToken هنا
        public int ExpiresIn { get; set; }
    }
}
