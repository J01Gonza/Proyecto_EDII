using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WEB.Models;

namespace WEB.Controllers
{
    public class UserController : Controller
    {
        const string SessionUser = "_User";

        public IActionResult Index()
        {
            string x = HttpContext.Session.GetString(SessionUser);
            //List<Chats> chats = DB.AllUsers().Find(x => x.User.Equals(user)).Chats;
            return View(new Chats());
        }

        public IActionResult Contacts()
        {
            string usuario = HttpContext.Session.GetString(SessionUser);
            //List<Contact> contacto = DB.AllUsers().Find(x => x.User.Equals(usuario)).Contacts.FindAll(x => x.Sent == true && x.Received == true);
            return View(new Contact());
        }
        public IActionResult AddFriend()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddFriend(IFormCollection collection)
        {
            //try
            //{
            //    User Ingreso = DB.AllUsers().Find(x => x.User.Equals(collection["User"]));
            //    User Sesion = DB.AllUsers().Find(x => x.User.Equals(HttpContext.Session.GetString(SessionUser)));
            //    if (Ingreso != null)
            //    {
            //        Contact newContact = new Contact()
            //        {
            //            UserContact = collection["User"],
            //            Sent = true,
            //            Received = false
            //        };
            //        if (Sesion.Contacts.Find(x => x.UserContact.Equals(newContact.UserContact)) == null)
            //        {
            //            Sesion.Contacts.Add(newContact);
            //            Ingreso.Contacts.Add(new Contact() { UserContact = Sesion.User, Sent = false, Received = true });
            //            DB.UpdateUser(Sesion);
            //            DB.UpdateUser(Ingreso);
            //            ViewData["Success"] = "Solicitud de amistad enviada a " + collection["User"];
            //        }
            //        else
            //        {
            //            ViewData["Error"] = "Contacto ya existente";
            //        }
            //    }
            //    else
            //    {
            //        ViewData["Error"] = "Usuario no encontrado en la base de datos";
            //    }
            //}
            //catch
            //{
            //    ViewData["Error"] = "Información ingresada no válida, intente de nuevo";
            //}
            return View();
        }

        public IActionResult SentRequests()
        {
            string usuario = HttpContext.Session.GetString(SessionUser);
            //List<Contact> contacto = DB.AllUsers().Find(x => x.User.Equals(usuario)).Contacts.FindAll(x => x.Sent == true && x.Received == false);
            return View(new Contact());
        }

        public IActionResult Requests()
        {
            string usuario = HttpContext.Session.GetString(SessionUser);
            //List<Contact> contacto = DB.AllUsers().Find(x => x.User.Equals(usuario)).Contacts.FindAll(x => x.Sent == false && x.Received == true);
            return View(new Contact());
        }

        public IActionResult AcceptRequests(string id)
        {
            //User upUsuario = DB.AllUsers().Find(x => x.User.Equals(HttpContext.Session.GetString(SessionUser)));
            //User upUsuario2 = DB.AllUsers().Find(x => x.User.Equals(id));
            //int x = upUsuario.Contacts.FindIndex(x => x.UserContact.Equals(id));
            //int x2 = upUsuario2.Contacts.FindIndex(x => x.UserContact.Equals(HttpContext.Session.GetString(SessionUser)));
            //upUsuario.Contacts[x].Sent = true;
            //upUsuario2.Contacts[x2].Received = true;
            //DB.UpdateUser(upUsuario);
            //DB.UpdateUser(upUsuario2);
            return RedirectToAction(nameof(Contacts));
        }

        public IActionResult DeleteRequests(string id, bool sent)
        {
            //User upUsuario = DB.AllUsers().Find(x => x.User.Equals(HttpContext.Session.GetString(SessionUser)));
            //User upUsuario2 = DB.AllUsers().Find(x => x.User.Equals(id));
            //int x = upUsuario.Contacts.FindIndex(x => x.UserContact.Equals(id));
            //int x2 = upUsuario2.Contacts.FindIndex(x => x.UserContact.Equals(HttpContext.Session.GetString(SessionUser)));
            //upUsuario.Contacts.RemoveAt(x);
            //upUsuario2.Contacts.RemoveAt(x2);
            //DB.UpdateUser(upUsuario);
            //DB.UpdateUser(upUsuario2);
            //ViewData["Success"] = "Solicitud de amistad eliminada correctamente";
            if (sent)
            {
                return RedirectToAction(nameof(SentRequests));
            }
            else
            {
                return RedirectToAction(nameof(Request));
            }
        }


    }
}
