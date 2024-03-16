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
    public class CustomerOrderController : ControllerBase
    {
        private readonly ApplicationDBContext _db;

        public CustomerOrderController(ApplicationDBContext db)
        {
            this._db = db;
        }

        [HttpGet]
        public IActionResult GetOrderByCustomer()
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

        [HttpGet("CancelOrder/{id}")]
        public IActionResult CancelOrder(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid order ID");
            }
            var order = this._db.orders.FirstOrDefault(o => o.orderId == id);
            if (order == null)
            {
                return NotFound("Order not found");
            }
            // Lấy thời gian hiện tại
            var currentTime = DateTime.Now;

            // Kiểm tra nếu đơn hàng đã được tạo ra từ hơn 24 giờ trước
            if ((currentTime - order.orderDate).TotalMinutes > 1)
            {
                return BadRequest("Order can't be cancelled after 24 hours");
            }
            order.orderStatusId = 4;
            this._db.SaveChanges();
            return Ok(order);
        }

    }
}