using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Core.SharedDomain.Security;
using ApplicationShared.Settings;
using Domain.Settings;

namespace Application.Controllers.Settings
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly MonaContext _context;
        private readonly UserManager<User> _userManager;
        public CurrencyController(MonaContext context,
            UserManager<User> userManager)

        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Currencies
        [HttpGet]
        public async Task<ActionResult<CurrencyOutputDto>> GetCurrencies()
        {
            var data = await _context.Currencies.Where(x => !x.VirtualDeleted).ToListAsync();
            var all = data.Select(x => new CurrencyDto()
            {
                Id = x.Id,
                Name = x.Name,
                ForeignName = x.ForeignName,
                IsActive = x.IsActive,
                Symbol = x.Symbol,
            }).ToList();
            var count = data.Count;
            return new CurrencyOutputDto() { Currencies = all, AllCount = count};
        }

        // GET: api/Currencies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CurrencyDto>> GetCurrency(int id)
        {
            var currency = await _context.Currencies.FindAsync(id);
            if (currency == null)
            {
                return NotFound();
            }
            var result = new CurrencyDto()
            {
                Id = currency.Id,
                Name = currency.Name,
                ForeignName = currency.ForeignName,
                IsActive = currency.IsActive,
                Symbol = currency.Symbol,
            };
            return result;
        }

        // PUT: api/Currencies/5
        [HttpPut("{id}")]
        public async Task<ActionResult<CurrencyDto>> PutCurrency(int id, CurrencyInputDto input)
        {
            var currency = await _context.Currencies.FindAsync(id);
            currency.Name = input.Name;
            currency.ForeignName = input.ForeignName;
            currency.IsActive = input.IsActive;
            currency.Symbol = input.Symbol;
            currency.UpdatedUserId = input.UserId;
            currency.UpdatedDate = DateTime.Now;
            if (currency.IsActive)
            {
                var currencies = await _context.Currencies.Where(x=> !x.VirtualDeleted && x.Id != id).ToListAsync();
                foreach (var item in currencies)
                {
                    item.IsActive = false;
                    _context.Entry(item).State = EntityState.Modified;
                }
            }
            _context.Entry(currency).State = EntityState.Modified;
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == input.UserId);
            var log = new Log()
            {
                DateTime = DateTime.Now,
                TypeFullName = typeof(Currency).FullName,
                Content = "@userName@updateAction@objTitle",
                TypeId = currency.Id,
                UserId = user.Id
            };
            _context.Logs.Add(log);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CurrencyExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            var result = new CurrencyDto()
            {
                Id = currency.Id,
                Name = currency.Name,
                ForeignName = currency.ForeignName,
                IsActive = currency.IsActive,
                Symbol = currency.Symbol,
            };
            return result;
        }

        // POST: api/Currencies
        [HttpPost]
        public async Task<ActionResult<CurrencyDto>> PostCurrency(CurrencyInputDto input)
        {
            try
            {
                var currency = new Currency()
                {
                    Name = input.Name,
                    ForeignName = input.ForeignName,
                    IsActive = input.IsActive,
                    Symbol = input.Symbol,
                    CreatedDate = DateTime.Now,
                    CreatedUserId = input.UserId
                };
                _context.Currencies.Add(currency);
                await _context.SaveChangesAsync();
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == input.UserId);
                var log = new Log()
                {
                    DateTime = DateTime.Now,
                    TypeFullName = typeof(Currency).FullName,
                    Content = "@userName@addAction@objTitle",
                    TypeId = currency.Id,
                    UserId = user.Id
                };
                _context.Logs.Add(log);
                await _context.SaveChangesAsync();
                var result = new CurrencyDto()
                {
                    Id = currency.Id,
                    Name = currency.Name,
                    ForeignName = currency.ForeignName,
                    IsActive = currency.IsActive,
                    Symbol = currency.Symbol,
                };
                return result;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // DELETE: api/Currencies/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<CurrencyDto>> DeleteCurrency(int id, string userId)
        {
            var currency = await _context.Currencies.FindAsync(id);
            if (currency == null)
            {
                return NotFound();
            }
            currency.DeletedDate = DateTime.Now;
            currency.DeletedUserId = userId;
            currency.VirtualDeleted = true;
            _context.Entry(currency).State = EntityState.Modified;
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            var log = new Log()
            {
                DateTime = DateTime.Now,
                TypeFullName = typeof(Currency).FullName,
                Content = "@userName@deleteAction@objTitle",
                TypeId = currency.Id,
                UserId = user.Id
            };
            _context.Logs.Add(log);


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CurrencyExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            var result = new CurrencyDto()
            {
                Id = currency.Id,
                Name = currency.Name,
                ForeignName = currency.ForeignName,
                IsActive = currency.IsActive,
                Symbol = currency.Symbol,
            };
            return result;
        }

        private bool CurrencyExists(int id)
        {
            return _context.Currencies.Any(e => e.Id == id);
        }
    }
}
