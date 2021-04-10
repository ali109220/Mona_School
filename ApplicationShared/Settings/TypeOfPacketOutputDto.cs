using ApplicationShared.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationShared.Settings
{
    public class TypeOfPacketDto : OutputIndexDto
    {
        public double Cost { get; set; }
        public int LessonsCount { get; set; }
    }
    public class TypeOfPacketOutputDto
    {
        public IEnumerable<TypeOfPacketDto> TypeOfPackets { get; set; }
        public int AllCount { get; set; }
    }
}
