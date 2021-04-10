using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.SharedDomain.Localiztion;

namespace Application.Resources
{
    public static class Localization
    {
        public static IList<Resource> EnglishResources { get; set; }
        public static IList<Resource> NethrlandResources { get; set; }
        public static string GetEnValue(string key)
        {
            var resource = EnglishResources.FirstOrDefault(x => x.Key == key);

            if (resource != null)
                return resource.Value;

            return key ?? Regex.Replace(key, @"(?<a>[a-z])(?<b>[A-Z0-9])", @"${a} ${b}");
        }
        public static string GetValue(string key, string lang)
        {
            if(string.IsNullOrEmpty(lang) || lang.Contains("ne"))
            {
                return GetNTValue(key);
            }
            else if (lang.Contains("en"))
            {
                return GetEnValue(key);
            }
            return key ?? Regex.Replace(key, @"(?<a>[a-z])(?<b>[A-Z0-9])", @"${a} ${b}");
        }


        public static string GetNTValue(string key)
        {
            var resource = NethrlandResources.FirstOrDefault(x => x.Key == key);

            if (resource != null)
                return resource.Value;

            return key ?? Regex.Replace(key, @"(?<a>[a-z])(?<b>[A-Z0-9])", @"${a} ${b}");
        }
        public static List<Resource> GetEnglishValues()
        {
            return EnglishResources.ToList();
        }
        public static List<Resource> GetNethrlandValues()
        {
            return NethrlandResources.ToList();
        }
    }
}
