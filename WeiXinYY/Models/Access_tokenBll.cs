using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using WeiXinYY.Content.Bean;
using WeiXinYY.Content.Utils;

namespace WeiXinYY.Models
{
    public class Access_tokenBll
    {
        private DBbll bll;
        public Access_tokenBll()
        {
            bll = new DBbll();
        }

        public OpenPat GetAccess_token(string code)
        {
            string appid = ConfigurationManager.AppSettings["appid"].ToString();
            string secret = ConfigurationManager.AppSettings["secret"].ToString();
            string grant = ConfigurationManager.AppSettings["grant_type"].ToString();
            string strUrl = "https://api.weixin.qq.com/sns/oauth2/access_token?appid=" + appid + "&secret=" + secret + "&code=" + code + "&grant_type=" + grant;
            string Token = string.Empty;
            OpenPat mode = new OpenPat();
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(strUrl);
            req.Method = "GET";
            using (WebResponse wr = req.GetResponse())
            {
                HttpWebResponse myResponse = (HttpWebResponse)req.GetResponse();
                StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                string content = reader.ReadToEnd();//在这里对Access_token 赋值 
                if (content.IndexOf("openid") != -1)
                {
                    mode.access_token = JsonHelper.GetVluesParentstr(content, "access_token");//获取json       
                    mode.expires_in = Convert.ToInt32(JsonHelper.GetVluesParentstr(content, "expires_in"));
                    mode.refresh_token = JsonHelper.GetVluesParentstr(content, "refresh_token");
                    mode.openid = JsonHelper.GetVluesParentstr(content, "openid");
                    mode.scope = JsonHelper.GetVluesParentstr(content, "scope");

                    DateTime _youxrq = DateTime.Now;
                    _youxrq = _youxrq.AddSeconds(mode.expires_in);
                    DBOpt.SelAccessToken(mode.access_token, _youxrq, mode.refresh_token, mode.openid);
                    //IsExistAccess_Token(mode.access_token, mode.expires_in, mode.refresh_token);

                    string count = bll.GetPatCount(mode.openid);
                    if (count.Equals("0"))
                    {
                        //新增
                        bll.SaveOpenid(mode.openid);
                    }
                }
                else
                {
                    mode.errcode = JsonHelper.GetVluesParentstr(content, "errcode");
                    mode.errmsg = JsonHelper.GetVluesParentstr(content, "errmsg");
                }
            }
          
            return mode;
        }

        /// <summary>
        /// 通过刷新获取Access_token
        /// </summary>
        /// <param name="refresh_token"></param>
        /// <returns></returns>
        private OpenPat GetReAccess_token(string refresh_token)
        {
            string appid = ConfigurationManager.AppSettings["appid"].ToString();
            string strUrl = "https://api.weixin.qq.com/sns/oauth2/refresh_token?appid=" + appid + "&grant_type=refresh_token" + "&refresh_token=" + refresh_token;
            OpenPat mode = new OpenPat();
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(strUrl);
            req.Method = "GET";
            using (WebResponse wr = req.GetResponse())
            {
                HttpWebResponse myResponse = (HttpWebResponse)req.GetResponse();
                StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                string content = reader.ReadToEnd();//在这里对Access_token 赋值  
                if (content.IndexOf("errcode") == -1)
                {
                    mode.access_token = JsonHelper.GetVluesParentstr(content, "access_token");
                    mode.expires_in = Convert.ToInt32(JsonHelper.GetVluesParentstr(content, "expires_in"));
                    mode.refresh_token = JsonHelper.GetVluesParentstr(content, "refresh_token");
                    mode.openid = JsonHelper.GetVluesParentstr(content, "openid");
                    mode.scope = JsonHelper.GetVluesParentstr(content, "scope");
                }
                else
                {
                    mode.errcode = JsonHelper.GetVluesParentstr(content, "errcode");
                    mode.errmsg = JsonHelper.GetVluesParentstr(content, "errmsg");

                }
            }
            return mode;
        }

        /// <summary>
        /// 获取Access_token值
        /// </summary>
        /// <returns></returns>
        //public string IsExistAccess_Token(string accss_token, int expir, string refresh_token)
        //{
        //    string Token = string.Empty;
        //    DateTime YouXRQ = Convert.ToDateTime("1999-01-01");
        //    if (!bll.GetCount().Equals("0"))
        //    {
        //        Dictionary<string, object> dic = bll.GetAccess();
        //        Token = dic["access_token_id"].ToString();
        //        if (dic["new_date"] != null && dic["new_date"].ToString() != "")
        //        {
        //            YouXRQ = Convert.ToDateTime(dic["new_date"]);
        //        }
        //    }

