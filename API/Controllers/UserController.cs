using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Conexiones;
using API.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private IUsersCollection db = new UsersCollection();
        
        [HttpGet]
        [Route("AllUsers")]
        public async Task <IActionResult> AllUsers()
        {
            return Ok(await db.AllUsers());
        }
        
        [HttpPost]
        [Route("SignUp")]
        public async Task<IActionResult> NewUser([FromBody] User newuser)
        {
            if(newuser == null)
            {
                return BadRequest();
            }
            else if(newuser.userName == string.Empty)
            {
                ModelState.AddModelError("Nombre de usuario", "Nombre de usuario no encontrado");
            }
            await db.NewUser(newuser);
            return Created("Created", newuser);
        }

        [HttpPut("id")]
        [Route("SignUp")]
        public async Task<IActionResult> UpdateUser([FromBody] User newuser, string id)
        {
            if (newuser == null)
            {
                return BadRequest();
            }
            else if (newuser.userName == string.Empty)
            {
                ModelState.AddModelError("Nombre de usuario", "Nombre de usuario no encontrado");
            }
            newuser.ID = new MongoDB.Bson.ObjectId(id);
            await db.UpdateUser(newuser);
            return Created("Created", newuser);
        }
    }
}
