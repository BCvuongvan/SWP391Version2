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
    public class OrderDetailController : ControllerBase
    {
        private readonly ApplicationDBContext _db;

        public OrderDetailController(ApplicationDBContext db)
        {
            this._db = db;
        }

        [HttpGet("orderId")]
        public ActionResult GetAllOrderDetail(int orderId)
        {
            var orderDetails = _db.orderDetails
                .Include(od => od.order)
                .Include(od => od.product)
                .FirstOrDefault(od => od.orderId == orderId);
            if (orderDetails == null)
            {
                return NotFound("No order details found for the given order ID.");
            }
            return Ok(orderDetails);
        }
        
    } 
}