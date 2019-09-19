
namespace AIService.Models
{
    using System;
    using System.Collections.Generic;
    
    public class Talk
    {
        public Talk()
        {
            this.Comments = new HashSet<Comment>();
        }
    
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
        public int Praise { get; set; }
        public DateTime TalkTime { get; set; }
    
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual User User { get; set; }
    }
}
