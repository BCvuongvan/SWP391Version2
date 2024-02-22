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
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDBContext _db;

        public OrderController(ApplicationDBContext db)
        {
            this._db = db;
        }

        [HttpGet]
        public IActionResult GetAllOrder()
        {
            var orderList = this._db.orders
            .Include(u => u.user)
            .Include(o => o.orderDetails)
            .Include(os => os.orderStatus)
            .ToList();
            return Ok(orderList);
        }

        [HttpGet("{id}")]
        public IActionResult GetOrderById(int id)
        {
            var orderId = this._db.orders
            .Include(u => u.user)
            .Include(os => os.orderStatus)
            .Include(o => o.orderDetails)
                .ThenInclude(o => o.product)
            .FirstOrDefault(o => o.orderId == id);
            if (orderId == null)
            {
                return NotFound();
            }
            return Ok(orderId);
        }

        [HttpPut("{id}")]
        public IActionResult Confirm(int id, Order order)
        {
            if (order == null)
            {
                return BadRequest("Invalid order data!!!");
            }

            // Check data is valid before processing it
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Order obj = this._db.orders
            .Include(o => o.orderStatus)
            .FirstOrDefault(o => o.orderId == id);

            if (obj == null)
            {
                return NotFound($"Order with ID {id} not found!");
            }

            obj.orderStatusId = order.orderStatusId;
            this._db.SaveChanges();
            return Ok(order);
        }

        [HttpGet("search/{userName}")]
        public IActionResult GetOrdersFiltered(string userName)
        {
            IQueryable<Order> query = this._db.orders
            .Include(u => u.user)
            .Include(o => o.orderDetails)
                .ThenInclude(p => p.product)
            .Include(os => os.orderStatus);

            if (!string.IsNullOrEmpty(userName))
            {
                query = query.Where(o => o.user.userName.Contains(userName.Trim().ToLower()));
            }
            var orderList = query.ToList();
            return Ok(orderList);

        }
    }
}