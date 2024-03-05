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

        // [HttpGet("ProductsByCategoryName/{categoryName}")]
        // public ActionResult<IEnumerable<object>> GetProductsByCategory(string categoryName)
        // {
        //     // Retrieve products along with their categories where the category name matches the provided value
        //     var products = _db.products
        //                        .Where(p => p.category.cateName == categoryName)
        //                        .Select(p => new
        //                        {
        //                            p.proId,
        //                            p.proName,
        //                            p.proDescription,
        //                            p.proPrice,
        //                            p.proQuantity,
        //                            p.proImage,
        //                            p.proStatus,
        //                            CategoryName = p.category.cateName // Include the category name in the response
        //                        })
        //                        .ToList();

        //     if (products == null || !products.Any())
        //     {
        //         return NotFound();
        //     }

        //     return Ok(products);
        // }



        [HttpGet("{cateName}")]
        public IActionResult GetOrdersFiltered(string cateName)
        {
            IQueryable<Product> query = this._db.products
            .Include(u => u.category);


            if (!string.IsNullOrEmpty(cateName))
            {
                query = query.Where(o => o.category.cateName.Contains(cateName.Trim().ToLower()));
            }
            var productsList = query.ToList();
            return Ok(productsList);

        }



    }
}
