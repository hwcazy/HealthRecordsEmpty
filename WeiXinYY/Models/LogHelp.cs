using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace WeiXinYY.Models
{
    public class LogHelp
    {
        /// <summary>  
        /// 写入日志到文本文件  ; action   动作,ruMessage 日志内容入参,chuMessage 日志内容出参,time 时间
        /// </summary>  
        /// <param name="action">动作</param>  
        /// <param name="ruMessage">日志内容</param>  
        /// <param name="time">时间</param>  
        public static void WriteTextLog(string action, string ruMessage, string jm, string chuMessage, DateTime time)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"Log\";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string fileFullPath = path + time.ToString("yyyy-MM-dd") + ".System.txt";
            StringBuilder str = new StringBuilder();
            str.Append("Time:    " + time.ToString() + "\r\n");
            str.Append("Action:  " + action + "\r\n");
            str.Append("Message: \r\n");
            str.Append("入参: " + ruMessage + "\r\n\r\n");
            str.Append("加密入参: " + jm + "\r\n\r\n");
            str.Append("出参: " + chuMessage + "\r\n");
            str.Append("-----------------------------------------------------------\r\n\r\n");
            //--------------------//
            StreamWriter sw;
            FileStream file;
            if (!File.Exists(fileFullPath))
            {
                file = new FileStream(fileFullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            }
            else
            {
                file = new FileStream(fileFullPath, FileMode.Append, FileAccess.Write);
            }


            using (sw = new StreamWriter(file))
            {
                sw.WriteLine(str.ToString());
                sw.Flush();
                sw.Close();
                file.Dispose();
                file.Close();
                //----------------------//


            }
        }
    }
}