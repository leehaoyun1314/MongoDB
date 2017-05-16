using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.IO;
using MongoDB.Driver.Linq;
using MongoDB.Bson;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace MongoDemo
{
    public class Program
    {
        private static IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        private static IConfigurationRoot configuration = builder.Build();
        private static IConfigurationSection section = configuration.GetSection("mongodb");
        private static string hostname = section["hostName"];
        private static int port = int.Parse(section["port"]);
        private static string dbName = section["dbName"];
        protected static IMongoClient client = new MongoClient(new MongoClientSettings() { Server = new MongoServerAddress(hostname, port) });

        public static void Main(string[] args)
        {
            var list = Get<sys_tables>();
            sys_tables table = list.Last();
            table.TableName = "测试表2";
            ReplaceOneResult result = Update(table.TableID, table);

            Console.ReadKey();
        }
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="document"></param>
        public static void Add<T>(T document)
        {
            IMongoDatabase db = client.GetDatabase(dbName);
            db.GetCollection<T>(typeof(T).Name).InsertOne(document);
        }
        /// <summary>
        /// 更新文档
        /// </summary>
        /// <param name="tableId"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public static ReplaceOneResult Update(int tableId, sys_tables document)
        {
            IMongoDatabase db = client.GetDatabase(dbName);
            IMongoCollection<sys_tables> collection = db.GetCollection<sys_tables>("sys_tables");
            FilterDefinition<sys_tables> filter = Builders<sys_tables>.Filter.Eq(x => x.TableID, tableId);

            //设置单个属性的新值
            // UpdateDefinition<Person> up = Builders<Person>.Update.Set(x => x.Sex, p.Sex);
            // 更新文档的单个属性
            // collection.UpdateOne(filter, up);

            //更新整个文档
            return collection.ReplaceOne(filter, document);
        }
        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> Get<T>()
        {
            IMongoDatabase db = client.GetDatabase(dbName);
            IMongoQueryable<T> tableCollection = db.GetCollection<T>(typeof(T).Name).AsQueryable();
            return tableCollection;
        }
    }

    public class sys_tables
    {
        public ObjectId _id { get; set; }
        public int TableID { get; set; }
        public string TableName { get; set; }
        public string TableDescription { get; set; }
        public bool IsDeleted { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Modifier { get; set; }
        public DateTime ModifyTime { get; set; }
    }
}
