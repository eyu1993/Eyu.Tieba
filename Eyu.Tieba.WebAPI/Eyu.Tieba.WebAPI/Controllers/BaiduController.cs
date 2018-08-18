using Eyu.Tieba.Common;
using Eyu.Tieba.Model;
using Eyu.Tieba.UnitOfWork;
using Eyu.Tieba.WebAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;

namespace Eyu.Tieba.WebAPI.Controllers
{
    [Authorize]
    public class BaiduController : ApiController
    {
        string gid = "A5CA91F-D99F-44F3-9415-49A6A3DC5254";
        //gid = this.guideRandom = function()
        //{
        //    return 'xxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(e) {
        //        var t = 16 * Math.random() | 0,
        //        n = 'x' == e ? t : 3 & t | 8;
        //        return n.toString(16)
        //}).toUpperCase()
        //}
        //()
        public IUnitOfWork UOW { get; set; }
        public BaiduController(IUnitOfWork unitOfWork)
        {
            this.UOW = unitOfWork;
        }
        //检查此手机号是否注册了百度
        [HttpGet]
        public IHttpActionResult CheckPhone(string phone)
        {
            PhoneLogin result = new PhoneLogin();

            CookieContainer container = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler()
            {
                UseCookies = true,
                CookieContainer = container
            };
            HttpClient client = new HttpClient(handler);
            string token = GetToken(client);

            string check_phone_url = "https://passport.baidu.com/v2/?getphonestatus&token=" + token + "&tpl=mn&apiver=v3&tt=" + GetTimestamp() + "&gid=" + gid + "&phone=" + phone + "&countrycode=&callback=bd__cbs__hs5d4l";
            string checkHtml = GetHtml(client, check_phone_url);
            Regex regCheck = new Regex("\"no\": \"([0-9]+)\"");
            string err_no = regCheck.Match(checkHtml).Groups[1].Value;
            switch (err_no)
            {
                case "0":
                    result.Error = 0;
                    result.Detail = "手机号可用";
                    result.Token = token;
                    foreach (Cookie item in container.GetCookies(new Uri(check_phone_url)))
                    {
                        if (item.Name == "BAIDUID")
                        {
                            result.BAIDUID = item.Value;
                        }
                    }
                    break;
                case "3":
                    result.Error = 3;
                    result.Detail = "此手机号未注册百度";
                    break;
                default:
                    result.Error = 98;
                    result.Detail = "服务器繁忙，请稍后再试";
                    break;
            }
            return Ok(result);
        }

