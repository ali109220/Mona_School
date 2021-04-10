using ApplicationShared.Security.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationShared.Security
{
    public interface IAccountAppService
    {
        Task<string> Login(InputLoginDto input);
    }
}
