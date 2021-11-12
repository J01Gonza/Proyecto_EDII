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
            try
            {
                var query = Collection.
                    Find(new BsonDocument()).ToListAsync();
                return query.Result;
            }
            catch
            {
                return null;
            }
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
            if(usuario != null && AllUsers().Find(x => x.User.Equals(usuario.User)) == null)
            {
                Collection.InsertOneAsync(usuario);
            }
            else
            {
                throw new Exception("Usuario ya ingresado");
            }
        }

        public void UpdateUser(Usuario usuario)
        {
            var filter = Builders<Usuario>.Filter.Eq(x => x.ID, usuario.ID);
            Collection.ReplaceOneAsync(filter, usuario);

        }
    }
}
