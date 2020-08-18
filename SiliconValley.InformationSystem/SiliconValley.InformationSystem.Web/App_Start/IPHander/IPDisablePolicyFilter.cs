using Newtonsoft.Json;
using SiliconValley.InformationSystem.Business.Base_SysManage;
using SiliconValley.InformationSystem.Entity.Entity;
using SiliconValley.InformationSystem.Web.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
namespace SiliconValley.InformationSystem.Web.App_Start.IPHander
{
    public class IPDisablePolicyFilter: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            string isEnabelIPintercep = ConfigurationManager.AppSettings["Enable"].ToString();

            if (isEnabelIPintercep == "false")
            {
                return;
            }

            //获取到IP地址
            var IPAddress = GetIPAddress();

            //对IP进行记录  ip地址 时间 

            //获取当前时间
           
            string NowDateTimeStr = $"{DateTime.Now.Year}/{DateTime.Now.Month}/{DateTime.Now.Day} {DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}";


            var BrowseVersion = this.GetBrowseVersion();

            var OSVersion = this.GetOSVersion();
            ReqClient client = new ReqClient();
            client.GuidKey = Guid.NewGuid().ToString();
            client.BrowseVersion = BrowseVersion;
            client.IPAddress = IPAddress;
            client.OSVersion = OSVersion;
            client.RequestTime =DateTime.Parse(NowDateTimeStr);
            //存入文件 
            SaveToJsonFile(client);

            //存入数据库
            SaveToDB(client);

            //是否需要拦截
            var isNeedIntercept = IsNeedInterceptIP(IPAddress);

