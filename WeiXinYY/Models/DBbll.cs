using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Web;
using WeiXinYY.Content.Bean;

namespace WeiXinYY.Models
{
    public class DBbll
    {
        private DBOpt opt;
        public DBbll()
        {
            opt = new DBOpt();
        }
   
        public DataTable QueryAllDept(string locationId)
        {
            string sql = @"select
BsDepartment.ID,
BsDepartment.FatherId,
BsDepartment.Name,
BsDepartment.PhotoPath
from BsDepartment
where BsDepartment.IsActive=1 and BsDepartment.IsRegistered=1 and BsDepartment.locationId=:locationId";
            OracleParameter[] parameters = { new OracleParameter(":locationId", OracleType.VarChar) };
            parameters[0].Value = locationId;
            return opt.GetTable(sql, parameters);
        }

        public DataTable QueryDoctByDept(int deptId,DateTime start,DateTime end)
        {
            string sql = @"SELECT max(t1.NAME) docname,t0.DOCTORID id,max(t3.NAME) deptname,max(t3.id) deptid,max(t2.name) docgra,max(t2.diagnosismoney) price,max(t1.resume) resume,max(t1.PICTUREPATH) picture
FROM OUDAYSCHEDULE t0
LEFT OUTER JOIN BSUSER t1
ON (t1.ID=t0.DOCTORID)
LEFT OUTER  JOIN BSDOCLEVEL t2
ON (t2.ID=t1.DOCLEVID)
LEFT OUTER JOIN BSDEPARTMENT t3
ON (t3.ID=t1.DEPARTMENTID)
LEFT OUTER JOIN BSLOCATION t4
ON (t4.ID=t0.LOCATIONID)
LEFT OUTER JOIN BSREGTIMESPAN t5
ON(t5.ID=t0.TIMESPANID)
WHERE (NOT t0.ISSTOP<>0 AND t0.ISACTIVE<>0) AND t0.ISDOCTOR=1 
AND t1.DEPARTMENTID=:deptId
AND t0.SCHEDULEDATE>=TO_DATE(:starttime,'YYYY-MM-DD')
AND t0.SCHEDULEDATE<=TO_DATE(:endtime,'YYYY-MM-DD')
and ((to_date(TO_CHAR(t0.SCHEDULEDATE,'YYYY-MM-DD')||' '||t5.TIMEBEGIN,'yyyy-MM-dd hh24:mi')
<=to_date(:nowtime,'yyyy-MM-dd hh24:mi')
AND to_date(TO_CHAR(t0.SCHEDULEDATE,'YYYY-MM-DD')||' '||t5.TIMEEND,'yyyy-MM-dd hh24:mi')
>=to_date(:nowtime,'yyyy-MM-dd hh24:mi'))
or (to_date(TO_CHAR(t0.SCHEDULEDATE,'YYYY-MM-DD')||' '||t5.TIMEBEGIN,'yyyy-MM-dd hh24:mi')
>=to_date(:nowtime,'yyyy-MM-dd hh24:mi')
AND to_date(TO_CHAR(t0.SCHEDULEDATE,'YYYY-MM-DD')||' '||t5.TIMEEND,'yyyy-MM-dd hh24:mi')
>=to_date(:nowtime,'yyyy-MM-dd hh24:mi')))
group by t0.DOCTORID order by t0.DOCTORID ";
            OracleParameter[] parameters = { new OracleParameter(":deptId", OracleType.Int32),
                                           new OracleParameter(":starttime", OracleType.VarChar),
                                           new OracleParameter(":endtime", OracleType.VarChar),
                                            new OracleParameter(":nowtime", OracleType.VarChar)};
            parameters[0].Value = deptId;
            parameters[1].Value = start.ToString("yyyy-MM-dd");
            parameters[2].Value = end.ToString("yyyy-MM-dd");
            parameters[3].Value = start.ToString("yyyy-MM-dd HH:mm");
            return opt.GetTable(sql, parameters);
        }

