using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialWeb.Models
{
    public class JsonResultData
    {
        public bool IsOk { get; set; }
        public string Msg { get; set; }
        public object dataObj { get; set; }
        public object dataErr { get; set; }

    }
}