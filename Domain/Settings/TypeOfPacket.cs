using Core.SharedDomain.IndexEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Settings
{
    public class TypeOfPacket : IndexEntity
    {
        public virtual int LessonsCount { get; set; }
        public virtual double Cost { get; set; }
    }
}
