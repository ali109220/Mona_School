using Core.SharedDomain.Security;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationShared.Security.Dto
{
    public class InputLoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class OutputUserDto
    {
        public List<UserCustomer> Users { get; set; }
        public int? Count { get; set; }
    }
    public class UserCustomer
    {
        public User User { get; set; }
        public Customer Customer { get; set; }
    }
    public class InputUserDto
    {
        public string Id { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        public bool HasWrittingAccess { get; set; }
        public bool IsCustomer { get; set; }
        public string Phone { get; set; }
    }
    public class InputChangePasswordDto
    {
        public string UserId { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
    public enum ChangePasswordResult
    {
        UserNotFound,
        NewPasswordDoesntMatchConfirm,
        OldPasswordNotCorrect,
        Successed,
        Exception,
        Failer
    }
}
