using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API1.Models;
using MongoDB.Bson;

namespace API1.Conexiones
{
    public class UsersCollection :IUsersCollection
    {
        internal mongoConnection _connection = new mongoConnection();
        private IMongoCollection<User> Collection;

        public UsersCollection()
        {
            Collection = _connection.DB.GetCollection<User>("Usuarios");
        }

        public async Task<List<User>> AllUsers()
        {
            var query = Collection.Find(new BsonDocument()).ToListAsync();
            return query.Result;
        }

        public async Task<User> UserbyID(string Id)
        {
            if (Id != null)
            {
                return await Collection.FindAsync(new BsonDocument { { "_id", new ObjectId(Id) } }).Result.FirstAsync();
            }
            return null;
        }

        public async Task NewUser(User usuario)
        {
            if (usuario != null )
            {
                if(AllUsers().Result.Count != 0)
                {
                    if(AllUsers().Result.Find(x => x.userName.Equals(usuario.userName)) == null)
                    {
                        await Collection.InsertOneAsync(usuario);
                    }
                    else
                    {
                        throw new Exception("Usuario ya ingresado");
                    }
                }
                else
                {
                    await Collection.InsertOneAsync(usuario);
                }
            }
            else
            {
                throw new Exception("Error de usuario");
            }
        }

        public async Task UpdateUser(User usuario)
        {
            await Collection.ReplaceOneAsync(x=> x.id == usuario.id, usuario);
        }
        public async Task<User> UserbyName(string id)
        {
            var filter = Builders<User>.Filter.Eq(x => x.userName, id);
            return await Collection.FindAsync(filter).Result.FirstAsync();
        }
    }
}
