using System;
using ApplicationShared.Security;
using ApplicationShared.Security.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Linq;

using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Core.SharedDomain.Security;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Application.Services;
using System.Net.Mail;
using System.Globalization;

namespace Application.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase , IAccountAppService
    {
        private readonly CodeContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly SmtpClient _smtp;

        public AccountController(CodeContext context,
            UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration, SmtpClient smtp)
        {
           
            _smtp = smtp;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;


        }
        [Route("api/[controller]/[action]")]
        [HttpGet]
        public async Task<OutputUserDto> GetAllUsers()
        {
            var customersUsers = new List<UserCustomer>();
            var users = await _context.Users.Where(x => x.Id != "").ToListAsync();
            var allCount = await _context.Users.Where(x => x.Id != "").CountAsync();
            foreach (var u in users)
            {
                if (u.IsCustomer)
                {
                    var customer = await _context.Customers.FirstOrDefaultAsync(x => x.UserId == u.Id);
                    customersUsers.Add(new UserCustomer()
                    {
                        Customer = customer,
                        User = u
                    });
                }
                else
                    customersUsers.Add(new UserCustomer()
                    {
                        Customer = null,
                        User = u
                    });
            }

            var all = new List<UserCustomer>((customersUsers as IEnumerable<UserCustomer>).AsQueryable());
            return new OutputUserDto () { Users = all, Count = allCount };
        }
        [Route("api/[controller]/[action]")]
        [HttpPost]
        public async Task<User> Post(InputUserDto input)
        {
            IdentityResult IR = null;
            var user = new User { UserName = input.UserName, Email = input.Email, PhoneNumber = input.Phone, IsCustomer = input.IsCustomer , IsAdmin = input.IsAdmin , HasWrittingAccess = input.HasWrittingAccess };
            IR = await _userManager.CreateAsync(user, input.Password);
            if (!IR.Succeeded)
            {
                return null;
            }
            user = await _userManager.FindByIdAsync(user.Id);
            return user;
        }
        [Route("api/[controller]/[action]")]
        [HttpPost]
        public async Task<int> ResetPassword(string email)
        {
            EmailService emailService = new EmailService(_configuration, _smtp);
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                user = await _userManager.FindByNameAsync(email);
            if (user != null)
            {
                string sbjCode = "SY-Store Reset Password";
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                string htmlMsgCode = "<div style ='width: 100%; padding: 2%; margin: 2%;color:black;font-size: 13px; font-weight: 400;'>" +
                                        "<div><a href=''><div><img src='https://sy-store.com/assets/logo.png' width='200' height='22'></div></a></div>" +
                                        "<br>" +
                                        "<br>" +
                                        "<div><h3 style='text-align: center;font-size: 23px;'> Password Reset </h3></div>" +
                                        "<br>" +
                                        "<br>" +
                                        "<div><span style='color: black; font-size: 15px; font-weight: 600;'> Dear" + user.UserName +", </span></div>" +
                                        "<br>" +
                                        "<div><span>We have received your request to reset your password:</span></div>" +
                                        "<br>" +
                                        "<div><span>Code: <br>"+ code +"</span></div>" +
                                        "<br>" +
                                        "<div><span>Use above code, copy it and put in code text input then click submit to reset password.</span></div>" +
                                        "<br>" +
                                        "<div><span>If you didn't request a password reset, please ignore this email and do something fun. It's a nice day.</span></div>" +
                                        "<br>" +
                                        "<div><span>In case of any problem, do not hesitate to contact us at<span style='text-decoration: underline;'>support@sy-store.com . </span></span></div>" +
                                        "</div>";
                var isSent = await emailService.SendEmail(user.Email, sbjCode, htmlMsgCode);
                //_context.Entry(user).State = EntityState.Modified;
                //await _context.SaveChangesAsync();
                return 1;
            }
            else
            {
                return 0;
            }
        }
        [Route("api/[controller]/[action]")]
        [HttpPost]
        public async Task<string> ConfirmResetPassword(ResetPasswordInputDto input)
        {
            var user = await _userManager.FindByEmailAsync(input.Email);
            if (user == null)
                user = await _userManager.FindByNameAsync(input.Email);
            if (user != null)
            {
                var identityResult = await _userManager.ResetPasswordAsync(user,input.Code, "123456");
                if(!identityResult.Succeeded)
                    return "0";
                return user.Id;
            }
            else
            {
                return "0";
            }
        }
        [Route("api/[controller]/[action]")]
        [HttpPut]
        public async Task<User> PutUser(InputUserDto input)
        {
            var usr = await _userManager.FindByIdAsync(input.Id);

            try
            {
                if (usr == null)
                {
                    return null;
                }
                usr.Email = input.Email;
                usr.PhoneNumber = input.Phone;
                usr.UserName = input.UserName;
                usr.IsAdmin = input.IsAdmin;
                usr.HasWrittingAccess = input.HasWrittingAccess;
                usr.IsCustomer = input.IsCustomer;
                _context.Entry(usr).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return usr;
        }
        [Route("api/[controller]/[action]")]
        [HttpGet]
        public async Task<User> GetUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user;
        }
        [Route("api/[controller]/[action]")]
        [HttpPost]
        public async Task<ChangePasswordResult> ChangePassword([FromBody] InputChangePasswordDto input)
        {
            try
            {
                EmailService emailService = new EmailService(_configuration, _smtp);
                var user = await _userManager.FindByIdAsync(input.UserId);
                if (user == null)
                    return ChangePasswordResult.UserNotFound;
                else
                {
                    if (input.ConfirmPassword != input.NewPassword)
                        return ChangePasswordResult.NewPasswordDoesntMatchConfirm;
                    var result = await _signInManager.PasswordSignInAsync(user, input.Password, false, false);
                    if (!result.Succeeded)
                        return ChangePasswordResult.OldPasswordNotCorrect;
                    var resultChange = await _userManager.ChangePasswordAsync(user, input.Password, input.NewPassword);
                    if (resultChange.Succeeded)
                    {
                        string sbjCode = "SY-Store Password change";
                        string htmlMsgCode = "<div style ='width: 100%; padding: 2%; margin: 2%;color:black;font-size: 13px; font-weight: 400;'>" +
                                                "<div><a href=''><div><img src='https://sy-store.com/assets/logo.png' width='200' height='22'></div></a></div>" +
                                                "<br>" +
                                                "<br>" +
                                                "<div><h3 style='text-align: center;font-size: 23px;'> Your password changed </h3></div>" +
                                                "<br>" +
                                                "<br>" +
                                                "<div><span style='color: black; font-size: 15px; font-weight: 600;'> Dear" + user.UserName + ", </span></div>" +
                                                "<br>" +
                                                "<div><span>Your password has change on " + DateTime.Now.ToString("G", CultureInfo.CreateSpecificCulture("en-US")) + ".</span></div>" +
                                                "<br>" +
                                                "<div><span>Thanks.</span></div>" +
                                                "<br>" +
                                                "<div><span>In case of any problem, do not hesitate to contact us at<span style='text-decoration: underline;'>support@sy-store.com . </span></span></div>" +
                                                "</div>";
                        if(user.IsCustomer)
                            await emailService.SendEmail(user.Email, sbjCode, htmlMsgCode);
                        result = await _signInManager.PasswordSignInAsync(user, input.NewPassword, true, false);
                        if (result.Succeeded)
                        {
                            var token = GenerateJwtToken(user.UserName, user);
                            return ChangePasswordResult.Successed;
                        }
                        return ChangePasswordResult.Failer;
                    }
                    return ChangePasswordResult.Failer;
                }
            }
            catch (Exception ex)
            {
                return ChangePasswordResult.Exception;
            }
        }
        [Route("api/[controller]/[action]")]
        [HttpPost]
        public async Task<string> Login([FromBody] InputLoginDto input)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(input.Email);
                if(user == null)
                    user = await _userManager.FindByNameAsync(input.Email);
                if(user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, input.Password, true, false);
                    if (result.Succeeded)
                    {
                        var token =  GenerateJwtToken(user.UserName, user);
                        ////var claimsIdentity = new ClaimsIdentity(new Claim[]
                        ////        {
                        ////              new Claim("cookie", token)
                        ////        }, "Cookies");

                        ////var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        ////await Request.HttpContext.SignInAsync("Cookies", claimsPrincipal);

                        //var identity = _userManager.AddClaimAsync(user, new Claim("token", token));
                        return user.Id;
                    }
                    return "Invalid_UserName_Or_Password";
                }
                return "Invalid_UserName_Or_Password";
            }
            catch (Exception ex)
            {
                return "Login_Exception";
            }
        }
        private string GenerateJwtToken(string username, IdentityUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
              _configuration["Jwt:Issuer"],
              null,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}