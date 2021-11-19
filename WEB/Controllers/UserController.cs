using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WEB.Models;

namespace WEB.Controllers
{
    public class UserController : Controller
    {
        const string SessionUser = "_User";
        const string ActualChat = "_Chat";
        public IActionResult Index()
        {
            User user = UserbyName(HttpContext.Session.GetString(SessionUser));
            return View(user.chats);
        }

        public IActionResult Contacts(string id)
        {
            User user = UserbyName(HttpContext.Session.GetString(SessionUser));
            return View(user.contacts.FindAll(x=> x.sent == true && x.received == true));
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
                string uri = "User/UserbyUN/" + collection["userName"];
                var userSession = GlobalVariables.webClient.GetAsync(uri).Result;
                if (userSession.ReasonPhrase != "No Content")
                {
                    User activeUser = UserbyName(HttpContext.Session.GetString(SessionUser));
                    if (collection["userName"] != activeUser.userName)
                    {
                        User addUser = UserbyName(collection["userName"]);
                        Contact newContact = new Contact()
                        {
                            userContact = collection["userName"],
                            sent = true,
                            received = false
                        };
                        if (activeUser.contacts.Find(x => x.userContact == collection["userName"]) == null)
                        {
                            activeUser.contacts.Add(newContact);
                            addUser.contacts.Add(new Contact() { userContact = activeUser.userName, sent = false, received = true });
                            UpdateUser(activeUser);
                            UpdateUser(addUser);
                            return RedirectToAction("SentRequests");
                        }
                        else
                        {
                            ViewData["Error"] = "Ya enviaste esta solicitud, viajero.";
                        }
                    }
                    else
                    {
                        ViewData["Error"] = "No se puede agregar a usted mismo, intente encontrando viajeros nuevos :)";
                    }
                }
                else
                {
                    ViewData["Error"] = "Viajero no registrado, intenta de nuevo.";
                }
            }
            catch
            {
                ViewData["Error"] = "Ingresa los datos correctamente, viajero.";
            }
            return View();
        }

        public IActionResult SentRequests()
        {
            User activeUser = UserbyName(HttpContext.Session.GetString(SessionUser));
            List<Contact> sent = activeUser.contacts.FindAll(x => x.sent == true && x.received == false);
            return View(sent);
        }

        public IActionResult Requests()
        {
            User activeUser = UserbyName(HttpContext.Session.GetString(SessionUser));
            List<Contact> received = activeUser.contacts.FindAll(x => x.sent == false && x.received == true);
            return View(received);
        }

        public IActionResult AcceptRequests(string id)
        {
            User activeUser = UserbyName(HttpContext.Session.GetString(SessionUser));
            User addUser = UserbyName(id);
            int x = activeUser.contacts.FindIndex(x => x.userContact.Equals(id));
            int x2 = addUser.contacts.FindIndex(x => x.userContact.Equals(activeUser.userName));
            activeUser.contacts[x].sent = true;
            addUser.contacts[x2].received = true;
            UpdateUser(activeUser);
            UpdateUser(addUser);
            return RedirectToAction("Contacts");
        }

