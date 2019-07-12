using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialWeb.Models
{
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Scope { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Authentication { get; set; }
        public string Signature { get; set; }
        public string Certificate { get; set; }
        public string PrivateKey { get; set; }
        public string apiUrl { get; set; }
        public string apiTokenUrl { get; set; }
    }
}