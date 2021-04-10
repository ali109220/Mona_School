using ApplicationShared.Settings;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationShared.Entites.Course.Lesson
{
    public class LessonOutputDto
    {
        public IEnumerable<LessonDto> Lessons { get; set; }
        public int AllCount { get; set; }
    }
    public class LessonDto
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Date { get; set; }
        public string TimeInTimes { get; set; }
        public string Time { get; set; }
        public LessonStatus Status { get; set; }
        public string LessonPeriodString { get; set; }
        public LessonPeriod LessonPeriod { get; set; }
        public string Note { get; set; }
        public int CourseId { get; set; }
        public string InChargeId { get; set; }
        public string InChargeName { get; set; }

    }
}

