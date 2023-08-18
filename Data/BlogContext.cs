using API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace API.Data
{
    public class BlogContext : DbContext
    {
        public BlogContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<CreateUser> NewUser { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CreateUser>().Property(c => c.UserID).HasDefaultValueSql("NEWID()");
        }
        public DbSet<CreateBlog> NewBlogs { get; set; }
        public DbSet<RegisterUser> RegisterUser { get; set; }
    }
}
