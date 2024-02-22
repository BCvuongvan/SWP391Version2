using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TingStoreAPI.DB;
using TingStoreAPI.Models;

namespace TingStoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserManagerController : ControllerBase
    {
        private readonly ApplicationDBContext _db;
        public UserManagerController(ApplicationDBContext db)
        {
            this._db = db;
        }
        [HttpGet]
        public IActionResult GetAllOfUser()
        {
            var ListOfUser = this._db.users.Include(u => u.orders).Include(u => u.feedbacks);
            return Ok(ListOfUser);
        }

        [HttpGet("{id}")]
        public IActionResult GetUserByUsername(string id)
        {
            var user = this._db.users
        .Include(u => u.orders)
            .ThenInclude(o => o.orderStatus)
        .Include(u => u.feedbacks)
            .ThenInclude(f => f.product)
        .FirstOrDefault(u => u.userName == id);
            return Ok(user);
        }

        [HttpPost]
        public IActionResult AddUser(User user)
        {
            if (user == null)
            {
                return BadRequest("User can't be null");
            }
            this._db.users.Add(new User(user.userName, user.email, user.password, user.fullName, user.phoneNumber, user.address, user.picture, user.createdAt, user.updateAt, user.userType));
            this._db.SaveChanges();
            // return CreatedAtRoute("GetUserByUsername", new { id = user.userName }, user);
            return Ok(user);
        }


        [HttpPut("{id}")]
        public IActionResult UpdateUsername(string id, User user)
        {
            if (user == null)
            {
                return BadRequest("User can't be null");
            }
            User obj = this._db.users.AsNoTracking().FirstOrDefault(u => u.userName.Equals(id));
            if (obj == null)
            {
                return NotFound("Could not be found");
            }
            this._db.users.Update(new User(user.userName, user.email, user.password, user.fullName, user.phoneNumber, user.address, user.picture, user.createdAt, user.updateAt, user.userType));
            this._db.SaveChanges();
            // return CreatedAtRoute("GetUserByUsername", new { id = user.userName }, user);
            return Ok(user);
        }


    }
}