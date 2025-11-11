using Microsoft.EntityFrameworkCore;
using RetailCore.Model;

namespace RetailCore.DataAccess.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options):base(options)
        {
            
        }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Clothing", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Books", DisplayOrder = 3 }
            );
        }
    }
}
