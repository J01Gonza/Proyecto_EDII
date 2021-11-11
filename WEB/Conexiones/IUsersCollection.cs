using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WEB.Models;

namespace WEB.Conexiones
{
    interface IUsersCollection
    {
        void NewUser(Usuario usuario);
        void UpdateUser(Usuario usuario);
        List<Usuario> AllUsers();
        Usuario GetbyUser(string Id); 
    }
}