        //发送短信验证码
        [HttpPost]
        public IHttpActionResult SendCode(PhoneLogin model)
        {
            CookieContainer container = new CookieContainer();
            container.Add(new Cookie("BAIDUID", model.BAIDUID, "/", "baidu.com"));
            HttpClientHandler handler = new HttpClientHandler()
            {
                UseCookies = true,
                CookieContainer = container
            };
            HttpClient client = new HttpClient(handler);
            string send_url = "";
            if (!string.IsNullOrEmpty(model.CodeString) && !string.IsNullOrEmpty(model.VCodeSign) && !string.IsNullOrEmpty(model.VerifyCode))
            {
                //send_url = string.Format("https://passport.baidu.com/v2/api/senddpass?gid={0}&username={1}&countrycode=&bdstoken={2}&tpl=mn&vcodestr={3}&vcodesign={4}&verifycode={5}&flag_code=0&apiver=v3&tt={6}&callback=bd__cbs__d1z1zt", gid, model.Phone, model.Token, model.CodeString, model.VCodeSign, model.VerifyCode, GetTimestamp());
                send_url = string.Format("https://passport.baidu.com/v2/api/senddpass?gid={0}&username={1}&countrycode=&bdstoken={2}&tpl=mn&vcodestr={3}&vcodesign={4}&verifycode={5}&flag_code=0&dv=MDEzAAoAVgANBHQALgAAAFk3AAsCAArErKzvEO_v78N0CwIACsSsrO8Q7-_vxuQLAgAKxKys7xDv7-_KKgcCAATFxcXFCwIACsSsrO8Q7-_v9aILAgAKxKys7xDv7-_4_AsCAArErKzvEO_v7_yPDAIAINzq6urq-pDEhcuM3p_SjdKC0YHe79-A36zBsuKK5YvuBwIABMXFxcUMAgAg3Ovr6-vkgtaX2Z7MjcCfwJDDk8z9zZLNvtOg8Jj3mfwHAgAExcXFxQwCACDc7-_v7-HWgsONypjZlMuUxJfHmKmZxpnqh_SkzKPNqAwCACDc7e3t7eCn87L8u-mo5brltea26djot-ib9oXVvdK82QsCAArErKzvEO_v7-WnBwIABMXFxcUHAgAExcXFxQkCAC7f3Lm59vb29vb1FhcUNDRxcSYmKHw9czRmJ2o1ajppOWZXZzhnFHkKWS5HM1A4DQIABcXFxCwsEwIAGsXT09O7z7vLuIKtgvWC9du52LHVoI7tgu_AFgIAIuSQ-8vl1ufU7N3s1ObU4dfl3enc69rv3u3Y6d_p3ujb49ABAgAGxccmX_wXBQIABMXFxdEVAgAIxcXEpMBf6kAEAgAGx8fFxPHJFwIADsTH1dXcr_SjzZC5kLzdEAIAAcUGAgAoxcXFq6urq6urq64hISEglZWVkDAwMDPz8_P2VlZWVc3NzchnZ2dkwwcCAATFxcXFDQIAHsXFxDghdTR6PW8uYzxjM2Awb15uMW4efwx_CGcVcQ0CAB7FxcH277v6tPOh4K3yrf2u_qGQoP-g0LHCscap278IAgAi3N5KSuzs7OshdTR6PW8uYzxjM2Awb15uMW4dcANTO1Q6XwkCACXc3k1N6Ojo6Ojg0NCExYvMnt-SzZLCkcGer5_An-yB8qLKpcuuDQIAHsXFzaqz56bor_288a7xofKi_cz8o_yP4pHBqcaozQcCAATFxcXFCwIACsSsrO8Q7-_v4MQMAgAg3OTk5OT2cydmKG89fDFuMWEyYj0MPGM8TyJRAWkGaA0JAgAl3N_Y2AEBAQEBHays-Ln3sOKj7rHuvu294tPjvOOQ_Y7as967yQ0CAB7FxdkwKX08cjVnJms0aztoOGdWZjlmFXgLWzNcMlcLAgAKxKys7xDv7-_OtggCAC7g4hIS8_Pz0cqe35HWhMWI14jYi9uEtYXaheaJ54HomvehxLbfucCD7IjtpMmuCQIALuflhYVubm5ubkrp6b38svWn5qv0q_uo-KeWpvmmxarEosu51ILnlfya46DPq84IAgAJxcbPzxoaGjPMCQIAPO3u3t47Ozs7Oxd4eXppaYiIrKy86KnnoPKz_qH-rv2t8sPzrPOQ_5H3nuyB17LAqc-29Zr-m9iw0b_YvQgCACvn5XR0gICAr6_7uvSz4aDtsu297r7h0OC_4IPsguSN_5LEodO63KXmie2ICwIACsSsrO8Q7-_v32gIAgAu4OPi4gsLCznClteZ3ozNgN-A0IPTjL2N0o3uge-J4JL_qcy-17HIi-SA5azBpg&apiver=v3&tt={6}&traceid=&callback=bd__cbs__defme7", gid, model.Phone, model.Token, model.CodeString, model.VCodeSign, model.VerifyCode, GetTimestamp());
            }
            else
            {
                //send_url = string.Format("https://passport.baidu.com/v2/api/senddpass?gid={0}&username={1}&countrycode=&bdstoken={2}&tpl=mn&flag_code=0&apiver=v3&tt={3}&callback=bd__cbs__o94n1p", gid, model.Phone, model.Token, GetTimestamp());
                send_url = string.Format("https://passport.baidu.com/v2/api/senddpass?gid={0}&username={1}&countrycode=&bdstoken={2}&tpl=mn&flag_code=0&dv=MDEzAAoA6AANA-UALAAAAFk3AAcCAATFxcXFCQIAJdzf5-c0NDQ0NEzg4LT1u_yu76L9ovKh8a6fr_Cv3LHClv-S94UMAgAg3Dk5OTkNJXEwfjlrKmc4ZzdkNGtaajVqGXQHVz9QPlsHAgAExcXFxQwCACDcOTk5ORdjN3Y4fy1sIX4hcSJyLRwscyxfMkEReRZ4HQgCACfb2bCxOjo6Ev-r6qTjsfC94r3tvu6xgLDvsMOu3YvunPWT6qnGoscLAgAKxcDAg3yDg4Oh6gsCAArFwMCDfIODg5yVCwIACsXAwIN8g4ODltUIAgAJxcaOjDk5OTGcCwIACsXAwIN8g4ODi6cLAgAKxcDAg3yDg4OHQAgCACPf3IyNi4uLiF8LSgRDEVAdQh1NHk4RIBBPEGMOfS5ZMEQnTw0CAAXFxcYICAcCAATFxcXFBgIAKMXFxaurq6urq6uuDg4ODScnJyKCgoKBQUFBROTk5Od_f3961dXV1nETAgAaxdPT07vPu8u4gq2C9YL127nYsdWgju2C78AQAgABxRYCACHlkfrK5NXs3eTR5N3r0-fe59bm0-LX5tXg0eLR5NDn0OMEAgAGx8fFxPHJBQIABMXFxdEBAgAGxccmX_wXFQIACMXFxKTArauOFwIAE8bCAwMNIWtCbj8WOnlQfDQdMXQHAgAExcXFxQkCAC7f3JOSlZWVlZWWU1JReXmGhtHR34vKhMOR0J3Cnc2ezpGgkM-Q4479rtmwxKfPCAIAHdHSzM7z8_P1FEABTwhaG1YJVgZVBVprWwRbKEU2CwIACsXAwIN8g4ODiOULAgAKxcDAg3yDg4ONMgsCAArFwMCDfIODg5GQCwIACsXAwIN8g4ODmx4LAgAKxcDAg3yDg4OYXwkCACXc3lVUAQEBAQErxcWR0J7Zi8qH2IfXhNSLuorVivmU57ffsN67BwIABMXFxcUNAgAexcXvFg9bGlQTQQBNEk0dTh5BcEAfQDNeLX0VehRxDAIAINw5OTk5FeC09bv8ru-i_aLyofGun6_wr9yxwpL6lfueBwIABMXFxcUHAgAExcXFxQwCACDcOTk5ORa86KnnoPKz_qH-rv2t8sPzrPOA7Z7OpsmnwgwCACDcOTk5OQup_bzyteem67Tru-i459bmueaV-Ivbs9yy1wgCACHd30JDODg4BlMHRghPHVwRThFBEkIdLBxDHG8CcTdYKkcNAgAexcWPoLntrOKl97b7pPur-Kj3xvap9oXom8ujzKLHDQIAHsXFvcjRhcSKzZ_ek8yTw5DAn66ewZ7tgPOjy6TKrw0CAB7Fxb1HXgpLBUIQURxDHEwfTxAhEU4RYg98LEQrRSA&apiver=v3&tt={3}&traceid=&callback=bd__cbs__56c7gq", gid, model.Phone, model.Token, GetTimestamp());
            }
            string sendHtml = GetHtml(client, send_url);
            Regex regSend = new Regex("\"errno\":([0-z]+)");
            string err_no = regSend.Match(sendHtml).Groups[1].Value;
            PhoneLogin sendResult = new PhoneLogin();

            switch (err_no)
            {
                case "0":
                    sendResult.Error = 0;
                    sendResult.Detail = "短信发送成功";
                    break;
                case "16":
                    sendResult.Error = 16;
                    sendResult.Detail = "获取验证码次数过多，请24小时之后再试";
                    break;
                case "18":
                    Regex regex2 = new Regex("\"vcodestr\":\"([A-Za-z0-9]+)\"");
                    string vcodestr = regex2.Match(sendHtml).Groups[1].Value;

                    Regex regex3 = new Regex("\"vcodesign\":\"([A-Za-z0-9]+)\"");
                    string vcodesign = regex3.Match(sendHtml).Groups[1].Value;

                    sendResult.Error = 18;
                    sendResult.Detail = "请先输入图片验证码";
                    sendResult.CodeString = vcodestr;
                    sendResult.VCodeSign = vcodesign;
                    break;
                default:
                    sendResult.Error = 98;
                    sendResult.Detail = "服务器繁忙，请稍后再试";
                    break;
            }
            return Ok(sendResult);
        }

