using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using NTTools.Jsons;
using NTTools.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefreshWeChatCountService.Models
{
    public class TimerConfigModel : SurperModel
    {
        [JsonConverter(typeof(DateConverterEndMinute))]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateTime { get; set; } = DateTime.Now;
        [JsonConverter(typeof(DateConverterEndMinute))]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastExecuteTime { get; set; } = DateTime.Now;
        public int RefreshMerchantStartHour { get; set; } = 3;
        public int RefreshMerchantStartMinutes { get; set; } = 3;

        public int RefreshWeChatCountStartHour { get; set; } = 8;
        public int RefreshWeChatCountStartMinutes { get; set; } = 13;
        
        public RefreshDate MerchantRefreshDate { get; set; }
        public RefreshDate MiniAppRefreshDate { get; set; }
        public RefreshDate WeChatCountRefreshDate { get; set; }


    }

    public class RefreshDate
    {
        [JsonConverter(typeof(DateConverterEndMinute))]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastRefreshEndTime { get; set; }
        [JsonConverter(typeof(DateConverterEndMinute))]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastRefreshStartTime { get; set; }
        public long RefreshUseSeconds { get; set; }
    }
}
