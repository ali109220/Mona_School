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
using Application.Resources;

namespace Application.Controllers.Settings
{
    [Route("api/[controller]")]
    [ApiController]
    public class TypeOfExamsController : ControllerBase
    {
        private readonly MonaContext _context;
        private readonly UserManager<User> _userManager;
        public TypeOfExamsController(MonaContext context,
            UserManager<User> userManager)

        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/TypeOfExams
        [HttpGet]
        public async Task<ActionResult<TypeOfExamOutputDto>> GetTypeOfExams()
        {
            var data = await _context.TypeOfExams.Where(x => !x.VirtualDeleted).ToListAsync();
            var all = data.Select(x => new TypeOfExamDto()
            {
                Id = x.Id,
                Name = x.Name,
                ForeignName = x.ForeignName,
                Cost= x.Cost
            }).ToList();
            var count = data.Count;
            return new TypeOfExamOutputDto() { TypeOfExams = all, AllCount = count};
        }

        // GET: api/TypeOfExams/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TypeOfExamDto>> GetTypeOfExam(int id)
        {
            var typeOfExam = await _context.TypeOfExams.FindAsync(id);
            if (typeOfExam == null)
            {
                return NotFound();
            }
            var result = new TypeOfExamDto()
            {
                Id = typeOfExam.Id,
                Name = typeOfExam.Name,
                ForeignName = typeOfExam.ForeignName,
                Cost = typeOfExam.Cost
            };
            return result;
        }

        // PUT: api/TypeOfExams/5
        [HttpPut("{id}")]
        public async Task<ActionResult<TypeOfExamDto>> PutTypeOfExam(int id, TypeOfExamInputDto input)
        {
            var typeOfExam = await _context.TypeOfExams.FindAsync(id);
            typeOfExam.Name = input.Name;
            typeOfExam.ForeignName = input.ForeignName;
            typeOfExam.Cost = input.Cost;
            typeOfExam.UpdatedUserId = input.UserId;
            typeOfExam.UpdatedDate = DateTime.Now;
            _context.Entry(typeOfExam).State = EntityState.Modified;
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == input.UserId);
            var log = new Log()
            {
                DateTime = DateTime.Now,
                TypeFullName = typeof(TypeOfExam).FullName,
                Content = "@userName@updateAction@objTitle",
                TypeId = typeOfExam.Id,
                UserId = user.Id
            };
            _context.Logs.Add(log);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TypeOfExamExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            var result = new TypeOfExamDto()
            {
                Id = typeOfExam.Id,
                Name = typeOfExam.Name,
                ForeignName = typeOfExam.ForeignName,
                Cost = typeOfExam.Cost
            };
            return result;
        }

        // POST: api/TypeOfExams
        [HttpPost]
        public async Task<ActionResult<TypeOfExamDto>> PostTypeOfExam(TypeOfExamInputDto input)
        {
            try
            {
                var typeOfExam = new TypeOfExam()
                {
                    Name = input.Name,
                    ForeignName = input.ForeignName,
                    Cost = input.Cost,
                    CreatedDate = DateTime.Now,
                    CreatedUserId = input.UserId
                };
                _context.TypeOfExams.Add(typeOfExam);
                await _context.SaveChangesAsync();
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == input.UserId);
                var log = new Log()
                {
                    DateTime = DateTime.Now,
                    TypeFullName = typeof(TypeOfExam).FullName,
                    Content = "@userName@addAction@objTitle",
                    TypeId = typeOfExam.Id,
                    UserId = user.Id
                };
                _context.Logs.Add(log);
                await _context.SaveChangesAsync();
                var result = new TypeOfExamDto()
                {
                    Id = typeOfExam.Id,
                    Name = typeOfExam.Name,
                    ForeignName = typeOfExam.ForeignName,
                    Cost = typeOfExam.Cost
                };
                return result;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // DELETE: api/TypeOfExams/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TypeOfExamDto>> DeleteTypeOfExam(int id, string userId)
        {
            var typeOfExam = await _context.TypeOfExams.FindAsync(id);
            if (typeOfExam == null)
            {
                return NotFound();
            }
            typeOfExam.DeletedDate = DateTime.Now;
            typeOfExam.DeletedUserId = userId;
            typeOfExam.VirtualDeleted = true;
            _context.Entry(typeOfExam).State = EntityState.Modified;
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            var log = new Log()
            {
                DateTime = DateTime.Now,
                TypeFullName = typeof(TypeOfExam).FullName,
                Content = "@userName@deleteAction@objTitle",
                TypeId = typeOfExam.Id,
                UserId = user.Id
            };
            _context.Logs.Add(log);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TypeOfExamExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            var result = new TypeOfExamDto()
            {
                Id = typeOfExam.Id,
                Name = typeOfExam.Name,
                ForeignName = typeOfExam.ForeignName,
                Cost = typeOfExam.Cost
            };
            return result;
        }

        private bool TypeOfExamExists(int id)
        {
            return _context.TypeOfExams.Any(e => e.Id == id);
        }
    }
}
