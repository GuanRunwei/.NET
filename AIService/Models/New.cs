
namespace AIService.Models
{
    using System;
    using System.Collections.Generic;
    
    public class New
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime IssueTime { get; set; }
        public string PicUrl1 { get; set; }
        public string PicUrl2 { get; set; }
        public string Source { get; set; }
        public string NewsSource { get; set; }
    }
}