        public DataTable QuerySchBydoct(int doctid, DateTime start, DateTime end)
        {
            string sql = @"SELECT distinct t0.scheduledate 
FROM OUDAYSCHEDULE t0
LEFT OUTER JOIN BSUSER t1
ON(t1.ID=t0.DOCTORID)
LEFT OUTER  JOIN BSDOCLEVEL t2
ON(t2.ID=t1.DOCLEVID)
LEFT OUTER JOIN BSDEPARTMENT t3
ON(t3.ID=t1.DEPARTMENTID)
LEFT OUTER JOIN BSLOCATION t4
ON(t4.ID=t0.LOCATIONID)
LEFT OUTER JOIN BSREGTIMESPAN t5
ON(t5.ID=t0.TIMESPANID)
WHERE (NOT t0.ISSTOP<>0 AND t0.ISACTIVE<>0) AND t0.ISDOCTOR=1 
and t0.DOCTORID=:doctid
AND t0.SCHEDULEDATE>=TO_DATE(:starttime,'YYYY-MM-DD')
AND t0.SCHEDULEDATE<=TO_DATE(:endtime,'YYYY-MM-DD')
and ((to_date(TO_CHAR(t0.SCHEDULEDATE,'YYYY-MM-DD')||' '||t5.TIMEBEGIN,'yyyy-MM-dd hh24:mi')
<=to_date(:nowtime,'yyyy-MM-dd hh24:mi')
AND to_date(TO_CHAR(t0.SCHEDULEDATE,'YYYY-MM-DD')||' '||t5.TIMEEND,'yyyy-MM-dd hh24:mi')
>=to_date(:nowtime,'yyyy-MM-dd hh24:mi'))
or (to_date(TO_CHAR(t0.SCHEDULEDATE,'YYYY-MM-DD')||' '||t5.TIMEBEGIN,'yyyy-MM-dd hh24:mi')
>=to_date(:nowtime,'yyyy-MM-dd hh24:mi')
AND to_date(TO_CHAR(t0.SCHEDULEDATE,'YYYY-MM-DD')||' '||t5.TIMEEND,'yyyy-MM-dd hh24:mi')
>=to_date(:nowtime,'yyyy-MM-dd hh24:mi')))
 order by t0.scheduledate ";
            OracleParameter[] parameters = { new OracleParameter(":doctid", OracleType.Int32),
                                           new OracleParameter(":starttime", OracleType.VarChar),
                                           new OracleParameter(":endtime", OracleType.VarChar),
                                           new OracleParameter(":nowtime", OracleType.VarChar)};
            parameters[0].Value = doctid;
            parameters[1].Value = start.ToString("yyyy-MM-dd");
            parameters[2].Value = end.ToString("yyyy-MM-dd");
            parameters[3].Value = start.ToString("yyyy-MM-dd HH:mm");
            return opt.GetTable(sql, parameters);
        }


