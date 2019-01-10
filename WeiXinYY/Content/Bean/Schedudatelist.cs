using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeiXinYY.Content.Bean
{
    public class Schedudatelist
    {
        public string scheduledate { get; set; }

        public string schedule { get; set; }
        public string week { get; set; }

        public List<ScheduShow> list { get; set; }
    }
}