using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeiXinYY.Content.Bean
{
    public class BsUser
    {
        public int doctorid { get; set; }
        public string name { get; set; }
        public int deptid { get; set; }
        public string deptname { get; set; }
        public string docgra { get; set; }
        public double price { get; set; }
        public string resume { get; set; }

        public string picture { get; set; }
    }
}