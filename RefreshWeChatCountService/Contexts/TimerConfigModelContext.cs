using MongoDB.Driver;
using NTTools.DB;
using RefreshWeChatCountService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefreshWeChatCountService.Contexts
{
    public class TimerConfigModelContext : MongoDBModel<TimerConfigModel>
    {
        private static TimerConfigModelContext tcmc;
        private TimerConfigModelContext()
        {

        }
        private static TimerConfigModelContext ConfigModel
        {
            get
            {
                if (tcmc == null)
                {
                    tcmc = new TimerConfigModelContext();
                }
                return tcmc;
            }
        }
        public static TimerConfigModel GetConfig()
        {
            var model = ConfigModel.GetCollection().Find(ConfigModel.Filter.Empty).FirstOrDefault();
            if (model == null)
            {
                model = new TimerConfigModel();
                ConfigModel.GetCollection().InsertOne(model);
            }
            return model;
        }

        public static void SaveConfigLog(TimerConfigModel timerConfigModel)
        {
            ConfigModel.GetCollection().UpdateOne(x => x.ID.Equals(timerConfigModel.ID), ConfigModel.Update.Set(x => x.LastExecuteTime, DateTime.Now));
        }
    }
}
