

namespace AIService.Models
{
    using System;
    using System.Collections.Generic;
    
    public class Comment
    {
        public int Id { get; set; }
        public int TalkId { get; set; }
        public string Point { get; set; }
        public DateTime CommentTime { get; set; }
        public int UserId { get; set; }
        public string Commenter { get; set; }
    
        public virtual Talk Talk { get; set; }
    }
}
