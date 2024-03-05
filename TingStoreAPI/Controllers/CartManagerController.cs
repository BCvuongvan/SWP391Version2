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
    public class CartManagerController : ControllerBase
    {
        private readonly ApplicationDBContext _db;
        public CartManagerController(ApplicationDBContext db)
        {
            this._db = db;
        }

        [HttpGet("{id}", Name = "GetCartByUsername")]
        public IActionResult GetCartByUsername(string id)
        {
            var ListCart = this._db.carts.Include(c => c.product).ThenInclude(p => p.discountPercents).Where(c => c.userName.Equals(id));
            return Ok(ListCart);
        }
        [HttpGet("GetItemCartByUsernameProId/{IUsername}/{IProId}", Name = "GetItemCartByUsernameProId")]
        public IActionResult GetItemCartByUsernameProId(string IUsername, int IProId)
        {
            var carItem = this._db.carts.Include(c => c.product).SingleOrDefault(c => c.userName.Equals(IUsername) && c.proId == IProId);
            if (carItem == null)
            {
                return BadRequest("These is no this product in this cart");
            }
            return Ok(carItem);
        }

        [HttpGet("AddToCart/{Iusername}/{IProId}", Name = "AddToCart")]
        public IActionResult AddToCart(string Iusername, int IProId)
        {
            var carItem = this._db.carts.Include(c => c.product).SingleOrDefault(c => c.userName.Equals(Iusername) && c.proId == IProId);
            if (carItem == null)
            {
                carItem = new Cart(Iusername, IProId, 1);
                this._db.Add(carItem);
            }
            else
            {
                carItem.quantity++;
                if (carItem.quantity > 10)
                {
                    carItem.quantity = 10;
                }
            }
            this._db.SaveChanges();
            return CreatedAtRoute("GetItemCartByUsernameProId", new { IUsername = carItem.userName, IProId = carItem.proId }, carItem);
        }
        [HttpGet("DecreaseQuantity/{Iusername}/{IProId}", Name = "DecreaseQuantity")]
        public IActionResult DecreaseQuantity(string Iusername, int IProId)
        {
            var carItem = this._db.carts.Include(c => c.product).SingleOrDefault(c => c.userName.Equals(Iusername) && c.proId == IProId);
            if (carItem != null)
            {
                if (carItem.quantity > 1)
                {
                    carItem.quantity--;
                    this._db.SaveChanges();
                    return CreatedAtRoute("GetItemCartByUsernameProId", new { IUsername = carItem.userName, IProId = carItem.proId }, carItem);
                }
                else
                {
                    this._db.Remove(carItem);
                    this._db.SaveChanges();
                    return CreatedAtRoute("GetItemCartByUsernameProId", new { IUsername = carItem.userName, IProId = carItem.proId }, carItem);
                }
            }
            return BadRequest("Invalid request or item not found.");
        }
        [HttpDelete("RemoveItem/{Iusername}/{IProId}", Name = "RemoveItem")]
        public IActionResult RemoveItem(string Iusername, int IProId)
        {
            var carItem = this._db.carts.Include(c => c.product).SingleOrDefault(c => c.userName.Equals(Iusername) && c.proId == IProId);
            if (carItem != null)
            {
                this._db.carts.Remove(carItem);
                this._db.SaveChanges();
                return Ok(carItem);
            }
            return BadRequest("Invalid request or item not found.");
        }
        
    }
}