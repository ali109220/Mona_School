using Core.SharedDomain.Audit;
using Core.SharedDomain.Security;
using Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Lesson : AuditEntity
    {
        public Lesson()
        {
            this.Number = 0;
        }
        public virtual DateTime? Date { get; set; }
        public virtual DateTime? Time { get; set; }
        public virtual LessonStatus Status { get; set; }
        public virtual LessonPeriod LessonPeriod { get; set; }
        public virtual string Note { get; set; }
        public virtual string InChargeId { get; set; }

        [ForeignKey("InChargeId")]
        public virtual User InCharge { get; set; }
        public virtual int Number { get; set; }
        public virtual int? CourseId { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }
    }
}
