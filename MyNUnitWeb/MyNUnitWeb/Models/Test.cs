using Methods;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyNUnitWeb.Models
{
    public class Test
    { 
        public Test()
        {

        }

        public int Id { get; set; }

        public string Name { get; set; }

        public TimeSpan ExecutionTime { get; set; }

        [EnumDataType(typeof(ExecutionStatus))]
        public ExecutionStatus ExecutionStatus { get; set; }

        public string Message { get; set; }
    }
}
