using System;
using System.Collections.Generic;
using System.Text;

namespace RateMyProduction.Core.Entities
{
    public class ReviewTag
    {
        public int ReviewID { get; set; }
        public int TagID { get; set; }

        // Composite primary key
        public virtual Review Review { get; set; } = null!;
        public virtual Tag Tag { get; set; } = null!;
    }
}
