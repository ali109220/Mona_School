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
    public class PeroidBeforeSendEmailsController : ControllerBase
    {
        private readonly MonaContext _context;
        private readonly UserManager<User> _userManager;
        public PeroidBeforeSendEmailsController(MonaContext context,
            UserManager<User> userManager)

        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/PeroidBeforeSendEmails
        [HttpGet]
        public async Task<ActionResult<PeroidBeforeSendEmailOutputDto>> GetPeroidBeforeSendEmails()
        {
            var data = await _context.PeroidBeforeSendEmails.Where(x => !x.VirtualDeleted).ToListAsync();
            var all = data.Select(x => new PeroidBeforeSendEmailDto()
            {
                Id = x.Id,
                Name = x.Name,
                ForeignName = x.ForeignName,
                Hours = x.Hours,
            }).ToList();
            var count = data.Count;
            return new PeroidBeforeSendEmailOutputDto() { PeroidBeforeSendEmails = all, AllCount = count};
        }

        // GET: api/PeroidBeforeSendEmails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PeroidBeforeSendEmailDto>> GetPeroidBeforeSendEmail(int id)
        {
            var peroidBeforeSendEmail = await _context.PeroidBeforeSendEmails.FindAsync(id);
            if (peroidBeforeSendEmail == null)
            {
                return NotFound();
            }
            var result = new PeroidBeforeSendEmailDto()
            {
                Id = peroidBeforeSendEmail.Id,
                Name = peroidBeforeSendEmail.Name,
                ForeignName = peroidBeforeSendEmail.ForeignName,
                Hours = peroidBeforeSendEmail.Hours,
            };
            return result;
        }

        // PUT: api/PeroidBeforeSendEmails/5
        [HttpPut("{id}")]
        public async Task<ActionResult<PeroidBeforeSendEmailDto>> PutPeroidBeforeSendEmail(int id, PeroidBeforeSendEmailInputDto input)
        {
            var peroidBeforeSendEmail = await _context.PeroidBeforeSendEmails.FindAsync(id);
            peroidBeforeSendEmail.Name = input.Name;
            peroidBeforeSendEmail.ForeignName = input.ForeignName;
            peroidBeforeSendEmail.Hours = input.Hours;
            peroidBeforeSendEmail.UpdatedUserId = input.UserId;
            peroidBeforeSendEmail.UpdatedDate = DateTime.Now;
            _context.Entry(peroidBeforeSendEmail).State = EntityState.Modified;
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == input.UserId);
            var log = new Log()
            {
                DateTime = DateTime.Now,
                TypeFullName = typeof(PeroidBeforeSendEmail).FullName,
                Content = "@userName@updateAction@objTitle",
                TypeId = peroidBeforeSendEmail.Id,
                UserId = user.Id
            };
            _context.Logs.Add(log);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PeroidBeforeSendEmailExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            var result = new PeroidBeforeSendEmailDto()
            {
                Id = peroidBeforeSendEmail.Id,
                Name = peroidBeforeSendEmail.Name,
                ForeignName = peroidBeforeSendEmail.ForeignName,
                Hours = peroidBeforeSendEmail.Hours,
            };
            return result;
        }

        // POST: api/PeroidBeforeSendEmails
        [HttpPost]
        public async Task<ActionResult<PeroidBeforeSendEmailDto>> PostPeroidBeforeSendEmail(PeroidBeforeSendEmailInputDto input)
        {
            try
            {
                var peroidBeforeSendEmail = new PeroidBeforeSendEmail()
                {
                    Name = input.Name,
                    ForeignName = input.ForeignName,
                    Hours = input.Hours,
                    CreatedDate = DateTime.Now,
                    CreatedUserId = input.UserId
                };
                _context.PeroidBeforeSendEmails.Add(peroidBeforeSendEmail);
                await _context.SaveChangesAsync();
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == input.UserId);
                var log = new Log()
                {
                    DateTime = DateTime.Now,
                    TypeFullName = typeof(PeroidBeforeSendEmail).FullName,
                    Content = "@userName@addAction@objTitle",
                    TypeId = peroidBeforeSendEmail.Id,
                    UserId = user.Id
                };
                _context.Logs.Add(log);
                await _context.SaveChangesAsync();
                var result = new PeroidBeforeSendEmailDto()
                {
                    Id = peroidBeforeSendEmail.Id,
                    Name = peroidBeforeSendEmail.Name,
                    ForeignName = peroidBeforeSendEmail.ForeignName,
                    Hours = peroidBeforeSendEmail.Hours,
                };
                return result;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // DELETE: api/PeroidBeforeSendEmails/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<PeroidBeforeSendEmailDto>> DeletePeroidBeforeSendEmail(int id, string userId)
        {
            var peroidBeforeSendEmail = await _context.PeroidBeforeSendEmails.FindAsync(id);
            if (peroidBeforeSendEmail == null)
            {
                return NotFound();
            }
            peroidBeforeSendEmail.DeletedDate = DateTime.Now;
            peroidBeforeSendEmail.DeletedUserId = userId;
            peroidBeforeSendEmail.VirtualDeleted = true;
            _context.Entry(peroidBeforeSendEmail).State = EntityState.Modified;
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            var log = new Log()
            {
                DateTime = DateTime.Now,
                TypeFullName = typeof(PeroidBeforeSendEmail).FullName,
                Content = "@userName@deleteAction@objTitle",
                TypeId = peroidBeforeSendEmail.Id,
                UserId = user.Id
            };
            _context.Logs.Add(log);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PeroidBeforeSendEmailExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            var result = new PeroidBeforeSendEmailDto()
            {
                Id = peroidBeforeSendEmail.Id,
                Name = peroidBeforeSendEmail.Name,
                ForeignName = peroidBeforeSendEmail.ForeignName,
                Hours = peroidBeforeSendEmail.Hours,
            };
            return result;
        }

        private bool PeroidBeforeSendEmailExists(int id)
        {
            return _context.PeroidBeforeSendEmails.Any(e => e.Id == id);
        }
    }
}
