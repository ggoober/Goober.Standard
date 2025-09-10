using System;

namespace Goober.Base.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class SwaggerHideInDocsAttribute : Attribute
    {
        public static string DefaultCookieName { get; set; } = "indusoft";

        public static string DefaultPassword { get; set; }

        public SwaggerHideInDocsAttribute(string cookieName = null, string password = null)
        {
            Password = password ?? DefaultPassword;
            CookieName = cookieName ?? DefaultCookieName;
        }

        public string Password { get; set; }
        public string CookieName { get; set; }
    }
}
