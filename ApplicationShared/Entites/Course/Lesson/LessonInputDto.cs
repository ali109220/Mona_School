using ApplicationShared.Constants;
using ApplicationShared.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationShared.Entites.Course.Lesson
{
    public class LessonInputDto
    {
        public string Date { get; set; }
        public string Time { get; set; }
        public LessonPeriod LessonPeriod { get; set; }
        public string Note { get; set; }
        public int CourseId { get; set; }
        public string UserId { get; set; }
        public string InChargeId { get; set; }
    }
    public class LessonCancelInputDto
    {
        public int Id { get; set; }
        public LessonStatus Status { get; set; }
        public string UserId { get; set; }
    }
}
