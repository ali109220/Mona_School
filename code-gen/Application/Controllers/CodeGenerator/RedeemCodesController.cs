using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCore;
using Domain.Entities;
using ApplicationShared.Entities;
using ApplicationShared.Entites;
using ApplicationShared.Entites.Code;
using Application.Services;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Text;
using System.IO;
using ClosedXML.Excel;
using Domain.Enums;
//using System.Linq.Dynamic.Core;


namespace Application.Controllers.CodeGenerator
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedeemCodesController : ControllerBase
    {
        private readonly CodeContext _context;
        private readonly IConfiguration _config;
        private readonly SmtpClient _smtp;

        public RedeemCodesController(CodeContext context, IConfiguration config, SmtpClient smtp)
        {
            _context = context;
            _config = config;
            _smtp = smtp;
        }
        [Route("api/[controller]/[action]")]
        [HttpPost]
        public async Task<ActionResult<RedeemCodeOutputDto>> GetRedeemCodes(RedeemCodeFilterDto filter)
        {
             var redeemCodes = await _context.RedeemCodes.Where(x => !x.VirtualDeleted).ToListAsync();
            var allData = await _context.RedeemCodes.Where(x => !x.VirtualDeleted).ToListAsync();
            if (filter.FromDate != null || filter.ToDate != null)
            {
                if(filter.FromDate != null)
                    allData = allData.Where(x => (x.UpdatedDate != null && x.UpdatedDate.Value.Date >= filter.FromDate.Value.Date) ||
                    (x.CreatedDate != null && x.CreatedDate.Value.Date >= filter.FromDate.Value.Date) ||
                    (x.ActivationDate != null && x.ActivationDate.Value.Date >= filter.FromDate.Value.Date)).ToList();
                if (filter.ToDate != null)
                    allData = allData.Where(x => (x.UpdatedDate != null && x.UpdatedDate.Value.Date <= filter.ToDate.Value.Date) ||
                    (x.CreatedDate != null && x.CreatedDate.Value.Date <= filter.ToDate.Value.Date) ||
                    (x.ActivationDate != null && x.ActivationDate.Value.Date <= filter.ToDate.Value.Date)).ToList();
            }
            if(!string.IsNullOrEmpty(filter.TypeId))
                allData = allData.Where(x => x.TypeId == (int) int.Parse(filter.TypeId)).ToList();
            if (filter.Status != null)
                allData = allData.Where(x => x.Status == (Status) filter.Status).ToList();
            var types = await _context.ProductTypes.Where(x => !x.VirtualDeleted).ToListAsync();
            var licenceTypes = await _context.LicenceTypes.Where(x => !x.VirtualDeleted).ToListAsync();
            var customers = await _context.Customers.Where(x => !x.VirtualDeleted).ToListAsync();
            //.WhereIf(filter.FromDate != null, x => x.Date > filter.FromDate)
            var result = allData.Select(z => new CodeOutputDto()
            {
                Code = z.Code,
                CreatedDate = z.CreatedDate.Value.ToString("G"),
                ActivatedDate = z.ActivationDate != null ? z.ActivationDate.Value.ToString("G") : "",
                Id = z.Id,
                ProductType = new ApplicationShared.Constants.OutputIndexDto()
                {
                    ArabicName = types.FirstOrDefault(y => y.Id == z.TypeId).ArabicName,
                    Name = types.FirstOrDefault(y => y.Id == z.TypeId).Name,
                    Id = z.TypeId.Value
                },
                LicenseType = new ApplicationShared.Constants.OutputIndexDto()
                {
                    ArabicName = licenceTypes.FirstOrDefault(y => y.Id == types.FirstOrDefault(f => f.Id == z.TypeId).LicenceTypeId).ArabicName,
                    Name = licenceTypes.FirstOrDefault(y => y.Id == types.FirstOrDefault(f => f.Id == z.TypeId).LicenceTypeId).Name,
                    Id = types.FirstOrDefault(y => y.Id == z.TypeId).LicenceTypeId.Value
                },
                Status = z.Status,
                StrStatus = z.Status.ToString(),
                Customer = new ApplicationShared.Constants.OutputIndexDto()
                {
                    Name = z.CustomerId == null ? "" : customers.FirstOrDefault(y => y.Id == z.CustomerId.Value).FullName,
                    Id = z.CustomerId == null ? 0 : z.CustomerId.Value
                },
                Country = z.CustomerId == null ? "" : customers.FirstOrDefault(y => y.Id == z.CustomerId.Value).Country,
                email = z.CustomerId == null ? "" : customers.FirstOrDefault(y => y.Id == z.CustomerId.Value).Email,
            });
            var all = new List<CodeOutputDto>((result as IEnumerable<CodeOutputDto>).AsQueryable());
            var allCount = redeemCodes.Count();
            var activatedCount = redeemCodes.Where(x => x.Status == Domain.Enums.Status.Activated).Count();
            var waitingActivationCount = redeemCodes.Where(x => x.Status == Domain.Enums.Status.WaitingActivation).Count();
            var notActivatedCount = redeemCodes.Where(x => x.Status == Domain.Enums.Status.NotActivated).Count();
            return new RedeemCodeOutputDto() { RedeemCodes = all, AllCount = allCount, ActivatedCount = activatedCount, WaitingCount = waitingActivationCount, NotActivatedCount = notActivatedCount };
        }
        // GET: api/RedeemCodes
        [Route("api/[controller]/[action]")]
        [HttpPost]
        public async Task<List<CustomerCodeOutputDto>> GetCustomersRedeemCodes(string country)
        {
            var all = new List<CustomerCodeOutputDto>();

            try
            {
                var allData = await _context.RedeemCodes.Where(x => !x.VirtualDeleted).ToListAsync();
                var waitingData = await _context.RedeemCodes.Where(x => !x.VirtualDeleted && x.CustomerId != null).ToListAsync();
                var codes = waitingData;
                var types = await _context.ProductTypes.Where(x => !x.VirtualDeleted).ToListAsync();
                var licenceTypes = await _context.LicenceTypes.Where(x => !x.VirtualDeleted).ToListAsync();
                var customers = await _context.Customers.Where(x => !x.VirtualDeleted).ToListAsync();
                //.WhereIf(filter.FromDate != null, x => x.Date > filter.FromDate)
                var result = waitingData.GroupBy(x => new { x.CustomerId }).Select(x => new CustomerCodeOutputDto()
                {
                    Customer = new Customer()
                    {
                        FirstName = customers.FirstOrDefault(y => y.Id == x.Key.CustomerId.Value).FirstName,
                        LastName = customers.FirstOrDefault(y => y.Id == x.Key.CustomerId.Value).LastName,
                        Email = customers.FirstOrDefault(y => y.Id == x.Key.CustomerId.Value).Email,
                        Country = customers.FirstOrDefault(y => y.Id == x.Key.CustomerId.Value).Country,
                        City = customers.FirstOrDefault(y => y.Id == x.Key.CustomerId.Value).City,
                        Blocked = customers.FirstOrDefault(y => y.Id == x.Key.CustomerId.Value).Blocked,
                        Phone = customers.FirstOrDefault(y => y.Id == x.Key.CustomerId.Value).Phone,
                        IpAddress = customers.FirstOrDefault(y => y.Id == x.Key.CustomerId.Value).IpAddress,
                        NumberOfActivations = codes.Where(y=> y.CustomerId == x.Key.CustomerId.Value &&( y.Status == Status.WaitingActivation || y.Status == Status.Activated)).Count(),
                        Id = x.Key.CustomerId.Value
                    },
                    RedeemCodes = codes.Where(z => z.CustomerId == x.Key.CustomerId.Value).Select(z => new CodeOutputDto()
                    {
                        Code = z.Code,
                        CreatedDate = z.CreatedDate.Value.ToString("G"),
                        ActivatedDate = z.ActivationDate == null ? z.UpdatedDate == null ? null : z.UpdatedDate.Value.ToString("G") :z.ActivationDate.Value.ToString("G"),
                        Id = z.Id,
                        ProductType = new ApplicationShared.Constants.OutputIndexDto()
                        {
                            ArabicName = types.FirstOrDefault(y => y.Id == z.TypeId).ArabicName,
                            Name = types.FirstOrDefault(y => y.Id == z.TypeId).Name,
                            Id = z.TypeId.Value
                        },
                        LicenseType = new ApplicationShared.Constants.OutputIndexDto()
                        {
                            ArabicName = licenceTypes.FirstOrDefault(y => y.Id == types.FirstOrDefault(f => f.Id == z.TypeId).LicenceTypeId).ArabicName,
                            Name = licenceTypes.FirstOrDefault(y => y.Id == types.FirstOrDefault(f => f.Id == z.TypeId).LicenceTypeId).Name,
                            Id = types.FirstOrDefault(y => y.Id == z.TypeId).LicenceTypeId.Value
                        },
                        Status = z.Status,
                        StrStatus = z.Status.ToString()
                    })
                });
                all = new List<CustomerCodeOutputDto>((result as IEnumerable<CustomerCodeOutputDto>).AsQueryable());
                return all;
            }
            catch (Exception e)
            {
                return all;
            }
        }

        // GET: api/RedeemCodes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RedeemCode>> GetRedeemCode(int id)
        {
            var redeemCode = await _context.RedeemCodes.FindAsync(id);

            if (redeemCode == null)
            {
                return NotFound();
            }

            return redeemCode;
        }

        // POST: api/RedeemCodes/BlockCode
        [Route("api/[controller]/[action]")]
        [HttpPost]
        public async Task<int> BlockCode(BlockInputDto input)
        {
            var redeemCode = await _context.RedeemCodes.FindAsync(input.Id);
            if (redeemCode == null)
            {
                return 0;
            }
            redeemCode.Status = Domain.Enums.Status.Blocked;
            redeemCode.UpdatedUserId = input.UserId;
            redeemCode.UpdatedDate = DateTime.Now;
            _context.Entry(redeemCode).State = EntityState.Modified;

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
        public async Task<int> UnBlockCode(BlockInputDto input)
        {
            var redeemCode = await _context.RedeemCodes.FindAsync(input.Id);
            if (redeemCode == null)
            {
                return 0;
            }
            redeemCode.Status = Domain.Enums.Status.NotActivated;
            redeemCode.UpdatedUserId = input.UserId;
            redeemCode.UpdatedDate = DateTime.Now;
            _context.Entry(redeemCode).State = EntityState.Modified;

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
        // POST: api/RedeemCodes/ActiveCode
        [Route("api/[controller]/[action]")]
        [HttpPost]
        public async Task<bool> ActiveCode(BlockInputDto input)
        {
            var redeemCode = await _context.RedeemCodes.FindAsync(input.Id);
            if (redeemCode == null)
            {
                return false;
            }
            var type = await _context.ProductTypes.FindAsync(redeemCode.TypeId);
            type.ActivatedCount += 1;
            redeemCode.Status = Domain.Enums.Status.Activated;
            redeemCode.UpdatedUserId = input.UserId;
            redeemCode.UpdatedDate = DateTime.Now;
            _context.Entry(type).State = EntityState.Modified;
            _context.Entry(redeemCode).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RedeemCodeExists(input.Id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }

            return true;
        }

        // POST: api/RedeemCodes/CheckCode
        [Route("api/[controller]/[action]")]
        [HttpPost]
        public async Task<ActionResult<int>> CheckCode(CheckInputDto input)
        {
            EmailService emailService = new EmailService(_config, _smtp);
            //var isSent = await emailService.SendEmail("alisouccar@hotmail.com", sbj, htmlMsg);
            var redeemCode = await _context.RedeemCodes.Where(x => x.Code == input.Code).FirstOrDefaultAsync();
            var customer = await _context.Customers.Where(x => x.UserId == input.UserId).FirstOrDefaultAsync();
            if (redeemCode == null || customer == null)
            {
                return 0;
            }
            if(redeemCode.Status == Domain.Enums.Status.NotActivated)
            {
                var productType = await _context.ProductTypes.FirstOrDefaultAsync(x => x.Id == redeemCode.TypeId);
                var brand = await _context.Brands.Where(x => x.Id == productType.BrandId).FirstOrDefaultAsync();
                var platform = await _context.Platforms.Where(x => x.Id == productType.PlatformId).FirstOrDefaultAsync();
                var licence = await _context.LicenceTypes.Where(x => x.Id == productType.LicenceTypeId).FirstOrDefaultAsync();
                redeemCode.CustomerId = customer.Id;
                redeemCode.Status = Domain.Enums.Status.WaitingActivation;
                redeemCode.UpdatedUserId = input.UserId;
                redeemCode.UpdatedDate = DateTime.Now;
                redeemCode.ActivationDate = DateTime.Now;
                _context.Entry(redeemCode).State = EntityState.Modified;
                try
                {

                    await _context.SaveChangesAsync();
                    string sbjRequest = "Request Code Activation";
                    string sbjCode = "Code Activation";
                    string htmlMsgRequest = "<div style ='width: 100%; padding: 2%; margin: 2%;color:black;font-size: 13px; font-weight: 400;'>" +
                                        "<div><a href=''><div><img src='https://sy-store.com/assets/logo.png' width='200' height='22'></div></a></div>" +
                                        "<br>" +
                                        "<br>" +
                                        "<div><span style='color: forestgreen; font-size: 14px; font-weight: 500;'> Code activation request: </span></div>" +
                                        "<br>" +
                                        "<br>" +
                                        "<div><span style='color: black; font-size: 15px; font-weight: 600;'> Dear , </span></div>" +
                                        "<br>" +
                                        "<div><span>There is a request code activation for " + customer.FullName + "which his email is <span style='font-weight: 500;'> " + customer.Email + "</span></span></div>" +
                                        "<br>" +
                                        "<div><span>The code " + redeemCode.Code + "which ID is " + redeemCode.Id + " for the following product:</span></div>" +
                                        "<br>" +
                                        "<div><span>Product name:" + productType.Name + "</span></div>" +
                                        "<div><span>The Company's name:" + brand.Name + "</span></div>" +
                                        "<div><span>Platform:" + platform.Name + "</span></div>" +
                                        "<div><span>Licence:" + licence.Name + "</span></div>" +
                                        "<div><span>Creation date:" + redeemCode.CreatedDate.Value.ToString("G") + "</span></div>" +
                                        "<div><span>Activation date:" + redeemCode.UpdatedDate.Value.ToString("G") + "</span></div>" +
                                        "<br></div>";
                    
                    string htmlMsgCode = "<div style ='width: 100%; padding: 2%; margin: 2%;color:black;font-size: 13px; font-weight: 400;'>" +
                                        "<div><a href=''><div><img src='https://sy-store.com/assets/logo.png' width='200' height='22'></div></a></div>" +
                                        "<br>" +
                                        "<br>" +
                                        "<div><span style='color: forestgreen; font-size: 14px; font-weight: 500;'> Activated successfully, your order is on the way </span></div>" +
                                        "<br>" +
                                        "<br>" +
                                        "<div><span style='color: black; font-size: 15px; font-weight: 600;'> Dear" + customer.FirstName + " " + customer.LastName + ", </span></div>" +
                                        "<br>" +
                                        "<div><span>Thank you for purchasing from a <a style='font-weight: 500;' href='https://sy-store.com'>SY-STORE.COM</a></span></div>" +
                                        "<br>" +
                                        "<div><span>We have received the redemption code for the following product:</span></div>" +
                                        "<br>" +
                                        "<div><span>Product name:" + productType.Name + "</span></div>" +
                                        "<div><span>The Company's name:" +brand.Name + "</span></div>" +
                                        "<div><span>Platform:" + platform.Name + "</span></div>" +
                                        "<div><span>Activation date:" + redeemCode.UpdatedDate.Value.ToString("G") + "</span></div>" +
                                        "<br>" +
                                        "<div><span>The activation key and download link and activation instructions will be sent to this mail.<span style='font-weight: 500;'> " + customer.Email + " within 1 hour to a maximum of 6 hours </span></span></div>" +
                                        "<br>" +
                                        "<div><span>We are sorry to wait for you, but this is in order to protect your product from any problems</span></div>" +
                                        "<br>" +
                                        "<div><span>In case of any problem, do not hesitate to contact us at<span style='text-decoration: underline;'>support@sy-store.com . </span></span></div>" +
                                        "</div>";
                    //EmailService emailService = new EmailService(_config, _smtp);
                    var isSent = await emailService.SendEmail(customer.Email, sbjCode, htmlMsgCode);
                    var isSentReq = await emailService.SendEmail("info@sy-store.com", sbjRequest, htmlMsgRequest);
                    if (isSent)
                    {
                        return productType.Id;
                    }
                    //send Email to user and owner
                }
                catch (Exception e)
                {
                    var msg = e.Message;
                    return 0;
                }
                return productType.Id;
            }
            else if (redeemCode.Status == Domain.Enums.Status.Blocked)
            {
                return -5;
            }
            else
            {
                return -1;
            }
        }

        // POST: api/RedeemCodes/GenerateCode
        [Route("api/[controller]/[action]")]
        [HttpPost]
        public async Task<List<GenerateOutPutDto>> GenerateCode(CodeInputDto input)
        {
            var result = new List<GenerateOutPutDto>();
            try
            {
                var createdDate = DateTime.Now;
                var codes = await _context.RedeemCodes.Select(x => x.Code).ToListAsync();
                var type = await _context.ProductTypes.FindAsync(input.TypeId);
                type.GeneratedCount += input.Count;
                _context.Entry(type).State = EntityState.Modified;
                if (input.Count > 0)
                    for (int i = 0; i < input.Count; i++)
                    {
                        var redeemCode = new RedeemCode()
                        {
                            TypeId = input.TypeId,
                            Code = CodeService.GenerateNewCode(codes),
                            Status = Domain.Enums.Status.NotActivated,
                            CreatedDate = createdDate,
                            CreatedUserId = input.UserId
                        };
                        _context.RedeemCodes.Add(redeemCode);
                    }
                await _context.SaveChangesAsync();
                var _result = await _context.RedeemCodes.Where(x => !x.VirtualDeleted && x.CreatedDate.Value == createdDate).ToListAsync();
                var __result = _result.Select(x => new GenerateOutPutDto()
                {
                    Code = x.Code,
                    Id = x.Id,
                    CreatedDate = x.CreatedDate.Value.ToString(),
                    CreatedDateTime = x.CreatedDate.Value,
                    ProductType = x.ProductType.Name
                });
                return __result.ToList();

            }
            catch (Exception ex)
            {
                return result;
            }
        }
        [Route("api/[controller]/[action]")]
        [HttpGet]
        public async Task<List<CodeOutputDto>> GetRedeemCodesByTypeId(int typeId)
        {
            var allData = await _context.RedeemCodes.Where(x => !x.VirtualDeleted && x.TypeId == typeId).ToListAsync();
            var type = await _context.ProductTypes.FindAsync(typeId);
            var licenceType = await _context.LicenceTypes.FindAsync(type.LicenceTypeId);
            //.WhereIf(filter.FromDate != null, x => x.Date > filter.FromDate)
            var result = allData.Select(x => new CodeOutputDto()
            {
                Code = x.Code,
                CreatedDate = x.CreatedDate.Value.ToString("G"),
                Id = x.Id,
                LicenseType = new ApplicationShared.Constants.OutputIndexDto()
                {
                    Id = licenceType.Id,
                    Name = licenceType.Name,
                    ArabicName = licenceType.ArabicName
                },
                ProductType = new ApplicationShared.Constants.OutputIndexDto()
                {
                    Id = type.Id,
                    Name = type.Name,
                    ArabicName = type.ArabicName
                },
                Status = x.Status,
                ActivatedDate = x.ActivationDate != null ? x.ActivationDate.Value.ToString("G") : "",
                StrStatus = x.Status.ToString(),
            });
            var all = new List<CodeOutputDto>((result as IEnumerable<CodeOutputDto>).AsQueryable());
            return all;
        }
        [Route("api/[controller]/[action]")]
        [HttpPost]
        public async Task<ActionResult<bool>> DeleteRedeemCode(BlockInputDto input)
        {
            var redeemCode = await _context.RedeemCodes.FindAsync(input.Id);
            if (redeemCode == null)
            {
                return false;
            }
            redeemCode.DeletedDate = DateTime.Now;
            redeemCode.DeletedUserId = input.UserId;
            redeemCode.VirtualDeleted = true;
            _context.Entry(redeemCode).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RedeemCodeExists(input.Id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }

            return true;
        }
        [Route("api/[controller]/[action]")]
        [HttpPost]
        public async Task<ActionResult> ExportToCsv(int count)
        {
            try
            {
                var data = await _context.RedeemCodes.Where(x => !x.VirtualDeleted).ToListAsync();
                if (count != 0)
                    data = data.OrderByDescending(x=> x.CreatedDate).Take(count).ToList();
                var types = await _context.ProductTypes.Where(x => !x.VirtualDeleted).ToListAsync();
                var licences = await _context.LicenceTypes.Where(x => !x.VirtualDeleted).ToListAsync();
                var builder = new StringBuilder();
                builder.AppendLine("Id,Code,Product Type,Licence Type,Created Date");
                foreach (var row in data)
                {
                    var type = types.FirstOrDefault(x => x.Id == row.TypeId);
                    var licence = licences.FirstOrDefault(x => x.Id == type.LicenceTypeId);
                    builder.AppendLine($"{row.Id},{row.Code},{type.Name},{licence.Name},{row.CreatedDate.Value.ToString("G")}");
                }
                return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "codes.csv");
            }
            catch (Exception e)
            {
                return File(Encoding.UTF8.GetBytes("error when trying export"), "text", "error.text"); ;
            }
        }
        [Route("api/[controller]/[action]")]
        [HttpPost]
        public async Task<ActionResult> ExportToExcel(int count)
        {
            try
            {
                var data = await _context.RedeemCodes.Where(x => !x.VirtualDeleted).ToListAsync();
                if (count != 0)
                    data = data.OrderByDescending(x => x.CreatedDate).Take(count).ToList();
                var types = await _context.ProductTypes.Where(x => !x.VirtualDeleted).ToListAsync();
                var licences = await _context.LicenceTypes.Where(x => !x.VirtualDeleted).ToListAsync();
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Codes");
                    var currentRow = 1;
                    worksheet.Cell(currentRow, 1).Value = "Id";
                    worksheet.Cell(currentRow, 2).Value = "Code";
                    worksheet.Cell(currentRow, 3).Value = "Product Type";
                    worksheet.Cell(currentRow, 4).Value = "Licence Type";
                    worksheet.Cell(currentRow, 5).Value = "Created Date";
                    foreach (var row in data)
                    {
                        var type = types.FirstOrDefault(x => x.Id == row.TypeId);
                        var licence = licences.FirstOrDefault(x => x.Id == type.LicenceTypeId);
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = row.Id;
                        worksheet.Cell(currentRow, 2).Value = row.Code;
                        worksheet.Cell(currentRow, 3).Value = type.Name;
                        worksheet.Cell(currentRow, 4).Value = licence.Name;
                        worksheet.Cell(currentRow, 5).Value = row.CreatedDate.Value.ToString("G");
                    }

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "codes.xlsx");
                    }
                }
            }
            catch (Exception e)
            {
                return File(Encoding.UTF8.GetBytes("error when trying export"), "text", "error.text"); ;
            }
        }
        private bool RedeemCodeExists(int id)
        {
            return _context.RedeemCodes.Any(e => e.Id == id);
        }
    }
}