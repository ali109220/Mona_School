

using Domain.Enums;
using System;

namespace ApplicationShared.Entities.Customer
{
    public class CustomerInputDto
    {
        public string FullName { get; set; }
        public string BirthDate { get; set; }
        public string StartCourse { get; set; }
        public string Country { get; set; }
        public string Note { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string PostalCode { get; set; }
        public Gender Gender { get; set; }
        public string UserId { get; set; }
        public string InChargeId { get; set; }



    }
}
