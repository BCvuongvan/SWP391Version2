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
    public class HandlingCartController : ControllerBase
    {
        private readonly ApplicationDBContext _db;

        public HandlingCartController(ApplicationDBContext db)
        {
            this._db = db;
        }
        [HttpGet("{id}", Name = "GetOrderById")]
        public IActionResult GetOrderById(int id)
        {
            var Order = this._db.orders.Include(o => o.user).Include(o => o.orderDetails).SingleOrDefault(o => o.orderId == id);
            return Ok(Order);
        }

        [HttpPost("CreateOrder")]
        public IActionResult CreateOrder(Order order)
        {
            if (order == null)
            {
                return NotFound();
            }
            this._db.orders.Add(new Order(order.userName, order.orderDate, order.TotalAmount, 1));
            this._db.SaveChanges();
            int generatedOrderId = 0;
            var ListOrder = this._db.orders.Where(o => o.userName.Equals(order.userName)).ToList();
            foreach (var item in ListOrder)
            {
                generatedOrderId = item.orderId;
            }
            var ListCart = this._db.carts.Include(c => c.product).ThenInclude(p => p.discountPercents).Where(c => c.userName.Equals(order.userName));
            foreach (var item in ListCart)
            {
                this._db.orderDetails.Add(new OrderDetail(generatedOrderId, item.proId, item.quantity, (decimal)item.product.proPrice * item.quantity));
                // UpdateProduct(item.proId, item.quantity);
            }
            this._db.SaveChanges();
            return Ok("sucessfull");
        }

        [HttpGet("UpdateProduct/{id}/{quantity}")]
        public IActionResult UpdateProduct(int id, int quantity)
        {
            Product product = this._db.products.AsNoTracking().FirstOrDefault(p => p.proId == id);
            if (product == null)
            {
                return NotFound("Could not be found!!!");
            }
            this._db.products.Update(new Product(product.proId, product.proName, product.proDescription, product.proPrice, product.proQuantity -= quantity, product.proImage, product.cateId, product.proStatus));
            this._db.SaveChanges();
            return CreatedAtRoute("GetProductById", new { id = product.proId }, product);
        }
    }
}