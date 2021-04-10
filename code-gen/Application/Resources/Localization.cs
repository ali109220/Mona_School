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
        public static IList<Resource> ArabicResources { get; set; }
        public static string GetEnValue(string key)
        {
            var resource = EnglishResources.FirstOrDefault(x => x.Key == key);

            if (resource != null)
                return resource.Value;

            return key ?? Regex.Replace(key, @"(?<a>[a-z])(?<b>[A-Z0-9])", @"${a} ${b}");
        }
        
        public static string GetArValue(string key)
        {
            var resource = ArabicResources.FirstOrDefault(x => x.Key == key);

            if (resource != null)
                return resource.Value;

            return key ?? Regex.Replace(key, @"(?<a>[a-z])(?<b>[A-Z0-9])", @"${a} ${b}");
        }
        public static List<Resource> GetEnglishValues()
        {
            return EnglishResources.ToList();
        }
        public static List<Resource> GetArabicValues()
        {
            return ArabicResources.ToList();
        }
    }
}
