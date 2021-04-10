using ApplicationShared.Constants;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationShared.Entites.Product
{
    public class ProductTypeOutputDto
    {
        public IEnumerable<ProductOutputDto> ProductTypes { get; set; }
        public int AllCount { get; set; }
    }
    public class ProductOutputDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ArabicName { get; set; }
        public OutputIndexDto LicenceType { get; set; }
        public string DateCreated { get; set; }
        public string Description { get; set; }
        public string ArabicDescription { get; set; }
        public int ActivatedCount { get; set; }
        public int GeneratedCount { get; set; }
        public OutputIndexDto Brand { get; set; }
        public OutputIndexDto Platform { get; set; }
        public string HowToActivate { get; set; }
        public string ArabicHowToActivate { get; set; }
    }
}
