using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.SharedDomain.Localiztion;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application.Resources;
using EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Core.SharedDomain.Security;

namespace Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocalizationController : ControllerBase
    {
        private readonly MonaContext _context;
        private readonly UserManager<User> _userManager;
        public LocalizationController(MonaContext context,
            UserManager<User> userManager)

        {
            _context = context;
            _userManager = userManager;
        }
        [Route("api/[controller]/[action]")]
        [HttpPost]
        public async Task<string> GetResourceValue(string key, string userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                var lang = await _context.Languages.FindAsync(user.LanguageId);
                var value = Localization.GetValue(key, lang.Abbreviation);
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
        public string GetNethrlandResourceValue(string key)
        {
            try
            {
                var value = Localization.GetNTValue(key);
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
        public List<Resource> GetNethrlandResources()
        {
            var resources = new List<Resource>();
            try
            {
                resources = Localization.GetNethrlandValues();
                return resources;
            }
            catch (Exception ex)
            {
                return resources;
            }
        }
    }
}