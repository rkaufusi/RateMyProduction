using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.Text;

namespace RateMyProduction.Core.Entities
{
    public class User : IdentityUser<int>
    {

        [Required, MaxLength(50)]
        public override string UserName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public override string Email { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? DisplayName { get; set; }

        [MaxLength(100)]
        public string? PrimaryRole { get; set; }

        public bool IsEmailVerified { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public DateTime DateJoined { get; set; } = DateTime.UtcNow;

        public DateTime? LastLogin { get; set; }

        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
