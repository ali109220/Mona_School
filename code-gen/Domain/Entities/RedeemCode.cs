using Core.SharedDomain.Audit;
using Core.SharedDomain.Security;
using Domain.Constants;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.Entities
{
    public class RedeemCode : AuditEntity
    {
        public virtual string Code { get; set; }
        public virtual Status Status { get; set; }
        public virtual DateTime? ActivationDate { get; set; }
        public virtual int? CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
        public virtual int? TypeId { get; set; }

        [ForeignKey("TypeId")]
        public virtual ProductType ProductType { get; set; }
    }
}
