using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationShared.Entites.Customer
{
    public class CustomerOutputDto
    {
        public IEnumerable<Domain.Entities.Customer> Customers { get; set; }
        public int AllCount { get; set; }
    }
}