         public DataTable QueryYH(int doctid, string date)
        {
            string sql = @"SELECT t0.scheduledate,t0.TIMESPANID,t5.name,t0.booklimitnum,
(
SELECT COUNT(*)
FROM OUHOSINFO t7
WHERE(((((((CAST(TO_DATE(TO_CHAR(t7.REGTIME,'YYYY-MM-DD'),'YYYY-MM-DD')AS VARCHAR2(100))||' ')||t7.TIMEBEGIN)>=((CAST(TO_DATE(TO_CHAR(t0.SCHEDULEDATE,'YYYY-MM-DD'),'YYYY-MM-DD')AS VARCHAR2(100))||' ')
||t5.TIMEBEGIN))AND(((CAST(TO_DATE(TO_CHAR(t7.REGTIME,'YYYY-MM-DD'),'YYYY-MM-DD')AS VARCHAR2(100))||' ')||t7.TIMEEND)<=((CAST(TO_DATE(TO_CHAR(t0.SCHEDULEDATE,'YYYYMMDD'),'YYYYMMDD')
AS VARCHAR2(100))||' ')||t5.TIMEEND)))AND(t7.DOCTORID=t0.DOCTORID))AND(t7.REGDEPT=t0.LOCATIONID))AND((t7.ISCANCEL=0)) and t7.isprereg=1)) ygh
FROM OUDAYSCHEDULE t0
LEFT OUTER JOIN BSUSER t1
ON (t1.ID=t0.DOCTORID)
LEFT OUTER  JOIN BSDOCLEVEL t2
ON (t2.ID=t1.DOCLEVID)
LEFT OUTER JOIN BSDEPARTMENT t3
ON (t3.ID=t1.DEPARTMENTID)
LEFT OUTER JOIN BSLOCATION t4
ON (t4.ID=t0.LOCATIONID)
LEFT OUTER JOIN BSREGTIMESPAN t5
ON(t5.ID=t0.TIMESPANID)
WHERE (NOT t0.ISSTOP<>0 AND t0.ISACTIVE<>0) AND t0.ISDOCTOR=1 
and t0.DOCTORID=:doctid
AND t0.SCHEDULEDATE=TO_DATE(:starttime,'yyyy-MM-dd')";
            OracleParameter[] parameters = { new OracleParameter(":doctid", OracleType.Int32),
                                           new OracleParameter(":starttime", OracleType.VarChar)
                                         };
            parameters[0].Value = doctid;
            parameters[1].Value = date;
            return opt.GetTable(sql, parameters);
        }

         public string GetCount(string openid)
         {
             string sql = @"SELECT count(*) FROM access_token where openid=:openid";
             OracleParameter parameters =
             new OracleParameter(":openid", OracleType.VarChar);
             parameters.Value = openid;
             return opt.GetValue(sql, parameters).ToString();
         }
         public Dictionary<string, object> GetAccess(string openid)
         {
             string sql = @"SELECT access_token_id,new_date,refresh_token FROM access_token where openid=:openid";
             OracleParameter parameters =
             new OracleParameter(":openid", OracleType.VarChar);
             parameters.Value = openid;
             return opt.GetRow(sql,parameters);
         }

        /// <summary>
        /// 查询openid有没有
        /// </summary>
        /// <returns></returns>
         public string GetPatCount(string openid)
         {
             string sql = @"SELECT count(*) FROM wechatpatient where openid=:paopenid";
             OracleParameter parameters =
               new OracleParameter(":paopenid", OracleType.VarChar);
             parameters.Value = openid;
             return opt.GetValue(sql, parameters).ToString();
         }

         public bool SaveOpenid(string openid)
         {
             string sql = @"insert into wechatpatient(openid)  values (:openid)";
             OracleParameter[] parameters = { new OracleParameter(":openid", OracleType.VarChar),
                                               
            };
             parameters[0].Value =openid;
             return opt.SavaChange(sql, parameters);
         }

         public string GetVipCard(string flag)
         {
             string sql = "";
             string carno = "";
             if (flag.Equals("1"))//000001
             {
                 sql = "select max(cardno) cardno from VipCard where instr(upper(cardno),'E')=0";
             }
             else //E000001
             {
                 sql = "select max(cardno) cardno from VipCard where instr(upper(cardno),'E')>0";
             }
             carno=opt.GetValue(sql,new OracleParameter()).ToString();
             int no = 0;
             if (carno.IndexOf("E")==0)
             {
                 carno = carno.Substring(1,2);
                 no = Convert.ToInt32(carno) + 1;
                 carno = no.ToString();
                 for (int i = 0; i < 6 - no.ToString().Length; i++)
                 {
                     carno = "0" + carno;
                 }
                 carno="E"+carno;
             }
             else
             {
                 no = Convert.ToInt32(carno) + 1;
                 carno = no.ToString();
                 for (int i = 0; i < 6 - no.ToString().Length;i++ )
                 {
                     carno = "0" + carno;
                 }

             }
  
             return carno;
         }