        //获取图片验证码
        [HttpGet]
        public HttpResponseMessage GetImageCode(string codestring, string BAIDUID)
        {
            HttpResponseMessage resp = new HttpResponseMessage();
            CookieContainer container = new CookieContainer();
            container.Add(new Cookie("BAIDUID", BAIDUID, "/", "baidu.com"));
            HttpClientHandler handler = new HttpClientHandler()
            {
                UseCookies = true,
                CookieContainer = container
            };
            HttpClient client = new HttpClient(handler);
            string get_img_url = "https://passport.baidu.com/cgi-bin/genimage?" + codestring;
            Stream stream = GetImage(client, get_img_url);
            Image img = Image.FromStream(stream);
            Bitmap bmp = new Bitmap(img);
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] arr = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(arr, 0, (int)ms.Length);
            ms.Close();
            resp.Content = new StringContent(Convert.ToBase64String(arr));
            return resp;
        }

        //检查图片验证码是否正确
        [HttpGet]
        public IHttpActionResult CheckImageCode(string token, string verifycode, string codestring, string BAIDUID)
        {
            Result result = new Result();
            string url = string.Format("https://passport.baidu.com/v2/?checkvcode&token={0}&tpl=mn&apiver=v3&tt={1}&verifycode={2}&codestring={3}&callback=bd__cbs__fpb7n6", token, GetTimestamp(), verifycode, codestring);
            CookieContainer container = new CookieContainer();
            container.Add(new Cookie("BAIDUID", BAIDUID, "/", "baidu.com"));
            HttpClientHandler handler = new HttpClientHandler()
            {
                UseCookies = true,
                CookieContainer = container
            };
            HttpClient client = new HttpClient(handler);
            var resultHtml = GetHtml(client, url);
            Regex regex = new Regex("\"msg\": \"(.*?)\"");
            string err_no = regex.Match(resultHtml).Groups[1].ToString();
            if (err_no.Length > 0)
            {
                result.Error = 1;
                result.Detail = "图片验证码错误";
            }
            else
            {
                result.Error = 0;
                result.Detail = "图片验证码正确";
            }
            return Ok(result);
        }

