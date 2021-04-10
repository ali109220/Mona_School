using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Models
{
    public class GridFilter
    {
        public GridFilter()
        {

        }
        public string Field { get; set; }
        public string Value { get; set; }
        public string Operator { get; set; }
    }
}
