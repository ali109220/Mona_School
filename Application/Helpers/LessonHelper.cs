using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Helpers
{
    public static class LessonHelper
    {
        public static double GetTimeOfLesson(LessonPeriod lessonPeriod)
        {
            switch (lessonPeriod)
            {
                case LessonPeriod.Fifteen:
                    return 0.25;
                case LessonPeriod.Thirteen:
                    return 0.5;
                case LessonPeriod.FourtyToFour:
                    return 0.75;
                case LessonPeriod.Sixteen:
                    return 1;
                case LessonPeriod.SeventeenToFive:
                    return 1.25;
                case LessonPeriod.Nineteen:
                    return 1.5;
                case LessonPeriod.HundredToFive:
                    return 1.75;
                case LessonPeriod.HundredToTwinty:
                    return 2;
                default:
                    return 0.5;
            }
        }
    }
}
