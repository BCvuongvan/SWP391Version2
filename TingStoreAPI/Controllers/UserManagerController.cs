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
            var ListOfUser = this._db.users.Include(u => u.orders).Include(u => u.feedbacks).Select(u => new
            {
                u.userName,
                u.email,
                u.password,
                u.fullName,
                u.phoneNumber,
                u.address,
                u.picture,
                u.createdAt,
                u.updateAt,
                u.userType,
                Feedback = u.feedbacks.Select(f => new
                {
                    f.proId,
                    f.product.proName,
                    f.product.proPrice
                }),
                Orser = u.orders.Select(o => new
                {
                    o.orderId,
                    o.orderDate,
                    o.TotalAmount,
                    o.orderStatusId,
                    o.orderStatus.statusName
                })
            }).ToList();
            return Ok(ListOfUser);
        }

        [HttpGet("{inputName}")]
        public IActionResult GetUserByUsername(string inputName)
        {
            var user = this._db.users.Include(u => u.orders).Include(u => u.feedbacks).Where(u => u.userName == inputName).Select(u => new
            {
                u.userName,
                u.email,
                u.password,
                u.fullName,
                u.phoneNumber,
                u.address,
                u.picture,
                u.createdAt,
                u.updateAt,
                u.userType,
                Feedback = u.feedbacks.Select(f => new
                {
                    f.proId,
                    f.product.proName,
                    f.product.proPrice
                }),
                Orser = u.orders.Select(o => new
                {
                    o.orderId,
                    o.orderDate,
                    o.TotalAmount,
                    o.orderStatusId,
                    o.orderStatus.statusName
                })
            });
            return Ok(user);
        }

    }
}