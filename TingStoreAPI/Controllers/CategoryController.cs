using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
            var categoryList = this._db.categories.ToList();
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
    }
}