        //    if (DateTime.Now > YouXRQ)
        //    {
        //        //刷新
        //        DateTime _youxrq = DateTime.Now;
        //        _youxrq = _youxrq.AddSeconds(expir);
        //        DBOpt.SelAccessToken(accss_token, _youxrq, refresh_token);
        //        Token = accss_token;
        //    }
        //    return Token;
        //}
       public Wechatpat GetPatient(string openid)
        {
           //查询acccess_token
            string Token = string.Empty;
            DateTime YouXRQ = Convert.ToDateTime("1999-01-01");
            string appid = "";
            Wechatpat mode = new Wechatpat();

            Dictionary<string, object> dic = bll.GetAccess(openid);
            appid = dic["access_token_id"].ToString();
            if (dic["new_date"] != null && dic["new_date"].ToString() != "")
            {
                YouXRQ = Convert.ToDateTime(dic["new_date"]);
            }
            Token = dic["refresh_token"].ToString(); 

           
           //调用第三个接口，验证token有效性 
            //string url = "https://api.weixin.qq.com/cgi-bin/getcallbackip?access_token=" + appid;

            if (DateTime.Now > YouXRQ)
            {
                //利用refresh刷新
                OpenPat open=GetReAccess_token(Token);
                if (open.errmsg!=null)
                {
                    if (open.errmsg.IndexOf("invalid refresh_token") == 0)
                    {
                        //refresh_token过期，需重新授权
                        mode.errcode = "-1";
                        mode.errmsg = "refresh_token过期,需重新授权";
                        return mode;
                    }
                }              
                DateTime _youxrq = DateTime.Now;
                _youxrq = _youxrq.AddSeconds(open.expires_in);
                if (open.access_token!=null)
                {
                    DBOpt.SelAccessToken(open.access_token, _youxrq, open.refresh_token, open.openid);
                    appid = open.access_token;
                }
            }

            mode=shuashuashua(appid, openid, Token);
           
            return mode;
        }

       public Wechatpat shuashuashua(string appid, string openid,string Token)
       {
           Wechatpat mode = new Wechatpat();
           string strUrl = "https://api.weixin.qq.com/sns/userinfo?access_token=" + appid + "&openid=" + openid + "&lang=zh_CN";

           HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(strUrl);
           req.Method = "GET";
           using (WebResponse wr = req.GetResponse())
           {
               HttpWebResponse myResponse = (HttpWebResponse)req.GetResponse();
               StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
               string content = reader.ReadToEnd();
               if (content.IndexOf("openid") != -1)
               {
                   mode.openid = JsonHelper.GetVluesParentstr(content, "openid");
                   mode.nickname = JsonHelper.GetVluesParentstr(content, "nickname");
                   mode.sex = Convert.ToInt32(JsonHelper.GetVluesParentstr(content, "sex"));
                   mode.province = JsonHelper.GetVluesParentstr(content, "province");
                   mode.city = JsonHelper.GetVluesParentstr(content, "city");
                   mode.country = JsonHelper.GetVluesParentstr(content, "country");
                   mode.headimgurl = JsonHelper.GetVluesParentstr(content, "headimgurl");
                   mode.privilege = JsonHelper.GetVluesParentstr(content, "privilege");
                   mode.language = JsonHelper.GetVluesParentstr(content, "language");
                   LogHelp.WriteTextLog("【返回】" + "\r\n" + mode.nickname, mode.openid, "", "" + "\r\n" + "", System.DateTime.Now);
               }
               else
               {
                   mode.errcode = JsonHelper.GetVluesParentstr(content, "errcode");
                   mode.errmsg = JsonHelper.GetVluesParentstr(content, "errmsg");

                   if (mode.errcode.Equals("40001"))
                   {
                       OpenPat open = GetReAccess_token(Token);
                       if (open.errmsg != null)
                       {
                           if (open.errmsg.IndexOf("invalid refresh_token") == 0)
                           {
                               //refresh_token过期，需重新授权
                               mode.errcode = "-1";
                               mode.errmsg = "refresh_token过期,需重新授权";
                               return mode;
                           }
                       }
                       DateTime _youxrq = DateTime.Now;
                       _youxrq = _youxrq.AddSeconds(open.expires_in);
                       if (open.access_token != null)
                       {
                           DBOpt.SelAccessToken(open.access_token, _youxrq, open.refresh_token, open.openid);
                           shuashuashua(open.access_token, open.openid, open.refresh_token);
                       }
                   }
               }
               return mode;

           }
       }
    }
}