using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API1.Conexiones;
using API1.Models;

namespace API1.Controllers
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

        [HttpGet]
        [Route("UserbyUN/{username}")]
        public async Task<IActionResult> UserbyName(string username)
        {
            return Ok(await db.UserbyName(username));
        }

        [HttpGet]
        [Route("UserbyID/{id}")]
        public async Task <IActionResult> GetbyID([FromRoute]string id)
        {
            return Ok(await db.UserbyID(id));
        }

        [HttpPost]
        [Route("SignUp")]
        public async Task<IActionResult> NewUser([FromBody] User newuser)
        {
            if(newuser == null)
            {
                return BadRequest();
            }
            else if(newuser.userName == null)
            {
                ModelState.AddModelError("Nombre de usuario", "Nombre de usuario no encontrado");
            }
            await db.NewUser(newuser);
            return Created("Created", newuser);
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> UpdateUser([FromBody] User newuser)
        {
            if (newuser == null)
            {
                return BadRequest();
            }
            else if (newuser.userName == string.Empty)
            {
                ModelState.AddModelError("Nombre de usuario", "Nombre de usuario no encontrado");
            }
            await db.UpdateUser(newuser);
            return Created("Created", newuser);
        }
    }
}
