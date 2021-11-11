using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WEB.Models;

namespace WEB.Conexiones
{
    public class UsersCollection : IUsersCollection
    {
        internal mongoConnection _connection = new mongoConnection();
        private IMongoCollection<Usuario> Collection;

        public UsersCollection()
        {
            Collection = _connection.DB.GetCollection<Usuario>("Usuarios");
        }

        public List<Usuario> AllUsers()
        {
            var query = Collection.
                Find(new BsonDocument()).ToListAsync();
            return query.Result;
        }

        public Usuario GetbyUser(string Id)
        {
            if(Id != null)
            {
                return (Usuario)Collection.Find(x => x.User.Equals(Id));
            }
            return null;
        }

        public void NewUser(Usuario usuario)
        {
            if(usuario != null)
            {
                Collection.InsertOneAsync(usuario);
            }
        }

        public void UpdateUser(Usuario usuario)
        {
           
        }
    }
}
