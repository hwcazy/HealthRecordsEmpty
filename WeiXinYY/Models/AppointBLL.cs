using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Xml;
using WeiXinYY.Content.Bean;
using WeiXinYY.Content.Utils;

namespace WeiXinYY.Models
{
    public class AppointBLL
    {
        private DBbll bll;
        private Access_tokenBll acc;
        private JsApiPay pay;
        public AppointBLL()
        {
            bll = new DBbll();
            acc = new Access_tokenBll();
            pay = new JsApiPay();
        }
 
        public string Get_Openid(string json)
        {
            string msg = "";
            string code = JsonHelper.GetVluesParentstr(json, "code");
            OpenPat pat = acc.GetAccess_token(code);
            msg = JsonHelper.getJsonByObject((Object)pat);
            return msg;
        }

        public string Get_WXPatient(string json)
        {
            string msg = "";
            //string token = JsonHelper.GetVluesParentstr(json, "access_token");
            string code = JsonHelper.GetVluesParentstr(json, "openid");
            Wechatpat pat = acc.GetPatient(code);
            msg = JsonHelper.getJsonByObject((Object)pat);
            return msg;
         }

        public string Register(string json)
        {
            Back<BsPatient> back = new Back<BsPatient>();
            BsPatient pat = new BsPatient();
            string msg = "";
            try
            {
                List<BsPatient> list = new List<BsPatient>();
                pat.openid = JsonHelper.GetVluesParentstr(json, "openid");
                pat.name = JsonHelper.GetVluesParentstr(json, "name");
                pat.isET = JsonHelper.GetVluesParentstr(json, "isET");
                pat.mobile = JsonHelper.GetVluesParentstr(json, "mobile");
                pat.idcardno = JsonHelper.GetVluesParentstr(json, "idcardno");

         
                        ////判断身份证号为15位还是18位
                        if (pat.idcardno.ToString().Length == 18)
                        {
                           ////性别
                            pat.sex = (Convert.ToInt32(pat.idcardno.Substring(16, 1)) % 2 == 1) ? "M" : "F";
                           var birthday = pat.idcardno.Substring(6, 8);
                            ////出生日期
                           pat.birthdate = Convert.ToDateTime(birthday.Substring(0, 4) + "-" + birthday.Substring(4, 2) + "-" + birthday.Substring(6, 2));
             
                        }
                        else
                        {
                           ////性别
                            pat.sex = (Convert.ToInt32(pat.idcardno.Substring(14, 1)) % 2 == 1) ? "M" : "F";
        
                           var birthday = pat.idcardno.Substring(6, 6);
                           ////出生日期
                           pat.birthdate = Convert.ToDateTime("19" + birthday.Substring(0, 2) + "-" + birthday.Substring(2, 2) + "-" + birthday.Substring(4, 2));
                        }
                bool result=bll.SavePat(ref pat);
                if (result)
                {
                    list.Add(pat);
                    back.errcode = 0;
                    back.errmsg = "";
                    back.data = list;
                }
                else
                {
                    back.errcode = -1;
                    back.errmsg = "保存数据库失败！";
                    back.data = new List<BsPatient>();
                }
               
            }
            catch (Exception ex)
            {
                back.errcode = -1;
                back.errmsg = ex.Message;
                back.data = new List<BsPatient>();
            }
            msg = JsonHelper.getJsonByObject((Object)back);
            return msg;

        }

        public string Get_Registerpat(string json)
        {
            Back<BsPatient> back = new Back<BsPatient>();
            string msg = "";
            string openid = "";
            try
            {
                List<BsPatient> list = new List<BsPatient>();
                openid = JsonHelper.GetVluesParentstr(json, "openid");

                DataTable result = bll.QueryRegister(openid);

                if (result == null || result.Rows.Count == 0)
                {
                    back.errcode = 0;
                    back.errmsg = "暂无添加就诊人";
                    back.data = new List<BsPatient>();
                }
                else
                {

                    foreach (DataRow row in result.Rows)
                    {
                        BsPatient pat = new BsPatient();
                        pat.cardno = row["cardno"].ToString();
                        pat.name = row["name"].ToString();
                        pat.idcardno = row["idcardno"].ToString();
                        pat.mobile = row["mobile"].ToString();
                        if (pat.cardno.IndexOf("E")==0)
                        {
                            pat.isET = "2";
                        }
                        else
                        {
                            pat.isET = "1";
                        }
                        pat.birthdate = System.DateTime.Now;
                        list.Add(pat);
                    }
                    
                    back.errcode = 0;
                    back.errmsg = "";
                    back.data = list;
                }

            }
            catch (Exception ex)
            {
                back.errcode = -1;
                back.errmsg = ex.Message;
                back.data = new List<BsPatient>();
            }
            msg = JsonHelper.getJsonByObject((Object)back);
            return msg;
        }

