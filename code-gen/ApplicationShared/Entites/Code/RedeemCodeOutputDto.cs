using ApplicationShared.Constants;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationShared.Entites.Code
{
    public class RedeemCodeOutputDto
    {
        public IEnumerable<CodeOutputDto> RedeemCodes { get; set; }
        public int AllCount { get; set; }
        public int ActivatedCount { get; set; }
        public int WaitingCount { get; set; }
        public int NotActivatedCount { get; set; }
    }
    public class CustomerCodeOutputDto
    {
        public Domain.Entities.Customer Customer { get; set; }
        public bool Expand { get; set; }
        public IEnumerable<CodeOutputDto> RedeemCodes { get; set; }
    }
    public class CodeOutputDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string CreatedDate { get; set; }
        public string ActivatedDate { get; set; }
        public Status Status { get; set; }
        public string StrStatus { get; set; }
        public OutputIndexDto ProductType { get; set; }
        public OutputIndexDto LicenseType { get; set; }
        public OutputIndexDto Customer { get; set; }
        public string Country { get; set; }
        public string email { get; set; }
    }
}
