using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasfaty.Domain.Entities.Audit
{
    public class AuditLog
    {
        public long Id { get; set; }
        public string? UserId { get; set; }
        public string? UserEmail { get; set; }
        public string? UserRole { get; set; }
        public string Action { get; set; } = null!;
        public string EntityName { get; set; } = null!;
        public string? EntityId { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? Details { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