            if (isNeedIntercept)
            {
                // 对这次请求进行拦截

                filterContext.Result = new HttpNotFoundResult("拦截");

                return;
            }
            //进行IP拦截策略
            IPInterceptStrategy(client);


        }

        /// <summary>
        /// 获取IP地址
        /// </summary>
        /// <returns></returns>
        public string GetIPAddress()
        {

            var result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (!string.IsNullOrEmpty(result))
            {
                //可能有代理    
                if (result.IndexOf(".") == -1)        //没有“.”肯定是非IPv4格式    
                    result = null;
                else
                {
                    if (result.IndexOf(",") != -1)
                    {
                        //有“,”，估计多个代理。取第一个不是内网的IP。    
                        result = result.Replace("  ", "").Replace("'", "");
                        string[] temparyip = result.Split(",;".ToCharArray());
                        for (int i = 0; i < temparyip.Length; i++)
                        {
                            if (IsIPAddress(temparyip[i])
                                    && temparyip[i].Substring(0, 3) != "10."
                                    && temparyip[i].Substring(0, 7) != "192.168"
                                    && temparyip[i].Substring(0, 7) != "172.16.")
                            {
                                return temparyip[i];
                            }
                        }
                    }
                    else if (IsIPAddress(result))  //代理即是IP格式    
                        return result;
                    else
                        result = null;        //代理中的内容  非IP，取IP    
                }

            }

            string IpAddress = (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null && HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != String.Empty) ? HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] : HttpContext.Current.Request.ServerVariables["HTTP_X_REAL_IP"];

            if (string.IsNullOrEmpty(result))
                result = HttpContext.Current.Request.ServerVariables["HTTP_X_REAL_IP"];

            if (string.IsNullOrEmpty(result))
                result = HttpContext.Current.Request.UserHostAddress;

            return result;

        }

        public bool IsIPAddress(string str1)
        {
            if (string.IsNullOrEmpty(str1) || str1.Length < 7 || str1.Length > 15) return false;

            const string regFormat = @"^d{1,3}[.]d{1,3}[.]d{1,3}[.]d{1,3}$";

            var regex = new Regex(regFormat, RegexOptions.IgnoreCase);
            return regex.IsMatch(str1);
        }

        /// <summary> 
        /// 获取浏览器版本号
        /// </summary>
        /// <returns></returns>
        public string GetBrowseVersion()
        {
            HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;
            return bc.Browser + bc.Version;
        }

        /// <summary>
        /// 获取操作系统
        /// </summary>
        /// 
        /// <returns></returns>
        public string GetOSVersion()
        {
            //UserAgent   
            var userAgent = HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"];

            var osVersion = "未知";

            if (userAgent.Contains("NT 6.1"))
            {
                osVersion = "Windows 7";
            }
            else if (userAgent.Contains("NT 10.0"))
            {
                osVersion = "Windows 10";
            }
            else if (userAgent.Contains("NT 6.0"))
            {
                osVersion = "Windows Vista/Server 2008";
            }
            else if (userAgent.Contains("NT 5.2"))
            {
                osVersion = "Windows Server 2003";
            }
            else if (userAgent.Contains("NT 5.1"))
            {
                osVersion = "Windows XP";
            }
            else if (userAgent.Contains("NT 5"))
            {
                osVersion = "Windows 2000";
            }
            else if (userAgent.Contains("NT 4"))
            {
                osVersion = "Windows NT4";
            }
            else if (userAgent.Contains("Me"))
            {
                osVersion = "Windows Me";
            }
            else if (userAgent.Contains("98"))
            {
                osVersion = "Windows 98";
            }
            else if (userAgent.Contains("95"))
            {
                osVersion = "Windows 95";
            }
            else if (userAgent.Contains("Mac"))
            {
                osVersion = "Mac";
            }
            else if (userAgent.Contains("Unix"))
            {
                osVersion = "UNIX";
            }
            else if (userAgent.Contains("Linux"))
            {
                osVersion = "Linux";
            }
            else if (userAgent.Contains("SunOS"))
            {
                osVersion = "SunOS";
            }
            return osVersion;
        }

        /// <summary>
        /// 将请求记录写入文件
        /// </summary>
        /// <param name="client"></param>
        public void SaveToJsonFile(ReqClient client)
        {
            string fp = "/ClientRequestInfo.txt";

            if (!File.Exists(HttpContext.Current.Server.MapPath(fp)))  // 判断是否已有相同文件 
            {
                FileStream fs1 = new FileStream(HttpContext.Current.Server.MapPath(fp), FileMode.Create, FileAccess.ReadWrite);
                fs1.Close();
                fs1.Dispose();
            }
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(fp), FileMode.Append, FileAccess.Write);
            StreamWriter ws = new StreamWriter(fs);
            ws.WriteLine(JsonConvert.SerializeObject(client));
            ws.Flush();
            fs.Flush();
            ws.Close();
            ws.Dispose();
            fs.Close();
            fs.Dispose();
        }

        /// <summary>
        /// 将请求记录存入数据库
        /// </summary>
        /// <param name="client"></param>
        public void SaveToDB(ReqClient client)
        {

            ReqClientBusiness db_reqClient = new ReqClientBusiness();
            db_reqClient.Insert(client);

        }

        /// <summary>
        ///是否需要拦截
        /// </summary>
        /// <param name="IpAddress">IPAddress</param>
        /// <returns>bool</returns>
        public bool IsNeedInterceptIP(string IpAddress)
        {
            string path = HttpContext.Current.Server.MapPath("/IPIntercept.txt");
            if (!File.Exists(path))
            {
                //创建文件
                FileStream fs1 = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
                fs1.Close();
                fs1.Dispose();
            }

            //读取内容

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);

            StreamReader sr = new StreamReader(fs);
            string str = sr.ReadToEnd();

            sr.Close();
            sr.Dispose();
            fs.Close();
            fs.Dispose();

            return str.Contains(IpAddress);

        }

        /// <summary>
        /// IP拦截策略
        /// </summary>
        public void IPInterceptStrategy(ReqClient client)
        {
            int second = int.Parse( ConfigurationManager.AppSettings["RequestTimeSpan"].ToString());

            int MaxreqCount = int.Parse(ConfigurationManager.AppSettings["MaxRequestCount"].ToString());

            var begInTime = client.RequestTime.AddSeconds(-second);

            string sql = $"select * from Client where RequestTime BETWEEN '{begInTime}' AND '{client.RequestTime}'";

            ReqClientBusiness db_reqClient = new ReqClientBusiness();

            var list = db_reqClient.GetListBySql<ReqClient>(sql);

            int count = list.Count;

            if (count > MaxreqCount)
            {

                //写入黑名单
                List<string> iplist = new List<string>();
                iplist.Add(client.IPAddress);

                AddIpInterceptItem(iplist);

            }

        }

        /// <summary>
        /// 将ip地址列入黑名单
        /// </summary>
        /// <param name="iplist"></param>
        public void AddIpInterceptItem(List<string> iplist)
        {
            string path = HttpContext.Current.Server.MapPath("/IPIntercept.txt");
            if (!File.Exists(path))
            {
                //创建文件
                FileStream fs1 = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
                fs1.Close();
                fs1.Dispose();
            }

            //读取内容

            FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);

            StreamWriter sw = new StreamWriter(fs);

            if (iplist == null)
            {
                return;
            }

            iplist.ForEach(ip =>
            {
                sw.Write(ip + ",");
            });

            sw.Close();
            sw.Dispose();
            fs.Close();
            fs.Dispose();
        }
    }
}