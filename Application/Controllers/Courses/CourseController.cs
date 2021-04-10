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
using ApplicationShared.Entites.Course;
using Domain.Entities;
using System.Collections.Generic;
using Application.Services;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Application.Resources;
using Application.Helpers;

namespace Application.Controllers.Courses
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly MonaContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly SmtpClient _smtp;
        public CourseController(MonaContext context,
            UserManager<User> userManager, IConfiguration configuration, SmtpClient smtp)

        {
            _context = context;
            _userManager = userManager;
            _smtp = smtp;
            _configuration = configuration;
        }

        // GET: api/Courses

        [Route("api/[controller]/[action]")]
        [HttpGet]
        public async Task<ActionResult<CourseOutputDto>> GetCourses()
        {
            try
            {
                var courses = new List<CourseDto>();
                var users = await _context.Users.Where(x => x.FullName != null).ToListAsync();
                var lessons = await _context.Lessons.Where(x => !x.VirtualDeleted).ToListAsync();
                var customers = await _context.Customers.Where(x => !x.VirtualDeleted).
                        Include(x => x.Courses).ThenInclude(c => c.InCharge).
                        Include(x => x.Courses).ThenInclude(c => c.Currency).
                        Include(x => x.Courses).ThenInclude(c => c.TypeOfExam).
                        Include(x => x.Courses).ThenInclude(c => c.TypeOfPacket).
                        Include(x => x.Courses).ThenInclude(c => c.PeroidBeforeSendEmail).ToListAsync();
                foreach (var customer in customers)
                {
                    if (customer.Courses.Any())
                    {
                        var all = customer.Courses.Select(x => new CourseDto()
                        {
                            Id = x.Id,
                            ExamDate = x.ExamDate.HasValue ? x.ExamDate.Value.ToString("G") : "",
                            ExamDateInDays = x.ExamDate.HasValue ? (x.ExamDate.Value - DateTime.Now.Date).TotalDays.ToString() : "0",
                            InChargeId = x.InChargeId,
                            InChargeName = users.SingleOrDefault(y => y.Id == x.InChargeId).FullName,
                            CustomerId = customer.Id,
                            CustomerName = customer.FullName,
                            NextLessonDate = lessons.Where(y => y.CourseId == x.Id).Any() &&
                            lessons.Any(y => y.CourseId == x.Id && y.Status == Domain.Enums.LessonStatus.NotStarted && y.Date.HasValue) ?
                            lessons.Where(y => y.CourseId == x.Id && y.Status == Domain.Enums.LessonStatus.NotStarted && y.Date.HasValue).OrderBy(y => y.Date).FirstOrDefault().Date.Value.ToString("yyyy/MM/dd") : "",
                            NextLessonTime = lessons.Where(y => y.CourseId == x.Id).Any() &&
                            lessons.Any(y => y.CourseId == x.Id && y.Status == Domain.Enums.LessonStatus.NotStarted && y.Date.HasValue && y.Time.HasValue) ?
                            lessons.Where(y => y.CourseId == x.Id && y.Status == Domain.Enums.LessonStatus.NotStarted && y.Date.HasValue && y.Time.HasValue).OrderBy(y => y.Date).ThenBy(y => y.Time).FirstOrDefault().Time.Value.ToString("HH:mm") : "",
                            LessonPeriod = x.LessonPeriod.ToString(),
                            Note = customer.Note,
                            RemainingMoney = x.RemainingMoney,
                            TakedHours = lessons.Where(y => y.CourseId == x.Id).Any() ?
                            SumLessonPeriod(lessons.Where(y => y.CourseId == x.Id && y.Status == Domain.Enums.LessonStatus.Finished).ToList()) : 0.00,
                            StartCourse = x.StartCourse.HasValue ? x.StartCourse.Value.ToString("G") : "",
                            Status = x.Status.ToString(),
                            TotalAmount = x.TotalAmount,
                            Currency = x.Currency == null ? new CurrencyDto() : new CurrencyDto()
                            {
                                Id = x.Currency.Id,
                                Name = x.Currency.Name,
                                ForeignName = x.Currency.ForeignName,
                                IsActive = x.Currency.IsActive,
                                Symbol = x.Currency.Symbol,
                            },
                            PeroidBeforeSendEmail = x.PeroidBeforeSendEmail == null ? new PeroidBeforeSendEmailDto() : new PeroidBeforeSendEmailDto()
                            {
                                Id = x.PeroidBeforeSendEmail.Id,
                                Name = x.PeroidBeforeSendEmail.Name,
                                ForeignName = x.PeroidBeforeSendEmail.ForeignName,
                                Hours = x.PeroidBeforeSendEmail.Hours,
                            },
                            TypeOfExam = x.TypeOfExam == null ? new TypeOfExamDto() : new TypeOfExamDto()
                            {
                                Id = x.TypeOfExam.Id,
                                Name = x.TypeOfExam.Name,
                                ForeignName = x.TypeOfExam.ForeignName,
                                Cost = x.TypeOfExam.Cost
                            },
                            TypeOfPacket = x.Currency == null ? new TypeOfPacketDto() : new TypeOfPacketDto()
                            {
                                Id = x.TypeOfPacket.Id,
                                Name = x.TypeOfPacket.Name,
                                ForeignName = x.TypeOfPacket.ForeignName,
                                Cost = x.TypeOfPacket.Cost
                            }
                        }).ToList();
                        courses.AddRange(all);
                    }
                    else
                    {
                        courses.Add(new CourseDto()
                        {
                            Id = 0,
                            ExamDate = "",
                            ExamDateInDays = "0",
                            InChargeId = "",
                            InChargeName = "",
                            CustomerId = customer.Id,
                            CustomerName = customer.FullName,
                            NextLessonDate = "",
                            LessonPeriod = "",
                            Note = customer.Note,
                            RemainingMoney = 0,
                            StartCourse = "",
                            Status = "",
                            TotalAmount = 0,
                            Currency = new CurrencyDto(),
                            PeroidBeforeSendEmail = new PeroidBeforeSendEmailDto(),
                            TypeOfExam = new TypeOfExamDto(),
                            TypeOfPacket = new TypeOfPacketDto()
                        });
                    }
                }

                var count = courses.Count;
                return new CourseOutputDto() { Courses = courses, AllCount = count };

            }
            catch (Exception ex)
            {
                return new CourseOutputDto() { Courses = new List<CourseDto>(), AllCount = 0 };
            }
        }

        private double SumLessonPeriod(List<Lesson> list)
        {
            var res = 0.00;
            foreach (var item in list)
            {
                res += LessonHelper.GetTimeOfLesson(item.LessonPeriod);
            }
            return res;
        }

        [Route("api/[controller]/[action]")]
        [HttpGet]
        public async Task<ActionResult<CourseOutputDto>> GetClientCourses(int customerId)
        {
            try
            {

                DateTime? datetime = null;
                var users = await _context.Users.Where(x => x.FullName != null).ToListAsync();
                var lessons = await _context.Lessons.Where(x => !x.VirtualDeleted).ToListAsync();
                var data = await _context.Courses.Where(x => !x.VirtualDeleted && x.CustomerId == customerId)
                    .Include(x => x.InCharge)
                    .Include(x => x.Currency)
                    .Include(x => x.Customer)
                    .Include(x => x.TypeOfExam)
                    .Include(x => x.TypeOfPacket)
                    .Include(x => x.PeroidBeforeSendEmail).ToListAsync();
                var all = data.Select(x => new CourseDto()
                {
                    Id = x.Id,
                    ExamDate = x.ExamDate.HasValue ? x.ExamDate.Value.ToString("G") : "",
                    ExamDateInDays = x.ExamDate.HasValue ? (x.ExamDate.Value - DateTime.Now.Date).TotalDays.ToString() : "0",
                    InChargeId = x.InChargeId,
                    InChargeName = users.SingleOrDefault(y => y.Id == x.InChargeId).FullName,
                    CustomerId = x.CustomerId.HasValue ? x.CustomerId.Value : 0,
                    CustomerName = x.Customer.FullName,
                    NextLessonDate = lessons.Where(y => y.CourseId == x.Id).Any() &&
                       lessons.Any(y => y.CourseId == x.Id && y.Status == Domain.Enums.LessonStatus.NotStarted && y.Date.HasValue) ?
                       lessons.Where(y => y.CourseId == x.Id && y.Status == Domain.Enums.LessonStatus.NotStarted && y.Date.HasValue).OrderBy(y => y.Date).FirstOrDefault().Date.Value.ToString("yyyy/MM/dd") : "",
                    NextLessonTime = lessons.Where(y => y.CourseId == x.Id).Any() &&
                       lessons.Any(y => y.CourseId == x.Id && y.Status == Domain.Enums.LessonStatus.NotStarted && y.Date.HasValue && y.Time.HasValue) ?
                       lessons.Where(y => y.CourseId == x.Id && y.Status == Domain.Enums.LessonStatus.NotStarted && y.Date.HasValue && y.Time.HasValue).OrderBy(y => y.Date).ThenBy(y => y.Time).FirstOrDefault().Time.Value.ToString("HH:mm") : "",
                    LessonPeriod = x.LessonPeriod.ToString(),
                    Note = x.Customer.Note,
                    RemainingMoney = x.RemainingMoney,
                    StartCourse = x.StartCourse.HasValue ? x.StartCourse.Value.ToString("G") : "",
                    Status = x.Status.ToString(),
                    TakedHours = lessons.Where(y => y.CourseId == x.Id).Any() ?
                            SumLessonPeriod(lessons.Where(y => y.CourseId == x.Id && y.Status == Domain.Enums.LessonStatus.Finished).ToList()) : 0.00,
                    TotalAmount = x.TotalAmount,
                    Currency = x.Currency == null ? new CurrencyDto() : new CurrencyDto()
                    {
                        Id = x.Currency.Id,
                        Name = x.Currency.Name,
                        ForeignName = x.Currency.ForeignName,
                        IsActive = x.Currency.IsActive,
                        Symbol = x.Currency.Symbol,
                    },
                    PeroidBeforeSendEmail = x.PeroidBeforeSendEmail == null ? new PeroidBeforeSendEmailDto() : new PeroidBeforeSendEmailDto()
                    {
                        Id = x.PeroidBeforeSendEmail.Id,
                        Name = x.PeroidBeforeSendEmail.Name,
                        ForeignName = x.PeroidBeforeSendEmail.ForeignName,
                        Hours = x.PeroidBeforeSendEmail.Hours,
                    },
                    TypeOfExam = x.TypeOfExam == null ? new TypeOfExamDto() : new TypeOfExamDto()
                    {
                        Id = x.TypeOfExam.Id,
                        Name = x.TypeOfExam.Name,
                        ForeignName = x.TypeOfExam.ForeignName,
                        Cost = x.TypeOfExam.Cost
                    },
                    TypeOfPacket = x.TypeOfPacket == null ? new TypeOfPacketDto() : new TypeOfPacketDto()
                    {
                        Id = x.TypeOfPacket.Id,
                        Name = x.TypeOfPacket.Name,
                        ForeignName = x.TypeOfPacket.ForeignName,
                        Cost = x.TypeOfPacket.Cost
                    }
                }).ToList();
                var count = data.Count;
                return new CourseOutputDto() { Courses = all, AllCount = count };

            }
            catch (Exception ex)
            {
                return new CourseOutputDto() { Courses = new List<CourseDto>(), AllCount = 0 };
            }
        }

        // GET: api/Courses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDto>> GetCourse(int id)
        {
            DateTime? datetime = null;
            var users = await _context.Users.Where(x => x.FullName != null).ToListAsync();
            var lessons = await _context.Lessons.Where(x => !x.VirtualDeleted && x.CourseId == id).ToListAsync();
            var course = await _context.Courses.Where(x=> x.Id == id).Include(x => x.InCharge)
                .Include(x => x.Currency)
                .Include(x => x.Customer)
                .Include(x => x.TypeOfExam)
                .Include(x => x.TypeOfPacket)
                .Include(x => x.PeroidBeforeSendEmail).FirstOrDefaultAsync();
            if (course == null)
            {
                return NotFound();
            }
            var result = new CourseDto()
            {
                Id = course.Id,
                ExamDate = course.ExamDate.HasValue ? course.ExamDate.Value.ToString("G") : "",
                ExamDateInDays = course.ExamDate.HasValue ? (course.ExamDate.Value - DateTime.Now.Date).TotalDays.ToString() : "0",
                InChargeId = course.InChargeId,
                NextLessonDate = lessons.Any() && lessons.OrderBy(x => x.Date).FirstOrDefault().Date.HasValue? lessons.OrderBy(x=> x.Date).FirstOrDefault().Date.Value.ToString("G"): "",
                NextLessonTime = lessons.Any() && lessons.OrderBy(x => x.Date).OrderBy(x => x.Time).FirstOrDefault().Time.HasValue ? lessons.OrderBy(x=> x.Date).OrderBy(x=> x.Time).FirstOrDefault().Time.Value.ToString("T") : "",
                InChargeName = users.SingleOrDefault(y => y.Id == course.InChargeId).FullName,
                CustomerId = course.CustomerId.HasValue ? course.CustomerId.Value : 0,
                LessonPeriod = course.LessonPeriod.ToString(),
                Note = course.Note,
                RemainingMoney = course.RemainingMoney,
                StartCourse = course.StartCourse.HasValue ? course.StartCourse.Value.ToString("G") : "",
                Status = course.Status.ToString(),
                TotalAmount = course.TotalAmount,
                Currency = course.Currency == null ? new CurrencyDto() : new CurrencyDto()
                {
                    Id = course.Currency.Id,
                    Name = course.Currency.Name,
                    ForeignName = course.Currency.ForeignName,
                    IsActive = course.Currency.IsActive,
                    Symbol = course.Currency.Symbol,
                },
                PeroidBeforeSendEmail = course.PeroidBeforeSendEmail == null ? new PeroidBeforeSendEmailDto() :  new PeroidBeforeSendEmailDto()
                {
                    Id = course.PeroidBeforeSendEmail.Id,
                    Name = course.PeroidBeforeSendEmail.Name,
                    ForeignName = course.PeroidBeforeSendEmail.ForeignName,
                    Hours = course.PeroidBeforeSendEmail.Hours,
                },
                TypeOfExam = course.TypeOfExam == null ? new TypeOfExamDto() :  new TypeOfExamDto()
                {
                    Id = course.TypeOfExam.Id,
                    Name = course.TypeOfExam.Name,
                    ForeignName = course.TypeOfExam.ForeignName,
                    Cost = course.TypeOfExam.Cost
                },
                TypeOfPacket = course.TypeOfPacket == null ? new TypeOfPacketDto() :  new TypeOfPacketDto()
                {
                    Id = course.TypeOfPacket.Id,
                    Name = course.TypeOfPacket.Name,
                    ForeignName = course.TypeOfPacket.ForeignName,
                    Cost = course.TypeOfPacket.Cost
                }
            };
            return result;
        }

        // PUT: api/Courses/5
        [HttpPut("{id}")]
        public async Task<ActionResult<CourseDto>> PutCourse(int id, CourseInputDto input)
        {
            DateTime? datetime = null;
            var users = await _context.Users.Where(x => x.FullName != null).ToListAsync();
            var currency = await _context.Currencies.Where(x => !x.VirtualDeleted && x.IsActive).FirstOrDefaultAsync();
            var defaultCurrency = await _context.Currencies.Where(x => !x.VirtualDeleted).FirstOrDefaultAsync();
            var lessons = await _context.Lessons.Where(x => !x.VirtualDeleted && x.CourseId == id).ToListAsync();
            var typeOfPacket = await _context.TypeOfPackets.FirstOrDefaultAsync(x => !x.VirtualDeleted && x.Id == input.TypeOfPacketId);
            var course = await _context.Courses.FindAsync(id);
            course.ExamDate = DateTimeString.TryParsingDate(input.ExamDate,false);
            course.InChargeId = input.InChargeId;
            course.CustomerId = input.CustomerId;
            course.LessonPeriod = input.LessonPeriod;
            course.Note = input.Note;
            course.RemainingMoney = input.TotalAmount - (course.TotalAmount - course.RemainingMoney);
            course.StartCourse = DateTimeString.TryParsingDate(input.StartCourse, false);
            course.TotalAmount = input.TotalAmount;
            course.CurrencyId = currency == null ? defaultCurrency?.Id : currency.Id;
            course.PeroidBeforeSendEmailId = input.PeroidBeforeSendEmailId;
            course.TypeOfExamId = input.TypeOfExamId;
            course.TypeOfPacketId = input.TypeOfPacketId;
            course.UpdatedUserId = input.UserId;
            course.UpdatedDate = DateTime.Now;
            if(typeOfPacket.LessonsCount > lessons.Count)
            {
                var lastLesson = await _context.Lessons.Where(x => !x.VirtualDeleted &&
                      x.CourseId == course.Id).OrderByDescending(x => x.Number).FirstOrDefaultAsync();
                var number = lastLesson.Number;
                course.Status = Domain.Enums.CourseStatus.Started;
                for (int i = 0; i < typeOfPacket.LessonsCount - lessons.Count; i++)
                {
                    number += 1;
                    var lesson = new Lesson()
                    {
                        Date = datetime,
                        Time = datetime,
                        InChargeId = input.InChargeId,
                        CourseId = id,
                        LessonPeriod = input.LessonPeriod,
                        Note = input.Note,
                        Number = number,
                        Status = Domain.Enums.LessonStatus.NotStarted,
                        CreatedDate = DateTime.Now,
                        CreatedUserId = input.UserId
                    };
                    _context.Lessons.Add(lesson);
                }
            }
            else if(!lessons.Any(x=> x.Status != Domain.Enums.LessonStatus.Active || x.Status != Domain.Enums.LessonStatus.NotStarted))
                course.Status = Domain.Enums.CourseStatus.Finished;
            _context.Entry(course).State = EntityState.Modified;
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == input.UserId);
            var log = new Log()
            {
                DateTime = DateTime.Now,
                TypeFullName = typeof(Course).FullName,
                Content = "@userName@updateAction@objTitle",
                TypeId = course.Id,
                UserId = user.Id
            };
            _context.Logs.Add(log);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            var result = new CourseDto();
            //var result = new CourseDto()
            //{
            //    Id = course.Id,
            //    ExamDate = course.ExamDate.HasValue ? course.ExamDate.Value.ToString("G") : "",
            //    ExamDateInDays = course.ExamDate.HasValue ? (course.ExamDate.Value - DateTime.Now.Date).TotalDays.ToString() : "0",
            //    InChargeId = course.InChargeId,
            //    NextLessonDate = lessons.Any() ? lessons.OrderBy(x => x.Date).FirstOrDefault().Date.Value.ToString("G") : "",
            //    NextLessonTime = lessons.Any() ? lessons.OrderBy(x => x.Date).OrderBy(x => x.Time).FirstOrDefault().Time.Value.ToString("T") : "",
            //    InChargeName = users.SingleOrDefault(y => y.Id == course.InChargeId).FullName,
            //    CustomerId = course.CustomerId.HasValue ? course.CustomerId.Value : 0,
            //    LessonPeriod = course.LessonPeriod.ToString(),
            //    Note = course.Note,
            //    RemainingMoney = course.RemainingMoney,
            //    StartCourse = course.StartCourse.HasValue ? course.StartCourse.Value.ToString("G") : "",
            //    Status = course.Status.ToString(),
            //    TotalAmount = course.TotalAmount,
            //    Currency = course.Currency == null ? new CurrencyDto() : new CurrencyDto()
            //    {
            //        Id = course.Currency.Id,
            //        Name = course.Currency.Name,
            //        ForeignName = course.Currency.ForeignName,
            //        IsActive = course.Currency.IsActive,
            //        Symbol = course.Currency.Symbol,
            //    },
            //    PeroidBeforeSendEmail = course.PeroidBeforeSendEmail == null ? new PeroidBeforeSendEmailDto() : new PeroidBeforeSendEmailDto()
            //    {
            //        Id = course.PeroidBeforeSendEmail.Id,
            //        Name = course.PeroidBeforeSendEmail.Name,
            //        ForeignName = course.PeroidBeforeSendEmail.ForeignName,
            //        Hours = course.PeroidBeforeSendEmail.Hours,
            //    },
            //    TypeOfExam = course.TypeOfExam == null ? new TypeOfExamDto() : new TypeOfExamDto()
            //    {
            //        Id = course.TypeOfExam.Id,
            //        Name = course.TypeOfExam.Name,
            //        ForeignName = course.TypeOfExam.ForeignName,
            //        Cost = course.TypeOfExam.Cost
            //    },
            //    TypeOfPacket = course.TypeOfPacket == null ? new TypeOfPacketDto() : new TypeOfPacketDto()
            //    {
            //        Id = course.TypeOfPacket.Id,
            //        Name = course.TypeOfPacket.Name,
            //        ForeignName = course.TypeOfPacket.ForeignName,
            //        Cost = course.TypeOfPacket.Cost
            //    }
            //};
            return result;
        }

        // cancel course

        [Route("api/[controller]/[action]")]
        [HttpPost]
        public async Task<ActionResult<bool>> CancelCourse(CourseCancelInputDto input)
        {
            var course = await _context.Courses.FindAsync(input.Id);
            course.Status = input.Status;
            _context.Entry(course).State = EntityState.Modified;
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == input.UserId);
            var log = new Log()
            {
                DateTime = DateTime.Now,
                TypeFullName = typeof(Course).FullName,
                Content = "@userName@cancelAction@objTitle",
                TypeId = course.Id,
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
        [Route("api/[controller]/[action]")]
        [HttpPost]
        public async Task<ActionResult<bool>> FinishCourse(CourseCancelInputDto input)
        {
            var course = await _context.Courses.FindAsync(input.Id);
            course.Status = input.Status;
            _context.Entry(course).State = EntityState.Modified;
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == input.UserId);
            var log = new Log()
            {
                DateTime = DateTime.Now,
                TypeFullName = typeof(Course).FullName,
                Content = "@userName@cancelAction@objTitle",
                TypeId = course.Id,
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
        //send Email about next Lesson
        [Route("api/[controller]/[action]")]
        [HttpPost]
        public async Task<ActionResult<bool>> SendNextLessonInfoEmail(int id, string userId)
        {
            EmailService emailService = new EmailService(_configuration, _smtp);
            var course = await _context.Courses.Where(x => x.Id == id).Include(x => x.InCharge)
                .Include(x => x.Currency)
                .Include(x => x.Customer)
                .Include(x => x.TypeOfExam)
                .Include(x => x.TypeOfPacket)
                .Include(x=> x.Lessons)
                .Include(x => x.PeroidBeforeSendEmail).FirstOrDefaultAsync();
            var nextLessonDate = course.Lessons.OrderByDescending(y => y.Date).FirstOrDefault().Date;
            var nextLessonTime = course.Lessons.OrderByDescending(y => y.Date).ThenByDescending(y => y.Time).FirstOrDefault().Time;
            var nextLessonDateTime = new DateTime(nextLessonDate.Value.Year,
                                                     nextLessonDate.Value.Month,
                                                     nextLessonDate.Value.Day,
                                                     nextLessonTime.Value.Hour,
                                                     nextLessonTime.Value.Minute, 0);
            var timeInTimes = nextLessonDate.HasValue && nextLessonTime.HasValue ? ((DateTime.Now.Date - nextLessonDate.Value).TotalHours + (DateTime.Now.Hour - nextLessonTime.Value.Hour)).ToString() : "0";
            string sbjCode = Localization.GetValue("NextLessonInfoEmailSubject", "");
            var email = course.Customer.Email;
            string htmlMsgCode = "<div style ='width: 100%; padding: 2%; margin: 2%;color:black;font-size: 13px; font-weight: 400;'>" +
                                    //"<div><a href=''><div><img src='https://sy-store.com/assets/logo.png' width='200' height='22'></div></a></div>" +
                                    "<br>" +
                                    "<br>" +
                                    "<div><h3 style='text-align: center;font-size: 23px;'> "+ Localization.GetValue("NextLessonInfoEmailTitle", "")  + "</h3></div>" +
                                    "<br>" +
                                    "<br>" +
                                    "<div><span style='color: black; font-size: 15px; font-weight: 600;'>"+ Localization.GetValue("Dear", "") + " " + course.Customer.FullName + ", </span></div>" +
                                    "<br>" +
                                    "<div><span>"+ Localization.GetValue("YouHaveALessonAtMonaSchoolAfter", "")+ timeInTimes + " " + Localization.GetValue("Hours", "") + "</span></div>" +
                                    "<br>" +
                                    "<div><span>"+ Localization.GetValue("LessonDate", "") + "<br>" + nextLessonDateTime.ToString("G")  + "</span></div>" +
                                    "<br>" +
                                    "<div><span>"+ Localization.GetValue("PleaseBeOnTime", "")  + "</span></div>" +
                                    "<br>" +
                                    "<div><span>" + Localization.GetValue("WithBestRegards", "") + "</span></div>" +
                                    "</div>";
            var isSent = await emailService.SendEmail(email, sbjCode, htmlMsgCode);
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            var log = new Log()
            {
                DateTime = DateTime.Now,
                TypeFullName = typeof(Course).FullName,
                Content = "@userName@sendNextLessonInfoEmailAction@objTitle",
                TypeId = course.Id,
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

        //send Email about Payment
        [Route("api/[controller]/[action]")]
        [HttpPost]
        public async Task<ActionResult<bool>> SendRemainingMoneyInfoEmail(int id, string userId)
        {
            EmailService emailService = new EmailService(_configuration, _smtp);
            var course = await _context.Courses.Where(x => x.Id == id).Include(x => x.InCharge)
                .Include(x => x.Currency)
                .Include(x => x.Customer)
                .Include(x => x.TypeOfExam)
                .Include(x => x.TypeOfPacket)
                .Include(x => x.Payments)
                .Include(x => x.Lessons)
                .Include(x => x.PeroidBeforeSendEmail).FirstOrDefaultAsync();

            string sbjCode = Localization.GetValue("RemainingMoneyInfoEmailSubject", "");
            var email = course.Customer.Email;
            string htmlMsgCode = "<div style ='width: 100%; padding: 2%; margin: 2%;color:black;font-size: 13px; font-weight: 400;'>" +
                                    //"<div><a href=''><div><img src='https://sy-store.com/assets/logo.png' width='200' height='22'></div></a></div>" +
                                    "<br>" +
                                    "<br>" +
                                    "<div><h3 style='text-align: center;font-size: 23px;'> " + Localization.GetValue("RemainingMoneyInfoEmailTitle", "") + "</h3></div>" +
                                    "<br>" +
                                    "<br>" +
                                    "<div><span style='color: black; font-size: 15px; font-weight: 600;'>" + Localization.GetValue("Dear", "") + " " + course.Customer.FullName + ", </span></div>" +
                                    "<br>" +
                                    "<div><span>" + Localization.GetValue("TheRemainigMoneyOfyourCourseInMonaSchoolIs", "") + " "+ course.RemainingMoney +" " + Localization.GetValue("Euros", "") + Localization.GetValue("PleasePayYourRemainingFees", "") + "</span></div>" +
                                    "<br>" +
                                    "<div><span>" + Localization.GetValue("ThankYouForYourInterestInOurDrivingSchool", "") + "</span></div>" +
                                    "<br>" +
                                    "<div><span>" + Localization.GetValue("WithBestRegards", "") + "</span></div>" +
                                    "<br>" +
                                    "<div><span>" + "Monarijschool" + "</span></div>" +
                                    "</div>";
            var isSent = await emailService.SendEmail(email, sbjCode, htmlMsgCode);
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            var log = new Log()
            {
                DateTime = DateTime.Now,
                TypeFullName = typeof(Course).FullName,
                Content = "@userName@sendRemainingMoneyInfoEmailAction@objTitle",
                TypeId = course.Id,
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

        // POST: api/Courses

        [Route("api/[controller]/[action]")]
        [HttpPost]
        public async Task<ActionResult<CourseDto>> PostCourse(CourseInputDto input)
        {
            DateTime? datetime = null;
            var users = await _context.Users.Where(x => x.FullName != null).ToListAsync();
            var currency = await _context.Currencies.Where(x => !x.VirtualDeleted && x.IsActive).FirstOrDefaultAsync();
            var defaultCurrency = await _context.Currencies.Where(x => !x.VirtualDeleted).FirstOrDefaultAsync();
            try
            {
                var typeOfPacket = await _context.TypeOfPackets.FirstOrDefaultAsync(x => !x.VirtualDeleted && x.Id == input.TypeOfPacketId);
                var course = new Course()
                {
                    ExamDate = DateTimeString.TryParsingDate(input.ExamDate, false),
                    InChargeId = input.InChargeId,
                    CustomerId = input.CustomerId,
                    LessonPeriod = input.LessonPeriod,
                    Note = input.Note,
                    StartCourse = DateTimeString.TryParsingDate(input.StartCourse, false),
                    TotalAmount = input.TotalAmount,
                    CurrencyId = currency == null ? defaultCurrency?.Id : currency.Id,
                    PeroidBeforeSendEmailId = input.PeroidBeforeSendEmailId,
                    TypeOfExamId = input.TypeOfExamId,
                    TypeOfPacketId = input.TypeOfPacketId,
                    CreatedDate = DateTime.Now,
                    CreatedUserId = input.UserId
                };
                course.RemainingMoney = course.TotalAmount;
                course.Status = Domain.Enums.CourseStatus.NotStarted;
                _context.Courses.Add(course);
                await _context.SaveChangesAsync();
                for (int i = 0; i < typeOfPacket.LessonsCount; i++)
                {
                    var lesson = new Lesson()
                    {
                        Date = datetime,
                        Time = datetime,
                        InChargeId = input.InChargeId,
                        CourseId = course.Id,
                        Number = i + 1,
                        LessonPeriod = input.LessonPeriod,
                        Note = input.Note,
                        Status = Domain.Enums.LessonStatus.NotStarted,
                        CreatedDate = DateTime.Now,
                        CreatedUserId = input.UserId
                    };
                    _context.Lessons.Add(lesson);
                }
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == input.UserId);
                var log = new Log()
                {
                    DateTime = DateTime.Now,
                    TypeFullName = typeof(Course).FullName,
                    Content = "@userName@addAction@objTitle",
                    TypeId = course.Id,
                    UserId = user.Id
                };
                _context.Logs.Add(log);
                await _context.SaveChangesAsync();
                course = await _context.Courses.Where(x => x.Id == course.Id).Include(x => x.InCharge)
                .Include(x => x.Currency)
                .Include(x => x.Customer)
                .Include(x => x.TypeOfExam)
                .Include(x => x.TypeOfPacket)
                .Include(x => x.PeroidBeforeSendEmail).FirstOrDefaultAsync();
                var result = new CourseDto()
                {
                    Id = course.Id,
                    ExamDate = course.ExamDate.HasValue ? course.ExamDate.Value.ToString("G") : "",
                    ExamDateInDays = course.ExamDate.HasValue ? (DateTime.Now.Date - course.ExamDate.Value).TotalDays.ToString() : "0",
                    InChargeId = course.InChargeId,
                    InChargeName = users.SingleOrDefault(y => y.Id == course.InChargeId).FullName,
                    CustomerId = course.CustomerId.HasValue ? course.CustomerId.Value : 0,
                    LessonPeriod = course.LessonPeriod.ToString(),
                    Note = course.Note,
                    RemainingMoney = course.RemainingMoney,
                    StartCourse = course.StartCourse.HasValue ? course.StartCourse.Value.ToString("G") : "",
                    Status = course.Status.ToString(),
                    TotalAmount = course.TotalAmount,
                    Currency = course.Currency == null ? new CurrencyDto() : new CurrencyDto()
                    {
                        Id = course.Currency.Id,
                        Name = course.Currency.Name,
                        ForeignName = course.Currency.ForeignName,
                        IsActive = course.Currency.IsActive,
                        Symbol = course.Currency.Symbol,
                    },
                    PeroidBeforeSendEmail = course.PeroidBeforeSendEmail == null ? new PeroidBeforeSendEmailDto() : new PeroidBeforeSendEmailDto()
                    {
                        Id = course.PeroidBeforeSendEmail.Id,
                        Name = course.PeroidBeforeSendEmail.Name,
                        ForeignName = course.PeroidBeforeSendEmail.ForeignName,
                        Hours = course.PeroidBeforeSendEmail.Hours,
                    },
                    TypeOfExam = course.TypeOfExam == null ? new TypeOfExamDto() : new TypeOfExamDto()
                    {
                        Id = course.TypeOfExam.Id,
                        Name = course.TypeOfExam.Name,
                        ForeignName = course.TypeOfExam.ForeignName,
                        Cost = course.TypeOfExam.Cost
                    },
                    TypeOfPacket = course.TypeOfPacket == null ? new TypeOfPacketDto() : new TypeOfPacketDto()
                    {
                        Id = course.TypeOfPacket.Id,
                        Name = course.TypeOfPacket.Name,
                        ForeignName = course.TypeOfPacket.ForeignName,
                        Cost = course.TypeOfPacket.Cost
                    }
                };
                return result;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //Delete filtered Ids 
        [Route("api/[controller]/[action]")]
        [HttpPost]
        public async Task<ActionResult<bool>> DeleteFilterdCourses(IList<int> list, string userId)
        {
            var courses = list.Count > 0 ? await _context.Courses.Where(x=> list.Any(y=> y == x.Id)).ToListAsync() : await _context.Courses.Where(x => !x.VirtualDeleted).ToListAsync();
            if (courses.Count == 0)
            {
                return NotFound();
            }
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            foreach (var course in courses)
            {
                course.DeletedDate = DateTime.Now;
                course.DeletedUserId = userId;
                course.VirtualDeleted = true;
                course.Lessons = new List<Lesson>();
                _context.Entry(course).State = EntityState.Modified; 
                var log = new Log()
                {
                    DateTime = DateTime.Now,
                    TypeFullName = typeof(Course).FullName,
                    Content = "@userName@deleteAction@objTitle",
                    TypeId = course.Id,
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

        // DELETE: api/Courses/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<CourseDto>> DeleteCourse(int id, string userId)
        {
            DateTime? datetime = null;
            var users = await _context.Users.Where(x => x.FullName != null).ToListAsync();
            var course = await _context.Courses.Where(x => x.Id == id).Include(x => x.InCharge)
                .Include(x => x.Currency)
                .Include(x => x.Customer)
                .Include(x => x.TypeOfExam)
                .Include(x => x.TypeOfPacket)
                .Include(x => x.PeroidBeforeSendEmail).FirstOrDefaultAsync(); ;
            if (course == null)
            {
                return NotFound();
            }
            course.DeletedDate = DateTime.Now;
            course.DeletedUserId = userId;
            course.VirtualDeleted = true;
            _context.Entry(course).State = EntityState.Modified;
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            var log = new Log()
            {
                DateTime = DateTime.Now,
                TypeFullName = typeof(Course).FullName,
                Content = "@userName@deleteAction@objTitle",
                TypeId = course.Id,
                UserId = user.Id
            };
            _context.Logs.Add(log);


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            var result = new CourseDto()
            {
                Id = course.Id,
                ExamDate = course.ExamDate.HasValue ? course.ExamDate.Value.ToString("G") : "",
                ExamDateInDays = course.ExamDate.HasValue ? (DateTime.Now.Date - course.ExamDate.Value).TotalDays.ToString() : "0",
                InChargeId = course.InChargeId,
                InChargeName = users.SingleOrDefault(y => y.Id == course.InChargeId).FullName,
                CustomerId = course.CustomerId.HasValue ? course.CustomerId.Value : 0,
                LessonPeriod = course.LessonPeriod.ToString(),
                Note = course.Note,
                RemainingMoney = course.RemainingMoney,
                StartCourse = course.StartCourse.HasValue ? course.StartCourse.Value.ToString("G") : "",
                Status = course.Status.ToString(),
                TotalAmount = course.TotalAmount,
                Currency = new CurrencyDto()
                {
                    Id = course.Currency.Id,
                    Name = course.Currency.Name,
                    ForeignName = course.Currency.ForeignName,
                    IsActive = course.Currency.IsActive,
                    Symbol = course.Currency.Symbol,
                },
                PeroidBeforeSendEmail = new PeroidBeforeSendEmailDto()
                {
                    Id = course.PeroidBeforeSendEmail.Id,
                    Name = course.PeroidBeforeSendEmail.Name,
                    ForeignName = course.PeroidBeforeSendEmail.ForeignName,
                    Hours = course.PeroidBeforeSendEmail.Hours,
                },
                TypeOfExam = new TypeOfExamDto()
                {
                    Id = course.TypeOfExam.Id,
                    Name = course.TypeOfExam.Name,
                    ForeignName = course.TypeOfExam.ForeignName,
                    Cost = course.TypeOfExam.Cost
                },
                TypeOfPacket = new TypeOfPacketDto()
                {
                    Id = course.TypeOfPacket.Id,
                    Name = course.TypeOfPacket.Name,
                    ForeignName = course.TypeOfPacket.ForeignName,
                    Cost = course.TypeOfPacket.Cost
                }
            };
            return result;
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
    }
}
