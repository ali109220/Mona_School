using Core.SharedDomain.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.SharedDomain.Audit
{
    public class AuditEntity
    {
        [Key]
        [Required]
        public virtual int Id { get; set; }

        public virtual bool VirtualDeleted { get; set; }

        public virtual string CreatedUserId { get; set; }

        [ForeignKey("CreatedUserId")]
        public virtual User CreatedUser { get; set; }
        
        public virtual DateTime? CreatedDate { get; set; }

        public virtual string UpdatedUserId { get; set; }

        [ForeignKey("UpdatedUserId")]
        public virtual User UpdatedUser { get; set; }

        public virtual DateTime? UpdatedDate { get; set; }
        
        public virtual string DeletedUserId { get; set; }

        [ForeignKey("DeletedUserId")]
        public virtual User DeletedUser { get; set; }

        public virtual DateTime? DeletedDate { get; set; }
    }
}
