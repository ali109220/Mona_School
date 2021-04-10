using ApplicationShared.Constants;
using ApplicationShared.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationShared.Entites.Course
{
    public class CourseInputDto
    {
        public string StartCourse { get; set; }
        public string ExamDate { get; set; }
        public LessonPeriod LessonPeriod { get; set; }
        public double TotalAmount { get; set; }
        public string Note { get; set; }
        public int? CustomerId { get; set; }
        public string UserId { get; set; }
        public string InChargeId { get; set; }
        public int? TypeOfPacketId { get; set; }
        public int? TypeOfExamId { get; set; }
        public int? PeroidBeforeSendEmailId { get; set; }
        public int? CurrencyId { get; set; }
    }
    public class CourseCancelInputDto
    {
        public int Id { get; set; }
        public CourseStatus Status { get; set; }
        public string UserId { get; set; }
    }
}
