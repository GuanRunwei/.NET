
namespace AIService.Models
{
    using System;
    using System.Collections.Generic;
    
    public class PraiseRecord
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Count { get; set; }
    
        public virtual User User { get; set; }
    }
}
