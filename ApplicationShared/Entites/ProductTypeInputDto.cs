

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ApplicationShared.Entities
{
    public class ProductTypeInputDto
    {
        public string Name { get; set; }
        public string ArabicName { get; set; }
        public int LicenceTypeId { get; set; }
        public int BrandId { get; set; }
        public string HowToActivate { get; set; }
        public string ArabicHowToActivate { get; set; }
        public int PlatformId { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public string ArabicDescription { get; set; }
    }
}
