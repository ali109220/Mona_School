using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Core.SharedDomain.Security;
using ApplicationShared.Entites.Customer;
using ApplicationShared.Entities.Customer;
using Domain.Settings;
using Domain.Entities;
using Application.Services;

namespace Application.Controllers.Client
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly MonaContext _context;
        private readonly UserManager<User> _userManager;
        public CustomerController(MonaContext context,
            UserManager<User> userManager)

        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<CustomerOutputDto>> GetCustomers()
        {
            var data = await _context.Customers.Where(x => !x.VirtualDeleted).ToListAsync();
            var courses = await _context.Courses.Where(x => !x.VirtualDeleted).ToListAsync();
            var all = data.Select(x => new CustomerDto()
            {
                Id = x.Id,
                FullName = x.FullName,
                BirthDate = x.BirthDate,
                Address = x.Address,
                RemainingAmount = courses.Where(y=> y.CustomerId == x.Id).Sum(y=> y.RemainingMoney),
                City = x.City,
                Country = x.Country,
                Email = x.Email,
                Gender = x.Gender,
                Phone = x.Phone,
                PostalCode = x.PostalCode,
                Note = x.Note
            }).ToList();
            var count = data.Count;
            return new CustomerOutputDto() { Customers = all, AllCount = count };
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDto>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            var courses = await _context.Courses.Where(x => !x.VirtualDeleted && x.CustomerId == id).ToListAsync();
            if (customer == null)
            {
                return NotFound();
            }
            var result = new CustomerDto()
            {
                Id = customer.Id,
                FullName = customer.FullName,
                BirthDate = customer.BirthDate,
                Address = customer.Address,
                RemainingAmount = courses.Any() ? courses.Sum(y => y.RemainingMoney) :0,
                City = customer.City,
                Country = customer.Country,
                Email = customer.Email,
                Gender = customer.Gender,
                Phone = customer.Phone,
                PostalCode = customer.PostalCode,
                Note = customer.Note
            };
            return result;
        }

        // PUT: api/Customers/5
        [HttpPut("{id}")]
        public async Task<ActionResult<CustomerDto>> PutCustomer(int id, CustomerInputDto input)
        {
            DateTime? dateime = null;
            var customer = await _context.Customers.FindAsync(id);
            customer.FullName = input.FullName;
            customer.BirthDate = DateTimeString.TryParsingDate(input.BirthDate, false);
            customer.Address = input.Address;
            customer.City = input.City;
            customer.Country = input.Country;
            customer.Email = input.Email;
            customer.Gender = input.Gender;
            customer.Phone = input.Phone;
            customer.PostalCode = input.PostalCode;
            customer.UpdatedUserId = input.UserId;
            customer.UpdatedDate = DateTime.Now;
            customer.Note = input.Note;
            _context.Entry(customer).State = EntityState.Modified;
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == input.UserId);
            var log = new Log()
            {
                DateTime = DateTime.Now,
                TypeFullName = typeof(Domain.Entities.Customer).FullName,
                Content = "@userName@updateAction@objTitle",
                TypeId = customer.Id,
                UserId = user.Id
            };
            _context.Logs.Add(log);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            var result = new CustomerDto()
            {
                Id = customer.Id,
                FullName = customer.FullName,
                BirthDate = customer.BirthDate,
                Address = customer.Address,
                City = customer.City,
                Country = customer.Country,
                Email = customer.Email,
                Gender = customer.Gender,
                Phone = customer.Phone,
                PostalCode = customer.PostalCode
            };
            return result;
        }

        // POST: api/Customers
        [HttpPost]
        public async Task<ActionResult<CustomerDto>> PostCustomer(CustomerInputDto input)
        {
            try
            {
                DateTime? dateime = null;
                var customer = new Domain.Entities.Customer()
                {
                    FullName = input.FullName,
                    Address = input.Address,
                    City = input.City,
                    Country = input.Country,
                    BirthDate = DateTimeString.TryParsingDate(input.BirthDate, false),
                    Email = input.Email,
                    Gender = input.Gender,
                    Phone = input.Phone,
                    PostalCode = input.PostalCode,
                    CreatedDate = DateTime.Now,
                    CreatedUserId = input.UserId,
                    Note = input.Note
                };
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
                //var course = new Course()
                //{
                //    ExamDate = dateime,
                //    InChargeId = input.InChargeId,
                //    CustomerId = customer.Id,
                //    LessonPeriod = Domain.Enums.LessonPeriod.TwoHours,
                //    Note = input.Note,
                //    StartCourse = DateTimeString.TryParsingDate(input.StartCourse, false),
                //    TotalAmount = 0,
                //    CurrencyId = null,
                //    PeroidBeforeSendEmailId = null,
                //    TypeOfExamId = null,
                //    TypeOfPacketId = null,
                //    CreatedDate = DateTime.Now,
                //    CreatedUserId = input.UserId
                //};
                //course.RemainingMoney = course.TotalAmount;
                //course.Status = Domain.Enums.CourseStatus.NotStarted;
                //_context.Courses.Add(course);
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == input.UserId);
                var log = new Log()
                {
                    DateTime = DateTime.Now,
                    TypeFullName = typeof(Domain.Entities.Customer).FullName,
                    Content = "@userName@addAction@objTitle",
                    TypeId = customer.Id,
                    UserId = user.Id
                };
                _context.Logs.Add(log);
                await _context.SaveChangesAsync();
                var result = new CustomerDto()
                {
                    Id = customer.Id,
                    FullName = customer.FullName,
                    BirthDate = customer.BirthDate,
                    Address = customer.Address,
                    City = customer.City,
                    Country = customer.Country,
                    Email = customer.Email,
                    Gender = customer.Gender,
                    Phone = customer.Phone,
                    PostalCode = customer.PostalCode
                };
                return result;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<CustomerDto>> DeleteCustomer(int id, string userId)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            customer.DeletedDate = DateTime.Now;
            customer.DeletedUserId = userId;
            customer.VirtualDeleted = true;
            _context.Entry(customer).State = EntityState.Modified;
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            var log = new Log()
            {
                DateTime = DateTime.Now,
                TypeFullName = typeof(Domain.Entities.Customer).FullName,
                Content = "@userName@deleteAction@objTitle",
                TypeId = customer.Id,
                UserId = user.Id
            };
            _context.Logs.Add(log);


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            var result = new CustomerDto()
            {
                Id = customer.Id,
                FullName = customer.FullName,
                BirthDate = customer.BirthDate,
                Address = customer.Address,
                City = customer.City,
                Country = customer.Country,
                Email = customer.Email,
                Gender = customer.Gender,
                Phone = customer.Phone,
                PostalCode = customer.PostalCode
            };
            return result;
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}
