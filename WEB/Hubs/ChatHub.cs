using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace WEB.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
       
    }
    public class ImagesMessageHub : Hub
    {
        public Task ImageMessage(ImageMessage file)
        {
            return Clients.All.SendAsync("ImageMessage", file);
        }
    }
    public class ImageMessage
    {
        public byte[] ImageBinary { get; set; }
        public string ImageHeaders { get; set; }
    }
}