        public IActionResult DeleteRequests(string id, bool sent)
        {
            User activeUser = UserbyName(HttpContext.Session.GetString(SessionUser));
            User addUser = UserbyName(id);
            int x = activeUser.contacts.FindIndex(x => x.userContact.Equals(id));
            int x2 = addUser.contacts.FindIndex(x => x.userContact.Equals(HttpContext.Session.GetString(SessionUser)));
            activeUser.contacts.RemoveAt(x);
            addUser.contacts.RemoveAt(x2);
            UpdateUser(activeUser);
            UpdateUser(activeUser);
            ViewData["Success"] = "Solicitud de amistad eliminada correctamente.";
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
            ViewData["users"] = id;
            User activeUser = UserbyName(HttpContext.Session.GetString(SessionUser));
            Chats returnm = new Chats();
            if (!group)
            {
                User member = UserbyName(id);
                Chats chat = activeUser.chats.Find(x => x.members.Count == 2 && x.members.Contains(id));
                if (chat == null)
                {
                    Chats newChat = new Chats()
                    {
                        members = new List<string>(),
                        messages = new List<Messages>(),
                        //DECLARAR LLAVES DE CIFRADO
                        group = false
                    };
                    newChat.members.Add(activeUser.userName);
                    newChat.members.Add(member.userName);
                    activeUser.chats.Add(newChat);
                    member.chats.Add(newChat);
                    UpdateUser(activeUser);
                    UpdateUser(member);
                    returnm = newChat;
                }
                else
                {
                    returnm = chat;
                }
            }
            else
            {
                var members = id.Split(",");
                List<Chats> groups = activeUser.chats.FindAll(x => x.group == true);
                foreach (var g in groups)
                {
                    if (!g.members.Except(members.ToList()).Any() && g.members.Count == members.Length)
                    {
                        returnm = g;
                    }
                }
            }
            HttpContext.Session.SetString(ActualChat, id);
            //descifrar mensajes antes de devolver;
            return View(returnm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Chat(IFormCollection collection)
        {
            Messages incoming = new Messages();
            //cifrar antes de hacer esto
            incoming.content = collection["Contenido"];
            incoming.sender = HttpContext.Session.GetString("_User");
            User activeUser = UserbyName(HttpContext.Session.GetString(SessionUser));
            var Miembros = HttpContext.Session.GetString(ActualChat).Split(",");
            Chats actualChat = new Chats();
            if (Miembros.Length >1)
            {
                actualChat = activeUser.chats.Find(x => !x.members.Except(Miembros).Any() && x.members.Count == Miembros.Length + 1);
                int pos = activeUser.chats.IndexOf(actualChat);
                activeUser.chats[pos].messages.Add(incoming);
                foreach (var integrante in ActualChat.Split(","))
                {
                    User member = UserbyName(integrante);
                    actualChat = member.chats.Find(x => !x.members.Except(Miembros).Any() && x.members.Count == Miembros.Length + 1 && x.members.Contains(activeUser.userName));
                    pos = member.chats.IndexOf(actualChat);
                    member.chats[pos].messages.Add(incoming);
                    UpdateUser(member);
                }
            }
            else
            {
                User member = UserbyName(HttpContext.Session.GetString(ActualChat));
                actualChat = activeUser.chats.Find(x => x.members.Contains(HttpContext.Session.GetString(ActualChat)) && x.members.Count == 2);
                int posactualuser = activeUser.chats.IndexOf(actualChat);
                actualChat = member.chats.Find(x => x.members.Contains(activeUser.userName) && x.members.Count == 2);
                int posmember = member.chats.IndexOf(actualChat);
                activeUser.chats[posactualuser].messages.Add(incoming);
                member.chats[posmember].messages.Add(incoming);
                UpdateUser(member);
            }
            UpdateUser(activeUser);
            return View(actualChat);
        }

        public IActionResult SearchMessages(Chats search)
        {
            string miembros = "";
            foreach(var m in search.members)
            {
                if(m != HttpContext.Session.GetString(SessionUser))
                {
                    miembros += m;
                    if(m != search.members.Last() && !search.group)
                    {
                        miembros += ", ";
                    }
                }
            }
            ViewData["user"] = miembros;
            return View(search);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SearchMessages()
        {
            return View();
        }
        private string UpdateUser(User upUser)
        {
            string p = upUser.id.ToString();
            string json = System.Text.Json.JsonSerializer.Serialize(upUser);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage ans = GlobalVariables.webClient.PutAsync("User/Update", content).Result;
            return ans.StatusCode.ToString();
        }

        private User UserbyID(string id)
        {
            string uri = "User/GetbyID/" + id;
            var userSession = GlobalVariables.webClient.GetAsync(uri).Result;
            User activeUser = System.Text.Json.JsonSerializer.Deserialize<User>(userSession.Content.ReadAsStringAsync().Result);
            return activeUser;
        }

        private User UserbyName(string user)
        {
            var ingreso = GlobalVariables.webClient.GetAsync("User/AllUsers").Result;
            List<User> allusers = System.Text.Json.JsonSerializer.Deserialize<List<User>>(ingreso.Content.ReadAsStringAsync().Result);
            User sessionUser = allusers.Find(x => x.userName == user);
            return sessionUser;
        }
    }
}