        public string Get_AppointDept(string json)
        {
            Back<BsDepartment> back = new Back<BsDepartment>();
            string msg = "";
              try
            {
            DataTable table=bll.QueryAllDept("5037");
            List<BsDepartment> list = new List<BsDepartment>();
            foreach (DataRow row in table.Rows)
            {
                BsDepartment dept = new BsDepartment();
                dept.ID = Convert.ToInt32(row["id"]);
                dept.FatherId = row["fatherid"]==DBNull.Value?999:Convert.ToInt32(row["fatherid"]);
                dept.Name = row["name"].ToString();
                dept.PhotoPath = row["photopath"].ToString();
                list.Add(dept);
            }
            back.errcode = 0;
            back.errmsg = "";
            back.data= list;
            }
              catch (Exception ex)
              {
                  back.errcode = -1;
                  back.errmsg = ex.Message;
                  back.data = new List<BsDepartment>();
              }
            msg = JsonHelper.getJsonByObject((Object)back);
            return msg;
        }
      

        //DateTime startMonth = dt.AddDays(1 - dt.Day);  //本月月初
        //DateTime endMonth = startMonth.AddMonths(1).AddDays(-1);  //本月月末//

        public string Get_DoctByDept(string json)
        {
            Back<Oudayschedule> back = new Back<Oudayschedule>();
            string msg = "";
            string xml = "";
            try
            {
                int deptid = Convert.ToInt32(JsonHelper.GetVluesParentstr(json, "deptid"));
                DateTime today, subst, subend;

                today = DateTime.Now;
                subst = CommUtil.GetTimeStartByType("Week", today);
                subend = CommUtil.GetTimeEndByType("Week", today);

                DataTable doct = bll.QueryDoctByDept(deptid, today, subend);
                List<Oudayschedule> list = new List<Oudayschedule>();
                if (doct == null || doct.Rows.Count == 0)
                {
                    //提示无挂号医生
                    back.errcode = 0;
                    back.errmsg = "暂无可挂号医生";
                    back.data = new List<Oudayschedule>();
                }
                else
                {
                    int mainNum = doct.Rows.Count;

                    foreach (DataRow row in doct.Rows)
                    {
                        Oudayschedule days = new Oudayschedule();
                        BsUser user = new BsUser();
                        user.doctorid = Convert.ToInt32(row["id"]);
                        user.name = row["docname"].ToString();
                        user.deptid = Convert.ToInt32(row["deptid"]);
                        user.deptname = row["deptname"].ToString();
                        user.docgra = row["docgra"].ToString();
                        user.price = Convert.ToDouble(row["price"]);
                        user.resume = row["resume"].ToString();
                        user.picture = row["picture"].ToString();
                        days.user = user;
                        DataTable datalist = bll.QuerySchBydoct(user.doctorid, today, subend);
                        List<Schdatecs> dates = new List<Schdatecs>();
                        foreach (DataRow rw in datalist.Rows)
                        {
                            Schdatecs sch = new Schdatecs();
                            //sch.id = Convert.ToInt32(rw["id"]);
                            sch.scheduledate = Convert.ToDateTime(rw["scheduledate"]).ToString("MM-dd");
                            sch.week = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(Convert.ToDateTime(rw["scheduledate"]).DayOfWeek);
                            dates.Add(sch);
                        }
                        days.list = dates;
                        list.Add(days);
                    }
                    back.errcode = 0;
                    back.errmsg = "";
                    xml = JsonHelper.getJsonByObject((Object)list);
                    back.data = list;
                }
            }catch(Exception ex){
                back.errcode = -1;
                back.errmsg =ex.Message;
                back.data = new List<Oudayschedule>();
            }
            msg = JsonHelper.getJsonByObject((Object)back);
            return msg;
        }

