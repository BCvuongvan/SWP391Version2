using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TingStoreAPI.DB;

namespace TingStoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminProfileController : ControllerBase
    {
        private readonly ApplicationDBContext _db;
        public AdminProfileController(ApplicationDBContext db)
        {
            this._db = db;
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
    }
}