        //通过手机验证码登录百度，并绑定
        [HttpPost]
        public async Task<IHttpActionResult> LoginByPhone(PhoneLogin model)
        {
            Result result = new Result();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("staticpage", "http://yun.baidu.com/res/static/thirdparty/pass_v3_jump.html");
            dic.Add("charset", "utf-8");
            dic.Add("token", model.Token);
            dic.Add("tpl", "mn");
            dic.Add("subpro", "mn");
            dic.Add("apiver", "v3");
            dic.Add("tt", GetTimestamp());
            dic.Add("isdpass", "1");
            dic.Add("switchuname", "");
            dic.Add("smscodestring", "");
            dic.Add("smsvcodesign", model.VCodeSign);
            dic.Add("smsvcodestr", model.CodeString);
            dic.Add("is_voice_sms", "0");
            dic.Add("voice_sms_flag", "0");
            dic.Add("fp_uid", "35f4d9ec115e594a820b141f9b21ae05");
            dic.Add("fp_info", "6f8f743e7e4f2faabe097f77279062d2002%7E%7E%7EqIgEDynLoQ4_mqqCvgyov-%7EobSfo1Sxggyov-%7EobSfo1YxAqiScYqiScBqqcVqikuSgI9K9ANDct9nNdNMOn0FU_HmqsLmqcWmqsNmqcJgI9K9ANDct9nNdNMOn0S__Dw0KqssssS8sssscsxcusF1sss2slcssStsYlss4sbssssxs8sssusssSSYYxkDxtR2ts4sCsYsssssu%7EEs-9sSBUcSsSSsfOmqcUmqcwmqcymqcTqq4rgENK2hDDo_EgwoY__qmqciqzzsCB-1sqqsCmqshqzD7G5-uZmqsKmqsGmqsRmqsegA3vtLB14fB1Y2X14QX10eBQ4eBs__");
            dic.Add("u", "http://yun.baidu.com/");
            dic.Add("username", model.Phone);
            dic.Add("password", model.Password);
            dic.Add("gid", gid);
            string url = "https://passport.baidu.com/v2/api/?login ";
            CookieContainer container = new CookieContainer();
            container.Add(new Cookie("BAIDUID", model.BAIDUID, "/", "baidu.com"));
            HttpClientHandler handler = new HttpClientHandler()
            {
                UseCookies = true,
                CookieContainer = container
            };
            HttpClient client = new HttpClient(handler);
            string loginHtml = DoPost(client, url, dic);
            Regex regex = new Regex("err_no=([a-zA-Z0-9]+)&");
            string err_no = regex.Match(loginHtml).Groups[1].ToString();
            switch (err_no)
            {
                case "0":
                    CookieCollection cookies = container.GetCookies(new Uri(url));
                    string str = await BindCookie(cookies);
                    if (str == "0")
                    {
                        result.Error = 0;
                        result.Detail = "绑定成功";
                        break;
                    }
                    result.Error = 98;
                    result.Detail = str;
                    break;
                case "4":
                    result.Error = 4;
                    result.Detail = "短信验证码错误";
                    break;
                default:
                    result.Error = 98;
                    result.Detail = "百度繁忙，请稍后再试。";
                    break;
            }
            return Ok(result);
        }



