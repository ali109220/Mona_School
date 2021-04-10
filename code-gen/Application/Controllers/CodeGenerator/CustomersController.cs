using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCore;
using Domain.Entities;
using ApplicationShared.Entities;
using ApplicationShared.Entites.Code;
using ApplicationShared.Entites.Customer;
using Core.SharedDomain.Security;
using Microsoft.AspNetCore.Identity;

namespace Application.Controllers.CodeGenerator
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly CodeContext _context;

        public CustomersController(UserManager<User> userManager, CodeContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        //GET: api/Customers
       [Route("api/[controller]/GetAll")]
       [HttpPost]
        public async Task<ActionResult<CustomerOutputDto>> GetCustomers(CustomerFilterDto filter)
        {
            var data = await _context.Customers.Where(x => !x.VirtualDeleted).ToListAsync();
            var all = new List<Customer>((data as IEnumerable<Customer>).AsQueryable()); ;
            var allCount = await _context.RedeemCodes.Where(x => !x.VirtualDeleted).CountAsync();
            return new CustomerOutputDto() { Customers = all, AllCount = allCount };
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }
        [Route("api/[controller]/[action]")]
        [HttpGet]
        public async Task<ActionResult<CustomerInputDto>> GetCustomerByUserId(string id)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.UserId == id);

            if (customer == null)
            {
                return NotFound();
            }
            var output = new CustomerInputDto()
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Country = customer.Country,
                City = customer.City,
                Phone = customer.Phone,
                Email = customer.Email,
            };
            return output;
        }
        [Route("api/[controller]/[action]")]
        [HttpGet]
        public async Task<List<GenerateOutPutDto>> GetCodesCustomer(int id)
        {
            var result = new List<GenerateOutPutDto>();
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == id);
            
            if (customer == null)
            {
                return result;
            }
            var codes = await _context.RedeemCodes.Where(x => x.CustomerId == id).ToArrayAsync();
            var types = await _context.ProductTypes.Where(x => codes.Any(y => y.TypeId == x.Id)).ToArrayAsync();
            var licences = await _context.LicenceTypes.Where(x => types.Any(y => y.LicenceTypeId == x.Id)).ToArrayAsync();

            foreach (var code in codes)
            {
                var type = types.FirstOrDefault(x => x.Id == code.TypeId);
                var licence = licences.FirstOrDefault(x => x.Id == type.LicenceTypeId);
                result.Add(new GenerateOutPutDto()
                {
                    Code = code.Code,
                    Id = code.Id,
                    LicenceType = licence.Name,
                    ProductType = type.Name,
                    CreatedDate = code.CreatedDate.Value.ToString("G"),
                    ActivationDate = code.UpdatedDate != null ? code.UpdatedDate.Value.ToString("G") : "",
                    StrStatus = code.Status.ToString()
                });
            }
            return result;
        }
        //PUT: api/Customers/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Customer>> PutCustomer(int id, CustomerInputDto input)
        {
            var customer = await _context.Customers.FindAsync(id);
            var usr = await _userManager.FindByIdAsync(input.UserId);
            usr.Email = input.Email;
            usr.PhoneNumber = input.Phone;
            customer.FirstName = input.FirstName;
            customer.LastName = input.LastName;
            customer.Email = input.Email;
            customer.Country = input.Country;
            customer.City = input.City;
            customer.Phone = input.Phone;
            customer.UpdatedUserId = input.UpdatedUserId;
            customer.UpdatedDate = DateTime.Now;
            _context.Entry(customer).State = EntityState.Modified;
            _context.Entry(usr).State = EntityState.Modified;

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

            return customer;
        }
        // POST: api/Customers/Block

        [Route("api/[controller]/[action]")]
        [HttpPost]
        public async Task<int> Block(BlockInputDto input)
        {
            var customer = await _context.Customers.FindAsync(input.Id);
            if (customer == null)
            {
                return 0;
            }
            customer.Blocked = true;
            customer.UpdatedUserId = input.UserId;
            customer.UpdatedDate = DateTime.Now;
            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return -1;
            }

            return 1;
        }
        [Route("api/[controller]/[action]")]
        [HttpPost]
        public async Task<int> UnBlock(BlockInputDto input)
        {
            var customer = await _context.Customers.FindAsync(input.Id);
            if (customer == null)
            {
                return 0;
            }
            customer.Blocked = false;
            customer.UpdatedUserId = input.UserId;
            customer.UpdatedDate = DateTime.Now;
            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return -1;
            }

            return 1;
        }

        // POST: api/Customers/BlockList

        [Route("api/[controller]/[action]")]
        [HttpPost]
        public async Task<ActionResult<bool>> BlockList(BlockListInputDto input)
        {
            var customers = await _context.Customers.Where(x => input.Ids.Any(y => y == x.Id)).ToListAsync();
            foreach (var customer in customers)
            {
                if (customer == null)
                {
                    continue;
                }
                customer.Blocked = true;
                customer.UpdatedUserId = input.UserId;
                customer.UpdatedDate = DateTime.Now;
                _context.Entry(customer).State = EntityState.Modified;
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }

            return false;
        }

        // POST: api/Customers

        [Route("api/[controller]/[action]")]
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(CustomerInputDto input)
        {
            try
            {
                if (input.UserId == "")
                    input.UserId = null;
                if (input.UpdatedUserId == "")
                    input.UpdatedUserId = null;
                var existed = await _context.Customers.Where(x=> x.Email == input.Email).FirstOrDefaultAsync();

                if (existed != null)
                    return null;
                var customer = new Customer()
                {
                    FirstName = input.FirstName,
                    LastName = input.LastName,
                    Country = input.Country,
                    City = input.City,
                    IpAddress = input.IpAddress,
                    Phone = input.Phone,
                    UserId = input.UserId,
                    Blocked = false,
                    Email = input.Email,
                    CreatedDate = DateTime.Now,
                    CreatedUserId = input.UpdatedUserId
                };
                IdentityResult IR = null;
                var user = new User { UserName = (input.Email), Email = input.Email, PhoneNumber = input.Phone, IsCustomer = true };
                IR = await _userManager.CreateAsync(user, input.Password);
                if (!IR.Succeeded)
                {
                    return null;
                }
                var usr = await _userManager.FindByEmailAsync(input.Email);
                customer.UserId = usr.Id;
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
                return customer;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Customer>> DeleteCustomer(int id, string userId)
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

            return customer;
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}

