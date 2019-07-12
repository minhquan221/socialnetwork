using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialWeb.Models
{
    public class ObjectCacheMongo<T>
    {
        public string _id { get; set; }
        public string ExpirateDate { get; set; }
        public List<T> ListObject { get; set; }
        public long TotalRows { get; set; }
    }
}