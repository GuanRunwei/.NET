
namespace AIService.Models
{
    using System;
    using System.Collections.Generic;
    
    public class Knowledge
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string PossibleQuestion { get; set; }
    }
}
