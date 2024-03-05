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
    public class CustomerProfileController : ControllerBase
    {
        private readonly ApplicationDBContext _db;

        public CustomerProfileController(ApplicationDBContext db)
        {
            this._db = db;
        }

        [HttpGet]
        public IActionResult GetAllUser()
        {
            var listOfUser = this._db.users
            .Include(u => u.orders)
                .ThenInclude(o => o.orderDetails)
            .Include(u => u.feedbacks)
                .ThenInclude(f => f.product)
            .ToList();
            return Ok(listOfUser);
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


        [HttpPut("{id}")]
        public IActionResult UpdateUserProfile(string id, User user)
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