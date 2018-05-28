using MongoDB.Driver;
using NTConfigData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTTools.DB
{
    public class MongoDBTool
    {

        /// <summary>
        /// 数据库连接
        /// </summary>
        private const string localConn = MainConfig.MongoDBLocalConn;
        private const string lineConn = MainConfig.MongoDBLineConn;


        //private const string debugConn = "mongodb://192.168.2.85:27027";
        /// <summary>
        /// 指定的数据库
        /// </summary>
        private const string dbName = MainConfig.MongoDBName;

        private static IMongoDatabase mongoDatabase;
        /// <summary>
        /// 获取数据库
        /// </summary>
        /// <returns>当前数据库</returns>
        public IMongoDatabase GetMongoDatabase()
        {
            if (mongoDatabase == null)
            {
                var connectionString = localConn;
#if DEBUG
                connectionString = lineConn;
#endif
                MongoClient mongoClient = new MongoClient(connectionString);
                mongoDatabase = mongoClient.GetDatabase(dbName);
            }
            return mongoDatabase;
        }
        /// <summary>
        /// 获取集合
        /// </summary>
        /// <typeparam name="T">集合类型</typeparam>
        /// <returns>该类型集合</returns>
        public IMongoCollection<T> GetMongoCollection<T>()
        {

            string packageName = typeof(T).ToString();
            string collectionName = packageName.Substring(packageName.LastIndexOf(".") + 1);
            return GetMongoDatabase().GetCollection<T>(collectionName);
        }

        /// <summary>
        /// 获取集合
        /// </summary>
        /// <typeparam name="T">集合类型</typeparam>
        /// <param name="collectionName">集合名称</param>
        /// <returns>该类型集合</returns>
        public IMongoCollection<T> GetMongoCollection<T>(string collectionName)
        {
            return GetMongoDatabase().GetCollection<T>(collectionName);
        }


    }
}
