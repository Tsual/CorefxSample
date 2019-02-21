using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using System;
using System.Runtime.Serialization.Json;
using System.Diagnostics;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading;
using System.Text;

namespace NetcoreAppA
{

    class mongoTest
    {
        static readonly Random random = new Random();
        static string conn_url = "mongodb://localhost:27017/", db_name = "localdb", collection_name = "col_test_6";
        static string CharSet = "0123456789QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm";

        static string RandomString(int length)
        {
            StringBuilder stringBuilder = new StringBuilder();
            while (length-- > 0)
                stringBuilder.Append(CharSet[random.Next(CharSet.Length)]);
            return stringBuilder.ToString();
        }

        static BsonDocument CreateRandomBson()
        {
            var doc = new BsonDocument();
            doc.Add(new BsonElement("Name", "User" + random.Next(100)));

            doc.Add(new BsonElement("IntA", Math.Abs(random.Next()) % 10));
            doc.Add(new BsonElement("IntB", Math.Abs(random.Next()) % 100));
            doc.Add(new BsonElement("IntC", Math.Abs(random.Next()) % 1000));
            doc.Add(new BsonElement("IntD", Math.Abs(random.Next())));

            doc.Add(new BsonElement("CreateDate", DateTime.Now));

            doc.Add(new BsonElement("ProtectKey", RandomString(50)));

            byte[] byte_array = new byte[256];
            random.NextBytes(byte_array);
            doc.Add(new BsonElement("ProtectArray", byte_array));
            return doc;
        }

        public static void Create()
        {
            var client = new MongoClient(new MongoUrl(conn_url));
            var localdb = client.GetDatabase(db_name);
            IMongoCollection<BsonDocument> collection;

            using (var session = client.StartSession())
            {
                session.StartTransaction();
                if (!localdb.ListCollectionNames().ToList().Contains(collection_name))
                {
                    localdb.CreateCollection(collection_name);
                };
                collection = localdb.GetCollection<BsonDocument>(collection_name);
                for (int i = 0; i < 100; i++)
                {
                    List<BsonDocument> list = new List<BsonDocument>();
                    for (int ix = 0; ix < 1000; ix++)
                    {
                        list.Add(CreateRandomBson());
                    }
                    collection.InsertMany(list);
                    Console.WriteLine(i);
                }
                session.CommitTransaction();
            }
        }

        public static void Read()
        {

        }

        public static void Modify()
        {

        }

        public static void Delete()
        {

        }
    }

    class Program
    {
        static readonly Random random = new Random();

        static void Main(string[] args)
        {
            mongoTest.Create();
        }

        static BsonDocument CreateRandomBson()
        {
            var doc = new BsonDocument();
            byte[] byte_array = new byte[256];
            random.NextBytes(byte_array);
            doc.Add(new BsonElement("bta", byte_array));

            doc.Add(new BsonElement("a", Math.Abs(random.Next()) % 10));
            doc.Add(new BsonElement("b", Math.Abs(random.Next()) % 100));
            doc.Add(new BsonElement("c", Math.Abs(random.Next()) % 1000));
            doc.Add(new BsonElement("d", Math.Abs(random.Next())));
            return doc;
        }
        static void Test_5(string conn_url = "mongodb://localhost:27017/",
            string db_name = "localdb", string collection_name = "col_test_5")
        {
            var client = new MongoClient(new MongoUrl(conn_url));
            IMongoCollection<BsonDocument> collection;
            var localdb = client.GetDatabase(db_name);

            using (var session = client.StartSession())
            {
                session.StartTransaction();
                collection = localdb.GetCollection<BsonDocument>(collection_name);

                var list = collection.Find(BsonDocument.Create(new Dictionary<string, int>() { { "d", 444709743 } })).ToList();

                var fliter = Builders<BsonDocument>.Filter.Eq("d", 444709743);
                var update = Builders<BsonDocument>.Update.Set("a", -1);
                var result = collection.UpdateOne(fliter, update);

                session.CommitTransaction();
            }

            //NumberInt(444709743)
        }

        static void Test_4(string conn_url = "mongodb://localhost:27017/",
            string db_name = "localdb", string collection_name = "col_test_5")
        {
            var client = new MongoClient(new MongoUrl(conn_url));
            var localdb = client.GetDatabase(db_name);
            IMongoCollection<BsonDocument> collection;

            using (var session = client.StartSession())
            {
                session.StartTransaction();
                if (!localdb.ListCollectionNames().ToList().Contains(collection_name))
                {
                    localdb.CreateCollection(collection_name);
                };
                collection = localdb.GetCollection<BsonDocument>(collection_name);

                for (int ix = 0; ix < 1000; ix++)
                {
                    List<BsonDocument> list = new List<BsonDocument>();
                    for (int i = 0; i < 1000; i++)
                    {
                        list.Add(CreateRandomBson());
                    }
                    collection.InsertMany(list);
                    Console.WriteLine(ix);
                }
                session.CommitTransaction();
            }
        }

        static void Test_3()
        {
            var client = new MongoClient(new MongoUrl("mongodb://localhost:27017/"));
            var localdb = client.GetDatabase("localdb");
            IMongoCollection<BsonDocument> collection;

            using (var session = client.StartSession())
            {
                session.StartTransaction();
                if (!localdb.ListCollectionNames().ToList().Contains("col_test_3"))
                {
                    localdb.CreateCollection("col_test_3");
                };
                collection = localdb.GetCollection<BsonDocument>("col_test_3");
                for (int i = 0; i < 999999; i++)
                {
                    collection.InsertOne(CreateRandomBson());
                    Console.WriteLine(i);
                }
                session.CommitTransaction();
            }
        }

        static void Test_1()
        {
            var client = new MongoClient(new MongoUrl("mongodb://localhost:27017/"));
            var dbs = client.ListDatabases().ToList();
            var localdb = client.GetDatabase("localdb");
            var cols = localdb.GetCollection<BsonDocument>("col_test_1");


            var queryCondition = new BsonDocument();
            queryCondition.Add(new BsonElement("a", "1"));
            var findResult_1 = cols.Find(queryCondition).ToList()[0].ToDictionary();


            var findResult = cols.Find((doc) => true).ToList();

            string str = "";
            foreach (var bson in findResult)
            {
                foreach (var bse in bson)
                {
                    str += "Name:" + bse.Name + "|Value:" + bse.Value + '\n';
                }
            }


        }



        static void Test_2()
        {
            var client = new MongoClient(new MongoUrl("mongodb://localhost:27017/"));
            var cols = client.GetDatabase("localdb").GetCollection<BsonDocument>("col_test_1");
            var linq_result = (from t in cols.AsQueryable()
                               where t.GetElement("a").Value.AsString == "1"
                               select t).ToList();

        }

        public static bool CheckJson(string json_str)
        {
            var json = JsonConvert.DeserializeObject(json_str);
            return true;
        }
    }
}
