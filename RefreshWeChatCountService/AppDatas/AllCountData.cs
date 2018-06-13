using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NTTools.Models;
using RefreshWeChatCountService.Contexts;
using RefreshWeChatCountService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RefreshWeChatCountService.AppDatas
{
    public class AllCountData : IDisposable
    {
        const string merchantsUrl = "http://we7api.360yingketong.com/index/index/users?key=dlkZ%24%23H1Ky%5E%252n%5E%5Dz%25%2Cv0VlC%40ktvb~B%40&p=",
          appsUrl = "http://we7api.360yingketong.com/index/index/wxapplist?key=dlkZ%24%23H1Ky%5E%252n%5E%5Dz%25%2Cv0VlC%40ktvb~B%40&p=";
        private MongoDBContext mongoDBContext;

        public MongoDBContext MongoDBContext
        {
            get
            {
                if (mongoDBContext == null)
                    mongoDBContext = new MongoDBContext();
                return mongoDBContext;
            }
        }

        internal void RefreshMerchants()
        {
            ServicePointManager.DefaultConnectionLimit =100;
            CreateRefreshMerchantsTasks();
        }

        private void CreateRefreshMerchantsTasks()
        {
            var cfg = TimerConfigModelContext.GetConfig();
            if (cfg.MerchantRefreshDate == null)
                cfg.MerchantRefreshDate = new RefreshDate();
            cfg.MerchantRefreshDate.LastRefreshStartTime = DateTime.Now;
            TimerConfigModelContext.SaveConfigLog(cfg);
            string json = null;
            AllCountRequestJsonModel<MerchantModel> list = null;
            try
            {
                MongoDBContext.MerchantModelContext.GetCollection().DeleteMany(MongoDBContext.MerchantModelContext.Filter.Empty);
                json = WRGetJson(merchantsUrl + 1);
                list = JsonConvert.DeserializeObject<AllCountRequestJsonModel<MerchantModel>>(json);
                SaveMerchantData(list);
                var pageSum = list.data.maxpage;

                Parallel.For(2, pageSum + 1, pageIndex =>
                        GetMerchantsTask(pageIndex)
                );
                cfg.MerchantRefreshDate.LastRefreshEndTime = DateTime.Now;
                cfg.MerchantRefreshDate.RefreshUseSeconds = (long)(cfg.MerchantRefreshDate.LastRefreshEndTime - cfg.MerchantRefreshDate.LastRefreshStartTime).TotalSeconds;
                TimerConfigModelContext.SaveConfigLog(cfg);
                RefreshMerchantsApps();

            }
            catch (Exception e) { e.Save(); }
        }



        private void GetMerchantsTask(int index)
        {
            string json = null;
            try
            {
                json = WRGetJson(merchantsUrl + index);
                AllCountRequestJsonModel<MerchantModel> list = null;
                list = JsonConvert.DeserializeObject<AllCountRequestJsonModel<MerchantModel>>(json);
                SaveMerchantData(list);
            }
            catch (Exception e) { e.Save(); }

        }

        private void SaveMerchantData(AllCountRequestJsonModel<MerchantModel> list)
        {
            if (list == null || list.code != 0)
            {
                return;
            }
            MongoDBContext.MerchantModelContext.GetCollection().InsertMany(list.data.list);
        }
        private string WRGetJson(string url)
        {
            try
            {
                GC.Collect();
                string json = null;
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
                webRequest.Method = "get";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                using (HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse())
                {
                    json = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                }
                return json;
            }
            catch (Exception e)
            {
                e.Save();
                return null;
            }
        }
        private void RefreshMerchantsApps()
        {
            var cfg = TimerConfigModelContext.GetConfig();
            if (cfg.MiniAppRefreshDate == null)
                cfg.MiniAppRefreshDate = new RefreshDate();
            cfg.MiniAppRefreshDate.LastRefreshStartTime = DateTime.Now;
            TimerConfigModelContext.SaveConfigLog(cfg);
            string json = null;
            AllCountRequestJsonModel<MerchantAppModel> list = null;
            try
            {
                MongoDBContext.MerchantAppModelContext.GetCollection().DeleteMany(MongoDBContext.MerchantAppModelContext.Filter.Empty);
                json = WRGetJson(appsUrl + 1);
                list = JsonConvert.DeserializeObject<AllCountRequestJsonModel<MerchantAppModel>>(json);
                SaveMiniAppsData(list);
                var pageSum = list.data.maxpage;
                json = null;
                list = null;
                Parallel.For(2, pageSum + 1, pageIndex => GetMiniAppsTask(pageIndex));
                cfg.MiniAppRefreshDate.LastRefreshEndTime = DateTime.Now;
                cfg.MiniAppRefreshDate.RefreshUseSeconds = (long)(cfg.MiniAppRefreshDate.LastRefreshEndTime - cfg.MiniAppRefreshDate.LastRefreshStartTime).TotalSeconds;
                TimerConfigModelContext.SaveConfigLog(cfg);
            }
            catch (Exception e) { e.Save(); }


        }
        private void GetMiniAppsTask(int index)
        {
            string json = null;
            AllCountRequestJsonModel<MerchantAppModel> list = null;
            try
            {
                json = WRGetJson(appsUrl + index);
                list = JsonConvert.DeserializeObject<AllCountRequestJsonModel<MerchantAppModel>>(json);
                SaveMiniAppsData(list);
                json = null;
                list = null;
            }
            catch (Exception e)
            {
                e.Save();
                json = null;
                list = null;
            }

        }

        private void SaveMiniAppsData(AllCountRequestJsonModel<MerchantAppModel> list)
        {
            if (list == null || list.code != 0)
            {
                return;
            }
            var dataList =
            list.data.list.FindAll(x=>x.AppID.Length==18&&x.AppSecret.Length==32);
            MongoDBContext.MerchantAppModelContext.GetCollection().InsertMany(dataList);
        }


        /// <summary>
        /// 获取accessToken 异步方法
        /// </summary>
        /// <param name="wc"></param>
        /// <param name="appID"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public string GetWeChatAccessToken(WebClient wc, string appID, string appSecret)
        {
            JObject jObj = null;
            string access_token = null, response = null;
            JToken at = null, ei = null;
            try
            {

                var timeOut = 0;
                response = wc.DownloadString($"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={appID}&secret={appSecret}");

                jObj = JsonConvert.DeserializeObject<JObject>(response);


                if (jObj.TryGetValue("access_token", out at))
                {
                    access_token = at.ToString();
                }
                else
                {
                    access_token = null;
                }
                if (jObj.TryGetValue("expires_in", out ei))
                {
                    timeOut = Convert.ToInt32(ei.ToString());
                }
                else
                {
                    timeOut = 0;
                }
                return access_token;
            }
            catch (Exception e) { e.Save(); return null; }


        }

        internal string GetToken(string appID, string appSecret)
        {
            string url = $"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={appID}&secret={appSecret}";
            JObject jObj = null;
            string access_token = null, res = null;
            JToken at = null, ei = null;
            res = WRGetJson(url);
            jObj = JsonConvert.DeserializeObject<JObject>(res);
            if (jObj.TryGetValue("access_token", out at))
            {
                access_token = at.ToString();
            }
            else
            {
                access_token = null;
            }
            var timeOut = 0;

            if (jObj.TryGetValue("expires_in", out ei))
            {
                timeOut = Convert.ToInt32(ei.ToString());
            }
            else
            {
                timeOut = 0;
            }
            return access_token;
        }

        public void Dispose()
        {
            mongoDBContext = null;
            GC.SuppressFinalize(this);
        }
    }

    public class PageContent<T>
    {
        public long Sum { get; set; }
        public long PageIndex { get; set; }
        public long PageSum { get; set; }
        public T PageData { get; set; }
    }

    #region 刷新商户及商户的小程序所需的模型
    public class AllCountRequestJsonModel<T>
    {
        public int code { get; set; }
        public string msg { get; set; }
        public AllCountRequestPage<T> data { get; set; }
    }

    public class AllCountRequestPage<T>
    {
        public int count { get; set; }
        public int maxpage { get; set; }
        public List<T> list { get; set; }
    }

    #endregion
}