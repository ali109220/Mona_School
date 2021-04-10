using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationShared.Security.Dto
{
    public class ResetPasswordInputDto
    {
        public string Email { get; set; }
        public string Code { get; set; }
    }
}
