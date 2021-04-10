using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Models
{
    public class GridParameter
    {
        public GridParameter()
        {
            PageIndex = 1;
            PageSize = 10;
            GridFilters = new List<GridFilter>();
            GridSorts = new List<GridSort>();
        }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public List<GridFilter> GridFilters { get; set; }
        public List<GridSort> GridSorts { get; set; }
    }
}
