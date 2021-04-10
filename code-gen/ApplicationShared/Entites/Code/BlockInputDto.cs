using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationShared.Entites.Code
{
    public class BlockInputDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
    }
    public class CheckInputDto
    {
        public string Code { get; set; }
        public string UserId { get; set; }
    }
    public class CheckOutPutDto
    {
        public string Name { get; set; }
        public string ArabicName { get; set; }
        public string LicenceType { get; set; }
        public string Brand { get; set; }
        public string ArabicLicenceType { get; set; }
        public string ArabicBrand { get; set; }
        public string ArabicPlatform { get; set; }
        public string HowToActivate { get; set; }
        public string ArabicHowToActivate { get; set; }
        public string Platform { get; set; }
        public string Description { get; set; }
        public string ArabicDescription { get; set; }
        public int ActivatedCount { get; set; }
        public int GeneratedCount { get; set; }
    }
    public class GenerateOutPutDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string ProductType { get; set; }
        public string LicenceType { get; set; }
        public string ActivationDate { get; set; }
        public string CreatedDate { get; set; }
        public string StrStatus { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
    public class BlockListInputDto
    {
        public List<int> Ids { get; set; }
        public string UserId { get; set; }
    }
    
}
