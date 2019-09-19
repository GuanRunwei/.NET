
namespace AIService.Models
{
    using System;
    using System.Collections.Generic;
    
    public class Guide
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime GuideTime { get; set; }
        public string Title { get; set; }
        public string PicUrl1 { get; set; }
        public string PicUrl2 { get; set; }
        public string PicUrl3 { get; set; }
    }
}
