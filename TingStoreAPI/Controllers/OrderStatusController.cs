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
    public class OrderStatusController : ControllerBase
    {
        private readonly ApplicationDBContext _db;

        public OrderStatusController(ApplicationDBContext db)
        {
            this._db = db;
        }

        [HttpGet]
        public IActionResult GetAllOrderStatus()
        {
            var orderStatus = this._db.orderStatuses
            .Include(o => o.orders)
            .ToList();
            return Ok(orderStatus);
        }

        [HttpGet("{orderStatusId}")]
        public IActionResult GetOrderStatusById(int orderStatusId)
        {
            var orderStatus = this._db.orderStatuses
            .Find(orderStatusId);
            if (orderStatus == null)
            {
                return NotFound($"Order status with ID {orderStatusId} not found.");
            }
            return Ok(orderStatus);
        }
    }
}