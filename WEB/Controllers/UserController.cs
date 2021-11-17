using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WEB.Conexiones;
using WEB.Models;

namespace WEB.Controllers
{
    public class UserController : Controller
    {
        private IUsersCollection DB = new UsersCollection();
        static string userchat;
        static bool actualgroup;
        public IActionResult Index(string user)
        {
            List<Chats> chats = DB.AllUsers().Find(x => x.User.Equals(user)).Chats;
            return View(chats);
        }

        public IActionResult Contacts()
        {
            string usuario = HttpContext.Session.GetString("_User");
            List<Contacto> contacto = DB.AllUsers().Find(x => x.User.Equals(usuario)).Contacts.FindAll(x => x.Sent == true && x.Received == true);
            return View(contacto);
        }
        public IActionResult AddFriend()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddFriend(IFormCollection collection)
        {
            try
            {
                Usuario Ingreso = DB.AllUsers().Find(x => x.User.Equals(collection["User"]));
                Usuario Sesion = DB.AllUsers().Find(x => x.User.Equals(HttpContext.Session.GetString("_User")));
                if (Ingreso != null)
                {
                    Contacto newContact = new Contacto()
                    {
                        UserContact = collection["User"],
                        Sent = true,
                        Received = false
                    };
                    if (Sesion.Contacts.Find(x => x.UserContact.Equals(newContact.UserContact)) == null)
                    {
                        Sesion.Contacts.Add(newContact);
                        Ingreso.Contacts.Add(new Contacto() { UserContact = Sesion.User, Sent = false, Received = true });
                        DB.UpdateUser(Sesion);
                        DB.UpdateUser(Ingreso);
                        ViewData["Success"] = "Solicitud de amistad enviada a " + collection["User"];
                    }
                    else
                    {
                        ViewData["Error"] = "Contacto ya existente";
                    }
                }
                else
                {
                    ViewData["Error"] = "Usuario no encontrado en la base de datos";
                }
            }
            catch
            {
                ViewData["Error"] = "Información ingresada no válida, intente de nuevo";
            }
            return View();
        }

        public IActionResult SentRequests()
        {
            string usuario = HttpContext.Session.GetString("_User");
            List<Contacto> contacto = DB.AllUsers().Find(x => x.User.Equals(usuario)).Contacts.FindAll(x => x.Sent == true && x.Received == false);
            return View(contacto);
        }

        public IActionResult Requests()
        {
            string usuario = HttpContext.Session.GetString("_User");
            List<Contacto> contacto = DB.AllUsers().Find(x => x.User.Equals(usuario)).Contacts.FindAll(x => x.Sent == false && x.Received == true);
            return View(contacto);
        }

        public IActionResult AcceptRequests(string id)
        {
            Usuario upUsuario = DB.AllUsers().Find(x => x.User.Equals(HttpContext.Session.GetString("_User")));
            Usuario upUsuario2 = DB.AllUsers().Find(x => x.User.Equals(id));
            int x = upUsuario.Contacts.FindIndex(x => x.UserContact.Equals(id));
            int x2 = upUsuario2.Contacts.FindIndex(x => x.UserContact.Equals(HttpContext.Session.GetString("_User")));
            upUsuario.Contacts[x].Sent = true;
            upUsuario2.Contacts[x2].Received = true;
            DB.UpdateUser(upUsuario);
            DB.UpdateUser(upUsuario2);
            return RedirectToAction(nameof(Contacts));
        }

        public IActionResult DeleteRequests(string id, bool sent)
        {
            Usuario upUsuario = DB.AllUsers().Find(x => x.User.Equals(HttpContext.Session.GetString("_User")));
            Usuario upUsuario2 = DB.AllUsers().Find(x => x.User.Equals(id));
            int x = upUsuario.Contacts.FindIndex(x => x.UserContact.Equals(id));
            int x2 = upUsuario2.Contacts.FindIndex(x => x.UserContact.Equals(HttpContext.Session.GetString("_User")));
            upUsuario.Contacts.RemoveAt(x);
            upUsuario2.Contacts.RemoveAt(x2);
            DB.UpdateUser(upUsuario);
            DB.UpdateUser(upUsuario2);
            ViewData["Success"] = "Solicitud de amistad eliminada correctamente";
            if (sent)
            {
                return RedirectToAction(nameof(SentRequests));
            }
            else
            {
                return RedirectToAction(nameof(Request));
            }
        }