        //获取BAIDUID,Token,PubKey,Key
        [HttpGet]
        public IHttpActionResult GetParameter()
        {
            AccountLogin par = new AccountLogin();
            CookieContainer container = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler()
            {
                UseCookies = true,
                CookieContainer = container
            };
            HttpClient client = new HttpClient(handler);

            //1.获取BAIDUID和Token
            string token = GetToken(client);
            string url = string.Format("https://passport.baidu.com/v2/getpublickey?token={0}&tpl=mn&apiver=v3&tt={1}&callback=bd__cbs__fwnq4r", token, GetTimestamp());

            string BAIDUID = "";
            foreach (Cookie item in container.GetCookies(new Uri(url)))
            {
                if (item.Name == "BAIDUID")
                {
                    BAIDUID = item.Value;
                }
            }

            //2.获取key
            string keyHtml = GetHtml(client, url);
            Regex regex = new Regex("{.*}");
            var jsonStr = regex.Match(keyHtml).ToString();
            RsaKey RsaKey = JsonConvert.DeserializeObject<RsaKey>(jsonStr);
            if (RsaKey.Pubkey.Length > 0 && RsaKey.Key.Length > 0)
            {
                par.Error = 0;
                par.Detail = "成功获取到Key";
                par.BAIDUID = BAIDUID;
                par.Token = token;
                par.PubKey = RsaKey.Pubkey;
                par.Key = RsaKey.Key;
            }
            else
            {
                par.Error = 98;
                par.Detail = "服务器繁忙，请稍后再试。";
            }
            return Ok(par);
        }