        public string Get_NextDoctByDept(string json)
        {
            Back<Oudayschedule> back = new Back<Oudayschedule>();
            string msg = "";
            string xml = "";
             try
            {
            int deptid = Convert.ToInt32(JsonHelper.GetVluesParentstr(json, "deptid"));
            DateTime today,endtoday,nextst, nextend;

            today = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
            endtoday = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 23:59:59"));
            var nextday = today.AddDays(Convert.ToInt32(1 - Convert.ToInt32(today.DayOfWeek)) + 7);
            nextst = CommUtil.GetTimeStartByType("Week", nextday);
            var nextendday = endtoday.AddDays(Convert.ToInt32(1 - Convert.ToInt32(endtoday.DayOfWeek)) + 7);
            nextend = CommUtil.GetTimeEndByType("Week", nextendday);

            DataTable doct = bll.QueryDoctByDept(deptid, nextst, nextend);
            List<Oudayschedule> list = new List<Oudayschedule>();
            if (doct == null || doct.Rows.Count == 0)
            {
                //提示无挂号医生
                back.errcode = 0;
                back.errmsg ="暂无可挂号医生";
                back.data=new List<Oudayschedule>();
            }
            else
            {
                int mainNum = doct.Rows.Count;

                foreach (DataRow row in doct.Rows)
                {
                    Oudayschedule days = new Oudayschedule();
                    BsUser user = new BsUser();
                    user.doctorid = Convert.ToInt32(row["id"]);
                    user.name = row["docname"].ToString();
                    user.deptid = Convert.ToInt32(row["deptid"]);
                    user.deptname = row["deptname"].ToString();
                    user.docgra = row["docgra"].ToString();
                    user.price = Convert.ToDouble(row["price"]);
                    user.resume = row["resume"].ToString();
                    user.picture = row["picture"].ToString();
                    days.user = user;
                    DataTable datalist = bll.QuerySchBydoct(user.doctorid, nextst, nextend);
                    List<Schdatecs> dates = new List<Schdatecs>();
                    foreach (DataRow rw in datalist.Rows)
                    {
                        Schdatecs sch = new Schdatecs();
                        //sch.id = Convert.ToInt32(rw["id"]);
                        sch.scheduledate = Convert.ToDateTime(rw["scheduledate"]).ToString("MM-dd");
                        sch.week = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(Convert.ToDateTime(rw["scheduledate"]).DayOfWeek);
                        dates.Add(sch);
                    }
                    days.list = dates;
                    list.Add(days);
                }
                back.errcode = 0;
                back.errmsg = "";
                xml = JsonHelper.getJsonByObject((Object)list);
                back.data = list;
            }
            }
             catch (Exception ex)
             {
                 back.errcode = -1;
                 back.errmsg = ex.Message;
                 back.data = new List<Oudayschedule>();
             }
            msg = JsonHelper.getJsonByObject((Object)back);
      
            return msg;

        }


