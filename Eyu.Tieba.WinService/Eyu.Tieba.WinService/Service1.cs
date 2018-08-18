using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Eyu.Tieba.WinService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();

            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(TimedEvent);
            timer.Interval = 12 * 60 * 60 * 1000;//每12h执行一次
            timer.Enabled = true;
            timer.AutoReset = true;
        }

        //定时执行事件
        private void TimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            LogHelper.Info("Event start");

            try
            {
                //1.获取所有BDUSS
                Tieba db = new Tieba();
                var users = db.User.Where(u => u.IsActive == true);
                var accounts = (from b in db.Baidu
                                from u in users
                                where b.UserId == u.UserId
                                select b).ToList();
                LogHelper.Info("BDUSS count is:" + accounts.Count);
                foreach (Baidu item in accounts)
                {
                    try
                    {
                        string BDUSS = item.BDUSS;
                        CookieContainer container = new CookieContainer();
                        container.Add(new Cookie("BDUSS", BDUSS, "/", "baidu.com"));
                        HttpClientHandler handler = new HttpClientHandler()
                        {
                            UseCookies = true,
                            CookieContainer = container
                        };
                        HttpClient client = new HttpClient(handler);

                        //2.获取tbs
                        string get_tbs_url = "http://tieba.baidu.com/dc/common/tbs";
                        string tbsHtml = GetHtml(client, get_tbs_url);
                        Regex regTbs = new Regex("\"tbs\":\"([0-9a-zA-Z]+)\"");
                        string tbs = regTbs.Match(tbsHtml).Groups[1].Value;


                        //3.获取所有喜欢的贴吧（最多获取200个）
                        string get_alllike_url = "http://tieba.baidu.com/mo/q---B8D06B9EB00241F919F47789D4FD3103%3AFG%3D1--1-1-0--2--wapp_1385540291997_626/m?tn=bdFBW&tab=favorite";
                        string allHtml = GetHtml(client, get_alllike_url);
                        Regex reg1 = new Regex("\"m\\?kw=([\\%\\w]+)\"");
                        MatchCollection matchs = reg1.Matches(allHtml);

                        string sign_url = "http://c.tieba.baidu.com/c/c/forum/sign";
                        for (int i = 0; i < matchs.Count; i++)
                        {
                            //4.获取每个吧的fid，
                            string kw = HttpUtility.UrlDecode(matchs[i].Groups[1].Value);
                            string url = "http://tieba.baidu.com/f/commit/share/fnameShareApi?ie=utf-8&fname=" + kw;
                            client.GetAsync(url).ContinueWith(res =>
                            {
                                Regex regFid = new Regex("\"fid\":([0-9]+)");
                                string fid = regFid.Match(res.Result.Content.ReadAsStringAsync().Result).Groups[1].Value;

                                //5.签到
                                Dictionary<string, string> postData = new Dictionary<string, string>();
                                postData.Add("BDUSS", BDUSS);
                                postData.Add("_client_id", "03-00-DA-59-05-00-72-96-06-00-01-00-04-00-4C-43-01-00-34-F4-02-00-BC-25-09-00-4E-36");
                                postData.Add("_client_type", "4");
                                postData.Add("_client_version", "1.2.1.17");
                                postData.Add("_phone_imei", "540b43b59d21b7a4824e1fd31b08e9a6");
                                postData.Add("fid", fid);
                                postData.Add("kw", kw);
                                postData.Add("net_type", "3");
                                postData.Add("tbs", tbs);
                                string str = "";
                                foreach (KeyValuePair<string, string> kv in postData)
                                {
                                    str += kv.Key + "=" + kv.Value;
                                }
                                string Sign = MD5(str + "tiebaclient!!!").ToUpper();
                                postData.Add("sign", Sign);
                                client.PostAsync(sign_url, new FormUrlEncodedContent(postData));
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
            }
        }

        protected override void OnStart(string[] args)
        {
            LogHelper.Info("Eyu.Tieba.WinService:[Service start]");
        }

        protected override void OnStop()
        {
            LogHelper.Info("Eyu.Tieba.WinService:[Service is stopped]");
        }
        protected override void OnShutdown()
        {
            LogHelper.Info("Eyu.Tieba.WinService[Computer shuts down]");
        }

        //获取时间戳
        private string GetTimestamp()
        {
            return (DateTime.UtcNow - DateTime.Parse("1970-01-01 0:0:0")).TotalMilliseconds.ToString("0");
        }

        //Get 请求
        private string GetHtml(HttpClient client, string url)
        {
            HttpResponseMessage res = client.GetAsync(url).Result;
            return res.Content.ReadAsStringAsync().Result;
        }

        //Post 请求
        private string DoPost(HttpClient client, string url, Dictionary<string, string> postData)
        {
            HttpResponseMessage res = client.PostAsync(url, new FormUrlEncodedContent(postData)).Result;
            return res.Content.ReadAsStringAsync().Result;
        }

        //计算MD5
        public string MD5(string input)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = Encoding.UTF8.GetBytes(input);
            byte[] md5data = md5.ComputeHash(data);
            md5.Clear();
            string str = "";
            for (int i = 0; i < md5data.Length; i++)
                str += md5data[i].ToString("x2").PadLeft(2, '0');
            return str;
        }
    }
}
