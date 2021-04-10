
using ApplicationShared.Entities;
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
    public class InChargeDto
    {
        public string Name { get; set; }
        public string Id { get; set; }
    }
    public class OutputUserDto
    {
        public IEnumerable<UserDto> Users { get; set; }
        public int AllCount { get; set; }
    }
    public class UserDto
    {
        public string Id { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        public string LastLogin { get; set; }
        public string Phone { get; set; }
    }
    public class InputUserDto
    {
        public string Id { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
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
