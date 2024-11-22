using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.DTOs
{

    public class ColorStatusDTO
    {
        public string? Color { get; set; }
        public bool Status { get; set; }
    }

    public class ConditionStatusDTO
    {
        public int? Condition { get; set; }
        public bool Status { get; set; }
    }


    public class SizeStatusDTO
    {
        public string? Size { get; set; }
        public bool Status { get; set; }
    }
}