         public string GetNO(string name)
         {
             string no = "";
             DataTable dt=opt.Getbillno(name);
             foreach(DataRow dr in dt.Rows){
                 for (int i = 0; i < dt.Columns.Count; i++)
                 {
                     no = dr[i].ToString();
                 }
             }
             return no;
         }
        /// <summary>
        /// 新增客户，绑定会员卡
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
         public bool SavePat(ref BsPatient patient)
         {
             var sqls = new List<string>();
             var parsList = new List<List<OracleParameter>>();
             var sql1 = @"insert into BSPATIENT(id,cardno,name,sex,lsbithabit,sleephour,lssleeptrouble,opertime,operid,isactive,moditime,birthdate,mobile,idcardno,medicareno,locationid,vipcardno,viplevel) 
             values(bspatient_id_seq.nextval,:cardno,:name,:sex,:lsbithabit,:sleephour,:lssleeptrouble,:opertime,:operid,:isactive,:moditime,:birthdate,:mobile,:idcardno,:medicareno,:locationid,:vipcardno,:viplevel)";

             var sql2 = @"insert into vipcard(id,patid,cardno,viplevelid,discount,password,createtime,invaliddate,operid,cardstatus,isactive,introducerid,introduceroperid) 
                  values (vipcard_id_seq.nextval,bspatient_id_seq.currval,:cardno,:viplevelid,:discount,:password,:createtime,:invaliddate,:operid,:cardstatus,:isactive,:introducerid,:introduceroperid)";

             var sql3 = @"insert into VipBinding (id,patid,vipcardid,iscardholder,isactive,operid,opertime) 
                  values (vipbinding_id_seq.nextval,bspatient_id_seq.currval,vipcard_id_seq.currval,:iscardholder,:isactive,:operid,:opertime)";

             var sql4 = @"insert into OuRecharge (id,patid,cardno,opertype,opertime,locationid,vipcardid) 
                  values (ourecharge_id_seq.nextval,bspatient_id_seq.currval,:cardno,:opertype,:opertime,:locationid,vipcard_id_seq.currval)";

             var sql5 = "update wechatpatient set patientid=bspatient_id_seq.currval where openid=:openid";

             sqls.Add(sql1);
             sqls.Add(sql2);
             sqls.Add(sql3);
             sqls.Add(sql4);
             sqls.Add(sql5);
             patient.cardno = GetVipCard(patient.isET);
             var childParas1 = new List<OracleParameter>();
             childParas1.Add(new OracleParameter(":cardno", GetNO("11")));
             childParas1.Add(new OracleParameter(":name", patient.name));
             childParas1.Add(new OracleParameter(":sex", patient.sex));
             OracleParameter param1 = new OracleParameter(":lsbithabit", OracleType.Number);
             param1.Value =0;
             childParas1.Add(param1);
             OracleParameter param2 = new OracleParameter(":sleephour", OracleType.Number);
             param2.Value = 0;
             childParas1.Add(param2);
             OracleParameter param3 = new OracleParameter(":lssleeptrouble", OracleType.Number);
             param3.Value = 0;
             childParas1.Add(param3);

             //childParas1.Add(new OracleParameter(":lsbithabit", 0));
             //childParas1.Add(new OracleParameter(":sleephour",0));
             //childParas1.Add(new OracleParameter(":lssleeptrouble",0));
             childParas1.Add(new OracleParameter(":opertime", System.DateTime.Now));
             childParas1.Add(new OracleParameter(":operid", 9));
             childParas1.Add(new OracleParameter(":isactive",1));
             childParas1.Add(new OracleParameter(":moditime", System.DateTime.Now));
             childParas1.Add(new OracleParameter(":birthdate", patient.birthdate));
             childParas1.Add(new OracleParameter(":mobile", patient.mobile));
             childParas1.Add(new OracleParameter(":idcardno", patient.idcardno));
             childParas1.Add(new OracleParameter(":medicareno", patient.idcardno));
             childParas1.Add(new OracleParameter(":locationid",5037));
             childParas1.Add(new OracleParameter(":vipcardno", patient.cardno));
             childParas1.Add(new OracleParameter(":viplevel", 30));
             parsList.Add(childParas1);

  
             var childParas2 = new List<OracleParameter>();
             childParas2.Add(new OracleParameter("cardno", patient.cardno));
             childParas2.Add(new OracleParameter("viplevelid",30));
             childParas2.Add(new OracleParameter("discount",100.00));
             childParas2.Add(new OracleParameter("password", "1"));
             childParas2.Add(new OracleParameter("createtime",DateTime.Now));
             childParas2.Add(new OracleParameter("invaliddate", DateTime.Now.AddYears(2)));
             childParas2.Add(new OracleParameter("operid", 9));
             childParas2.Add(new OracleParameter("cardstatus",1));
              childParas2.Add(new OracleParameter("isactive",1));
              OracleParameter param4 = new OracleParameter(":introducerid", OracleType.Number);
              param4.Value = 0;
              childParas2.Add(param4);
              OracleParameter param5 = new OracleParameter(":introduceroperid", OracleType.Number);
              param5.Value = 0;
              childParas2.Add(param5);
              //childParas2.Add(new OracleParameter("introducerid",0));
              //childParas2.Add(new OracleParameter("introduceroperid",0));
             parsList.Add(childParas2);

             var childParas3 = new List<OracleParameter>();
             OracleParameter param6 = new OracleParameter(":iscardholder", OracleType.Number);
             param6.Value = 0;
             childParas3.Add(param6);
            // childParas3.Add(new OracleParameter("iscardholder",0));
             childParas3.Add(new OracleParameter("isactive",1));
             childParas3.Add(new OracleParameter("operid",9));
             childParas3.Add(new OracleParameter("opertime", DateTime.Now));
            
             parsList.Add(childParas3);

             var childParas4 = new List<OracleParameter>();
             childParas4.Add(new OracleParameter("cardno", patient.cardno));
             OracleParameter param7 = new OracleParameter(":opertype", OracleType.Number);
             param7.Value = 0;
             childParas4.Add(param7);
             //childParas4.Add(new OracleParameter("opertype",0));
             childParas4.Add(new OracleParameter("opertime", DateTime.Now));
             childParas4.Add(new OracleParameter("locationid",5037));

             parsList.Add(childParas4);

             var childParas5 = new List<OracleParameter>();
             childParas5.Add(new OracleParameter("openid",patient.openid));
             parsList.Add(childParas5);

             return opt.ChangeDataWithTransaction(sqls, parsList);
         }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
         public Dictionary<string, object> QueryJZtime(string spanid)
         {
             string sql = @"select min(b.timebegin) timebeginm,max(b.timeend) timeendm from bsregspansub b where b.TIMESPANID=:spanid and b.isactive=1";
             OracleParameter parameters =
              new OracleParameter(":spanid", OracleType.VarChar);
             parameters.Value = spanid;
             return opt.GetRow(sql, parameters);
         }

