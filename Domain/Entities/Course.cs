using Core.SharedDomain.Audit;
using Core.SharedDomain.Security;
using Domain.Enums;
using Domain.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Course : AuditEntity
    {
        public Course()
        {
            Lessons = new List<Lesson>();
            Payments = new List<Payment>();
        }
        public virtual DateTime? StartCourse { get; set; }
        public virtual DateTime? ExamDate { get; set; }
        public virtual CourseStatus Status { get; set; }
        public virtual LessonPeriod LessonPeriod { get; set; }
        public virtual double TotalAmount { get; set; }
        public virtual double RemainingMoney { get; set; }
        public virtual string Note { get; set; }
        public virtual int? CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
        public virtual string InChargeId { get; set; }

        [ForeignKey("InChargeId")]
        public virtual User InCharge { get; set; }
        public virtual int? TypeOfPacketId { get; set; }

        [ForeignKey("TypeOfPacketId")]
        public virtual TypeOfPacket TypeOfPacket { get; set; }
        public virtual int? TypeOfExamId { get; set; }

        [ForeignKey("TypeOfExamId")]
        public virtual TypeOfExam TypeOfExam { get; set; }
        public virtual int? PeroidBeforeSendEmailId { get; set; }

        [ForeignKey("PeroidBeforeSendEmailId")]
        public virtual PeroidBeforeSendEmail PeroidBeforeSendEmail { get; set; }
        public virtual int? CurrencyId { get; set; }

        [ForeignKey("CurrencyId")]
        public virtual Currency Currency { get; set; }

        public virtual IList<Lesson> Lessons { get; set; }
        public virtual IList<Payment> Payments { get; set; }
    }
}
