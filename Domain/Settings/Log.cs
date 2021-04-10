using Core.SharedDomain.Audit;
using Core.SharedDomain.Security;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Settings
{
    public class Log 
    {
        public Log()
        {
        }
        [Key]
        [Required]
        public virtual int Id { get; set; }

        public virtual bool VirtualDeleted { get; set; }
        public virtual DateTime? DateTime { get; set; }
        public virtual string TypeFullName { get; set; }
        public virtual int TypeId { get; set; }
        public virtual string Content { get; set; }
        public virtual bool HasSeen { get; set; }
        public virtual string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
