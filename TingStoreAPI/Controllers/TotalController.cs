using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TingStoreAPI.DB;
using TingStoreAPI.Models;

namespace TingStoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TotalController : Controller
    {
        private readonly ApplicationDBContext _dbContext;
        public TotalController(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("GetTotalProducts")]
        public async Task<IActionResult> GetTotalProducts()
        {
            try
            {
                // Xài LINQ để lấy tổng số product
                int totalProducts = await _dbContext.products.CountAsync();

                return Ok(totalProducts);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("GetTotalOrders")]
        public async Task<IActionResult> GetTotalOrders()
        {
            try
            {
                // LINQ để lấy tổng orders đã xác nhận
                int confirmedOrders = await _dbContext.orders
                    .CountAsync(o => o.orderStatusId == 2); // Ví dụ set orderStatusId = 2 là đã xác nhận

                return Ok(confirmedOrders);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("GetTotalRevenue")]
        public async Task<decimal> TotalRevenue()
        {
            // Lấy tổng số tiền của các đơn hàng đã được xác nhận
            decimal totalRevenue = await _dbContext.orders
                .Where(o => o.orderStatusId == 2) // 2 là confirm
                .SumAsync(o => o.TotalAmount);

            return totalRevenue;
        }

        [HttpGet("GetTotalUser")]
        public async Task<IActionResult> GetTotalUser()
        {
            try
            {
                // Tổng số lượng user
                var totalUsers = await _dbContext.users.CountAsync();

                // Tổng số lượng user bị ban
                var bannedUsers = await _dbContext.users.CountAsync(u => u.userType == 0);

                // Tổng số lượng user chưa bị ban
                var activeUsers = totalUsers - bannedUsers;

                // Trả về một danh sách JSON chứa các số liệu
                return Ok(new List<int> { totalUsers, bannedUsers, activeUsers });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về thông báo lỗi
                return BadRequest(new { Message = "Error while fetching user information.", Error = ex.Message });
            }
        }
        [HttpGet("GetTotalUsersRegisteredBetweenDates")]
        public async Task<IActionResult> GetTotalUsersRegisteredBetweenDates(DateTime startDate, DateTime endDate)
        {
            try
            {
                var totalUsersRegistered = await _dbContext.users
                    .Where(u => u.createdAt >= startDate && u.createdAt <= endDate)
                    .CountAsync();

                return Ok(totalUsersRegistered);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}