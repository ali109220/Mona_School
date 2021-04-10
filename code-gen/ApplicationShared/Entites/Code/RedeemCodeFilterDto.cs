using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationShared.Entites.Code
{
    public class RedeemCodeFilterDto
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string TypeId { get; set; }
        public int? Status { get; set; }
    }
}

