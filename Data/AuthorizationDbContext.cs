using Microsoft.EntityFrameworkCore;
using authorization.Models;

namespace authorization.Data
{
    public class AuthorizationDbContext : DbContext
    {
        public AuthorizationDbContext() {}

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

            options.UseMySql(config.GetConnectionString("DefaultConnection"), 
                 new MySqlServerVersion(new Version(8, 0, 2)));
        }

        public DbSet<User> Users { get; set; }
    }
}
