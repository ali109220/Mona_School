using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Constants;
using EntityFrameworkCore;
using ApplicationShared.Constants.PlatformDto;
using Microsoft.AspNetCore.Identity;
using Core.SharedDomain.Security;
using Newtonsoft.Json;
using Application.Services;
using Core.SharedDomain.IndexEntity;
using Application.Models;

namespace Application.Controllers.Constants
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly CodeContext _context;
        private readonly UserManager<User> _userManager;
        public PlatformsController(CodeContext context,
            UserManager<User> userManager)

        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Platforms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IndexEntity>>> GetPlatforms()
        {
            var data = await _context.Platforms.Where(x => !x.VirtualDeleted).ToListAsync();
            var all = new List<IndexEntity>((data as IEnumerable<IndexEntity>).AsQueryable());
            //var constantService = new ConstantService();
            // all = constantService.GetResultData(gridParmeter.PageIndex, gridParmeter.PageSize, gridParmeter.GridSorts, gridParmeter.GridFilters, all);
            return all;
        }

        // GET: api/Platforms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Platform>> GetPlatform(int id)
        {
            var platform = await _context.Platforms.FindAsync(id);
            if (platform == null)
            {
                return NotFound();
            }

            return platform;
        }

        // PUT: api/Platforms/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Platform>> PutPlatform(int id, PlatformInputDto input)
        {
            var platform = await _context.Platforms.FindAsync(id);
            platform.Name = input.Name;
            platform.ArabicName = input.ArabicName;
            platform.UpdatedUserId = input.UserId;
            platform.UpdatedDate = DateTime.Now;
            _context.Entry(platform).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlatformExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return platform;
        }

        // POST: api/Platforms
        [HttpPost]
        public async Task<ActionResult<Platform>> PostPlatform(PlatformInputDto input)
        {
            try
            {
                var platform = new Platform()
                {
                    Name = input.Name,
                    ArabicName = input.ArabicName,
                    CreatedDate = DateTime.Now,
                    CreatedUserId = input.UserId
                };
                _context.Platforms.Add(platform);
                await _context.SaveChangesAsync();

                return platform;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // DELETE: api/Platforms/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Platform>> DeletePlatform(int id, string userId)
        {
            var platform = await _context.Platforms.FindAsync(id);
            if (platform == null)
            {
                return NotFound();
            }
            platform.DeletedDate = DateTime.Now;
            platform.DeletedUserId = userId;
            platform.VirtualDeleted = true;
            _context.Entry(platform).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlatformExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return platform;
        }

        private bool PlatformExists(int id)
        {
            return _context.Platforms.Any(e => e.Id == id);
        }
    }
}
