
using System;

namespace ApplicationShared.Settings
{
    public class LogInputDto
    {
        public LogInputDto()
        {
        }
        public DateTime? DateTime { get; set; }
        public string TypeFullName { get; set; }
        public int TypeId { get; set; }
        public string Content { get; set; }
        public bool HasSeen { get; set; }
        public string UserId { get; set; }
    }
}