        public IActionResult Chat(string id, bool group)
        {
            ViewData["usuarios"] = id;
            Usuario upUsuario = DB.AllUsers().Find(x => x.User.Equals(HttpContext.Session.GetString("_User")));
            Chats returnm = new Chats();
            if(!group)
            {
                actualgroup = false;
                Usuario upUsuario2 = DB.AllUsers().Find(x => x.User.Equals(id));
                Chats chat = upUsuario.Chats.Find(x => x.usuarios.Count == 2 && x.usuarios.Contains(id));
                if(chat == null)
                {
                    Chats newChat = new Chats();
                    newChat.usuarios = new List<string>();
                    newChat.usuarios.Add(upUsuario.User);
                    newChat.usuarios.Add(id);
                    newChat.mensajes = new List<Mensajes>();
                    newChat.Grupal = false;
                    upUsuario.Chats.Add(newChat);
                    upUsuario2.Chats.Add(newChat);
                    DB.UpdateUser(upUsuario);
                    DB.UpdateUser(upUsuario2);
                    returnm = newChat;
                }
                else
                {
                    returnm = chat;
                }
            }
            else
            {
                actualgroup = true;
                var members = id.Split(",");
                List<Chats> groups = upUsuario.Chats.FindAll(x => x.Grupal == true);
                foreach(var g in groups)
                {
                    if (!g.usuarios.Except(members.ToList()).Any() && g.usuarios.Count == members.Length)
                    {
                        
                        returnm = g;
                    }
                }
            }
            userchat = id;
            //descifrar mensajes antes de devolver;
            return View(returnm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Chat(IFormCollection collection)
        {
            Mensajes incoming = new Mensajes();
            incoming.Content = collection["Contenido"];
            incoming.Sender = HttpContext.Session.GetString("_User");
            Usuario upUsuario = DB.AllUsers().Find(x => x.User.Equals(HttpContext.Session.GetString("_User")));
            int pos = 0;
            Chats actualChat = new Chats();
            if (actualgroup)
            {
                actualChat = upUsuario.Chats.Find(x => !x.usuarios.Except(userchat.Split(",")).Any() && x.usuarios.Count == userchat.Split(",").Length + 1);
                pos = upUsuario.Chats.IndexOf(actualChat);
                upUsuario.Chats[pos].mensajes.Add(incoming);
                foreach (var integrante in userchat.Split(","))
                {
                    Usuario member = DB.AllUsers().Find(x => x.User.Equals(integrante));
                    actualChat = member.Chats.Find(x => !x.usuarios.Except(userchat.Split(",")).Any() && x.usuarios.Count == userchat.Split(",").Length + 1 && x.usuarios.Contains(upUsuario.User));
                    pos = member.Chats.IndexOf(actualChat);
                    member.Chats[pos].mensajes.Add(incoming);
                    DB.UpdateUser(member);
                }
            }
            else 
            {
                actualChat = upUsuario.Chats.Find(x => x.usuarios.Contains(userchat) && x.usuarios.Count == 2);
                pos = upUsuario.Chats.IndexOf(actualChat);
                upUsuario.Chats[pos].mensajes.Add(incoming);
                Usuario member = DB.AllUsers().Find(x => x.User.Equals(userchat));
                actualChat = member.Chats.Find(x => x.usuarios.Contains(upUsuario.User) && x.usuarios.Count == 2);
                pos = member.Chats.IndexOf(actualChat);
                member.Chats[pos].mensajes.Add(incoming);
                DB.UpdateUser(member);
            }
            DB.UpdateUser(upUsuario);
            return View(actualChat);
        }
    }
   
}
