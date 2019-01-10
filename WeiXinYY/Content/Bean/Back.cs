using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeiXinYY.Content.Bean
{
    public class Back<T>
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
        public List<T> data { get; set; } 

       
    }
}