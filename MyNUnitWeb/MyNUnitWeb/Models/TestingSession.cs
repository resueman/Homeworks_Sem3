using System;
using System.Collections.Generic;

namespace MyNUnitWeb.Models
{
    /// <summary>
    /// Identify assembly's testing run
    /// </summary>
    public class TestingSession
    {
        public TestingSession(DateTime dateTime, int assemblyId)
        {
            Tests = new List<Test>();
            DateTime = dateTime;
            AssemblyId = assemblyId;
        }

        /// <summary>
        /// Unique identifier in database
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Unique identifie of assembly whose tests were run
        /// </summary>
        public int AssemblyId { get; set; }

        /// <summary>
        /// Runned tests
        /// </summary>
        public List<Test> Tests { get; set; }

        /// <summary>
        /// Time when the tests were run
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// The assembly whose tests were run
        /// </summary>
        public Assembly Assembly { get; set; }
    }
}