         public Dictionary<string, object> QueryAppinfo(int doctid, string date, string spanname)
         {
             string sql = @"SELECT t0.scheduledate,t0.doctorid,t2.diagnosismoney,t0.regtypeid,t0.TIMESPANID,t1.DEPARTMENTID
FROM OUDAYSCHEDULE t0
LEFT OUTER JOIN BSUSER t1
ON (t1.ID=t0.DOCTORID)
LEFT OUTER  JOIN BSDOCLEVEL t2
ON (t2.ID=t1.DOCLEVID)
LEFT OUTER JOIN BSDEPARTMENT t3
ON (t3.ID=t1.DEPARTMENTID)
LEFT OUTER JOIN BSLOCATION t4
ON (t4.ID=t0.LOCATIONID)
LEFT OUTER JOIN BSREGTIMESPAN t5
ON(t5.ID=t0.TIMESPANID)
WHERE (NOT t0.ISSTOP<>0 AND t0.ISACTIVE<>0) AND t0.ISDOCTOR=1 
and t0.DOCTORID=:doctid
and t5.ID=:spanname
AND t0.SCHEDULEDATE=TO_DATE(:starttime,'yyyy-MM-dd')";
             OracleParameter[] parameters = { new OracleParameter(":doctid", OracleType.Int32),
                                              new OracleParameter(":spanname", OracleType.Int32),
                                           new OracleParameter(":starttime", OracleType.VarChar)
                                         };
             parameters[0].Value = doctid;
             parameters[1].Value =Convert.ToInt32(spanname);
             parameters[2].Value = date;
             return opt.GetRow(sql, parameters);
         }

