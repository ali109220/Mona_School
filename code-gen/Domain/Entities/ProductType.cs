using Core.SharedDomain.Audit;
using Core.SharedDomain.Security;
using Domain.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.Entities
{
    public class ProductType : AuditEntity
    {
        public ProductType()
        {
            this.Codes = new List<RedeemCode>();
        }
        public virtual string Name { get; set; }
        public virtual string ArabicName { get; set; }
        public virtual int? LicenceTypeId { get; set; }

        [ForeignKey("LicenceTypeId")]
        public virtual LicenceType LicenceType { get; set; }
        public virtual int? BrandId { get; set; }

        [ForeignKey("BrandId")]
        public virtual Brand Brand { get; set; }
        public virtual string HowToActivate { get; set; }
        public virtual string ArabicHowToActivate { get; set; }
        public virtual int? PlatformId { get; set; }

        [ForeignKey("PlatformId")]
        public virtual Platform Platform { get; set; }
        public virtual string Description { get; set; }
        public virtual string ArabicDescription { get; set; }
        public virtual int ActivatedCount { get; set; }
        public virtual int GeneratedCount { get; set; }

        public virtual IList<RedeemCode> Codes { get; set; }
    }
}
