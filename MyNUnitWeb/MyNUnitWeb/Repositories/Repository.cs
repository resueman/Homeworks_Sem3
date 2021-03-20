using Microsoft.EntityFrameworkCore;
using MyNUnitWeb.Models;

namespace MyNUnitWeb
{
    public class Repository : DbContext
    {
        public Repository(DbContextOptions<Repository> options)
            : base(options)
        {
            
        }

        public DbSet<Assembly> Assemblies { get; set; }

        public DbSet<Test> Tests { get; set; }
    }
}
