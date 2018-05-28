using NTTools.DB;
using RefreshWeChatCountService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefreshWeChatCountService.Contexts
{
    public class MongoDBContext
    {
        private MongoDBModel<MerchantModel> merchantModelContext;
        public MongoDBModel<MerchantModel> MerchantModelContext
        {
            get
            {
                if (merchantModelContext == null) merchantModelContext = new MongoDBModel<MerchantModel>(); return merchantModelContext;
            }
        }


        public MongoDBModel<MerchantAppModel> merchantAppModelContext;
        public MongoDBModel<MerchantAppModel> MerchantAppModelContext
        {
            get
            {
                if (merchantAppModelContext == null) merchantAppModelContext = new MongoDBModel<MerchantAppModel>(); return merchantAppModelContext;
            }
        }


        public MongoDBModel<WeChatCountModel> weChatCountModelContext;
        public MongoDBModel<WeChatCountModel> WeChatCountModelContext
        {
            get
            {
                if (weChatCountModelContext == null) weChatCountModelContext = new MongoDBModel<WeChatCountModel>(); return weChatCountModelContext;
            }
        }

        public MongoDBModel<WeChatCountAppModel> weChatCountAppModelContext;
        public MongoDBModel<WeChatCountAppModel> WeChatCountAppModelContext
        {
            get
            {
                if (weChatCountAppModelContext == null) weChatCountAppModelContext = new MongoDBModel<WeChatCountAppModel>(); return weChatCountAppModelContext;
            }
        }
        

    }
}
