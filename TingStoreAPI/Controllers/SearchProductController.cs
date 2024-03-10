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
    public class SearchProductController : ControllerBase
    {
        private readonly ApplicationDBContext _db; // định nghĩa đối tượng của lớp ApplicationDBContext

        public SearchProductController(ApplicationDBContext db)
        {
            this._db = db;
        }

        [HttpGet]
        public IActionResult GetListProduct()
        {
            var listProduct = this._db.products.ToList();
            return Ok(listProduct);
        }
        
        [HttpGet("{IProductName}")]
        public IActionResult GetOrdersFiltered(string IProductName)
        {            
            var listProduct = this._db.products.Include(p => p.category).Include(p => p.discountPercents).Where(p => p.proName.ToLower().StartsWith(IProductName.ToLower()));
            if(!listProduct.Any()){
                return BadRequest("Can't found this product "+ IProductName +"in stock"); 
            }
            return Ok(listProduct);
        }



    }
}
