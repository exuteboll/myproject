using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApplicatoin3.Domain.ModelsDb;

namespace WebApplication3.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<UserDb> UserDb { get; set; }

        public DbSet<RequestDb> RequestDb { get; set; }

        public DbSet<ProductImageDb> ProductImageDb { get; set; }

        public DbSet<ProductDb> ProductDb { get; set; }

        public DbSet<OrderDb> orderDb { get; set; }

        public DbSet<CategoryDb> CategoryDb { get; set; }

        protected readonly IConfiguration Configuration;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }


    }

}
