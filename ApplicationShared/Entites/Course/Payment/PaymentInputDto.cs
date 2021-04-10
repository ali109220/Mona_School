using ApplicationShared.Constants;
using ApplicationShared.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationShared.Entites.Course.Payment
{
    public class PaymentInputDto
    {
        public double PaymentAmount { get; set; }
        public string PaymentDate { get; set; }
        public string PaymentTime { get; set; }
        public string Note { get; set; }
        public string HandOverByUserId { get; set; }
        public int CourseId { get; set; }
        public string UserId { get; set; }
    }
}
