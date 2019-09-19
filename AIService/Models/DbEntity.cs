using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace AIService.Models
{
    public class DbEntity : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Dictionary> Dictionaries { get; set; }
        public DbSet<Guide> Guides { get; set; }
        public DbSet<Knowledge> Knowledges { get; set; }
        public DbSet<NeedlessWord> NeedlessWords { get; set; }
        public DbSet<New> News { get; set; }
        public DbSet<PraiseRecord> PraiseRecords { get; set; }
        public DbSet<SearchHistory> SearchHistories { get; set; }
        public DbSet<Talk> Talks { get; set; }
        public DbSet<WordsHistory> WordsHistories { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"data source=;initial catalog=;User Id=;Password= ");

        }

    }
}
