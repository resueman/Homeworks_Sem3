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
            TestingSessions = new List<TestingSession>();
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
        /// Contains history of all assembly's test runs
        /// </summary>
        public List<TestingSession> TestingSessions { get; set; }
    }
}
