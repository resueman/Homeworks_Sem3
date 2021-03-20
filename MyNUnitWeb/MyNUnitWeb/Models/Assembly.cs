using Methods;
using System.Collections.Generic;

namespace MyNUnitWeb.Models
{
    public class Assembly
    {
        public Assembly()
        {
            Tests = new List<Test>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public List<Test> Tests { get; set; }
    }
}
