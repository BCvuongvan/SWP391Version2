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
    public class DiscountProductController : ControllerBase
    {
        private readonly ApplicationDBContext _db; // định nghĩa đối tượng của lớp ApplicationDBContext

        public DiscountProductController(ApplicationDBContext db)
        { //định nghĩa constructor and instruction cái _db vào
            this._db = db;
        }
        [HttpGet]
        public IActionResult GetAllDiscount()
        {
            var discountList = _db.discountPercents.Include(p => p.product).ToList();
            return Ok(discountList);
        }
        [HttpGet("active")]
        public IActionResult GetAllDiscounActiveTrue()
        {
            var discountList = _db.discountPercents.Include(p => p.product).Where(d => d.isActive == true).ToList();
            return Ok(discountList);
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
            this._db.discountPercents.Update(new DiscountPercent(disPer.discountId, disPer.proId, disPer.discountPercentage, disPer.discountImage, disPer.isActive));
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

        [HttpGet("discountProduct/Latest")]
        public async Task<IActionResult> GetLatestDiscountEndTime()
        {
            var latestDiscount = await this._db.discountPercents
                                        .OrderBy(d => d.endDate).Where(d => d.isActive == true && d.discountPercentage >= 40)
                                        .FirstOrDefaultAsync();
            if (latestDiscount == null)
            {
                return NotFound();
            }

            return Ok(latestDiscount.endDate);
        }
    }
}