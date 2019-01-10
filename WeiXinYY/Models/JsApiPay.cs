using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WeiXinYY.Content.Bean;

namespace WeiXinYY.Models
{
    public class JsApiPay
    {

        /// <summary>
        /// 统一下单接口返回结果
        /// </summary>
        public WxPayData unifiedOrderResult { get; set; } 

        /**
       * 调用统一下单，获得下单结果
       * @return 统一下单结果
       * @失败时抛异常WxPayException
       */
        public WxPayData GetUnifiedOrderResult(string openid,int total_fee)
        {
            //统一下单
            WxPayData data = new WxPayData();
            data.SetValue("body", "test");
            data.SetValue("attach", "test");
            data.SetValue("out_trade_no", GenerateOutTradeNo());
            LogHelp.WriteTextLog("【out_trade_no】" + "\r\n" + GenerateOutTradeNo(),"", "", "" + "\r\n" + "", System.DateTime.Now);
            data.SetValue("total_fee", total_fee);
            data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));
            data.SetValue("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));
            data.SetValue("goods_tag", "test");
            data.SetValue("trade_type", "JSAPI");
            data.SetValue("openid", openid);

            WxPayData result = UnifiedOrder(data);
            if (!result.IsSet("appId") || !result.IsSet("package"))
            {
               // Log.Error(this.GetType().ToString(), "UnifiedOrder response error!");
                throw new WxPayException("UnifiedOrder response error!");
            }

            unifiedOrderResult = result;
            return result;
        }

        /**
       * 根据当前系统时间加随机序列来生成订单号
        * @return 订单号
       */
        public static string GenerateOutTradeNo()
        {
            var ran = new Random();
            return string.Format("{0}{1}{2}", WxPayConfig.GetConfig().GetMchID(), DateTime.Now.ToString("yyyyMMddHHmmss"), ran.Next(999));
        }

