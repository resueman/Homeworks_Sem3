using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyNUnitWeb.Models
{
    public class CurrentState
    {
        private readonly IWebHostEnvironment environment;

        public CurrentState(IWebHostEnvironment environment)
        {
            this.environment = environment;
            Tests = new List<Test>();
        }

        public List<Test> Tests { get; set; }

        public IEnumerable<string> Assemblies 
            => Directory.EnumerateFiles($"{environment.WebRootPath}/Assemblies").Select(f => Path.GetFileName(f));
    }
}
