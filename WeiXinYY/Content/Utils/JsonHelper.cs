﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;

namespace WeiXinYY.Content.Utils
{
    public class JsonHelper
    {
        /// <summary>
        /// 根据json串的节点返回节点值; joinstr json串;parentstr 一级节点名称
        /// </summary>
        /// <param name="joinstr">json串</param>
        /// <param name="parentstr">节点名称</param>
        /// <returns></returns>
        public static string GetVluesParentstr(string joinstr, string parentstr)
        {
            JObject jo = (JObject)JsonConvert.DeserializeObject(joinstr);

            return jo[parentstr].ToString();
        }
        /// <summary>
        /// 根据json串的节点返回子节点值;joinstr json串;parentstr 一级节点名称;childstr 子节点名称
        /// </summary>
        /// <param name="joinstr">json串</param>
        /// <param name="parentstr">一级节点名称</param>
        /// <param name="childstr">子节点名称</param>
        /// <returns></returns>
        public static string GetVluesChildstr(string joinstr, string parentstr, string childstr)
        {


            JObject jo = (JObject)JsonConvert.DeserializeObject(joinstr);
            return jo[parentstr][childstr].ToString();
        }

         public static string getJsonByObject(Object obj)
          {
              //实例化DataContractJsonSerializer对象，需要待序列化的对象类型
              DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
              //实例化一个内存流，用于存放序列化后的数据
              MemoryStream stream = new MemoryStream();
              //使用WriteObject序列化对象
              serializer.WriteObject(stream, obj);
              //写入内存流中
             byte[] dataBytes = new byte[stream.Length];
             stream.Position = 0;
            stream.Read(dataBytes, 0, (int)stream.Length);
             //通过UTF8格式转换为字符串
             return Encoding.UTF8.GetString(dataBytes);
         }
 
         public static Object getObjectByJson(string jsonString, Object obj)
         {
           //实例化DataContractJsonSerializer对象，需要待序列化的对象类型
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
             //把Json传入内存流中保存
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            // 使用ReadObject方法反序列化成对象
            return serializer.ReadObject(stream);
         }
    }
}