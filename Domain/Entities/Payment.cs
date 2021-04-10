using Core.SharedDomain.Audit;
using Core.SharedDomain.Security;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Payment : AuditEntity
    {
        public Payment()
        {
        }
        public virtual double PaymentAmount { get; set; }
        public virtual double RemainingMoney { get; set; }
        public virtual DateTime? PaymentDate { get; set; }
        public virtual DateTime? PaymentTime { get; set; }
        public virtual string Note { get; set; }
        public virtual string HandOverByUserId { get; set; }

        [ForeignKey("HandOverByUserId")]
        public virtual User HandOverByUser { get; set; }
        public virtual int? CourseId { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }
    }
}
