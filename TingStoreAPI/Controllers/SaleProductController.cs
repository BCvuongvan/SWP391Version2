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
    public class SaleProductController : ControllerBase
    {
        private readonly ApplicationDBContext _db;

        public SaleProductController(ApplicationDBContext db)
        {
            this._db = db;
        }

        [HttpGet]
        public IActionResult GetAllSaleProduct()
        {
            var saleProduct = _db.discountPercents.Include(d => d.product).ToList();
            return Ok(saleProduct);
        }

        [HttpGet("{id}", Name = "GetSaleProductById")]
        public IActionResult GetSaleProductById(int id)
        {
            var saleProduct = this._db.discountPercents.Include(p => p.product).FirstOrDefault(p => p.discountId == id);
            if (saleProduct == null)
            {
                return NotFound();
            }
            return Ok(saleProduct);
        }

        [HttpPost]
        public IActionResult AddSaleProduct(DiscountPercent disPer)
        {
            if (disPer == null)
            {
                return BadRequest("Sale product cannot be null.");
            }

            disPer.isActive = true;
            this._db.discountPercents.Add(new DiscountPercent(disPer.proId, disPer.discountPercentage, disPer.startDate, disPer.endDate, disPer.discountImage, disPer.isActive));
            this._db.SaveChanges();
            return CreatedAtRoute("GetSaleProductById", new { id = disPer.discountId }, disPer);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateSaleProduct(int id, DiscountPercent disPer)
        {
            if (disPer == null)
            {
                return BadRequest("Product cannot be null");
            }
            DiscountPercent discount = this._db.discountPercents.AsNoTracking().FirstOrDefault(p => p.discountId == id);
            if (discount == null)
            {
                return NotFound("Could not be found!!!");
            }
            disPer.isActive = true;
            this._db.discountPercents.Update(new DiscountPercent(disPer.discountId, disPer.proId, disPer.discountPercentage, disPer.startDate, disPer.endDate, disPer.discountImage, disPer.isActive));
            this._db.SaveChanges();
            return CreatedAtRoute("GetSaleProductById", new { id = disPer.discountId }, disPer);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteSaleProduct(int id)
        {
            DiscountPercent disPer = this._db.discountPercents.Find(id);
            if (disPer == null)
                return NotFound("Could not be found!!!");

            disPer.isActive = false;
            this._db.SaveChanges();

            return Ok(disPer);
        }

    }
}