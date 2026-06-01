using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasfaty.Domain.Entities.Accounts
{
    public class RefreshToken
    {
        public int Id { get; set; }

        [Required]
        public string Token { get; set; } = null!;

        public DateTime Expires { get; set; }

        public bool IsRevoked { get; set; }

        public DateTime? RevokedAt { get; set; }

        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? DeviceInfo { get; set; }  // اختياري: لمنع السرقة
        public string? IpAddress { get; set; }   // اختياري

        public bool IsActive => !IsRevoked && Expires > DateTime.UtcNow;


    }
}
