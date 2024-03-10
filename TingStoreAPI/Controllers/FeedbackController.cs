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
    public class FeedbackController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public FeedbackController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet("products-bought/{userName}")]
        public async Task<ActionResult<IEnumerable<object>>> GetProductsBoughtByUser(string userName)
        {
            var products = await _context.orders.Where(o => o.userName == userName).SelectMany(o => o.orderDetails.Select(od => new
            {
                ProId = od.product.proId,
                ProName = od.product.proName,
                ProImage = od.product.proImage
            }))
            .Distinct()
            .ToListAsync();

            if (products == null || products.Count == 0)
            {
                return NotFound("Not founds product for user");
            }
            return Ok(products);
        }

        [HttpPost("AddFeedback")]
        public IActionResult AddFeedback([FromBody] Feedback feedback)
        {
            if (feedback == null)
            {
                return BadRequest("Feedback not null");
            }
            if (feedback.proId == 0 || string.IsNullOrEmpty(feedback.userName))
            {
                return BadRequest("proId or userName illegal");
            }

            var product = this._context.products.FirstOrDefault(p => p.proId == feedback.proId);
            if (product == null)
            {
                return BadRequest("proId illegal");
            }

            var user = this._context.users.FirstOrDefault(u => u.userName == feedback.userName);
            if (user == null)
            {
                return BadRequest("userName illegal");
            }
            var existingFeedback = this._context.feedbacks.FirstOrDefault(f => f.userName.Equals(feedback.userName) && f.proId == feedback.proId);
            if (existingFeedback != null)
            {
                existingFeedback.comment = feedback.comment;
                existingFeedback.rating = feedback.rating;
            }
            else
            {
                this._context.feedbacks.Add(feedback);
            }
            this._context.SaveChanges();
            return Ok(feedback);

        }

    }
}
