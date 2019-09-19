
namespace AIService.Models
{
    using System;
    using System.Collections.Generic;
    
    public class User
    {
        //public User()
        //{
        //    this.PraiseRecords = new HashSet<PraiseRecord>();
        //    this.SearchHistories = new HashSet<SearchHistory>();
        //    this.Talks = new HashSet<Talk>();
        //    this.WordsHistories = new HashSet<WordsHistory>();
        //}
    
        public int Id { get; set; }
        public string Username { get; set; }
        public string Phonenumber { get; set; }
        public string Password { get; set; }
        public DateTime CreateTime { get; set; }
        public string Remark { get; set; }
        public string ImageUrl { get; set; }
        public string Feedback { get; set; }
    
        //public virtual ICollection<PraiseRecord> PraiseRecords { get; set; }
        //public virtual ICollection<SearchHistory> SearchHistories { get; set; }
        //public virtual ICollection<Talk> Talks { get; set; }
        //public virtual ICollection<WordsHistory> WordsHistories { get; set; }
    }
}
