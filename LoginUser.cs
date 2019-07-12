using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SocialWeb.Models;

namespace SocialWeb.Common
{
    public class LoginUser
    {
        public static LoginModel _current = null;

        public static LoginModel Current
        {
            get
            {
                if (HttpContext.Current.Session[Constants.LoginSession] != null)
                {
                    _current = (LoginModel)HttpContext.Current.Session[Constants.LoginSession];
                }
                else
                {
                    return null;
                }
                return _current;
            }
        }
    }
}