using ApplicationShared.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationShared.Entites.Customer
{
    public class CustomerOutputDto
    {
        public IEnumerable<CustomerDto> Customers { get; set; }
        public int AllCount { get; set; }
    }
    public class CustomerDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Country { get; set; }
        public double RemainingAmount { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string PostalCode { get; set; }
        public Gender Gender { get; set; }
        public string Note { get; set; }

    }
}
