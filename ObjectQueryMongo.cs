using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialWeb.Models
{
    public class ObjectQueryMongo<T>
    {
        public int error { get; set; }
        public string msg { get; set; }
        public long TotalRows { get; set; }
        public List<T> data { get; set; }
    }
}