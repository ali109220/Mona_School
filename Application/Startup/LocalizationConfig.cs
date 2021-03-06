using Core.SharedDomain.Localiztion;
using Microsoft.AspNetCore.Hosting;
using Application.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Application.Startup
{
    public class LocalizationConfig
    {
        public static void Localize(IHostingEnvironment env)
        {
            var enResources = new List<Resource>();
            var arResources = new List<Resource>();
            var resourcesDocument = new XmlDocument();
            var en_path = Path.Combine(env.ContentRootPath, "Resources\\Mona.xml");
            var nt_path = Path.Combine(env.ContentRootPath, "Resources\\Mona-NT.xml");
            if (System.IO.File.Exists(en_path))
            {
                resourcesDocument.Load(en_path);
                XmlNode node = resourcesDocument.DocumentElement.SelectSingleNode("/localizationDictionary/texts");
                foreach (XmlNode element in node.ChildNodes)
                {
                    enResources.Add(new Resource { Key = element.Attributes["key"].InnerText, Value = element.InnerText });
                }
            }
            resourcesDocument = new XmlDocument();
            if (System.IO.File.Exists(nt_path))
            {
                resourcesDocument.Load(nt_path);
                XmlNode node = resourcesDocument.DocumentElement.SelectSingleNode("/localizationDictionary/texts");
                foreach (XmlNode element in node.ChildNodes)
                {
                    arResources.Add(new Resource { Key = element.Attributes["key"].InnerText, Value = element.InnerText });
                }
            }
            Localization.EnglishResources = enResources;
            Localization.NethrlandResources = arResources;
        }
    }
}
