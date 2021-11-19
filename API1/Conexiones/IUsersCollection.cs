using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API1.Models;
namespace API1.Conexiones
{
    interface IUsersCollection
    {
        Task NewUser(User usuario);
        Task UpdateUser(User usuario);
        Task<List<User>> AllUsers();
        Task<User> UserbyID(string Id);
        Task<User> UserbyName(string id);
    }
}
