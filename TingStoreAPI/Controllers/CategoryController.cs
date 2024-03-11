using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TingStoreAPI.DB;
using TingStoreAPI.Models;

namespace TingStoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDBContext _db; // định nghĩa đối tượng của lớp ApplicationDBContext

        public CategoryController(ApplicationDBContext db)
        { //định nghĩa constructor and instruction cái _db vào
            this._db = db;
        }
        [HttpGet]
        public IActionResult GetAllCategory()
        {
            // var categoryList = this._db.categories.ToList();
            // var categoryList = from c in this._db.categories join p 
            // in this._db.products on c.cateId equals p.cateId 
            // select new {c.cateId, c.cateName, c.cateDescribe, numofpro = p.proQuantity};
            var categoryList = this._db.categories
            .Include(c => c.Products)  // Include related products
            .ToList();
            return Ok(categoryList);
        }
        [HttpGet("{cateId}", Name = "GetCategoryById")]
        public IActionResult GetCategoryById(int cateId)
        {
            Category cate = this._db.categories.Find(cateId);
            if (cate == null)
            {
                return NotFound();
            }
            return Ok(cate);
        }



        [HttpPost]
        public IActionResult AddCategory(Category category)
        {
            if (category == null)
            {
                return BadRequest("Category cannot be null");
            }
            var existedName = this._db.categories.FirstOrDefault(c => c.cateName == category.cateName); //check từ phần tử đầu tiên với tên đầu vào 
            if (existedName != null)
            {
                return BadRequest("Name category already exists.");
            }
            category.cateStatus = true;
            this._db.categories.Add(new Category(category.cateName, category.cateDescribe));
            this._db.SaveChanges();
            return CreatedAtRoute("GetCategoryById", new { cateId = category.cateId }, category);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCategory(int id, Category category)
        {
            if (category == null)
            {
                return BadRequest("Category cannot be null");
            }

            var existingCategory = this._db.categories.Find(id);
            if (existingCategory == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }
            category.cateStatus = true;
            existingCategory.cateName = category.cateName;
            existingCategory.cateDescribe = category.cateDescribe;
            existingCategory.cateStatus = category.cateStatus; 

            this._db.SaveChanges();

            return Ok(existingCategory); 
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Category cate = this._db.categories.Find(id);
            if (cate == null)
                return NotFound("Could not be found!!!");
            cate.cateStatus = false;
            this._db.SaveChanges();
            return Ok(cate);
        }

        [HttpGet("ProductsByCategoryID/{categoryId}")]
        public ActionResult<IEnumerable<Product>> GetCategoryByProduct(int categoryId)
        {
            var products = _db.products.Where(p => p.cateId == categoryId).ToList();
            var productList = _db.products.Include(p => p.category).ToList();
            if (products == null)
            {
                return NotFound();
            }
            return Ok(products);
        }


    }
}