        //通过账号密码登陆，并绑定
        [HttpPost]
        public async Task<IHttpActionResult> LoginByAccount(AccountLogin model)
        {
            string url = "https://passport.baidu.com/v2/api/?login";

            CookieContainer container = new CookieContainer();
            container.Add(new Cookie("BAIDUID", model.BAIDUID, "/", "baidu.com"));
            HttpClientHandler handler = new HttpClientHandler()
            {
                UseCookies = true,
                CookieContainer = container
            };
            HttpClient client = new HttpClient(handler);

            var pemToXml = RSAHelper.PemToXml(model.PubKey);
            string newPassword = RSAHelper.RSAEncrypt(pemToXml, model.Password);

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("staticpage", "http://app.baidu.com/sfile/v3Jump.html");
            dic.Add("charset", "utf-8");
            dic.Add("token", model.Token);
            dic.Add("tpm", "mn");
            dic.Add("subpro", "0");
            dic.Add("apiver", "v3");
            dic.Add("tt", GetTimestamp());
            dic.Add("codestring", model.CodeString);
            dic.Add("safeflg", "0");
            dic.Add("u", "http://app.baidu.com/index?regdev=1");
            dic.Add("isPhone", "false");
            dic.Add("quick_user", "0");
            dic.Add("logintype", "dialogLogin");
            dic.Add("logLoginType", "pc_loginDialog");
            dic.Add("idc", "");
            dic.Add("loginmerge", "true");
            dic.Add("splogin", "newuser");
            dic.Add("username", model.UserName);
            dic.Add("password", newPassword);
            dic.Add("verifycode", model.VerifyCode);
            dic.Add("mem_pass", "on");
            dic.Add("rsakey", model.Key);
            dic.Add("crypttype", "12");
            dic.Add("ppui_logintime", "426406");
            dic.Add("callback", "parent.bd__pcbs__mwrr8d");
            string loginHtml = DoPost(client, url, dic);
            Regex regErr = new Regex("err_no=([a-zA-Z0-9]+)&");
            string err_no = regErr.Match(loginHtml).Groups[1].ToString();
            Regex regCode = new Regex("codeString=([a-zA-Z0-9]+)&");
            AccountLogin loginResult = new AccountLogin();
            switch (err_no)
            {
                case "0":
                    CookieCollection cookies = container.GetCookies(new Uri(url));
                    string str = await BindCookie(cookies);
                    if (str == "0")
                    {
                        loginResult.Error = 0;
                        loginResult.Detail = "绑定成功";
                        break;
                    }
                    loginResult.Error = 98;
                    loginResult.Detail = str;
                    break;
                case "4":
                    loginResult.Error = 4;
                    loginResult.Detail = "账号或密码错误";
                    break;
                case "6":
                    loginResult.Error = 6;
                    loginResult.Detail = "验证码错误";
                    loginResult.CodeString = regCode.Match(loginHtml).Groups[1].Value;
                    break;
                case "7":
                    loginResult.Error = 7;
                    loginResult.Detail = "密码错误";
                    break;
                case "257":
                    loginResult.Error = 257;
                    loginResult.Detail = "需要填写验证码";
                    loginResult.CodeString = regCode.Match(loginHtml).Groups[1].Value;
                    break;
                default:
                    loginResult.Error = Convert.ToInt32(err_no);
                    loginResult.Detail = "未知错误，请联系工作人员";
                    LogHelper.Info(User.Identity.Name + "\r\n" + loginHtml);
                    break;
            }
            return Ok(loginResult);
        }

