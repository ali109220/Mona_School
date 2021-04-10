
using ApplicationShared.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationShared.Settings
{
    public class TypeOfExamDto : OutputIndexDto
    {
        public double Cost { get; set; }
    }
    public class TypeOfExamOutputDto
    {
        public IEnumerable<TypeOfExamDto> TypeOfExams { get; set; }
        public int AllCount { get; set; }
    }
}
