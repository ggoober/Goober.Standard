﻿using Microsoft.Extensions.Configuration;
using System;

namespace Goober.CommonModels
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SwaggerHideInDocsAttribute : Attribute
    {
        public SwaggerHideInDocsAttribute(string cookieName = "swagger-show", string password = "password")
        {
            Password = password;
            CookieName = cookieName;
        }

        public string Password { get; set; } = "password";
        public string CookieName { get; set; } = "swagger-show";
    }
}
