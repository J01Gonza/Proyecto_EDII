﻿using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WEB.Conexiones
{
    public class mongoConnection
    {
        public MongoClient Client;
        public IMongoDatabase DB;
        public mongoConnection()
        {
            Client = new MongoClient("mongodb://127.0.0.1:27017");
            DB = Client.GetDatabase("TeyvatExpress");
        }
    }
}