         public Dictionary<string, object> QueryPatient(string openid)
         {
             string sql = @"select a.id,a.cardno,a.name,a.mobile
    from BSPATIENT a,wechatpatient b
    where a.id=b.patientid 
    and b.openid=:openid and a.isactive=1";
             OracleParameter parameters =
              new OracleParameter(":openid", OracleType.VarChar);
             parameters.Value = openid;
             return opt.GetRow(sql, parameters);
         }
         public bool SaveOUHOSINFO(Dictionary<string, object> info, Dictionary<string, object> pat)
         {
             var sqls = new List<string>();
             var parsList = new List<List<OracleParameter>>();
             var sql1 = @"insert into OuHosinfo (id,regtime,cardno,patid,name,regtypeid,regfee,doctorid,isprereg,opertime,operid,timespanid,preregtime,bookingtel,ischarge,timebegin,timeend,departmentid,pattypeid)
                                       values (Ouhosinfo_Id_Seq.nextval,:regtime,:cardno,:patid,:patname,:regtypeid,:regfee,:doctorid,:isprereg,:opertime,:operid,:timespanid,:preregtime,:bookingtel,:ischarge,:timebegin,:timeend,:departmentid,:pattypeid)";


             var childParas1 = new List<OracleParameter>();
             childParas1.Add(new OracleParameter(":regtime", info["begin"]));
             childParas1.Add(new OracleParameter(":cardno", pat["cardno"]));
             childParas1.Add(new OracleParameter(":patid", pat["id"]));
             childParas1.Add(new OracleParameter(":patname",pat["name"]));
             childParas1.Add(new OracleParameter(":regtypeid", info["regtypeid"]));
             childParas1.Add(new OracleParameter(":regfee",info["diagnosismoney"]));
             childParas1.Add(new OracleParameter(":doctorid", info["doctorid"]));
             childParas1.Add(new OracleParameter(":isprereg",1));
             childParas1.Add(new OracleParameter(":opertime",System.DateTime.Now));
             childParas1.Add(new OracleParameter(":operid",9));
             childParas1.Add(new OracleParameter(":timespanid",info["timespanid"]));
             childParas1.Add(new OracleParameter(":preregtime",info["preregtime"]));
             childParas1.Add(new OracleParameter(":bookingtel", pat["mobile"] ?? ""));
             OracleParameter param1 = new OracleParameter(":ischarge", OracleType.Number);
             param1.Value = 0;
             childParas1.Add(param1);
             //childParas1.Add(new OracleParameter(":ischarge", 0));
             childParas1.Add(new OracleParameter(":timebegin",info["timebegin"]));
             childParas1.Add(new OracleParameter(":timeend",info["timeend"]));
             childParas1.Add(new OracleParameter(":departmentid", info["departmentid"]));
             childParas1.Add(new OracleParameter(":pattypeid",116));


             var sql2 = "insert into out_trade_no values (Ouhosinfo_Id_Seq.currval,:trade_no,:cash)";
             var childParas2 = new List<OracleParameter>();
             childParas2.Add(new OracleParameter(":trade_no",info["trade_no"]));
             childParas2.Add(new OracleParameter(":cash", info["diagnosismoney"]));

             sqls.Add(sql1);
             sqls.Add(sql2);
             parsList.Add(childParas1);
             parsList.Add(childParas2);
             return opt.ChangeDataWithTransaction(sqls, parsList);
         }

