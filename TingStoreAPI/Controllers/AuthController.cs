using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TingStoreAPI.DB;
using TingStoreAPI.Models;

namespace TingStoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDBContext _db;
        public AuthController(ApplicationDBContext db)
        {
            this._db = db;
        }
        [HttpGet("")]
        public IActionResult GetAllOfUser()
        {
            var listOfUser = this._db.users.ToList();
            return Ok(listOfUser);
        }
        [HttpGet("GetUser/{username}/{pass}")]
        public IActionResult GetUser(string username, string pass)
        {
            var user = this._db.users.FirstOrDefault(u => u.userName.Equals(username) && u.password.Equals(pass));
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        [HttpPost]
        public IActionResult SignUp(User user)
        {
            if (user == null)
            {
                return BadRequest("User data is required");
            }
            var existingUser = this._db.users.FirstOrDefault(u => u.userName.Equals(user.userName));
            if (existingUser != null)
            {
                return BadRequest("Username already exists. Please choose a different username.");
            }
            this._db.users.Add(new User(user.userName, user.email, user.password, user.fullName, user.phoneNumber, user.address, user.picture, user.createdAt, user.updateAt, user.userType));
            this._db.SaveChanges();
            return Ok(user);
        }
    }
}