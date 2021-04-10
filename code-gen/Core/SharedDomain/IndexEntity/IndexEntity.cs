using Core.SharedDomain.Audit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.SharedDomain.IndexEntity
{
    public class IndexEntity : AuditEntity
    {
        public virtual string Name { get; set; }
        public virtual string ArabicName { get; set; }
    }
}
