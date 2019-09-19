
namespace AIService.Models
{
    using System;
    using System.Collections.Generic;
    
    public class SearchHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime SearchTime { get; set; }
        public string HistoricalText { get; set; }
        public string Answer { get; set; }
    
        public virtual User User { get; set; }
    }
}