        //删除绑定的百度账号
        [HttpGet]
        public async Task<IHttpActionResult> CancelBinding(int Id)
        {
            Result result = new Result();
            User user = await UOW.UserRepository.GetSingleAsync(u => u.Name == User.Identity.Name && u.IsActive == true);
            if (user != null)
            {
                Baidu baidu = await UOW.BaiduRepository.GetSingleAsync(b => b.ID == Id && b.UserId == user.UserId);
                if (baidu != null)
                {
                    UOW.BaiduRepository.Delete(baidu);
                    if (await UOW.SaveChangesAsync() > 0)
                    {
                        result.Error = 0;
                        result.Detail = "解绑成功";
                        return Ok(result);
                    }
                }
            }
            result.Error = 1;
            result.Detail = "解绑失败";
            return Ok(result);
        }

        //获取用户绑定的贴吧信息
        [HttpGet]
        public async Task<IHttpActionResult> GetBaiduInfo()
        {
            List<BaiduInfo> list = new List<BaiduInfo>();
            string userName = User.Identity.Name;
            var user = await UOW.UserRepository.GetSingleAsync(u => u.Name == userName);
            if (user != null)
            {
                var accounts = await UOW.BaiduRepository.GetManyAsync(b => b.UserId == user.UserId);
                for (int i = 0; i < accounts.Count(); i++)
                {
                    BaiduInfo info = new BaiduInfo();
                    string name = GetBaiduInfo(accounts[i].BDUSS, accounts[i].STOKEN, accounts[i].PTOKEN);
                    info.Id = accounts[i].ID;
                    info.BDUSS = "*****" + accounts[i].BDUSS.Substring(5, accounts[i].BDUSS.Length - 10) + "*****";
                    info.Name = name.Length > 0 ? name : "百度账号已失效";
                    list.Add(info);
                }
            }
            return Ok(list);
        }



        //手动绑定
        [HttpPost]
        public async Task<IHttpActionResult> ManualBind(BaiduInfo model)
        {
            Result result = new Result();
            string name = GetBaiduInfo(model.BDUSS, model.STOKEN, model.PTOKEN);
            if (!string.IsNullOrEmpty(name))
            {
                string username = User.Identity.Name;
                var user = await UOW.UserRepository.GetSingleAsync(u => u.Name == username);
                if (user != null)
                {
                    var accounts = await UOW.BaiduRepository.GetManyAsync(b => b.UserId == user.UserId);
                    if (accounts.Count > 0)
                    {
                        result.Error = 1;
                        result.Detail = "已经绑定了百度账号";
                        return Ok(result);
                    }
                    Baidu Baidu = new Baidu();
                    Baidu.BDUSS = model.BDUSS;
                    Baidu.PTOKEN = model.PTOKEN;
                    Baidu.STOKEN = model.STOKEN;
                    Baidu.UserId = user.UserId;
                    UOW.BaiduRepository.Add(Baidu);
                    if (await UOW.SaveChangesAsync() > 0)
                    {
                        result.Error = 0;
                        result.Detail = "绑定成功";
                        return Ok(result);
                    }
                }
                result.Error = 1;
                result.Detail = "绑定失败";
            }
            else
            {
                result.Error = 1;
                result.Detail = "BDUSS不可用";
            }
            return Ok(result);
        }


