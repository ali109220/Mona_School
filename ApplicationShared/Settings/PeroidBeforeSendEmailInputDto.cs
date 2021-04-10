using ApplicationShared.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationShared.Settings
{
    public class PeroidBeforeSendEmailInputDto : IndexDto
    {
        public string Name { get; set; }
        public int Hours { get; set; }
    }
}
