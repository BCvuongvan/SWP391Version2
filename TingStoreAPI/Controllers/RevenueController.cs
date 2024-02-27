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
    [Route("api/[controller]")]
    [ApiController]
    public class RevenueController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public RevenueController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/revenue/day?dateString=2024-02-14
        [HttpGet("day")]
        public async Task<ActionResult<decimal>> GetDailyRevenue([FromQuery] string dateString)
        {
            if (!DateTime.TryParse(dateString, out DateTime date))
            {
                return BadRequest("Invalid date format. Please provide date in yyyy-MM-dd format.");
            }

            var dailyRevenue = await _context.orders
                .Where(o => o.orderDate.Date == date.Date)
                .SumAsync(o => o.TotalAmount);

            return Ok(dailyRevenue);
        }

        // GET: api/revenue/month?year=2024&month=2
        [HttpGet("month")]
        public async Task<ActionResult<decimal>> GetMonthlyRevenue([FromQuery] int year, [FromQuery] int month)
        {
            var monthlyRevenue = await _context.orders
                .Where(o => o.orderDate.Year == year && o.orderDate.Month == month)
                .SumAsync(o => o.TotalAmount);

            return Ok(monthlyRevenue);
        }

        // GET: api/revenue/year?year=2024
        [HttpGet("year")]
        public async Task<ActionResult<decimal>> GetYearlyRevenue([FromQuery] int year)
        {
            var yearlyRevenue = await _context.orders
                .Where(o => o.orderDate.Year == year)
                .SumAsync(o => o.TotalAmount);

            return Ok(yearlyRevenue);
        }

        // GET: api/revenue/yearly-revenue/{year}
        [HttpGet("yearly-revenue/{year}")]
        public async Task<ActionResult<Dictionary<int, decimal>>> GetYearlyRevenues(int year)
        {
            var yearlyRevenues = new Dictionary<int, decimal>();

            for (int month = 1; month <= 12; month++)
            {
                var monthlyRevenue = await GetMonthlyRevenues(year, month); // Gọi hàm GetMonthlyRevenue cho mỗi tháng trong năm
                yearlyRevenues.Add(month, monthlyRevenue);
            }

            return Ok(yearlyRevenues);
        }
        private async Task<decimal> GetMonthlyRevenues(int year, int month)
        {
            var monthlyRevenue = await _context.orders
                .Where(o => o.orderDate.Year == year && o.orderDate.Month == month)
                .SumAsync(o => o.TotalAmount);

            return monthlyRevenue;
        }
        // GET: api/statistics/orders-count/day?date=2024-02-25
        [HttpGet("orders-count/day")]
        public async Task<ActionResult<int>> GetOrdersCountByDay(DateTime date)
        {
            var ordersCount = await _context.orders
                .Where(o => o.orderDate.Date == date.Date)
                .CountAsync();

            return ordersCount;
        }

        // GET: api/statistics/orders-count/week?year=2024&month=2&day=25
        [HttpGet("orders-count/week")]
        public async Task<ActionResult<int>> GetOrdersCountByWeek(int year, int month, int day)
        {
            DateTime date = new DateTime(year, month, day);
            var firstDayOfWeek = date.AddDays(-(int)date.DayOfWeek);
            var lastDayOfWeek = firstDayOfWeek.AddDays(6);

            var ordersCount = await _context.orders
                .Where(o => o.orderDate.Date >= firstDayOfWeek.Date && o.orderDate.Date <= lastDayOfWeek.Date)
                .CountAsync();

            return ordersCount;
        }

        // GET: api/statistics/orders-count/month?year=2024&month=2
        [HttpGet("orders-count/month")]
        public async Task<ActionResult<int>> GetOrdersCountByMonth(int year, int month)
        {
            var ordersCount = await _context.orders
                .Where(o => o.orderDate.Year == year && o.orderDate.Month == month)
                .CountAsync();

            return ordersCount;
        }

        // GET: api/statistics/orders-count/year?year=2024
        [HttpGet("orders-count/year")]
        public async Task<ActionResult<int>> GetOrdersCountByYear(int year)
        {
            var ordersCount = await _context.orders
                .Where(o => o.orderDate.Year == year)
                .CountAsync();

            return ordersCount;
        }


        // GET: api/statistics/discounts
        [HttpGet("discounts")]
        public async Task<ActionResult<List<DiscountPercent>>> GetDiscountsWithExpiryDate()
        {
            var discounts = await _context.discountPercents
                .Where(d => d.endDate >= DateTime.Today)
                .ToListAsync();

            return discounts;
        }
        [HttpGet("yearly-orders/{year}")]
        public async Task<ActionResult<Dictionary<int, int>>> GetYearlyOrders(int year)
        {
            var yearlyOrders = new Dictionary<int, int>();

            for (int month = 1; month <= 12; month++)
            {
                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                var ordersCount = await _context.orders
                    .Where(o => o.orderDate >= startDate && o.orderDate <= endDate)
                    .CountAsync();

                yearlyOrders.Add(month, ordersCount);
            }

            return yearlyOrders;
        }


    }
}
