using Core.SharedDomain.Audit;
using Core.SharedDomain.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shared
{
    public class File: AuditEntity
    {
        public virtual string Name { get; set; }
        public virtual string Extension { get; set; }
        public virtual string Description { get; set; }
        public virtual string URL { get; set; }
        public virtual string Size { get; set; }
    }
}
