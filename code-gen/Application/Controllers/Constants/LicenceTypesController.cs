using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Constants;
using EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Core.SharedDomain.Security;
using Newtonsoft.Json;
using Application.Services;
using Core.SharedDomain.IndexEntity;
using Application.Models;
using ApplicationShared.Constants.LicenceTypeDto;

namespace Application.Controllers.Constants
{
    [Route("api/[controller]")]
    [ApiController]
    public class LicenceTypesController : ControllerBase
    {
        private readonly CodeContext _context;
        private readonly UserManager<User> _userManager;
        public LicenceTypesController(CodeContext context,
            UserManager<User> userManager)

        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/LicenceTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IndexEntity>>> GetLicenceTypes()
        {
            var data = await _context.LicenceTypes.Where(x => !x.VirtualDeleted).ToListAsync();
            var all = new List<IndexEntity>((data as IEnumerable<IndexEntity>).AsQueryable());
            return all;
        }

        // GET: api/LicenceTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LicenceType>> GetLicenceType(int id)
        {
            var licenceType = await _context.LicenceTypes.FindAsync(id);
            if (licenceType == null)
            {
                return NotFound();
            }

            return licenceType;
        }

        // PUT: api/LicenceTypes/5
        [HttpPut("{id}")]
        public async Task<ActionResult<LicenceType>> PutLicenceType(int id, LicenceTypeInputDto input)
        {
            var licenceType = await _context.LicenceTypes.FindAsync(id);
            licenceType.Name = input.Name;
            licenceType.ArabicName = input.ArabicName;
            licenceType.UpdatedUserId = input.UserId;
            licenceType.UpdatedDate = DateTime.Now;
            _context.Entry(licenceType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LicenceTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return licenceType;
        }

        // POST: api/LicenceTypes
        [HttpPost]
        public async Task<ActionResult<LicenceType>> PostLicenceType(LicenceTypeInputDto input)
        {
            try
            {
                var licenceType = new LicenceType()
                {
                    Name = input.Name,
                    ArabicName = input.ArabicName,
                    CreatedDate = DateTime.Now,
                    CreatedUserId = input.UserId
                };
                _context.LicenceTypes.Add(licenceType);
                await _context.SaveChangesAsync();

                return licenceType;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // DELETE: api/LicenceTypes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<LicenceType>> DeleteLicenceType(int id, string userId)
        {
            var licenceType = await _context.LicenceTypes.FindAsync(id);
            if (licenceType == null)
            {
                return NotFound();
            }
            licenceType.DeletedDate = DateTime.Now;
            licenceType.DeletedUserId = userId;
            licenceType.VirtualDeleted = true;
            _context.Entry(licenceType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LicenceTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return licenceType;
        }

        private bool LicenceTypeExists(int id)
        {
            return _context.LicenceTypes.Any(e => e.Id == id);
        }
    }
}
