using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RateMyProduction.Core.Entities
{
    public class Review
    {
        [Key]
        public int ReviewID { get; set; }

        public int ProductionID { get; set; }
        public int UserID { get; set; }

        [Range(1, 5)]
        public byte RatingOverall { get; set; }

        [Required, MaxLength(100)]
        public string RoleWorked { get; set; } = string.Empty;

        [Required]
        public string ReviewText { get; set; } = string.Empty;

        public DateTime DatePosted { get; set; } = DateTime.UtcNow;
        public bool IsAnonymous { get; set; } = false;

        // Navigation properties
        public virtual Production Production { get; set; } = null!;
        public virtual User User { get; set; } = null!;

        public virtual ICollection<ReviewTag> ReviewTags { get; set; } = new List<ReviewTag>();
    }
}
