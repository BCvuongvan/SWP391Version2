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
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDBContext _db; // định nghĩa đối tượng của lớp ApplicationDBContext

        public ProductController(ApplicationDBContext db)
        { //định nghĩa constructor and instruction cái _db vào
            this._db = db;
        }

        [HttpGet]
        public IActionResult GetAllProduct()
        {
            var productList = _db.products.Include(p => p.category).Include(p => p.discountPercents).ToList();
            return Ok(productList);
        }
        [HttpGet("{id}", Name = "GetProductById")]
        public IActionResult GetProductById(int id)
        {
            var pro = this._db.products.Include(u => u.feedbacks).ThenInclude(f => f.user).Include(p => p.productImages).FirstOrDefault(p => p.proId == id);
            if (pro == null)
            {
                return NotFound();
            }
            return Ok(pro);
        }

        [HttpPost]
        public IActionResult AddProduct(Product product)
        {
            if (product == null)
            {
                return BadRequest("Product cannot be null.");
            }
            var existedName = this._db.products.FirstOrDefault(p => p.proName == product.proName); //check từ phần tử đầu tiên với tên đầu vào 
            if (existedName != null)
            {
                return BadRequest("Name product already exists.");
            }
            product.proStatus = true;
            this._db.products.Add(new Product(product.proName, product.proDescription, product.proPrice, product.proQuantity, product.proImage, product.cateId, product.proStatus));
            this._db.SaveChanges();
            return CreatedAtRoute("GetProductById", new { id = product.proId }, product);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, Product product)
        {
            if (product == null)
            {
                return BadRequest("Product cannot be null");
            }
            Product pro = this._db.products.AsNoTracking().FirstOrDefault(p => p.proId == id);
            if (pro == null)
            {
                return NotFound("Could not be found!!!");
            }
            product.proStatus = true;
            this._db.products.Update(new Product(product.proId, product.proName, product.proDescription, product.proPrice, product.proQuantity, product.proImage, product.cateId, product.proStatus));
            this._db.SaveChanges();
            return CreatedAtRoute("GetProductById", new { id = product.proId }, product);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            Product pro = this._db.products.Find(id);
            if (pro == null)
                return NotFound("Could not be found!!!");

            pro.proStatus = false;
            this._db.SaveChanges();

            return Ok(pro);
        }

        [HttpGet("ProductsByCategoryID/{categoryId}")]
        public ActionResult<IEnumerable<Product>> GetProductsByCategory(int categoryId)
        {
            var products = _db.products.Where(p => p.cateId == categoryId).ToList();
            var productList = _db.products.Include(p => p.category).ToList();
            if (products == null)
            {
                return NotFound();
            }
            return Ok(products);
        }

        [HttpGet("ImagesByProductID/{productId}", Name = "GetProductsByCategory")]
        public ActionResult<IEnumerable<ProductImage>> GetImagesByProductId(int productId)
        {
            var images = _db.productImages.Where(p => p.proId == productId).ToList();
            if (images == null)
            {
                return NotFound();
            }
            return Ok(images);
        }
        [HttpPost("addImages")]
        public IActionResult AddImages(ProductImage productImage)
        {
            if (productImage == null)
            {
                return BadRequest("Product cannot be null");
            }
            productImage.imageStatus = true;
            this._db.productImages.Add(new ProductImage(productImage.proId, productImage.imageUrl, productImage.imageStatus));
            this._db.SaveChanges();
            return CreatedAtRoute("GetProductById", new { id = productImage.proId }, productImage);
        }

        [HttpGet("Picture/{id}", Name = "GetPictureById")]
        public IActionResult GetPicturetById(int id)
        {
            ProductImage proImg = this._db.productImages.Include(p => p.product).FirstOrDefault(p => p.imageId == id);
            if (proImg == null)
            {
                return NotFound();
            }
            return Ok(proImg);
        }

        [HttpDelete("DeleteImageProduct/{imageId}")]
        public IActionResult DeleteImageProduct(int imageId)
        {
            ProductImage proImg = this._db.productImages.Include(p => p.product).FirstOrDefault(p => p.imageId == imageId);
            if (proImg == null)
                return NotFound("Could not be found!!!");
            proImg.imageStatus = false;
            this._db.SaveChanges();

            return Ok(proImg);
        }

        [HttpGet("Hot")]
        public ActionResult<IEnumerable<Product>> GetHotProducts()
        {
            var hotProducts = this._db.orderDetails.GroupBy(od => od.proId).Select(g => new { proId = g.Key, TotalQuantity = g.Sum(od => od.quantity) }).OrderByDescending(g => g.TotalQuantity).Take(4).Join(this._db.products, g => g.proId, p => p.proId, (g, p) => p).ToList();
            if (hotProducts == null)
            {
                return NotFound();
            }
            return hotProducts;
        }
    }
}