         public Dictionary<string, object> QueryPatById(Int64 out_trade_no)
         {
             string sql = @"select patid,regtime,doctorid from OuHosinfo where id=:trade_no";
             OracleParameter parameters =
              new OracleParameter(":trade_no", OracleType.Number);
             parameters.Value = out_trade_no;
             return opt.GetRow(sql, parameters);
         }

         public Dictionary<string, object> Queryout_trade_no(string out_trade_no)
         {
             string sql = @"select id,cash from out_trade_no  where trade_no=:trade_no";
             OracleParameter parameters =
              new OracleParameter(":trade_no", OracleType.VarChar);
             parameters.Value = out_trade_no;
             return opt.GetRow(sql, parameters);
         }

       
         public bool SaveOuInvoice(Dictionary<string, object> info)
         {
             var sqls = new List<string>();
             var parsList = new List<List<OracleParameter>>();
             var sql2 = @"insert into OuInvoice (id,invono,mzregid,patid,invotime,amountpay,opertime,operid,mreatment) 
                          values (OuInvoice_Id_Seq.nextval,:invono,:mzregid,:patid,:invotime,:amountpay,:opertime,:operid,:mreatment) ";

             var sql3 = @"insert into OuInvoiceDtl (id,invoid,itemid,totality,unitid,price,amount,execlocid,doctorid,invitemid ,feeid,lsperform ) 
                  values (OuInvoiceDtl_ID_SEQ.nextval,OuInvoice_Id_Seq.currval,:itemid,:totality,:unitid,:price,:amount,:execlocid,:doctorid,:invitemid,:feeid,:lsperform)";
             sqls.Add(sql2);
             sqls.Add(sql3);

             var childParas2 = new List<OracleParameter>();
             childParas2.Add(new OracleParameter("invono",GetNO("1")));
             childParas2.Add(new OracleParameter("mzregid",info["id"]));
             childParas2.Add(new OracleParameter("patid", info["patid"]));
             childParas2.Add(new OracleParameter("invotime", info["regtime"]));//发票时间
             childParas2.Add(new OracleParameter("amountpay", info["price"]));
             childParas2.Add(new OracleParameter("opertime",DateTime.Now));
             childParas2.Add(new OracleParameter("operid",9));
             childParas2.Add(new OracleParameter("mreatment", info["price"]));//诊金
             parsList.Add(childParas2);

             var childParas3 = new List<OracleParameter>();
             childParas3.Add(new OracleParameter("itemid",1));
             childParas3.Add(new OracleParameter("totality",1));
             childParas3.Add(new OracleParameter("unitid",1));
             childParas3.Add(new OracleParameter("price", info["price"]));
             childParas3.Add(new OracleParameter("amount", info["price"]));
             childParas3.Add(new OracleParameter("execlocid",5037));
             childParas3.Add(new OracleParameter("doctorid", info["doctorid"]));
             childParas3.Add(new OracleParameter("invitemid", 221));
             childParas3.Add(new OracleParameter("feeid",5500));
             childParas3.Add(new OracleParameter("lsperform",1));
             parsList.Add(childParas3);

             return opt.ChangeDataWithTransaction(sqls, parsList);
         }


         public DataTable QueryRegister(string openid)
         {
             string sql = @"select c.cardno,a.name,a.idcardno,a.mobile
    from BSPATIENT a,wechatpatient b,vipcard c
    where a.id=b.patientid and a.id=c.patid
    and b.openid=:openid  and a.isactive=1";
             OracleParameter[] parameters = { new OracleParameter(":openid", OracleType.VarChar)    
                                         };
             parameters[0].Value = openid;
             return opt.GetTable(sql, parameters);
         }

    }
}