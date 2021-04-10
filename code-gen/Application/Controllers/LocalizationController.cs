using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.SharedDomain.Localiztion;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application.Resources;

namespace Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocalizationController : ControllerBase
    {
        [Route("api/[controller]/[action]")]
        [HttpPost]
        public string GetEnglishResourceValue(string key)
        {
            try
            {
                var value = Localization.GetEnValue(key);
                if (string.IsNullOrEmpty(value))
                    return "Err_Localization";
                return value;
            }
            catch (Exception ex)
            {
                return "localization_Exception " + ex.ToString();
            }
        }
        [Route("api/[controller]/[action]")]
        [HttpPost]
        public string GetArabicResourceValue(string key)
        {
            try
            {
                var value = Localization.GetArValue(key);
                if (string.IsNullOrEmpty(value))
                    return "Err_Localization";
                return value;
            }
            catch (Exception ex)
            {
                return "localization_Exception " + ex.ToString();
            }
        }
        [Route("api/[controller]/[action]")]
        [HttpGet]
        public List<Resource> GetEnglishResources()
        {
            var resources = new List<Resource>();
            try
            {
                resources = Localization.GetEnglishValues();
                return resources;
            }
            catch (Exception ex)
            {
                return resources;
            }
        }
        [Route("api/[controller]/[action]")]
        [HttpGet]
        public List<Resource> GetArabicResources()
        {
            var resources = new List<Resource>();
            try
            {
                resources = Localization.GetArabicValues();
                return resources;
            }
            catch (Exception ex)
            {
                return resources;
            }
        }
    }
}