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

        // [HttpGet("products-bought/{userName}")]
        // public async Task<ActionResult<IEnumerable<object>>> GetProductsBoughtByUser(string userName)
        // {
        //     var products = await _context.orders.Where(o => o.userName == userName).SelectMany(o => o.orderDetails.Select(od => new
        //     {
        //         ProId = od.product.proId,
        //         ProName = od.product.proName,
        //         ProImage = od.product.proImage
        //     }))
        //     .Distinct()
        //     .ToListAsync();

        //     if (products == null || products.Count == 0)
        //     {
        //         return NotFound("Not founds product for user");
        //     }
        //     return Ok(products);
        // }

        // [HttpPost("AddFeedback")]
        // public IActionResult AddFeedback([FromBody] Feedback feedback)
        // {
        //     if (feedback == null)
        //     {
        //         return BadRequest("Feedback not null");
        //     }
        //     if (feedback.proId == 0 || string.IsNullOrEmpty(feedback.userName))
        //     {
        //         return BadRequest("proId or userName illegal");
        //     }

        //     var product = this._context.products.FirstOrDefault(p => p.proId == feedback.proId);
        //     if (product == null)
        //     {
        //         return BadRequest("proId illegal");
        //     }

        //     var user = this._context.users.FirstOrDefault(u => u.userName == feedback.userName);
        //     if (user == null)
        //     {
        //         return BadRequest("userName illegal");
        //     }
        //     var existingFeedback = this._context.feedbacks.FirstOrDefault(f => f.userName.Equals(feedback.userName) && f.proId == feedback.proId);
        //     if (existingFeedback != null)
        //     {
        //         existingFeedback.comment = feedback.comment;
        //         existingFeedback.rating = feedback.rating;
        //     }
        //     else
        //     {
        //         this._context.feedbacks.Add(feedback);
        //     }
        //     this._context.SaveChanges();
        //     return Ok(feedback);

        // }

        [HttpGet("{username}")]
        public IActionResult getProductByUsername(string username)
        {
            var User = this._context.users.Include(u => u.orders).ThenInclude(u => u.orderDetails).ThenInclude(u => u.product).FirstOrDefault(u => u.userName.Equals(username));
            return Ok(User);
        }
        [HttpGet("GetFeedbackByProId/{Iusername}/{IproId}")]
        public IActionResult GetFeedbackByProId(string Iusername, int IproId)
        {
            var feedback = this._context.feedbacks.Include(u => u.product).FirstOrDefault(u => u.userName.Equals(Iusername) && u.proId == IproId);
            if(feedback == null){
                return NotFound();
            }
            return Ok(feedback);

        }

        [HttpPost]
        public IActionResult editOrUpdate(Feedback feedback)
        {
            var findFeedback = this._context.feedbacks.FirstOrDefault(o => o.userName.Equals(feedback.userName) && o.proId == feedback.proId);
            if (findFeedback == null)
            {
                this._context.feedbacks.Add(new Feedback(feedback.userName, feedback.proId, feedback.comment, feedback.rating));
                this._context.SaveChanges();
                return Ok("Add");
            }
            else
            {
                findFeedback.comment = feedback.comment;
                findFeedback.rating = feedback.rating;
                this._context.SaveChanges();
                return Ok("edit");
            }
        }



    }
}
