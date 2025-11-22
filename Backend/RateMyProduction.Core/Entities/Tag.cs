using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RateMyProduction.Core.Entities
{
    public class Tag
    {
        [Key]
        public int TagID { get; set; }

        [Required, MaxLength(50)]
        public string TagName { get; set; } = string.Empty;

        // Navigation
        public virtual ICollection<ReviewTag> ReviewTags { get; set; } = new List<ReviewTag>();
    }
}
