using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RateMyProduction.Core.Entities
{
    public class RefreshToken
    {
        [Key]
        public int TokenID { get; set; }

        public int UserID { get; set; }

        public string TokenHash { get; set; } = string.Empty;

        public string JwtId { get; set; } = string.Empty;

        public string? DeviceInfo { get; set; }

        public string? IpAddress { get; set; }

        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }

        public int? ReplacedByTokenID { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual User User { get; set; } = null!;
    }
}
