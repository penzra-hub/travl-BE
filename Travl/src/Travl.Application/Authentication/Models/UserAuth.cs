﻿using Travl.Domain.Enums;

namespace Travl.Application.Authentication.Models
{
    public class UserAuth
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public UserType UserType { get; set; }
    }
}
