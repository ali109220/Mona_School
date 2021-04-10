using Core.SharedDomain.IndexEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Settings
{
    public class PeroidBeforeSendEmail : IndexEntity
    {
        public virtual int Hours { get; set; }
    }
}