        public string Get_OuSchDtl(string json)
        {
            Back<Schedudatelist> back = new Back<Schedudatelist>();
            string msg = "";
            string xml = "";
            try
            {
                int doctorid = Convert.ToInt32(JsonHelper.GetVluesParentstr(json, "doctorid"));
                string flag = JsonHelper.GetVluesParentstr(json, "flag");
                DateTime today, subst, subend,endtoday, nextst, nextend;
                DataTable datalist = new DataTable();
            
                if(flag.Equals("1")){
                    today = DateTime.Now;
                    subst = CommUtil.GetTimeStartByType("Week", today);
                    subend = CommUtil.GetTimeEndByType("Week", today);

                    datalist=bll.QuerySchBydoct(doctorid, today, subend);
                }else{  //2

                 today = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
                 endtoday = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 23:59:59"));
                 var nextday = today.AddDays(Convert.ToInt32(1 - Convert.ToInt32(today.DayOfWeek)) + 7);
                 nextst = CommUtil.GetTimeStartByType("Week", nextday);
                 var nextendday = endtoday.AddDays(Convert.ToInt32(1 - Convert.ToInt32(endtoday.DayOfWeek)) + 7);
                 nextend = CommUtil.GetTimeEndByType("Week", nextendday);
                 datalist = bll.QuerySchBydoct(doctorid, nextst, nextend);
                }

                List<Schedudatelist> dates = new List<Schedudatelist>();
                if (datalist == null || datalist.Rows.Count == 0)
                {
                    //提示无挂号医生
                    back.errcode = 0;
                    back.errmsg = "暂无可挂号医生";
                    back.data = new List<Schedudatelist>();
                }
                else
                { 
                    foreach (DataRow row in datalist.Rows)
                    {
                        Schedudatelist sch = new Schedudatelist();
                        sch.scheduledate = Convert.ToDateTime(row["scheduledate"]).ToString("MM-dd");
                        sch.schedule = Convert.ToDateTime(row["scheduledate"]).ToString("yyyy-MM-dd");
                        sch.week = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(Convert.ToDateTime(row["scheduledate"]).DayOfWeek);

                        DataTable yhs = bll.QueryYH(doctorid, Convert.ToDateTime(row["scheduledate"]).ToString("yyyy-MM-dd"));
                        List<ScheduShow> yhslist = new List<ScheduShow>();
                        foreach (DataRow rw in yhs.Rows)
                        {
                            ScheduShow sche = new ScheduShow();
                            sche.name= rw["name"].ToString();
                            sche.spanid = Convert.ToInt32(rw["timespanid"]);
                            sche.yh = Convert.ToInt32(rw["booklimitnum"]) - Convert.ToInt32(rw["ygh"]);
                            yhslist.Add(sche);
                        }
                        sch.list = yhslist;
                        dates.Add(sch);
                    }
                    back.errcode = 0;
                    back.errmsg = "";
                    xml = JsonHelper.getJsonByObject((Object)dates);
                    back.data = dates;
                }
            }
            catch (Exception ex)
            {
                back.errcode = -1;
                back.errmsg = ex.Message;
                back.data = new List<Schedudatelist>();
            }
            msg = JsonHelper.getJsonByObject((Object)back);
            return msg;
        }

         //<summary>
         //具体就诊时段
         //</summary>
         //<param name="json"></param>
         //<returns></returns>
        public string Get_JzTime(string json)
        {
            Back<JzTime> back = new Back<JzTime>();
            string msg = "";
            try
            {
                //int doctorid = Convert.ToInt32(JsonHelper.GetVluesParentstr(json, "doctorid"));
                string schedate =JsonHelper.GetVluesParentstr(json, "schedate");
                string spanid = JsonHelper.GetVluesParentstr(json, "spanid");
                DateTime today,scheday;
                today = DateTime.Now;
                scheday = Convert.ToDateTime(schedate);

                Dictionary<string, object> jz = bll.QueryJZtime(spanid);
                string begin=jz["timebeginm"].ToString();
                string end = jz["timeendm"].ToString();

                DateTime dt1 = DateTime.Parse(schedate+" "+begin);
                DateTime dt2 = DateTime.Parse(schedate+" "+end);

                Dictionary<int, DateTime> list = new Dictionary<int,DateTime>();
                int i = 1;
                while (dt1 <= dt2)
                {
                    list.Add(i,dt1);
                    dt1 = dt1.AddMinutes(30);
                    i++;
                }

                List<JzTime> timeList = new List<JzTime>();
                for (int j = 1; j < i-1; j++)
                {
                    JzTime jztime = new JzTime();
                    DateTime d1 = list[j];
                    DateTime d2 = list[j+1];
                    string time = d1.ToString("HH:mm") + "-" + d2.ToString("HH:mm");
                    jztime.jzTime = time;

                    if (today<d2)
                    {
                        timeList.Add(jztime);
                    }     
                }
                    back.errcode = 0;
                    back.errmsg = "";
                    back.data = timeList; 
            }
            catch (Exception ex)
            {
                back.errcode = -1;
                back.errmsg = ex.Message;
                back.data = new List<JzTime>();
            }
            msg = JsonHelper.getJsonByObject((Object)back);
            return msg;
        }

