
using System;
using System.Collections.Generic;

namespace ApplicationShared.Settings
{
    public class LogDto
    {
        public DateTime? DateTime { get; set; }
        public string TypeFullName { get; set; }
        public int TypeId { get; set; }
        public string Content { get; set; }
        public bool HasSeen { get; set; }
        public string UserId { get; set; }
    }
    public class LogOutputDto
    {
        public IEnumerable<LogDto> Logs { get; set; }
        public int AllCount { get; set; }
    }
}
