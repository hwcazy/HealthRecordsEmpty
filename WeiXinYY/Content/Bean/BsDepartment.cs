using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeiXinYY.Content.Bean
{
    public class BsDepartment
    {
        public int ID { get; set; }
        public int FatherId { get; set; }

        public string Name { get; set; }
        public string PhotoPath { get; set; }
    }
}