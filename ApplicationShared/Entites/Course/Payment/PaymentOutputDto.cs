using ApplicationShared.Settings;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationShared.Entites.Course.Payment
{
    public class PaymentOutputDto
    {
        public IEnumerable<PaymentDto> Payments { get; set; }
        public int AllCount { get; set; }
    }
    public class PaymentDto
    {
        public int Id { get; set; }
        public double PaymentAmount { get; set; }
        public double RemainingMoney { get; set; }
        public string PaymentDate { get; set; }
        public string PaymentTime { get; set; }
        public string Note { get; set; }
        public string HandOverByUserId { get; set; }
        public string HandOverByUserName { get; set; }
        public int CourseId { get; set; }

    }
}

