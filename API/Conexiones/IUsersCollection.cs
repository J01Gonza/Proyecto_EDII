using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;
namespace API.Conexiones
{
    interface IUsersCollection
    {
        Task NewUser(User usuario);
        Task UpdateUser(User usuario);
        Task<List<User>> AllUsers();
        Task<User> UserbyID(string Id);
    }
}
