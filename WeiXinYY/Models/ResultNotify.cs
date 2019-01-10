﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using WeiXinYY.Content.Bean;

namespace WeiXinYY.Models
{
    /// <summary>
    /// 支付结果通知回调处理类
    /// 负责接收微信支付后台发送的支付结果并对订单有效性进行验证，将验证结果反馈给微信支付后台
    /// </summary>
    public class ResultNotify : Notify
    {
        //public ResultNotify(Page page)
        //    : base(page)
        //{
        //}

        public override string ProcessNotify(WxPayData notifyData)
        {
            string xml = "";
            //检查支付结果中transaction_id是否存在
            if (!notifyData.IsSet("transaction_id"))
            {
                //若transaction_id不存在，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", "支付结果中微信订单号不存在");
                //Log.Error(this.GetType().ToString(), "The Pay result is error : " + res.ToXml());
                xml=res.ToXml();
                return xml;
            }

            string transaction_id = notifyData.GetValue("transaction_id").ToString();
            LogHelp.WriteTextLog("【transaction_id】" + "\r\n" + transaction_id, "", "", "" + "\r\n" + "", System.DateTime.Now);
            //查询订单，判断订单真实性
            if (!QueryOrder(transaction_id))
            {
                //若订单查询失败，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", "订单查询失败");
                //Log.Error(this.GetType().ToString(), "Order query failure : " + res.ToXml());
                xml = res.ToXml();
                return xml;
            }
            //查询订单成功
            else
            {
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "SUCCESS");
                res.SetValue("return_msg", "OK");
                //Log.Info(this.GetType().ToString(), "order query success : " + res.ToXml());
                xml = res.ToXml();
                return xml;
            }
        }

        //查询订单
        private bool QueryOrder(string transaction_id)
        {
            WxPayData req = new WxPayData();
            req.SetValue("transaction_id", transaction_id);
            WxPayData res = WxPayApi.OrderQuery(req);
            if (res.GetValue("return_code").ToString() == "SUCCESS" &&
                res.GetValue("result_code").ToString() == "SUCCESS")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}