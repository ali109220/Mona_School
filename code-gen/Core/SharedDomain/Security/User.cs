using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.SharedDomain.Security
{
    public class User : IdentityUser
    {
        public virtual bool IsAdmin { get; set; }
        public virtual bool HasWrittingAccess { get; set; }
        public virtual bool IsCustomer { get; set; }
    }
}
