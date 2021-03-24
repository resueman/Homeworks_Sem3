using Microsoft.EntityFrameworkCore;
using MyNUnitWeb.Models;

namespace MyNUnitWeb
{
    /// <summary>
    /// History of results of all test runs ever run on the server
    /// </summary>
    public class Repository : DbContext
    {
        public Repository(DbContextOptions<Repository> options)
            : base(options)
        {
            
        }

        /// <summary>
        /// Assemblies that was ever loaded and tested
        /// </summary>
        public DbSet<Assembly> Assemblies { get; set; }

        /// <summary>
        /// Tests that was ever running
        /// </summary>
        public DbSet<Test> Tests { get; set; }
    }
}
