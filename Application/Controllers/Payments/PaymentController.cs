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
using ApplicationShared.Entites.Course.Payment;
using Application.Services;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Application.Resources;

namespace Application.Controllers.Payments
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly MonaContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly SmtpClient _smtp;
        public PaymentController(MonaContext context,
            UserManager<User> userManager, IConfiguration configuration, SmtpClient smtp)

        {
            _context = context;
            _userManager = userManager;
            _smtp = smtp;
            _configuration = configuration;
        }

        // GET: api/Payments
        [HttpGet]
        public async Task<ActionResult<PaymentOutputDto>> GetPayments(int courseId)
        {
            var users = await _context.Users.Where(x => x.FullName != null).ToListAsync();
            var data = await _context.Payments.Where(x => !x.VirtualDeleted && x.CourseId == courseId).ToListAsync();
            var all = data.OrderByDescending(x=> x.Id).Select(x => new PaymentDto()
            {
                Id = x.Id,
                HandOverByUserId = x.HandOverByUserId,
                HandOverByUserName = users.SingleOrDefault(y=> y.Id == x.HandOverByUserId).FullName,
                PaymentAmount = x.PaymentAmount,
                PaymentDate = x.PaymentDate.HasValue ? x.PaymentDate.Value.ToString("yyyy/MM/dd") : "",
                PaymentTime = x.PaymentTime.HasValue ? x.PaymentTime.Value.ToString("HH:mm") : "",
                RemainingMoney = x.RemainingMoney,
                Note = x.Note,
                CourseId = x.CourseId.HasValue? x.CourseId.Value:0,

            }).ToList();
            var count = data.Count;
            return new PaymentOutputDto() { Payments = all, AllCount = count};
        }

        // GET: api/Payments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentDto>> GetPayment(int id)
        {
            DateTime? datetime = null;
            var users = await _context.Users.Where(x => x.FullName != null).ToListAsync();
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            var result = new PaymentDto()
            {
                Id = payment.Id,
                HandOverByUserId = payment.HandOverByUserId,
                HandOverByUserName = users.SingleOrDefault(y => y.Id == payment.HandOverByUserId).FullName,
                PaymentAmount = payment.PaymentAmount,
                PaymentDate = payment.PaymentDate.HasValue ? payment.PaymentDate.Value.ToString("yyyy/MM/dd") : "",
                PaymentTime = payment.PaymentTime.HasValue ? payment.PaymentTime.Value.ToString("HH:mm") : "",
                RemainingMoney = payment.RemainingMoney,
                Note = payment.Note,
                CourseId = payment.CourseId.HasValue ? payment.CourseId.Value : 0,
            };
            return result;
        }

        // PUT: api/Payments/5
        [HttpPut("{id}")]
        public async Task<ActionResult<PaymentDto>> PutPayment(int id, PaymentInputDto input)
        {
            EmailService emailService = new EmailService(_configuration, _smtp);
            DateTime? datetime = null;
            var users = await _context.Users.Where(x => x.FullName != null).ToListAsync();
            var payment = await _context.Payments.FindAsync(id);
            var updatedAmount = payment.PaymentAmount != input.PaymentAmount;
            payment.PaymentAmount = input.PaymentAmount;
            payment.HandOverByUserId = input.HandOverByUserId;
            payment.CourseId = input.CourseId;
            payment.Note = input.Note;
            payment.PaymentDate = DateTimeString.TryParsingDate(input.PaymentDate,false);
            payment.PaymentTime = DateTimeString.TryParsingDate(input.PaymentTime,true);
            payment.UpdatedUserId = input.UserId;
            payment.UpdatedDate = DateTime.Now;
            var course = await _context.Courses.Where(x => x.Id == input.CourseId).Include(x => x.InCharge)
                .Include(x => x.Currency)
                .Include(x => x.Customer)
                .Include(x => x.TypeOfExam)
                .Include(x => x.TypeOfPacket)
                .Include(x => x.Payments)
                .Include(x => x.Lessons)
                .Include(x => x.PeroidBeforeSendEmail).FirstOrDefaultAsync();
            var previousPayments = await _context.Payments.Where(x => !x.VirtualDeleted && x.CourseId == input.CourseId && x.Id != id && (x.PaymentDate < payment.PaymentDate|| (x.PaymentDate == payment.PaymentDate && x.PaymentTime < payment.PaymentTime))).ToListAsync();
            var allPreviousPayments = await _context.Payments.Where(x => !x.VirtualDeleted && x.Id != id && x.CourseId == input.CourseId).ToListAsync();
            payment.RemainingMoney = previousPayments.Any() ? course.TotalAmount - previousPayments.Sum(x => x.PaymentAmount) - payment.PaymentAmount : course.TotalAmount - payment.PaymentAmount;
            course.RemainingMoney = allPreviousPayments.Any() ? course.TotalAmount - allPreviousPayments.Sum(x => x.PaymentAmount) - payment.PaymentAmount : course.TotalAmount - payment.PaymentAmount;
            _context.Entry(payment).State = EntityState.Modified;
            _context.Entry(course).State = EntityState.Modified;
            string sbjCode = Localization.GetValue("UpdatingPaymentSubject", "");
            var email = course.Customer.Email;
            string htmlMsgCode = "<div style ='width: 100%; padding: 2%; margin: 2%;color:black;font-size: 13px; font-weight: 400;'>" +
                                    //"<div><a href=''><div><img src='https://sy-store.com/assets/logo.png' width='200' height='22'></div></a></div>" +
                                    "<br>" +
                                    "<br>" +
                                    "<div><h3 style='text-align: center;font-size: 23px;'> " + Localization.GetValue("UpdatingPaymentTitle", "") + "</h3></div>" +
                                    "<br>" +
                                    "<br>" +
                                    "<div><span style='color: black; font-size: 15px; font-weight: 600;'>" + Localization.GetValue("Dear", "") + " " + course.Customer.FullName + ", </span></div>" +
                                    "<br>" +
                                    "<div><span>" + Localization.GetValue("YouHavePaidOnUpdating", "") + " " + payment.PaymentAmount + " " + Localization.GetValue("EurosAccordingToOurAdministration", "") + Localization.GetValue("ThankYouForYourInterestInOurDrivingSchool", "") + "</span></div>" +
                                    "<br>" +
                                    "<div><span>" + Localization.GetValue("WithBestRegards", "") + "</span></div>" +
                                    "<br>" +
                                    "<div><span>" + "Monarijschool" + "</span></div>" +
                                    "</div>";
            if(updatedAmount)
                await emailService.SendEmail(email, sbjCode, htmlMsgCode);
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == input.UserId);
            var log = new Log()
            {
                DateTime = DateTime.Now,
                TypeFullName = typeof(Payment).FullName,
                Content = "@userName@updateAction@objTitle",
                TypeId = payment.Id,
                UserId = user.Id
            };
            _context.Logs.Add(log);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            var result = new PaymentDto()
            {
                Id = payment.Id,
                HandOverByUserId = payment.HandOverByUserId,
                HandOverByUserName = users.SingleOrDefault(y => y.Id == payment.HandOverByUserId).FullName,
                PaymentAmount = payment.PaymentAmount,
                PaymentDate = payment.PaymentDate.HasValue ? payment.PaymentDate.Value.ToString("yyyy/MM/dd") : "",
                PaymentTime = payment.PaymentTime.HasValue ? payment.PaymentTime.Value.ToString("HH:mm") : "",
                RemainingMoney = payment.RemainingMoney,
                Note = payment.Note,
                CourseId = payment.CourseId.HasValue ? payment.CourseId.Value : 0,
            };
            return result;
        }

        // POST: api/Payments
        [HttpPost]
        public async Task<ActionResult<PaymentDto>> PostPayment(PaymentInputDto input)
        {
            EmailService emailService = new EmailService(_configuration, _smtp);
            DateTime? datetime = null;
            var users = await _context.Users.Where(x => x.FullName != null).ToListAsync();
            try
            {
                var payment = new Payment()
                {
                    PaymentDate = DateTimeString.TryParsingDate(input.PaymentDate, false),
                    PaymentTime = DateTimeString.TryParsingDate(input.PaymentTime, true),
                    PaymentAmount = input.PaymentAmount,
                    HandOverByUserId = input.HandOverByUserId,
                    CourseId = input.CourseId,
                    Note = input.Note,
                    CreatedDate = DateTime.Now,
                    CreatedUserId = input.UserId
                };
                var course = await _context.Courses.Where(x => x.Id == input.CourseId).Include(x => x.InCharge)
                .Include(x => x.Currency)
                .Include(x => x.Customer)
                .Include(x => x.TypeOfExam)
                .Include(x => x.TypeOfPacket)
                .Include(x => x.Payments)
                .Include(x => x.Lessons)
                .Include(x => x.PeroidBeforeSendEmail).FirstOrDefaultAsync();
                var allPreviousPayments = await _context.Payments.Where(x => !x.VirtualDeleted && x.CourseId == input.CourseId).ToListAsync();
                payment.RemainingMoney = allPreviousPayments.Any() ? course.TotalAmount - allPreviousPayments.Sum(x => x.PaymentAmount) - payment.PaymentAmount : course.TotalAmount - payment.PaymentAmount;
                course.RemainingMoney = allPreviousPayments.Any() ? course.TotalAmount - allPreviousPayments.Sum(x => x.PaymentAmount) - payment.PaymentAmount : course.TotalAmount - payment.PaymentAmount;
                _context.Payments.Add(payment);
                _context.Entry(course).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                string sbjCode = Localization.GetValue("AddingPaymentSubject", "");
                var email = course.Customer.Email;
                string htmlMsgCode = "<div style ='width: 100%; padding: 2%; margin: 2%;color:black;font-size: 13px; font-weight: 400;'>" +
                                        //"<div><a href=''><div><img src='https://sy-store.com/assets/logo.png' width='200' height='22'></div></a></div>" +
                                        "<br>" +
                                        "<br>" +
                                        "<div><h3 style='text-align: center;font-size: 23px;'> " + Localization.GetValue("AddingPaymentTitle", "") + "</h3></div>" +
                                        "<br>" +
                                        "<br>" +
                                        "<div><span style='color: black; font-size: 15px; font-weight: 600;'>" + Localization.GetValue("Dear", "") + " " + course.Customer.FullName + ", </span></div>" +
                                        "<br>" +
                                        "<div><span>" + Localization.GetValue("YouHavePaid", "") + " " + payment.PaymentAmount + " " + Localization.GetValue("EurosAccordingToOurAdministration", "") + Localization.GetValue("ThankYouForYourInterestInOurDrivingSchool", "") + "</span></div>" +
                                        "<br>" +
                                        "<div><span>" + Localization.GetValue("WithBestRegards", "") + "</span></div>" +
                                        "<br>" +
                                        "<div><span>" + "Monarijschool" + "</span></div>" +
                                        "</div>";
                await emailService.SendEmail(email, sbjCode, htmlMsgCode);
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == input.UserId);
                var log = new Log()
                {
                    DateTime = DateTime.Now,
                    TypeFullName = typeof(Payment).FullName,
                    Content = "@userName@addAction@objTitle",
                    TypeId = payment.Id,
                    UserId = user.Id
                };
                _context.Logs.Add(log);
                await _context.SaveChangesAsync();
                var result = new PaymentDto()
                {
                    Id = payment.Id,
                    HandOverByUserId = payment.HandOverByUserId,
                    HandOverByUserName = users.SingleOrDefault(y => y.Id == payment.HandOverByUserId).FullName,
                    PaymentAmount = payment.PaymentAmount,
                    PaymentDate = payment.PaymentDate.HasValue ? payment.PaymentDate.Value.ToString("yyyy/MM/dd") : "",
                    PaymentTime = payment.PaymentTime.HasValue ? payment.PaymentTime.Value.ToString("HH:mm") : "",
                    RemainingMoney = payment.RemainingMoney,
                    Note = payment.Note,
                    CourseId = payment.CourseId.HasValue ? payment.CourseId.Value : 0,
                };
                return result;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // DELETE: api/Payments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<PaymentDto>> DeletePayment(int id, string userId)
        {
            var users = await _context.Users.Where(x => x.FullName != null).ToListAsync();
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            payment.DeletedDate = DateTime.Now;
            payment.DeletedUserId = userId;
            payment.VirtualDeleted = true;
            _context.Entry(payment).State = EntityState.Modified;
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            var log = new Log()
            {
                DateTime = DateTime.Now,
                TypeFullName = typeof(Payment).FullName,
                Content = "@userName@deleteAction@objTitle",
                TypeId = payment.Id,
                UserId = user.Id
            };
            _context.Logs.Add(log);


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            var result = new PaymentDto()
            {
                Id = payment.Id,
                HandOverByUserId = payment.HandOverByUserId,
                HandOverByUserName = users.SingleOrDefault(y => y.Id == payment.HandOverByUserId).FullName,
                PaymentAmount = payment.PaymentAmount,
                PaymentDate = payment.PaymentDate.HasValue ? payment.PaymentDate.Value.ToString("yyyy/MM/dd") : "",
                PaymentTime = payment.PaymentTime.HasValue ? payment.PaymentTime.Value.ToString("HH:mm") : "",
                RemainingMoney = payment.RemainingMoney,
                Note = payment.Note,
                CourseId = payment.CourseId.HasValue ? payment.CourseId.Value : 0,
            };
            return result;
        }

        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.Id == id);
        }
        
    }
}
