using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WeiXinYY.Content.Bean;

namespace WeiXinYY.Models
{
    /**
       * 提交被扫支付API
       * 收银员使用扫码设备读取微信用户刷卡授权码以后，二维码或条码信息传送至商户收银台，
       * 由商户收银台或者商户后台调用该接口发起支付。
       * @param WxPayData inputObj 提交给被扫支付API的参数
       * @param int timeOut 超时时间
       * @throws WxPayException
       * @return 成功时返回调用结果，其他抛异常
       */
    public class WxPayApi
    {

        /**
       *    
       * 查询订单
       * @param WxPayData inputObj 提交给查询订单API的参数
       * @param int timeOut 超时时间
       * @throws WxPayException
       * @return 成功时返回订单查询结果，其他抛异常
       */
        public static WxPayData OrderQuery(WxPayData inputObj, int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/pay/orderquery";
            //检测必填参数
            if (!inputObj.IsSet("out_trade_no") && !inputObj.IsSet("transaction_id"))
            {
                throw new WxPayException("订单查询接口中，out_trade_no、transaction_id至少填一个！");
            }

            inputObj.SetValue("appid", WxPayConfig.GetConfig().GetAppID());//公众账号ID
            inputObj.SetValue("mch_id", WxPayConfig.GetConfig().GetMchID());//商户号
            inputObj.SetValue("nonce_str", JsApiPay.GenerateNonceStr());//随机字符串
            inputObj.SetValue("sign_type", WxPayData.SIGN_TYPE_MD5);//签名类型
            inputObj.SetValue("sign", inputObj.MakeSign(WxPayData.SIGN_TYPE_MD5));//签名


            string xml = inputObj.ToXml();

            var start = DateTime.Now;

            //Log.Debug("WxPayApi", "OrderQuery request : " + xml);
            string response = HttpService.Post(xml, url, false, timeOut);//调用HTTP通信接口提交数据
           // Log.Debug("WxPayApi", "OrderQuery response : " + response);
            LogHelp.WriteTextLog("【查询订单返回】" + "\r\n" + response,"", "", "" + "\r\n" + "", System.DateTime.Now);
            var end = DateTime.Now;
            int timeCost = (int)((end - start).TotalMilliseconds);//获得接口耗时

            //将xml格式的数据转化为对象以返回
            WxPayData result = new WxPayData();
            result.FromXml(response);

            JsApiPay.ReportCostTime(url, timeCost, result);//测速上报

            return result;
        }

        /**
       * 
       * 申请退款
       * @param WxPayData inputObj 提交给申请退款API的参数
       * @param int timeOut 超时时间
       * @throws WxPayException
       * @return 成功时返回接口调用结果，其他抛异常
       */
        public static WxPayData Refund(WxPayData inputObj, int timeOut = 6)
        {
            string url = "https://api.mch.weixin.qq.com/secapi/pay/refund";
            //检测必填参数
            if (!inputObj.IsSet("out_trade_no") && !inputObj.IsSet("transaction_id"))
            {
                throw new WxPayException("退款申请接口中，out_trade_no、transaction_id至少填一个！");
            }
            else if (!inputObj.IsSet("out_refund_no"))
            {
                throw new WxPayException("退款申请接口中，缺少必填参数out_refund_no！");
            }
            else if (!inputObj.IsSet("total_fee"))
            {
                throw new WxPayException("退款申请接口中，缺少必填参数total_fee！");
            }
            else if (!inputObj.IsSet("refund_fee"))
            {
                throw new WxPayException("退款申请接口中，缺少必填参数refund_fee！");
            }
            else if (!inputObj.IsSet("op_user_id"))
            {
                throw new WxPayException("退款申请接口中，缺少必填参数op_user_id！");
            }

            inputObj.SetValue("appid", WxPayConfig.GetConfig().GetAppID());//公众账号ID
            inputObj.SetValue("mch_id", WxPayConfig.GetConfig().GetMchID());//商户号
            inputObj.SetValue("nonce_str", Guid.NewGuid().ToString().Replace("-", ""));//随机字符串
            inputObj.SetValue("sign_type", WxPayData.SIGN_TYPE_HMAC_SHA256);//签名类型
            inputObj.SetValue("sign", inputObj.MakeSign());//签名

            string xml = inputObj.ToXml();
            var start = DateTime.Now;

            //Log.Debug("WxPayApi", "Refund request : " + xml);
            string response = HttpService.Post(xml, url, true, timeOut);//调用HTTP通信接口提交数据到API
           // Log.Debug("WxPayApi", "Refund response : " + response);

            var end = DateTime.Now;
            int timeCost = (int)((end - start).TotalMilliseconds);//获得接口耗时

            //将xml格式的结果转换为对象以返回
            WxPayData result = new WxPayData();
            result.FromXml(response);

            JsApiPay.ReportCostTime(url, timeCost, result);//测速上报

            return result;
        }

    }
}