        /// <summary>
        /// 根据BDUSS，STOKEN获取贴吧用户名
        /// </summary>
        /// <param name="BDUSS"></param>
        /// <param name="STOKEN"></param>
        /// <param name="PTOKEN"></param>
        /// <returns></returns>
        private string GetBaiduInfo(string BDUSS, string STOKEN, string PTOKEN)
        {
            string url = "https://m.baidu.com/usrprofile?action=home&model=user&ori=index";
            CookieContainer container = new CookieContainer();
            container.Add(new Cookie("BDUSS", BDUSS, "/", "baidu.com"));
            container.Add(new Cookie("STOKEN", STOKEN, "/", "baidu.com"));
            container.Add(new Cookie("PTOKEN", PTOKEN, "/", "baidu.com"));
            HttpClientHandler handler = new HttpClientHandler()
            {
                CookieContainer = container,
                UseCookies = true
            };
            HttpClient client = new HttpClient(handler);
            GetHtml(client, url);
            string bdHtml = GetHtml(client, url);
            Regex regex = new Regex("<div class=\"user-name\">(.*?)</div>");
            string name = regex.Match(bdHtml).Groups[1].Value;
            return name;
        }

        /// <summary>
        /// 获取Token
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private string GetToken(HttpClient client)
        {
            string url = "https://passport.baidu.com/v2/api/?getapi&tpl=mn&apiver=v3&tt=" + GetTimestamp() + "3&class=login&gid=" + gid + "&logintype=dialogLogin&callback=bd__cbs__1lo14k";
            GetHtml(client, url);
            string tokenHtml = GetHtml(client, url);
            Regex reg = new Regex("\"token\" : \"([0-za-zA-Z]+)\"");
            string token = reg.Match(tokenHtml).Groups[1].Value;
            return token;
        }

        /// <summary>
        /// Get 请求
        /// </summary>
        /// <param name="client"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private string GetHtml(HttpClient client, string url)
        {
            HttpResponseMessage res = client.GetAsync(url).Result;
            return res.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// Post 请求
        /// </summary>
        /// <param name="client"></param>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        private string DoPost(HttpClient client, string url, Dictionary<string, string> postData)
        {
            HttpResponseMessage res = client.PostAsync(url, new FormUrlEncodedContent(postData)).Result;
            return res.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        private string GetTimestamp()
        {
            return (DateTime.UtcNow - DateTime.Parse("1970-01-01 0:0:0")).TotalMilliseconds.ToString("0");
        }

        /// <summary>
        /// 获取图片验证码
        /// </summary>
        /// <param name="client"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private Stream GetImage(HttpClient client, string url)
        {
            HttpResponseMessage res = client.GetAsync(url).Result;
            Stream stream = res.Content.ReadAsStreamAsync().Result;
            return res.Content.ReadAsStreamAsync().Result;
        }

        /// <summary>
        /// 百度账号的cookie绑定至本站
        /// </summary>
        /// <param name="cookies"></param>
        /// <returns></returns>
        private async Task<string> BindCookie(CookieCollection cookies)
        {
            Baidu baidu = new Baidu();
            foreach (Cookie item in cookies)
            {
                if (item.Name == "BDUSS")
                {
                    baidu.BDUSS = item.Value;
                }
                else if (item.Name == "STOKEN")
                {
                    if (item.Value != "deleted")
                    {
                        baidu.STOKEN = item.Value;
                    }
                }
                else if (item.Name == "PTOKEN")
                {
                    if (item.Value != "deleted")
                    {
                        baidu.PTOKEN = item.Value;
                    }
                }
            }
            string username = User.Identity.Name;
            var user = await UOW.UserRepository.GetSingleAsync(u => u.Name == username);
            if (user != null)
            {
                var account = await UOW.BaiduRepository.GetSingleAsync(b => b.UserId == user.UserId);
                //暂时只允许绑定一个百度账号。
                if (account == null)
                {
                    baidu.UserId = user.UserId;
                    if (string.IsNullOrEmpty(baidu.BDUSS))
                    {
                        return "没有获取到BDUSS，请稍后再试。";
                    }
                    UOW.BaiduRepository.Add(baidu);
                    if (await UOW.SaveChangesAsync() > 0)
                    {
                        return "0";
                    }
                }
                else
                {
                    return "已经绑定了百度账号";
                }
            }
            return "绑定失败";
        }
    }
}