        /**
       * 
       * 统一下单
       * @param WxPaydata inputObj 提交给统一下单API的参数
       * @param int timeOut 超时时间
       * @throws WxPayException
       * @return 成功时返回，其他抛异常
       */
        public static WxPayData UnifiedOrder(WxPayData inputObj, int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/pay/unifiedorder";
            //检测必填参数
            if (!inputObj.IsSet("out_trade_no"))
            {
                throw new WxPayException("缺少统一支付接口必填参数out_trade_no！");
            }
            else if (!inputObj.IsSet("body"))
            {
                throw new WxPayException("缺少统一支付接口必填参数body！");
            }
            else if (!inputObj.IsSet("total_fee"))
            {
                throw new WxPayException("缺少统一支付接口必填参数total_fee！");
            }
            else if (!inputObj.IsSet("trade_type"))
            {
                throw new WxPayException("缺少统一支付接口必填参数trade_type！");
            }

            //关联参数
            if (inputObj.GetValue("trade_type").ToString() == "JSAPI" && !inputObj.IsSet("openid"))
            {
                throw new WxPayException("统一支付接口中，缺少必填参数openid！trade_type为JSAPI时，openid为必填参数！");
            }
            if (inputObj.GetValue("trade_type").ToString() == "NATIVE" && !inputObj.IsSet("product_id"))
            {
                throw new WxPayException("统一支付接口中，缺少必填参数product_id！trade_type为JSAPI时，product_id为必填参数！");
            }

            //异步通知url未设置，则使用配置文件中的url
            if (!inputObj.IsSet("notify_url"))
            {
                inputObj.SetValue("notify_url", WxPayConfig.GetConfig().GetNotifyUrl());//异步通知url
            }

            inputObj.SetValue("appid", WxPayConfig.GetConfig().GetAppID());//公众账号ID
            inputObj.SetValue("mch_id", WxPayConfig.GetConfig().GetMchID());//商户号
            inputObj.SetValue("spbill_create_ip", WxPayConfig.GetConfig().GetIp());//终端ip	  	    
            inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串
            inputObj.SetValue("sign_type", WxPayData.SIGN_TYPE_MD5);//签名类型SIGN_TYPE_HMAC_SHA256

            //签名
            inputObj.SetValue("sign", inputObj.MakeSign(WxPayData.SIGN_TYPE_MD5));

            WxPayData result1 = new WxPayData();
            result1.SetValue("trade_no", inputObj.GetValue("out_trade_no"));
            string xml = inputObj.ToXml();

            var start = DateTime.Now;

            //Log.Debug("WxPayApi", "UnfiedOrder request : " + xml);
            string response = HttpService.Post(xml, url, false, timeOut);
            //Log.Debug("WxPayApi", "UnfiedOrder response : " + response);

            var end = DateTime.Now;
            int timeCost = (int)((end - start).TotalMilliseconds);

            WxPayData result = new WxPayData();
            result.FromXml(response);

           

            result1.SetValue("appId", result.GetValue("appid"));
            result1.SetValue("nonceStr", GenerateNonceStr());//result.GetValue("nonce_str")
            result1.SetValue("package", "prepay_id=" + result.GetValue("prepay_id"));
            result1.SetValue("timeStamp", GetTimeStamp());
            result1.SetValue("signType", WxPayData.SIGN_TYPE_MD5);
            result1.SetValue("paySign", result1.MakeSign(WxPayData.SIGN_TYPE_MD5));
           
            ReportCostTime(url, timeCost, result);//测速上报

            return result1;
        }


        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        } 

        /**
      * 生成随机串，随机串包含字母或数字
      * @return 随机串
      */
        public static string GenerateNonceStr()
        {
            RandomGenerator randomGenerator = new RandomGenerator();
            return randomGenerator.GetRandomUInt().ToString();
        }

        /**
          * 
          * 测速上报
          * @param string interface_url 接口URL
          * @param int timeCost 接口耗时
          * @param WxPayData inputObj参数数组
          */
        public static void ReportCostTime(string interface_url, int timeCost, WxPayData inputObj)
        {
            //如果不需要进行上报
            if (WxPayConfig.GetConfig().GetReportLevel() == 0)
            {
                return;
            }

            //如果仅失败上报
            if (WxPayConfig.GetConfig().GetReportLevel() == 1 && inputObj.IsSet("return_code") && inputObj.GetValue("return_code").ToString() == "SUCCESS" &&
             inputObj.IsSet("result_code") && inputObj.GetValue("result_code").ToString() == "SUCCESS")
            {
                return;
            }

            //上报逻辑
            WxPayData data = new WxPayData();
            data.SetValue("interface_url", interface_url);
            data.SetValue("execute_time_", timeCost);
            //返回状态码
            if (inputObj.IsSet("return_code"))
            {
                data.SetValue("return_code", inputObj.GetValue("return_code"));
            }
            //返回信息
            if (inputObj.IsSet("return_msg"))
            {
                data.SetValue("return_msg", inputObj.GetValue("return_msg"));
            }
            //业务结果
            if (inputObj.IsSet("result_code"))
            {
                data.SetValue("result_code", inputObj.GetValue("result_code"));
            }
            //错误代码
            if (inputObj.IsSet("err_code"))
            {
                data.SetValue("err_code", inputObj.GetValue("err_code"));
            }
            //错误代码描述
            if (inputObj.IsSet("err_code_des"))
            {
                data.SetValue("err_code_des", inputObj.GetValue("err_code_des"));
            }
            //商户订单号
            if (inputObj.IsSet("out_trade_no"))
            {
                data.SetValue("out_trade_no", inputObj.GetValue("out_trade_no"));
            }
            //设备号
            if (inputObj.IsSet("device_info"))
            {
                data.SetValue("device_info", inputObj.GetValue("device_info"));
            }

            try
            {
                Report(data);
            }
            catch (WxPayException ex)
            {
                //不做任何处理
            }
        }

        /**
       * 
       * 测速上报接口实现
       * @param WxPayData inputObj 提交给测速上报接口的参数
       * @param int timeOut 测速上报接口超时时间
       * @throws WxPayException
       * @return 成功时返回测速上报接口返回的结果，其他抛异常
       */
        public static WxPayData Report(WxPayData inputObj, int timeOut = 1)
        {
            string url = "https://api.mch.weixin.qq.com/payitil/report";
            //检测必填参数
            if (!inputObj.IsSet("interface_url"))
            {
                throw new WxPayException("接口URL，缺少必填参数interface_url！");
            }
            if (!inputObj.IsSet("return_code"))
            {
                throw new WxPayException("返回状态码，缺少必填参数return_code！");
            }
            if (!inputObj.IsSet("result_code"))
            {
                throw new WxPayException("业务结果，缺少必填参数result_code！");
            }
            if (!inputObj.IsSet("user_ip"))
            {
                throw new WxPayException("访问接口IP，缺少必填参数user_ip！");
            }
            if (!inputObj.IsSet("execute_time_"))
            {
                throw new WxPayException("接口耗时，缺少必填参数execute_time_！");
            }

            inputObj.SetValue("appid", WxPayConfig.GetConfig().GetAppID());//公众账号ID
            inputObj.SetValue("mch_id", WxPayConfig.GetConfig().GetMchID());//商户号
            inputObj.SetValue("user_ip", WxPayConfig.GetConfig().GetIp());//终端ip
            inputObj.SetValue("time", DateTime.Now.ToString("yyyyMMddHHmmss"));//商户上报时间	 
            inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串
            inputObj.SetValue("sign_type", WxPayData.SIGN_TYPE_HMAC_SHA256);//签名类型
            inputObj.SetValue("sign", inputObj.MakeSign());//签名
            string xml = inputObj.ToXml();

            //Log.Info("WxPayApi", "Report request : " + xml);

            string response = HttpService.Post(xml, url, false, timeOut);

            //Log.Info("WxPayApi", "Report response : " + response);

            WxPayData result = new WxPayData();
            result.FromXml(response);
            return result;
        }


    }
}