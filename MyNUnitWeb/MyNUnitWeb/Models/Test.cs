using Methods;
using System.ComponentModel.DataAnnotations;

namespace MyNUnitWeb.Models
{
    /// <summary>
    /// Model of MyNUnit test method
    /// </summary>
    public class Test
    { 
        /// <summary>
        /// Unique key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Test method's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Test execution time
        /// </summary>
        public string ExecutionTime { get; set; }

        /// <summary>
        /// Result of test execution
        /// </summary>
        [EnumDataType(typeof(ExecutionStatus))]
        public ExecutionStatus ExecutionStatus { get; set; }

        /// <summary>
        /// Error mesage in case of fail or exception, message with ignore reason
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Name of assembly that contains this test method
        /// </summary>
        public string AssemblyName { get; set; }
    }
}
