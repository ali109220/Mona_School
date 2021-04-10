using Core.SharedDomain.Audit;
using Domain.Enums;
using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Customer : AuditEntity
    {
        public Customer()
        {
            this.Courses = new List<Course>();
        }
        public virtual string FullName {get; set;}
        public virtual DateTime? BirthDate {get; set; }
        public virtual string Country {get; set; }
        public virtual string City {get; set; }
        public virtual string Address {get; set; }
        public virtual string Phone {get; set; }
        public virtual string Email {get; set; }
        public virtual string PostalCode {get; set; }
        public virtual string Note { get; set; }
        public virtual Gender Gender { get; set; }
        public virtual IList<Course> Courses { get; set; }
    }
}
