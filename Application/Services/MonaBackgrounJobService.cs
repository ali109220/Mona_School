using Application.Helpers;
using Application.Resources;
using Core.SharedDomain.Security;
using Domain.Entities;
using Domain.Enums;
using Domain.Settings;
using EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Application.Services
{
    public class MonaBackgrounJobService : IMonaBackgrounJobService
    {
        private readonly MonaContext _context;
        private readonly UserManager<User> _userManager; 
        private readonly IConfiguration _configuration;
        private readonly SmtpClient _smtp;
        public MonaBackgrounJobService(MonaContext context,
            UserManager<User> userManager,
            IConfiguration configuration,
            SmtpClient smtp)

        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _smtp = smtp;
        }
        public async Task<bool> UpdateObjStatus()
        {
            try
            {
                var courses = await _context.Courses
                      .Where(x => !x.VirtualDeleted &&
                      (x.Status == CourseStatus.NotStarted ||
                       x.Status == CourseStatus.Started))
                      .Include(x => x.Customer)
                      .Include(x => x.Lessons)
                      .ToListAsync();
                foreach (var course in courses)
                {
                    if (course.Status == CourseStatus.NotStarted)
                    {
                        course.Status = course.StartCourse.HasValue && course.StartCourse.Value <= DateTime.Now.Date ?
                            CourseStatus.Started :
                            CourseStatus.NotStarted;
                        _context.Entry(course).State = EntityState.Modified;
                    }
                    else
                    {
                        if (course.Lessons.Any(x => x.Status == LessonStatus.NotStarted || x.Status == LessonStatus.Active))
                        {
                            var nextLessonNotStatrted = course.Lessons.Any(x => !x.VirtualDeleted && x.Status == LessonStatus.NotStarted) ?
                            course.Lessons.Where(x => x.Status == LessonStatus.NotStarted && x.Date.HasValue).OrderBy(y => y.Date).ThenBy(y => y.Time).FirstOrDefault() : null;
                            var nextLessonStatrted = course.Lessons.Any(x => !x.VirtualDeleted && x.Status == LessonStatus.Active) ?
                                course.Lessons.Where(x => x.Status == LessonStatus.Active && x.Date.HasValue).OrderBy(y => y.Date).ThenBy(y => y.Time).FirstOrDefault() : null;
                            var timeAfterAddLessonTime = nextLessonStatrted != null ? DateTime.Now.AddHours(LessonHelper.GetTimeOfLesson(nextLessonStatrted.LessonPeriod)) : DateTime.Now;
                            if (nextLessonNotStatrted != null &&
                                nextLessonNotStatrted.Date.HasValue &&
                                nextLessonNotStatrted.Time.HasValue &&
                                (nextLessonNotStatrted.Date.Value < DateTime.Now.Date ||
                                (nextLessonNotStatrted.Date.Value == DateTime.Now.Date && nextLessonNotStatrted.Time.Value.TimeOfDay <= DateTime.Now.TimeOfDay)))
                            {
                                nextLessonNotStatrted.Status = LessonStatus.Active;
                                _context.Entry(nextLessonNotStatrted).State = EntityState.Modified;
                            }
                            //if (nextLessonStatrted != null &&
                            //    nextLessonStatrted.Date.HasValue &&
                            //    nextLessonStatrted.Time.HasValue &&
                            //    (nextLessonStatrted.Date.Value < DateTime.Now.Date ||
                            //    (nextLessonStatrted.Date.Value == DateTime.Now.Date && nextLessonStatrted.Time.Value.TimeOfDay <= timeAfterAddLessonTime.TimeOfDay)))
                            //{
                            //    nextLessonStatrted.Status = LessonStatus.Finished;
                            //    _context.Entry(nextLessonStatrted).State = EntityState.Modified;
                            //}
                        }
                    }
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> SendNextLessonEmail()
        {
            try
            {
                EmailService emailService = new EmailService(_configuration, _smtp);
                var courses = await _context.Courses
                      .Where(x => !x.VirtualDeleted && x.PeroidBeforeSendEmailId != null &&
                       x.Status == CourseStatus.Started)
                      .Include(x => x.Lessons)
                      .Include(x=> x.Customer)
                      .Include(x=> x.PeroidBeforeSendEmail).ToListAsync();
                foreach (var course in courses)
                {
                    var hoursToSendEmail = course.PeroidBeforeSendEmail.Hours;
                    var nextLessonNotStatrted = course.Lessons.Any(x => !x.VirtualDeleted && x.Status == LessonStatus.NotStarted) ?
                        course.Lessons.Where(x => x.Status == LessonStatus.NotStarted && x.Date.HasValue).OrderBy(y => y.Date).ThenBy(y => y.Time).FirstOrDefault() : null;
                    if(nextLessonNotStatrted != null &&
                            nextLessonNotStatrted.Date.HasValue &&
                            nextLessonNotStatrted.Time.HasValue)
                    {
                        var nextLessonDateTime = new DateTime(nextLessonNotStatrted.Date.Value.Year,
                                                     nextLessonNotStatrted.Date.Value.Month,
                                                     nextLessonNotStatrted.Date.Value.Day,
                                                     nextLessonNotStatrted.Time.Value.Hour,
                                                     nextLessonNotStatrted.Time.Value.Minute, 0);
                        var timeToSendEmail = nextLessonNotStatrted != null ? DateTime.Now.AddHours(hoursToSendEmail) : DateTime.Now;
                        if (timeToSendEmail >= nextLessonDateTime.AddMinutes(-3) && timeToSendEmail < nextLessonDateTime.AddMinutes(2))
                        {
                            string sbjCode = Localization.GetValue("NextLessonInfoEmailSubject", "");
                            var email = course.Customer.Email;
                            string htmlMsgCode = "<div style ='width: 100%; padding: 2%; margin: 2%;color:black;font-size: 13px; font-weight: 400;'>" +
                                                    "<div><a href=''><div><img src='https://sy-store.com/assets/logo.png' width='200' height='22'></div></a></div>" +
                                                    "<br>" +
                                                    "<br>" +
                                                    "<div><h3 style='text-align: center;font-size: 23px;'> " + Localization.GetValue("NextLessonInfoEmailTitle", "") + "</h3></div>" +
                                                    "<br>" +
                                                    "<br>" +
                                                    "<div><span style='color: black; font-size: 15px; font-weight: 600;'>" + Localization.GetValue("Dear", "") + " " + course.Customer.FullName + ", </span></div>" +
                                                    "<br>" +
                                                    "<div><span>" + Localization.GetValue("YouHaveALessonAtMonaSchoolAfter", "") + hoursToSendEmail + " " + Localization.GetValue("Hours", "") + "</span></div>" +
                                                    "<br>" +
                                                    "<div><span>" + Localization.GetValue("LessonDate", "") + "<br>" + nextLessonDateTime.ToString("G") + "</span></div>" +
                                                    "<br>" +
                                                    "<div><span>" + Localization.GetValue("PleaseBeOnTime", "") + "</span></div>" +
                                                    "<br>" +
                                                    "<div><span>" + Localization.GetValue("InCaseOfAnyProblemDoNotHesitateToContactUsAt", "") + " < span style='text-decoration: underline;'>info@mona.com . </span></span></div>" +
                                                    "</div>";
                            var isSent = await emailService.SendEmail(email, sbjCode, htmlMsgCode);
                            var user = await _context.Users.FirstOrDefaultAsync();
                            var log = new Log()
                            {
                                DateTime = DateTime.Now,
                                TypeFullName = typeof(Lesson).FullName,
                                Content = "@userName@sendAutoNextLessonInfoEmailAction@objTitle",
                                TypeId = nextLessonNotStatrted.Id,
                                UserId = user.Id
                            };
                        }
                    }
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

    }
}
