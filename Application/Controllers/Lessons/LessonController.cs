using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Core.SharedDomain.Security;
using Domain.Settings;
using Domain.Entities;
using ApplicationShared.Entites.Course.Lesson;
using System.Collections.Generic;
using Application.Services;

namespace Application.Controllers.Lessons
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonController : ControllerBase
    {
        private readonly MonaContext _context;
        private readonly UserManager<User> _userManager;
        public LessonController(MonaContext context,
            UserManager<User> userManager)

        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Lessons
        [Route("api/[controller]/[action]")]
        [HttpPost]
        public async Task<ActionResult<LessonOutputDto>> GetLessons(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return NotFound();
            }
            var users = await _context.Users.Where(x => x.FullName != null).ToListAsync();
            var data = await _context.Lessons.Where(x => !x.VirtualDeleted && x.CourseId == courseId).ToListAsync();
            var all = data.Select(x => new LessonDto()
            {
                Id = x.Id,
                InChargeId = x.InChargeId,
                InChargeName = users.SingleOrDefault(y=> y.Id == x.InChargeId).FullName,
                LessonPeriod = x.LessonPeriod,
                LessonPeriodString = x.LessonPeriod.ToString(),
                Note = x.Note,
                Status = x.Status,
                Number = x.Number,
                CourseId = x.CourseId.HasValue? x.CourseId.Value:0,
                Date = x.Date.HasValue ? x.Date.Value.ToString("yyyy/MM/dd") : "",
                Time = x.Time.HasValue ? x.Time.Value.ToString("HH:mm") : "",
                TimeInTimes = x.Time.HasValue && x.Date.HasValue ? ((x.Date.Value - DateTime.Now.Date).TotalHours + (x.Time.Value.Hour - DateTime.Now.Hour)).ToString() : "0"

            }).OrderBy(x=> x.Number).ToList();
            //var num = 0;
            //foreach (var item in all)
            //{
            //    num++;
            //    item.Number = num;
            //}
            var count = data.Count;
            return new LessonOutputDto() { Lessons = all, AllCount = count};
        }

        // GET: api/Lessons/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LessonDto>> GetLesson(int id)
        {
            DateTime? datetime = null;
            var users = await _context.Users.Where(x => x.FullName != null).ToListAsync();
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null)
            {
                return NotFound();
            }
            var result = new LessonDto()
            {
                Id = lesson.Id,
                Date = lesson.Date.HasValue ? lesson.Date.Value.ToString("G") : "",
                Time = lesson.Time.HasValue ? lesson.Time.Value.ToString("T") : "",
                TimeInTimes = lesson.Time.HasValue && lesson.Date.HasValue ? ((DateTime.Now.Date - lesson.Date.Value).TotalHours + (DateTime.Now.Hour - lesson.Time.Value.Hour)).ToString() : "0",
                CourseId = lesson.CourseId.HasValue ? lesson.CourseId.Value : 0,
                InChargeId = lesson.InChargeId,
                InChargeName = users.SingleOrDefault(y => y.Id == lesson.InChargeId).FullName,
                LessonPeriod = lesson.LessonPeriod,
                Note = lesson.Note,
                Status = lesson.Status
            };
            return result;
        }

        // PUT: api/Lessons/5
        [HttpPut("{id}")]
        public async Task<ActionResult<LessonDto>> PutLesson(int id, LessonInputDto input)
        {
            DateTime? datetime = null;
            var users = await _context.Users.Where(x => x.FullName != null).ToListAsync();
            var lesson = await _context.Lessons.FindAsync(id);
            lesson.Date = DateTimeString.TryParsingDate(input.Date, false);
            lesson.Time = DateTimeString.TryParsingDate(input.Time, true);
            lesson.InChargeId = input.InChargeId;
            lesson.CourseId = input.CourseId;
            lesson.LessonPeriod = input.LessonPeriod;
            lesson.Note = input.Note;
            lesson.Status = 
                Domain.Enums.LessonStatus.NotStarted;
            lesson.UpdatedUserId = input.UserId;
            lesson.UpdatedDate = DateTime.Now;
            _context.Entry(lesson).State = EntityState.Modified;
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == input.UserId);
            var log = new Log()
            {
                DateTime = DateTime.Now,
                TypeFullName = typeof(Lesson).FullName,
                Content = "@userName@updateAction@objTitle",
                TypeId = lesson.Id,
                UserId = user.Id
            };
            _context.Logs.Add(log);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LessonExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            var result = new LessonDto()
            {
                Id = lesson.Id,
                Date = lesson.Date.HasValue ? lesson.Date.Value.ToString("G") : "",
                Time = lesson.Time.HasValue ? lesson.Time.Value.ToString("T") : "",
                TimeInTimes = lesson.Time.HasValue && lesson.Date.HasValue ? ((DateTime.Now.Date - lesson.Date.Value).TotalHours + (DateTime.Now.Hour - lesson.Time.Value.Hour)).ToString() : "0",
                CourseId = lesson.CourseId.HasValue ? lesson.CourseId.Value : 0,
                InChargeId = lesson.InChargeId,
                InChargeName = users.SingleOrDefault(y => y.Id == lesson.InChargeId).FullName,
                LessonPeriod = lesson.LessonPeriod,
                Note = lesson.Note,
                Status = lesson.Status
            };
            return result;
        }
        // cancel course

        [Route("api/[controller]/[action]")]
        [HttpPost]
        public async Task<ActionResult<bool>> ChangeStatusLesson(LessonCancelInputDto input)
        {
            var lesson = await _context.Lessons.FindAsync(input.Id);
            lesson.Status = input.Status;
            _context.Entry(lesson).State = EntityState.Modified;
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == input.UserId);
            var log = new Log()
            {
                DateTime = DateTime.Now,
                TypeFullName = typeof(Course).FullName,
                Content = "@userName@cancelAction@objTitle",
                TypeId = lesson.Id,
                UserId = user.Id
            };
            _context.Logs.Add(log);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }

            return true;
        }

        // POST: api/
        [Route("api/[controller]/[action]")]
        [HttpPost]
        public async Task<ActionResult<LessonDto>> PostLesson(LessonInputDto input)
        {
            DateTime? datetime = null;
            var users = await _context.Users.Where(x => x.FullName != null).ToListAsync();
            try
            {
                var lesson = new Lesson()
                {
                    Date = DateTimeString.TryParsingDate(input.Date, false),
                    Time = DateTimeString.TryParsingDate(input.Time, true),
                    InChargeId = input.InChargeId,
                    CourseId = input.CourseId,
                    LessonPeriod = input.LessonPeriod,
                    Note = input.Note,
                    CreatedDate = DateTime.Now,
                    CreatedUserId = input.UserId
                };
                lesson.Status = Domain.Enums.LessonStatus.NotStarted;
                _context.Lessons.Add(lesson);
                await _context.SaveChangesAsync();
                var lastLesson = await _context.Lessons.Where(x => !x.VirtualDeleted &&
                    x.CourseId == lesson.CourseId && x.Id != lesson.Id).OrderByDescending(x => x.Number).FirstOrDefaultAsync();

                lesson.Number += lastLesson.Number;
                _context.Entry(lesson).State = EntityState.Modified;
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == input.UserId);
                var log = new Log()
                {
                    DateTime = DateTime.Now,
                    TypeFullName = typeof(Lesson).FullName,
                    Content = "@userName@addAction@objTitle",
                    TypeId = lesson.Id,
                    UserId = user.Id
                };
                _context.Logs.Add(log);
                await _context.SaveChangesAsync();
                var result = new LessonDto()
                {
                    Id = lesson.Id,
                    Date = lesson.Date.HasValue ? lesson.Date.Value.ToString("G") : "",
                    Time = lesson.Time.HasValue ? lesson.Time.Value.ToString("T") : "",
                    TimeInTimes = lesson.Time.HasValue && lesson.Date.HasValue ? ((DateTime.Now.Date - lesson.Date.Value).TotalHours + (DateTime.Now.Hour - lesson.Time.Value.Hour)).ToString() : "0",
                    CourseId = lesson.CourseId.HasValue ? lesson.CourseId.Value : 0,
                    InChargeId = lesson.InChargeId,
                    InChargeName = users.SingleOrDefault(y => y.Id == lesson.InChargeId).FullName,
                    LessonPeriod = lesson.LessonPeriod,
                    Note = lesson.Note,
                    Status = lesson.Status
                };
                return result;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //delete filtered ids
        [Route("api/[controller]/[action]")]
        [HttpPost]
        public async Task<ActionResult<bool>> DeleteFilterdLessons(string userId, int courseId = 0, IList<int> list = null)
        {
            var lessons = courseId == 0 ?
                list != null && list.Count > 0 ? 
                await _context.Lessons.Where(x => list.Any(y => y == x.Id)).ToListAsync() : 
                await _context.Lessons.Where(x => !x.VirtualDeleted).ToListAsync() :
                await _context.Lessons.Where(x => !x.VirtualDeleted && x.CourseId == courseId).ToListAsync();
            if (lessons.Count == 0)
            {
                return NotFound();
            }
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            foreach (var lesson in lessons)
            {
                lesson.DeletedDate = DateTime.Now;
                lesson.DeletedUserId = userId;
                lesson.VirtualDeleted = true;
                _context.Entry(lesson).State = EntityState.Modified;
                var log = new Log()
                {
                    DateTime = DateTime.Now,
                    TypeFullName = typeof(Lesson).FullName,
                    Content = "@userName@deleteAction@objTitle",
                    TypeId = lesson.Id,
                    UserId = user.Id
                };
                _context.Logs.Add(log);
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        // DELETE: api/Lessons/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<LessonDto>> DeleteLesson(int id, string userId)
        {
            var users = await _context.Users.Where(x => x.FullName != null).ToListAsync();
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null)
            {
                return NotFound();
            }
            lesson.DeletedDate = DateTime.Now;
            lesson.DeletedUserId = userId;
            lesson.VirtualDeleted = true;
            _context.Entry(lesson).State = EntityState.Modified;
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            var log = new Log()
            {
                DateTime = DateTime.Now,
                TypeFullName = typeof(Lesson).FullName,
                Content = "@userName@deleteAction@objTitle",
                TypeId = lesson.Id,
                UserId = user.Id
            };
            _context.Logs.Add(log);


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LessonExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            var result = new LessonDto()
            {
                Id = lesson.Id,
                Date = lesson.Date.HasValue ? lesson.Date.Value.ToString("G") : "",
                Time = lesson.Time.HasValue ? lesson.Time.Value.ToString("T") : "",
                TimeInTimes = lesson.Time.HasValue && lesson.Date.HasValue ? ((DateTime.Now.Date - lesson.Date.Value).TotalHours + (DateTime.Now.Hour - lesson.Time.Value.Hour)).ToString() : "0",
                CourseId = lesson.CourseId.HasValue ? lesson.CourseId.Value : 0,
                InChargeId = lesson.InChargeId,
                InChargeName = users.SingleOrDefault(y => y.Id == lesson.InChargeId).FullName,
                LessonPeriod = lesson.LessonPeriod,
                Note = lesson.Note,
                Status = lesson.Status
            };
            return result;
        }

        private bool LessonExists(int id)
        {
            return _context.Lessons.Any(e => e.Id == id);
        }
    }
}
