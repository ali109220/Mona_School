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
    public class TypeOfPacketsController : ControllerBase
    {
        private readonly MonaContext _context;
        private readonly UserManager<User> _userManager;
        public TypeOfPacketsController(MonaContext context,
            UserManager<User> userManager)

        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/TypeOfPackets
        [HttpGet]
        public async Task<ActionResult<TypeOfPacketOutputDto>> GetTypeOfPackets()
        {
            var data = await _context.TypeOfPackets.Where(x => !x.VirtualDeleted).ToListAsync();
            var all = data.Select(x => new TypeOfPacketDto()
            {
                Id = x.Id,
                Name = x.Name,
                ForeignName = x.ForeignName,
                Cost = x.Cost,
                LessonsCount = x.LessonsCount
            }).ToList();
            var count = data.Count;
            return new TypeOfPacketOutputDto() { TypeOfPackets = all, AllCount = count};
        }

        // GET: api/TypeOfPackets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TypeOfPacketDto>> GetTypeOfPacket(int id)
        {
            var typeOfPacket = await _context.TypeOfPackets.FindAsync(id);
            if (typeOfPacket == null)
            {
                return NotFound();
            }
            var result = new TypeOfPacketDto()
            {
                Id = typeOfPacket.Id,
                Name = typeOfPacket.Name,
                ForeignName = typeOfPacket.ForeignName,
                Cost = typeOfPacket.Cost,
                LessonsCount = typeOfPacket.LessonsCount
            };
            return result;
        }

        // PUT: api/TypeOfPackets/5
        [HttpPut("{id}")]
        public async Task<ActionResult<TypeOfPacketDto>> PutTypeOfPacket(int id, TypeOfPacketInputDto input)
        {
            var typeOfPacket = await _context.TypeOfPackets.FindAsync(id);
            typeOfPacket.Name = input.Name;
            typeOfPacket.ForeignName = input.ForeignName;
            typeOfPacket.Cost = input.Cost;
            typeOfPacket.LessonsCount = input.LessonsCount;
            typeOfPacket.UpdatedUserId = input.UserId;
            typeOfPacket.UpdatedDate = DateTime.Now;
            _context.Entry(typeOfPacket).State = EntityState.Modified;
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == input.UserId);
            var log = new Log()
            {
                DateTime = DateTime.Now,
                TypeFullName = typeof(TypeOfPacket).FullName,
                Content = "@userName@updateAction@objTitle",
                TypeId = typeOfPacket.Id,
                UserId = user.Id
            };
            _context.Logs.Add(log);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TypeOfPacketExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            var result = new TypeOfPacketDto()
            {
                Id = typeOfPacket.Id,
                Name = typeOfPacket.Name,
                ForeignName = typeOfPacket.ForeignName,
                Cost = typeOfPacket.Cost,
                LessonsCount = typeOfPacket.LessonsCount
            };
            return result;
        }

        // POST: api/TypeOfPackets
        [HttpPost]
        public async Task<ActionResult<TypeOfPacketDto>> PostTypeOfPacket(TypeOfPacketInputDto input)
        {
            try
            {
                var typeOfPacket = new TypeOfPacket()
                {
                    Name = input.Name,
                    ForeignName = input.ForeignName,
                    Cost = input.Cost,
                    LessonsCount = input.LessonsCount,
                    CreatedDate = DateTime.Now,
                    CreatedUserId = input.UserId
                };
                _context.TypeOfPackets.Add(typeOfPacket);
                await _context.SaveChangesAsync();
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == input.UserId);
                var log = new Log()
                {
                    DateTime = DateTime.Now,
                    TypeFullName = typeof(TypeOfPacket).FullName,
                    Content = "@userName@addAction@objTitle",
                    TypeId = typeOfPacket.Id,
                    UserId = user.Id
                };
                _context.Logs.Add(log);
                await _context.SaveChangesAsync();
                var result = new TypeOfPacketDto()
                {
                    Id = typeOfPacket.Id,
                    Name = typeOfPacket.Name,
                    ForeignName = typeOfPacket.ForeignName,
                    Cost = typeOfPacket.Cost,
                    LessonsCount = typeOfPacket.LessonsCount
                };
                return result;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // DELETE: api/TypeOfPackets/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TypeOfPacketDto>> DeleteTypeOfPacket(int id, string userId)
        {
            var typeOfPacket = await _context.TypeOfPackets.FindAsync(id);
            if (typeOfPacket == null)
            {
                return NotFound();
            }
            typeOfPacket.DeletedDate = DateTime.Now;
            typeOfPacket.DeletedUserId = userId;
            typeOfPacket.VirtualDeleted = true;
            _context.Entry(typeOfPacket).State = EntityState.Modified;
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            var log = new Log()
            {
                DateTime = DateTime.Now,
                TypeFullName = typeof(TypeOfPacket).FullName,
                Content = "@userName@deleteAction@objTitle",
                TypeId = typeOfPacket.Id,
                UserId = user.Id
            };
            _context.Logs.Add(log);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TypeOfPacketExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            var result = new TypeOfPacketDto()
            {
                Id = typeOfPacket.Id,
                Name = typeOfPacket.Name,
                ForeignName = typeOfPacket.ForeignName,
                Cost = typeOfPacket.Cost,
                LessonsCount = typeOfPacket.LessonsCount
            };
            return result;
        }

        private bool TypeOfPacketExists(int id)
        {
            return _context.TypeOfPackets.Any(e => e.Id == id);
        }
    }
}
