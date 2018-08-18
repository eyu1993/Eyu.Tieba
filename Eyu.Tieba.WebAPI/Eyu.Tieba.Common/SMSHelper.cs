using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
using Microsoft.Win32;

namespace Eyu.Tieba.Common
{
    public class SMSHelper
    {
        public static SMSResult Send(string phone, string code)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE").OpenSubKey("TIEBA").OpenSubKey("SMS", false);
            string accessKeyId = key.GetValue("accessKeyId").ToString();
            string accessKeySecret = key.GetValue("accessKeySecret").ToString();
            string SignName = key.GetValue("SignName").ToString();
            string TemplateCode = key.GetValue("TemplateCode").ToString();
            key.Close();

            string product = "Dysmsapi";
            string domain = "dysmsapi.aliyuncs.com";

            IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", accessKeyId, accessKeySecret);
            DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", product, domain);
            IAcsClient acsClient = new DefaultAcsClient(profile);
            SendSmsRequest request = new SendSmsRequest();
            try
            {
                request.PhoneNumbers = phone;
                request.SignName = SignName;
                request.TemplateCode = TemplateCode;
                request.TemplateParam = "{\"code\":\"" + code + "\"}";
                SendSmsResponse sendSmsResponse = acsClient.GetAcsResponse(request);
                switch (sendSmsResponse.Code)
                {
                    case "OK":
                        return SMSResult.OK;
                    case "isv.BUSINESS_LIMIT_CONTROL":
                        return SMSResult.Frequently;
                    default:
                        return SMSResult.Failed;
                }
            }
            catch (ServerException ex)
            {
                LogHelper.Error(string.Format("短信发送至：{0}失败\r\n", phone) + ex.ToString());
                return SMSResult.Exception;
            }
            catch (ClientException ex)
            {
                LogHelper.Error(string.Format("短信发送至：{0}失败\r\n", phone) + ex.ToString());
                return SMSResult.Exception;
            }
        }
    }

    public enum SMSResult
    {
        OK,
        Failed,
        Frequently,
        Exception
    }
}
