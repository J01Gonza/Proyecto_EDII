using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;
using MongoDB.Bson;

namespace API.Conexiones
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
            return await Collection.FindAsync(new BsonDocument()).Result.ToListAsync();
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
            if (usuario != null && AllUsers().Result.Find(x => x.userName.Equals(usuario.userName)) == null)
            {
                await Collection.InsertOneAsync(usuario);
            }
            else
            {
                throw new Exception("Usuario ya ingresado");
            }
        }

        public async Task UpdateUser(User usuario)
        {
            var filter = Builders<User>.Filter.Eq(x => x.ID, usuario.ID);
            await Collection.ReplaceOneAsync(filter, usuario);

        }
    }
}
