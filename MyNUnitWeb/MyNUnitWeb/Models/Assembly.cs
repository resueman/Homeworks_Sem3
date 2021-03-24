using System.Collections.Generic;

namespace MyNUnitWeb.Models
{
    /// <summary>
    /// Assembly model
    /// </summary>
    public class Assembly
    {
        /// <summary>
        /// Creates instance of Assembly class
        /// </summary>
        public Assembly()
        {
            Tests = new List<Test>();
        }

        /// <summary>
        /// Unique key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of assembly
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Tests containing in assembly
        /// </summary>
        public List<Test> Tests { get; set; }
    }
}
