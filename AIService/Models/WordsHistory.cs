
namespace AIService.Models
{
    using System;
    using System.Collections.Generic;
    
    public class WordsHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime SearchTime { get; set; }
        public string Word { get; set; }
        public string Explain { get; set; }
    
        public virtual User User { get; set; }
    }
}
