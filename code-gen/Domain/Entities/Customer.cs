using Core.SharedDomain.Audit;
using Core.SharedDomain.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.Entities
{
    public class Customer : AuditEntity
    {
        public Customer()
        {
            this.Codes = new List<RedeemCode>();
        }
        public virtual string FirstName {get; set;}
        public virtual string LastName {get; set; }
        public virtual string FullName {get
            {
                return FirstName + " " + LastName;
            }
        }
        public virtual string Country {get; set; }
        public virtual string City {get; set; }
        public virtual string Phone {get; set; }
        public virtual string IpAddress {get; set; }
        public virtual string Email {get; set; }

        public virtual int NumberOfActivations { get; set; }
        public virtual bool Blocked { get; set; }
        public virtual string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public virtual IList<RedeemCode> Codes { get; set; }
    }
}
