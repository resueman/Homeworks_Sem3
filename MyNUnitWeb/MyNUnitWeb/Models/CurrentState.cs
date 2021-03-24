using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyNUnitWeb.Models
{
    /// <summary>
    /// Describes current session state
    /// </summary>
    public class CurrentState
    {
        private readonly IWebHostEnvironment environment;

        /// <summary>
        /// Creates instance of CurrentState class
        /// </summary>
        /// <param name="environment">Web hosting environment info</param>
        public CurrentState(IWebHostEnvironment environment)
        {
            this.environment = environment;
            Tests = new List<Test>();
        }

        /// <summary>
        /// Running tests in current session
        /// </summary>
        public List<Test> Tests { get; set; }

        /// <summary>
        /// Loaded assemblies to test
        /// </summary>
        public IEnumerable<string> Assemblies 
            => Directory.EnumerateFiles($"{environment.WebRootPath}/Assemblies").Select(f => Path.GetFileName(f));
    }
}
