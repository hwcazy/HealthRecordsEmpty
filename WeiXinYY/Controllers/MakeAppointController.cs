using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using WeiXinYY.Models;

namespace WeiXinYY.Controllers
{
    public class MakeAppointController : ApiController
    {
        private AppointBLL bll;
        public MakeAppointController()
        {
            bll = new AppointBLL();
        }

        //public string Options()
        //{
        //    return null;
        //}


        //[HttpGet]
        //public string Get()
        //{
        //    return "一个参数";
        //}

        public HttpResponseMessage HeaServer(string id)
        {
            Stream s = System.Web.HttpContext.Current.Request.InputStream;
            //System.Web.HttpContext.Current.Request.ContentType = "application/json"; 
            byte[] b = new byte[s.Length];
            s.Read(b, 0, (int)s.Length);
            string rumsg = Encoding.UTF8.GetString(b);

            if (id.Equals("Get_Openid", StringComparison.OrdinalIgnoreCase))
            {
                string strjson = bll.Get_Openid(rumsg);
                return new HttpResponseMessage { Content = new StringContent(strjson, Encoding.GetEncoding("UTF-8"), "application/json") };
            }
            else if (id.Equals("Get_WXPatient", StringComparison.OrdinalIgnoreCase))
            {
                string strjson = bll.Get_WXPatient(rumsg);
                return new HttpResponseMessage { Content = new StringContent(strjson, Encoding.GetEncoding("UTF-8"), "application/json") };
            }
            else if (id.Equals("Get_AppointDept", StringComparison.OrdinalIgnoreCase))
            {
                string strjson = bll.Get_AppointDept(rumsg);
                return new HttpResponseMessage { Content = new StringContent(strjson, Encoding.GetEncoding("UTF-8"), "application/json") };
            }
            else if (id.Equals("Get_DoctByDept", StringComparison.OrdinalIgnoreCase))
            {
                string strjson = bll.Get_DoctByDept(rumsg);
                return new HttpResponseMessage { Content = new StringContent(strjson, Encoding.GetEncoding("UTF-8"), "application/json") };
            }
            else if (id.Equals("Get_NextDoctByDept", StringComparison.OrdinalIgnoreCase))
            {
                string strjson = bll.Get_NextDoctByDept(rumsg);
                return new HttpResponseMessage { Content = new StringContent(strjson, Encoding.GetEncoding("UTF-8"), "application/json") };
            }
            else if (id.Equals("Get_OuSchDtl", StringComparison.OrdinalIgnoreCase))
            {
                string strjson = bll.Get_OuSchDtl(rumsg);
                return new HttpResponseMessage { Content = new StringContent(strjson, Encoding.GetEncoding("UTF-8"), "application/json") };
            }
            else if (id.Equals("Get_JzTime", StringComparison.OrdinalIgnoreCase))
            {
                string strjson = bll.Get_JzTime(rumsg);
                return new HttpResponseMessage { Content = new StringContent(strjson, Encoding.GetEncoding("UTF-8"), "application/json") };
            }
            else if (id.Equals("Save_OUHOSINFO", StringComparison.OrdinalIgnoreCase))
            {
                string strjson = bll.Save_OUHOSINFO(rumsg);
                return new HttpResponseMessage { Content = new StringContent(strjson, Encoding.GetEncoding("UTF-8"), "application/json") };
            }
            else if (id.Equals("Get_Url", StringComparison.OrdinalIgnoreCase))
            {
                string strjson = bll.Get_Url(rumsg);
                return RetMessage(strjson);
            }
            else if (id.Equals("Register", StringComparison.OrdinalIgnoreCase))
            {
                string strjson = bll.Register(rumsg);
                return new HttpResponseMessage { Content = new StringContent(strjson, Encoding.GetEncoding("UTF-8"), "application/json") };
            }
            else if (id.Equals("Get_Registerpat", StringComparison.OrdinalIgnoreCase))
            {
                string strjson = bll.Get_Registerpat(rumsg);
                return new HttpResponseMessage { Content = new StringContent(strjson, Encoding.GetEncoding("UTF-8"), "application/json") };
            }
            else
            {
                string strjson = @"{""msgCode"":""-1"",
                      ""msg"":""请求参数错误"",""data"":""""}";
                return new HttpResponseMessage { Content = new StringContent(strjson, Encoding.GetEncoding("UTF-8"), "application/json") };
            }
        }

        protected virtual HttpResponseMessage RetMessage(object msg)
        {
            return new HttpResponseMessage
            {
                Content = new StringContent(msg.ToString(), new UTF8Encoding(false)
                  , "text/plain")
            };

        }
    }
}