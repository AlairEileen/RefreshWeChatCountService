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
            CreateRefreshMerchantsTasks();
        }

        private void CreateRefreshMerchantsTasks()
        {

            byte[] da = null;
            string json = null;
            AllCountRequestJsonModel<MerchantModel> list = null;
            try
            {
                using (WebClient wcc = new WebClient())
                {
                    wcc.Encoding = Encoding.UTF8;

                    da = wcc.DownloadDataTaskAsync(merchantsUrl + 1).Result;
                    //var json = wcc.DownloadStringTaskAsync(merchantsUrl + 1).Result;
                    json = Encoding.UTF8.GetString(da);
                    list = JsonConvert.DeserializeObject<AllCountRequestJsonModel<MerchantModel>>(json);
                    var pageSum = list.data.maxpage;
                    //TaskFactory taskFactory = new TaskFactory();
                    //var tasks = new Task[pageSum];
                    //var tasks = new List<Task>();
                    //var pageIndexs = new List<int>();

                    MongoDBContext.MerchantModelContext.GetCollection().DeleteMany(MongoDBContext.MerchantModelContext.Filter.Empty);

                    Parallel.For(1, pageSum + 1, pageIndex =>
                            GetMerchantsTask(pageIndex)
                    );
                    //for (int i = 0; i < pageSum; i++)
                    //{
                    //    pageIndexs.Add(i + 1);
                    //    //tasks.Add(taskFactory.StartNew(() => GetMerchantsTask(i+1)));
                    //}
                    //foreach (var item in pageIndexs)
                    //{
                    //    tasks.Add(taskFactory.StartNew(() => GetMerchantsTask(item)));
                    //}
                    //taskFactory.ContinueWhenAll(tasks.ToArray(), t => { RefreshMerchantsApps(); taskFactory = null; tasks = null; pageIndexs = null; });


                }
            }
            catch (Exception e) { e.Save(); }


        }



        private void GetMerchantsTask(int index)
        {
            byte[] da = null;
            string json = null;
            AllCountRequestJsonModel<MerchantModel> list = null;
            try
            {
                using (WebClient wc = new WebClient())
                {

                    da = wc.DownloadData(merchantsUrl + index);
                    json = Encoding.UTF8.GetString(da);
                    list = JsonConvert.DeserializeObject<AllCountRequestJsonModel<MerchantModel>>(json);
                    if (list == null || list.code != 0)
                    {
                        return;
                    }
                    MongoDBContext.MerchantModelContext.GetCollection().InsertMany(list.data.list);
                }

            }
            catch (Exception e) { e.Save(); }

        }
        private void RefreshMerchantsApps()
        {

            byte[] da = null;
            string json = null;
            AllCountRequestJsonModel<MerchantAppModel> list = null;
            try
            {
                using (WebClient wcc = new WebClient())
                {
                    da = wcc.DownloadData(appsUrl + 1);
                    //var json = wcc.DownloadStringTaskAsync(merchantsUrl + 1).Result;
                    json = Encoding.UTF8.GetString(da);
                    //var json = wcc.DownloadStringTaskAsync(appsUrl + 1).Result;
                    list = JsonConvert.DeserializeObject<AllCountRequestJsonModel<MerchantAppModel>>(json);
                    var pageSum = list.data.maxpage;
                    //TaskFactory taskFactory = new TaskFactory();
                    //Task[] tasks = new Task[pageSum];
                    //var tasks = new List<Task>();

                    //var pageIndexs = new List<int>();
                    //for (int i = 0; i < pageSum; i++)
                    //{
                    //    pageIndexs.Add(i + 1);
                    //}
                    //foreach (var item in pageIndexs)
                    //{
                    //    tasks.Add(taskFactory.StartNew(() => GetMiniAppsTask(item)));
                    //}
                    //taskFactory.ContinueWhenAll(tasks.ToArray(), t =>
                    //{
                    //    taskFactory = null; tasks = null; pageIndexs = null;
                    //});
                    MongoDBContext.MerchantAppModelContext.GetCollection().DeleteMany(MongoDBContext.MerchantAppModelContext.Filter.Empty);
                    Parallel.For(1, pageSum + 1, pageIndex => GetMiniAppsTask(pageIndex));
                }
            }
            catch (Exception e) { e.Save(); }


        }
        private void GetMiniAppsTask(int index)
        {
            byte[] da = null;
            string json = null;
            AllCountRequestJsonModel<MerchantAppModel> list = null;
            try
            {
                using (WebClient wc = new WebClient())
                {
                    da = wc.DownloadData(appsUrl + index);
                    json = Encoding.UTF8.GetString(da);
                    list = JsonConvert.DeserializeObject<AllCountRequestJsonModel<MerchantAppModel>>(json);
                    if (list == null || list.code != 0)
                    {
                        return;
                    }
                    MongoDBContext.MerchantAppModelContext.GetCollection().InsertMany(list.data.list);
                    da = null;
                    json = null;
                    list = null;
                }
            }
            catch (Exception e)
            {
                e.Save();
                da = null;
                json = null;
                list = null;
            }

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
            try
            {
                //string postData = "token=" + steptoken + "&id=" + steporderid + "&driverId=" + stepdriverid;
                //byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
                webRequest.Method = "get";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                //webRequest.ContentLength = byteArray.Length;
                //System.IO.Stream newStream = webRequest.GetRequestStream();
                //newStream.Write(byteArray, 0, byteArray.Length);
                //newStream.Close();
                using (HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse())
                {
                    res = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                }
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
            catch (Exception e)
            {
                e.Save();
                return null;
            }
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