using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RateMyProduction.Core.Entities
{
    public class Production
    {
        [Key]
        public int ProductionID { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string ProductionType { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Studio { get; set; }

        [MaxLength(100)]
        public string? Director { get; set; }

        public int? YearReleased { get; set; }

        [Column(TypeName = "decimal(3,2)")]
        public decimal? AverageRating { get; set; }

        public int ReviewCount { get; set; } = 0;

        public string? Synopsis { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }

}
