using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Web;

namespace WeiXinYY.Models
{
    public class DBOpt
    {
        public static string ConnString
        {

            get
            {

                return ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            }
        }

        public DataTable Getbillno(string name)
        {
            using (var con = new OracleConnection(ConnString))
            {
                try
                {
                    var cmd = new OracleCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "getbillsequence";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new OracleParameter("V_SEQUENCENAME",name));
                    cmd.Parameters.Add(new OracleParameter("V_DYNAMICCONTENT", ""));

                    OracleParameter p1 = new OracleParameter("cv_1", OracleType.Cursor);
                    p1.Direction = System.Data.ParameterDirection.Output;
                    cmd.Parameters.Add(p1);
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    var dt = new DataTable();
                    dt.Load(dr);
                    dr.Close();
                    return dt;
                }
                catch (Exception exc)
                {
                    throw new Exception(exc.Message);
                }
                finally
                {
                    con.Close();
                }

            }
       
            //OracleDataAdapter da = new OracleDataAdapter(cmd);

            //DataSet ds = new DataSet();

            //da.Fill(ds);
        }

        public static string[] SelAccessToken(string token, DateTime newdate, string refreshtoken,string openid)
        {
            using (var con = new OracleConnection(ConnString))
            {
                 try
                {
                string[] param = new string[2];
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "GetAccessToken";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("accessToken", token));
                cmd.Parameters.Add(new OracleParameter("newDate", newdate));
                cmd.Parameters.Add(new OracleParameter("refreshtoken", refreshtoken));
                cmd.Parameters.Add(new OracleParameter("openNew", openid));
                var outPar = new OracleParameter();
                outPar.ParameterName = "serialNumber";
                outPar.OracleType = OracleType.VarChar;
                outPar.Size = 600;
                outPar.Direction = ParameterDirection.Output;

                var outPar1 = new OracleParameter();
                outPar1.ParameterName = "serialrefre";
                outPar1.OracleType = OracleType.VarChar;
                outPar1.Size = 600;
                outPar1.Direction = ParameterDirection.Output;
                //cmd.Parameters.Add(":serialrefre", OracleType.VarChar).Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outPar);
                cmd.Parameters.Add(outPar1);
                con.Open();
                cmd.ExecuteNonQuery();
                param[0]=cmd.Parameters["serialNumber"].Value.ToString();
                param[1]=cmd.Parameters["serialrefre"].Value.ToString();
                return param;
                }
                 catch (Exception exc)
                 {
                     throw new Exception(exc.Message);
                 }
                 finally
                 {
                     con.Close();
                 }

            }
        }
        /// <summary>
        /// 获取一个表
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>  
        public DataTable GetTable(string sql, params OracleParameter[] pars)
        {

            using (var con = new OracleConnection(ConnString))
            {
                try
                {
                    var cmd = new OracleCommand();

                    cmd.Connection = con;
                    cmd.CommandText = sql;
                    cmd.Parameters.AddRange(pars);
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    var dt = new DataTable();
                    dt.Load(dr);
                    dr.Close();
                    return dt;
                }
                catch (Exception exc)
                {
                    throw new Exception(exc.Message);
                }
                finally
                {
                    con.Close();
                }

            }
        }

        public DataSet GetSet(string sql, params OracleParameter[] pars)
        {

            using (var con = new OracleConnection(ConnString))
            {
                try
                {
                    var cmd = new OracleCommand();

                    cmd.Connection = con;

                    cmd.CommandText = sql;
                    cmd.Parameters.AddRange(pars);
                    con.Open();
                    OracleDataAdapter oda = new OracleDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    oda.Fill(ds);
                    return ds;//dgvShowMsg.DataSource = ds.Tables[0];
                }
                catch (Exception exc)
                {
                    throw new Exception(exc.Message);
                }
                finally
                {
                    con.Close();
                }

            }
        }

        /// <summary>
        /// 执行增，删，改
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public bool SavaChange(string sql, params OracleParameter[] pars)
        {

            using (var con = new OracleConnection(ConnString))
            {
                try
                {
                    var cmd = new OracleCommand();
                    cmd.Connection = con;
                    cmd.CommandText = sql;
                    cmd.Parameters.AddRange(pars);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception exc)
                {
                    throw new Exception(exc.Message);
                }
                finally
                {
                    con.Close();
                }

            }
        }

        /// <summary>
        /// 获取单值结果
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public object GetValue(string sql, params OracleParameter[] pars)
        {

            using (var con = new OracleConnection(ConnString))
            {
                try
                {
                    var cmd = new OracleCommand();
                    cmd.Connection = con;
                    cmd.CommandText = sql;
                    cmd.Parameters.AddRange(pars);
                    con.Open();
                    var result = cmd.ExecuteScalar();
                    return result;
                }
                catch (Exception exc)
                {
                    throw new Exception(exc.Message);
                }
                finally
                {
                    con.Close();
                }

            }
        }


        /// <summary>
        /// 获取sql执行后的第一行，转成字典保存
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetRow(string sql, params OracleParameter[] pars)
        {
            var table = GetTable(sql, pars);
            Dictionary<string, object> dic = new Dictionary<string, object>();

            if (table.Rows.Count > 0)
            {
                var row = table.Rows[0];
                foreach (DataColumn col in table.Columns)
                {
                    dic.Add(col.ColumnName.ToLower(), row[col.ColumnName]);
                }
            }
            else
            {
                throw new Exception(string.Format("SQL查询返回{0}条记录", table.Rows.Count));
            }
            return dic;
        }

        /// <summary>
        /// 执行事务的SQL语句
        /// </summary>
        /// <param name="childSqls">SQL语句集合</param>
        /// <param name="childParametersList">参数列表集合</param>
        /// <returns></returns>
        public bool ChangeDataWithTransaction(List<string> sqls, List<List<OracleParameter>> parametersList)
        {

            using (var con = new OracleConnection(ConnString))
            {
                con.Open();
                var tran = con.BeginTransaction();
                try
                {
                    var cmd = new OracleCommand();
                    cmd.Connection = con;
                    cmd.Transaction = tran;

                    for (int i = 0; i < sqls.Count; i++)
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandText = sqls[i];
                        cmd.Parameters.AddRange(parametersList[i].ToArray());
                        cmd.ExecuteNonQuery();
                    }
                    tran.Commit();
                    return true;
                }
                catch (Exception exc)
                {
                    tran.Rollback();
                    throw new Exception(exc.Message);
                }
                finally
                {
                    con.Close();
                }

            }
        }


    }
}