using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Constants;
using EntityFrameworkCore;
using ApplicationShared.Constants.BrandDto;
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
    public class BrandsController : ControllerBase
    {
        private readonly CodeContext _context;
        private readonly UserManager<User> _userManager;
        public BrandsController(CodeContext context,
            UserManager<User> userManager)

        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Brands
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IndexEntity>>> GetBrands()
        {
            var data = await _context.Brands.Where(x => !x.VirtualDeleted).ToListAsync();
            var all = new List<IndexEntity>((data as IEnumerable<IndexEntity>).AsQueryable());
            return all;
        }

        // GET: api/Brands/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Brand>> GetBrand(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }

            return brand;
        }

        // PUT: api/Brands/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Brand>> PutBrand(int id, BrandInputDto input)
        {
            var brand = await _context.Brands.FindAsync(id);
            brand.Name = input.Name;
            brand.ArabicName = input.ArabicName;
            brand.UpdatedUserId = input.UserId;
            brand.UpdatedDate = DateTime.Now;
            _context.Entry(brand).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BrandExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return brand;
        }

        // POST: api/Brands
        [HttpPost]
        public async Task<ActionResult<Brand>> PostBrand(BrandInputDto input)
        {
            try
            {
                var brand = new Brand()
                {
                    Name = input.Name,
                    ArabicName = input.ArabicName,
                    CreatedDate = DateTime.Now,
                    CreatedUserId = input.UserId
                };
                _context.Brands.Add(brand);
                await _context.SaveChangesAsync();

                return brand;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // DELETE: api/Brands/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Brand>> DeleteBrand(int id, string userId)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }
            brand.DeletedDate = DateTime.Now;
            brand.DeletedUserId = userId;
            brand.VirtualDeleted = true;
            _context.Entry(brand).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BrandExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return brand;
        }

        private bool BrandExists(int id)
        {
            return _context.Brands.Any(e => e.Id == id);
        }
    }
}
