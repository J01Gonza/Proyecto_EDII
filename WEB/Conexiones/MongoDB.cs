using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace API.Conexiones
{
    public class MongoDB
    {

        public MongoClient client;

        public IMongoDatabase DB;
        public MongoDB()
        {
            client = new MongoClient("mongodb://localhost:27017");
            DB = client.GetDatabase("TeyvatExpress");
        }
    }
}
