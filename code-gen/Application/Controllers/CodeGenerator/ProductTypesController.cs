using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCore;
using Domain.Entities;
using ApplicationShared.Entities;
using ApplicationShared.Entites.Product;
using ApplicationShared.Constants;

namespace Application.Controllers.CodeGenerator
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTypesController : ControllerBase
    {
        private readonly CodeContext _context;

        public ProductTypesController(CodeContext context)
        {
            _context = context;
        }

        // GET: api/ProductTypes
        [Route("api/[controller]/GetAll")]
        [HttpGet]
        public async Task<ActionResult<ProductTypeOutputDto>> GetProductTypes()
        {
            var data = await _context.ProductTypes.Where(x => !x.VirtualDeleted).ToListAsync();
            var types = await _context.LicenceTypes.Where(x => !x.VirtualDeleted).ToListAsync();
            var brands = await _context.Brands.Where(x => !x.VirtualDeleted).ToListAsync();
            var platforms = await _context.Platforms.Where(x => !x.VirtualDeleted).ToListAsync();
            var selectedData = data.Select(x => new ProductOutputDto()
            {
                Id = x.Id,
                Name = x.Name,
                ArabicName = x.ArabicName,
                Description = x.Description,
                ArabicDescription = x.ArabicDescription,
                ActivatedCount = x.ActivatedCount,
                GeneratedCount = x.GeneratedCount,
                DateCreated = x.CreatedDate.Value.ToString("G"),
                Brand = new OutputIndexDto()
                {
                    Id = brands.FirstOrDefault(y => y.Id == x.BrandId).Id,
                    Name = brands.FirstOrDefault(y => y.Id == x.BrandId).Name,
                    ArabicName = brands.FirstOrDefault(y => y.Id == x.BrandId).ArabicName,
                },
                Platform = new OutputIndexDto()
                {
                    Id = platforms.FirstOrDefault(y => y.Id == x.PlatformId).Id,
                    Name = platforms.FirstOrDefault(y => y.Id == x.PlatformId).Name,
                    ArabicName = platforms.FirstOrDefault(y => y.Id == x.PlatformId).ArabicName,
                },
                LicenceType = new OutputIndexDto()
                {
                    Id = types.FirstOrDefault(y => y.Id == x.LicenceTypeId).Id,
                    Name = types.FirstOrDefault(y => y.Id == x.LicenceTypeId).Name,
                    ArabicName = types.FirstOrDefault(y => y.Id == x.LicenceTypeId).ArabicName,
                },
                HowToActivate = x.HowToActivate,
                ArabicHowToActivate = x.ArabicHowToActivate,
            });
            var all = new List<ProductOutputDto>((selectedData as IEnumerable<ProductOutputDto>).AsQueryable());
            var allCount = await _context.ProductTypes.Where(x => !x.VirtualDeleted).CountAsync();
            return new ProductTypeOutputDto() { ProductTypes = all, AllCount = allCount };
        }

        // GET: api/ProductTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductOutputDto>> GetProductType(int id)
        {
            var productType = await _context.ProductTypes.FindAsync(id);
            if (productType == null)
            {
                return NotFound();
            }
            var platform = await _context.Platforms.FirstOrDefaultAsync(x => x.Id == productType.PlatformId);
            var brand = await _context.Brands.FirstOrDefaultAsync(x => x.Id == productType.BrandId);
            var licenceType = await _context.LicenceTypes.FirstOrDefaultAsync(x => x.Id == productType.LicenceTypeId);

            var output = new ProductOutputDto()
            {
                Id = productType.Id,
                Name = productType.Name,
                ArabicName = productType.ArabicName,
                Description = productType.Description,
                DateCreated = productType.CreatedDate.Value.ToString("G"),
                ArabicDescription = productType.ArabicDescription,
                ActivatedCount = productType.ActivatedCount,
                GeneratedCount = productType.GeneratedCount,
                Brand = new OutputIndexDto()
                {
                    Id = brand.Id,
                    Name = brand.Name,
                    ArabicName = brand.ArabicName,
                },
                Platform = new OutputIndexDto()
                {
                    Id = platform.Id,
                    Name = platform.Name,
                    ArabicName = platform.ArabicName,
                },
                LicenceType = new OutputIndexDto()
                {
                    Id = licenceType.Id,
                    Name = licenceType.Name,
                    ArabicName = licenceType.ArabicName,
                },
                HowToActivate = productType.HowToActivate,
                ArabicHowToActivate = productType.ArabicHowToActivate,
            };

           

            return output;
        }

        // PUT: api/ProductTypes/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductType>> PutProductType(int id, ProductTypeInputDto input)
        {
            var productType = await _context.ProductTypes.FindAsync(id);
            productType.ArabicDescription = input.ArabicDescription;
            productType.Description = input.Description;
            productType.ArabicHowToActivate = input.ArabicHowToActivate;
            productType.HowToActivate = input.HowToActivate;
            productType.ArabicName = input.ArabicName;
            productType.Name = input.Name;
            productType.LicenceTypeId = input.LicenceTypeId;
            productType.PlatformId = input.PlatformId;
            productType.BrandId = input.BrandId;
            productType.UpdatedUserId = input.UserId;
            productType.UpdatedDate = DateTime.Now;
            _context.Entry(productType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return productType;
        }

        // POST: api/ProductTypes
        [HttpPost]
        public async Task<ActionResult<ProductType>> PostProductType(ProductTypeInputDto input)
        {
            try
            {
                var productType = new ProductType()
                {
                    ArabicDescription = input.ArabicDescription,
                    Description = input.Description,
                    ArabicHowToActivate = input.ArabicHowToActivate,
                    HowToActivate = input.HowToActivate,
                    ArabicName = input.ArabicName,
                    Name = input.Name,
                    LicenceTypeId = input.LicenceTypeId,
                    PlatformId = input.PlatformId,
                    BrandId = input.BrandId,
                    CreatedDate = DateTime.Now,
                    CreatedUserId = input.UserId
                };
                _context.ProductTypes.Add(productType);
                await _context.SaveChangesAsync();

                return productType;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // DELETE: api/ProductTypes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ProductType>> DeleteProductType(int id, string userId)
        {
            var productType = await _context.ProductTypes.FindAsync(id);
            if (productType == null)
            {
                return NotFound();
            }
            productType.DeletedDate = DateTime.Now;
            productType.DeletedUserId = userId;
            productType.VirtualDeleted = true;
            _context.Entry(productType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return productType;
        }

        private bool ProductTypeExists(int id)
        {
            return _context.ProductTypes.Any(e => e.Id == id);
        }
    }
}

