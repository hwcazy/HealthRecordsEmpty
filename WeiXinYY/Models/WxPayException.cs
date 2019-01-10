using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeiXinYY.Models
{
    public class WxPayException : Exception 
    {
        public WxPayException(string msg)
            : base(msg)
        {

        }
    }
}