using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeiXinYY.Content.Bean
{
    public class Ouhosinfo
    {   
         public int id { get; set; }
         public DateTime regtime { get; set; }
         public string ardno { get; set; }
         public int patid { get; set; }
         public string name { get; set; }
         public int regtypeid { get; set; }
         public double regfee { get; set; }
         public int dotorid { get; set; }
         public int isprereg { get; set; } 
         public DateTime opertime { get; set; }
         public int operid { get; set; }
         public int timespanid { get; set; }
         public DateTime preregtime { get; set; }

         public string bookingtel { get; set; }

         public int ischarge { get; set; }

         public string timebegin { get; set; }

         public string timeend { get; set; }

         public int departmentid { get; set; }
       
    }
}