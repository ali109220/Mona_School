using ApplicationShared.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationShared.Settings
{
    public class PeroidBeforeSendEmailDto : OutputIndexDto
    {
        public int Hours { get; set; }
    }
    public class PeroidBeforeSendEmailOutputDto
    {
        public IEnumerable<PeroidBeforeSendEmailDto> PeroidBeforeSendEmails { get; set; }
        public int AllCount { get; set; }
    }
}