        public string Save_OUHOSINFO(string json)
        {
            WxPayData data=new WxPayData();
            string msg = "";
            try
            {
                string openid = JsonHelper.GetVluesParentstr(json, "openid");
                int doctorid = Convert.ToInt32(JsonHelper.GetVluesParentstr(json, "doctorid"));
                string schedate = JsonHelper.GetVluesParentstr(json, "schedate");
                string spanid = JsonHelper.GetVluesParentstr(json, "spanid");
                string timename = JsonHelper.GetVluesParentstr(json, "jztime");
                Decimal total_mon = Convert.ToDecimal(JsonHelper.GetVluesParentstr(json, "price")) * 100;//分
                var begin = timename.Substring(0, 5);
                int length = timename.Trim().Length;
                var end = timename.Substring(length - 5);

                DateTime dt1 = DateTime.Parse(schedate + " " + begin);
                DateTime dt2 = DateTime.Parse(schedate + " " + end);

                Dictionary<string, object> info = bll.QueryAppinfo(doctorid, schedate, spanid);
                info.Add("begin", dt1);
                info.Add("timebegin", begin);
                info.Add("timeend", end);
                info.Add("preregtime", DateTime.Now);
                Dictionary<string, object> patient = bll.QueryPatient(openid);

                data = pay.GetUnifiedOrderResult(openid, Convert.ToInt32(total_mon));
                info.Add("trade_no", data.GetValue("trade_no"));
                bool result = bll.SaveOUHOSINFO(info, patient);

                if (result==true)
                {
                    data.SetValue("errcode", 0);
                    data.SetValue("errmsg", "");
                  
                }
                else
                {
                    data.SetValue("errcode", -1);
                    data.SetValue("errmsg","保存数据库失败!");
                }
                msg = data.ToJson();
            }
            catch (Exception ex)
            {
                data.SetValue("errcode", -1);
                data.SetValue("errmsg", ex.Message);
                msg = data.ToJson();
            }
            return msg;

        }

        public string Get_Url(string xmlData)
        {
            //转换数据格式并验证签名
            WxPayData data = new WxPayData();
            WxPayData res = new WxPayData();
            string xml = "";
            double money=0;
            bool result = false;
            try
            {
                //SortedDictionary<string, object> datalist=data.FromXml(xmlData);
                //data.m_values = datalist;
                //money=Convert.ToInt32(data.GetValue("total_fee"))/100;
                money = 0.01;
                //data.GetValue("openid");
                //LogHelp.WriteTextLog("【返回】" + "\r\n" + xmlData, xml, "", "" + "\r\n" + "", System.DateTime.Now);
                //用订单号去存
                //Dictionary<string, object> dic=bll.Queryout_trade_no(data.GetValue("out_trade_no").ToString());
                Dictionary<string, object> dic = bll.Queryout_trade_no("151564898120190109204445649");
                if (money == Convert.ToDouble(dic["cash"]))
                {
                    Int64 id = Convert.ToInt64(dic["id"]);
                    Dictionary<string, object> by=bll.QueryPatById(id);
                    by.Add("price", money);
                    by.Add("id", id);
                    result=bll.SaveOuInvoice(by);
                }
                else
                {
                    res.SetValue("return_code", "FAIL");
                    res.SetValue("return_msg","");
                    xml = res.ToXml();
                }
                //保存数据库
                if(!result){
                    res.SetValue("return_code", "FAIL");
                    res.SetValue("return_msg", "");
                    xml = res.ToXml();
                }   
            }
            catch (WxPayException ex)
            {
                //若签名错误，则立即返回结果给微信支付后台
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", ex.Message);
                xml=res.ToXml();
            }
            ResultNotify ify = new ResultNotify();
            xml = ify.ProcessNotify(data);
            LogHelp.WriteTextLog("【返回】" + "\r\n" + xmlData, xml, "", "" + "\r\n" + "", System.DateTime.Now);
            return xml;
        }

      
  
    }
}