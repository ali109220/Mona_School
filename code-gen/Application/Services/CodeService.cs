using Core.SharedDomain.IndexEntity;
using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public static class CodeService
    {
        public static string GenerateNewCode(List<string> codes)
        {
            var code = GenerateRandomNoStr(5);
            return code;
        }
        public static string GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max).ToString();
        }
        public static string GenerateRandomNoForResestPass()
        {
            int _min = 100000;
            int _max = 999999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max).ToString();
        }
        public static string GenerateRandomNoStr(int count)
        {
            var num = GenerateRandomNo();
            for (int i = 1; i < count; i++)
            {
                num = num + "-"+ GenerateRandomNo();
            }
            return num;
        }
    }
}
