using ApplicationShared.Settings;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationShared.Entites.Course
{
    public class CourseOutputDto
    {
        public IEnumerable<CourseDto> Courses { get; set; }
        public int AllCount { get; set; }
    }
    public class CourseDto
    {
        public int Id { get; set; }
        public string StartCourse { get; set; }
        public string ExamDate { get; set; }
        public string NextLessonDate { get; set; }
        public string NextLessonTime { get; set; }
        public string ExamDateInDays { get; set; }
        public string Status { get; set; }
        public string LessonPeriod { get; set; }
        public double TotalAmount { get; set; }
        public double RemainingMoney { get; set; }
        public double TakedHours { get; set; }
        public string Note { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string InChargeId { get; set; }
        public string InChargeName { get; set; }
        public TypeOfPacketDto TypeOfPacket { get; set; }
        public TypeOfExamDto TypeOfExam { get; set; }
        public PeroidBeforeSendEmailDto PeroidBeforeSendEmail { get; set; }
        public CurrencyDto Currency { get; set; }

    }
}

