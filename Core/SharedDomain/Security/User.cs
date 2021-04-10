using Core.SharedDomain.Localiztion;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.SharedDomain.Security
{
    public class User : IdentityUser
    {

        public virtual string FullName { get; set; }
        public virtual bool IsAdmin { get; set; }
        public virtual DateTime? LastLogin { get; set; }
        public virtual int? LanguageId { get; set; }
        [ForeignKey("LanguageId")]
        public virtual Language Language { get; set; }
    }
}
