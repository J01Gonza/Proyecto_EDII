using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WEB.Models;
using DLL;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace WEB.Controllers
{
    public class UserController : Controller
    {
        const string SessionUser = "_User";
        const string ActualChat = "_Chat";

        private IHostingEnvironment Environment;

        public UserController(IHostingEnvironment _environment)
        {
            Environment = _environment;
        }

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
                diffiehellman DH = new diffiehellman();
                User member = UserbyName(id);
                Chats chat = activeUser.chats.Find(x => x.members.Count == 2 && x.members.Contains(id));
                if (chat == null)
                {
                    Chats newChat = new Chats()
                    {
                        id = 1,
                        messages = new List<Messages>(),
                        group = false,
                        members = new List<string>(),
                        keys = new List<char>()
                    };
                    int p = DH.pNumber();
                    int g = DH.gBase();
                    newChat.keys.Add((char)p);
                    newChat.keys.Add((char)g);
                    newChat.keys.Add((char)activeUser.key);
                    newChat.keys.Add((char)member.key);
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
                    int secretKey = DH.secretKey(chat.keys[2], chat.keys[3], chat.keys[1], chat.keys[0]);
                    for (int i = 0; i < chat.messages.Count(); i++)
                    {
                        chat.messages[i].content = sdesEncode(chat.messages[i].content, sdesKeys(secretKey).key2, sdesKeys(secretKey).key1);
                    }
                    returnm = chat;
                }
            }
            else
            {
                var members = id.Split(",");
                List<Chats> groups = activeUser.chats.FindAll(x => x.group == true);
                foreach (var g in groups)
                {
                    if (!g.members.Except(members.ToList()).Any() && g.members.Count == members.Length + 1)
                    {
                        returnm = g;
                    }
                }
            }
            HttpContext.Session.SetString(ActualChat, id);
            return View(returnm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Chat(IFormCollection collection)
        {
            Messages incoming = new Messages();
            incoming.content = collection["Contenido"];
            incoming.sender = HttpContext.Session.GetString("_User");
            User activeUser = UserbyName(HttpContext.Session.GetString(SessionUser));
            var Miembros = HttpContext.Session.GetString(ActualChat).Split(",");
            Chats actualChat = new Chats();
            if (Miembros.Length > 1)
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
                diffiehellman DH = new diffiehellman();
                User member = UserbyName(HttpContext.Session.GetString(ActualChat));
                actualChat = activeUser.chats.Find(x => x.members.Contains(HttpContext.Session.GetString(ActualChat)) && x.members.Count == 2);
                int secretKey = DH.secretKey(actualChat.keys[2], actualChat.keys[3], actualChat.keys[1], actualChat.keys[0]);
                incoming.content = sdesEncode(incoming.content, sdesKeys(secretKey).key1, sdesKeys(secretKey).key2);
                int posactualuser = activeUser.chats.IndexOf(actualChat);
                actualChat = member.chats.Find(x => x.members.Contains(activeUser.userName) && x.members.Count == 2);
                int posmember = member.chats.IndexOf(actualChat);
                incoming.id = activeUser.chats[posactualuser].messages.Count();
                activeUser.chats[posactualuser].messages.Add(incoming);
                incoming.id = member.chats[posmember].messages.Count();
                member.chats[posmember].messages.Add(incoming);
                UpdateUser(member);
            }
            UpdateUser(activeUser);
            return RedirectToAction("Chat", new { id = HttpContext.Session.GetString(ActualChat), group = false });
        }
        public IActionResult Search(IFormCollection collection)
        {
            diffiehellman DH = new diffiehellman();
            User actuser = UserbyName(HttpContext.Session.GetString(SessionUser));
            var Miembros = HttpContext.Session.GetString(ActualChat).Split(",");
            Chats search = new Chats();
            if (Miembros.Length > 1)
            {
                //busqueda grupal
                List<Chats> groups = actuser.chats.FindAll(x => x.group == true);
                foreach (var g in groups)
                {
                    if (!g.members.Except(Miembros).Any() && g.members.Count == Miembros.Length + 1)
                    {
                        search = g;
                    }
                }
            }
            else
            {
                search = actuser.chats.Find(x => x.members[0] == HttpContext.Session.GetString(ActualChat));
            }
            int secretKey = DH.secretKey(search.keys[2], search.keys[3], search.keys[1], search.keys[0]);
            for (int i = 0; i < search.messages.Count(); i++)
            {
                search.messages[i].content = sdesEncode(search.messages[i].content, sdesKeys(secretKey).key2, sdesKeys(secretKey).key1);
            }
            search.messages = search.messages.FindAll(x => x.content.Contains(collection["Contenido"]));
            return View(search);
        }

        public IActionResult DeleteForMe(int mID)
        {
            User actuser = UserbyName(HttpContext.Session.GetString(SessionUser));
            Chats search = GetChat(HttpContext.Session.GetString(SessionUser), HttpContext.Session.GetString(ActualChat));
            actuser.chats[search.id].messages[mID] = null;
            UpdateUser(actuser);
            return RedirectToAction("Chat", new { id = HttpContext.Session.GetString(ActualChat), group = search.group });
        }

        public IActionResult DeleteForAll(int mID)
        {
            User actuser = UserbyName(HttpContext.Session.GetString(SessionUser));
            Chats search = GetChat(HttpContext.Session.GetString(SessionUser), HttpContext.Session.GetString(ActualChat));
            actuser.chats[search.id].messages.RemoveAt(mID);
            for (int i = 0; i < actuser.chats[search.id].messages.Count; i++)
            {
                actuser.chats[search.id].messages[i].id = i;
            }
            UpdateUser(actuser);
            for (int i = 0; i < Miembros.Length; i++)
            {
                User member = UserbyName(Miembros[i]);
                int pos = member.chats.IndexOf(search);
                member.chats[pos].messages.RemoveAt(mID);
                for (int j = 0; j < member.chats[search.id].messages.Count; j++)
                {
                    member.chats[search.id].messages[j].id = j;
                }
                UpdateUser(member);
            }
            return RedirectToAction("Chat", new { id = HttpContext.Session.GetString(ActualChat), group = search.group });
        }

        private void Delete(string User1, string User2, int id)
        {
            User actuser = UserbyName(User1);
            Chats search = GetChat(User1, User2);
            actuser.chats[search.id].messages[id] = null;
            UpdateUser(actuser);
        }

                [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadFile(IFormFile file)
        {
            User activeUser = UserbyName(HttpContext.Session.GetString(SessionUser));
            Messages incoming = new Messages();
            Chats actualChat = new Chats();
            if (file != null)
            {
                string path = Path.Combine(this.Environment.WebRootPath, "Uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileName = Path.GetFileName(file.FileName);
                string filePath = Path.Combine(path, fileName);
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fs);
                }
                lzw LZW = new lzw();
                byte[] bytes = LZW.Compress(System.IO.File.ReadAllBytes(filePath));
                incoming.file.name = file.FileName;
                incoming.file.bytes = bytes;
                var Miembros = HttpContext.Session.GetString(ActualChat).Split(",");
                if (Miembros.Length > 1)
                {

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
            }
            return RedirectToAction("Chat", actualChat);
        }

        //public FileResult DownloadFile()
        //{
        //    return File();
        //}
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

        private Chats GetChat(string UserBase, string UserSearch)
        {
            User actuser = UserbyName(UserBase);
            var Miembros = UserSearch.Split(",");
            if (Miembros.Length > 1)
            {
                //busqueda grupal
                List<Chats> groups = actuser.chats.FindAll(x => x.group == true);
                foreach (var g in groups)
                {
                    if (!g.members.Except(Miembros).Any() && g.members.Count == Miembros.Length + 1)
                    {
                        return g;
                    }
                }
            }
            else
            {
                return actuser.chats.Find(x => x.members.Contains(HttpContext.Session.GetString(ActualChat)) && x.members.Count == 2);
            }
            return null;
        }

        public (string key1, string key2) sdesKeys(int key)
        {
            int[] P10 = { 3, 5, 2, 7, 4, 10, 1, 9, 8, 6 };
            int[] P8 = { 6, 3, 7, 4, 8, 5, 10, 9 };
            sdes SDES = new sdes();
            return SDES.generateKey(Convert.ToString(key, 2).PadLeft(10, '0'), P10, P8); ;
        }

        public string sdesEncode(string str, string key1, string key2)
        {
            List<byte> encode = new List<byte>();
            int[] P4 = { 2, 4, 3, 1 };
            int[] EP = { 4, 1, 2, 3, 2, 3, 4, 1 };
            int[] IP = { 2, 6, 3, 1, 4, 8, 5, 7 };
            int[] IP1 = { 4, 1, 3, 5, 7, 2, 8, 6 };
            sdes SDES = new sdes();
            byte[] bytes = Encoding.Unicode.GetBytes(str);
            foreach (var item in bytes)
            {
                string mainKey = Convert.ToString(item, 2).PadLeft(8, '0');
                encode.Add(SDES.Enconde(mainKey, key1, key2, P4, EP, IP, IP1));
            }
            return Encoding.Unicode.GetString(encode.ToArray());
        }
    }
}


