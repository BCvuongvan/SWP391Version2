using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using TingStoreClient.Models;

namespace TingStoreClient.Controllers
{
    public class RevenueController : Controller
    {
        private readonly HttpClient _httpClient;

        public RevenueController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:5001/api/"); // Cập nhật địa chỉ cơ sở của API
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var todayDateString = DateTime.Today.ToString("yyyy-MM-dd");
                var dailyRevenue = await GetDailyRevenue(todayDateString);
                var monthlyRevenue = await GetMonthlyRevenue(DateTime.Today.Year, DateTime.Today.Month);
                var yearlyRevenue = await GetYearlyRevenue(DateTime.Today.Year);
                var totalRevenue = await CalculateTotalRevenue();
                var moreMonthlyRevenues = await GetMoreMonthlyRevenues(DateTime.Today.Year);
                var moreYear = await GetYearlyRevenues(DateTime.Today.Year);

                // Gán giá trị vào ViewBag
                ViewBag.DailyRevenue = dailyRevenue;
                ViewBag.MonthlyRevenue = monthlyRevenue;
                ViewBag.YearlyRevenue = yearlyRevenue;
                ViewBag.CalculateTotalRevenue = totalRevenue;
                ViewBag.MonthlyRevenuesForYear = moreMonthlyRevenues;
                ViewBag.MoreYear = moreYear;
                return View();
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu cần thiết
                return BadRequest(ex.Message);
            }
        }

        // Trong phương thức GetDailyRevenue của RevenueController ở phía Client
        private async Task<decimal> GetDailyRevenue(string dateString)
        {
            var response = await _httpClient.GetAsync($"Revenue/day?dateString={dateString}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to retrieve daily revenue.");
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<decimal>(content);
        }

        private async Task<decimal> GetMonthlyRevenue(int year, int month)
        {
            var response = await _httpClient.GetAsync($"revenue/month?year={year}&month={month}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to retrieve monthly revenue.");
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<decimal>(content);
        }

        private async Task<decimal> GetYearlyRevenue(int year)
        {
            var response = await _httpClient.GetAsync($"revenue/year?year={year}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to retrieve yearly revenue.");
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<decimal>(content);
        }

        public async Task<List<decimal>> GetMoreMonthlyRevenues(int year)
        {
            List<decimal> yearlyRevenues = new List<decimal>();

            for (int month = 1; month <= 12; month++)
            {
                var monthlyRevenue = await GetMonthlyRevenue(year, month); // Gọi hàm GetMonthlyRevenue
                yearlyRevenues.Add(monthlyRevenue);
            }

            return yearlyRevenues;
        }

        public async Task<IActionResult> GetYearlyRevenues(int year)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Revenue/yearly-revenue/{year}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var content = JsonConvert.DeserializeObject<Dictionary<int, decimal>>(jsonString);

                    // Tạo một đối tượng mới để chứa thông tin và trả về nó
                    var result = new
                    {
                        Year = year,
                        MonthlyRevenues = content
                    };

                    return Ok(result);
                }
                else
                {
                    return StatusCode((int)response.StatusCode, response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task<decimal> CalculateTotalRevenue()
        {
            decimal totalRevenue = 0;

            // Lấy ngày hiện tại
            var today = DateTime.Today;

            // Lấy tổng doanh thu hàng năm cho từng năm trước đó và cộng vào tổng
            for (int i = 1; i <= today.Year; i++)
            {
                var yearlyRevenue = await GetYearlyRevenue(i);
                totalRevenue += yearlyRevenue;
            }

            return totalRevenue;
        }

        public async Task<int> GetOrdersCountByDay(DateTime date)
        {
            var response = await _httpClient.GetAsync($"orders-count/day?date={date:yyyy-MM-dd}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to retrieve orders count by day.");
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<int>(content);
        }

        public async Task<int> GetOrdersCountByMonth(int year, int month)
        {
            var response = await _httpClient.GetAsync($"orders-count/month?year={year}&month={month}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to retrieve orders count by month.");
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<int>(content);
        }

        public async Task<int> GetOrdersCountByYear(int year)
        {
            var response = await _httpClient.GetAsync($"orders-count/year?year={year}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to retrieve orders count by year.");
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<int>(content);
        }

        public async Task<List<DiscountPercent>> GetDiscountsWithExpiryDate()
        {
            var response = await _httpClient.GetAsync("discounts");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to retrieve discounts with expiry date.");
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<DiscountPercent>>(content);
        }
        public async Task<IActionResult> GetYearlyOrders(int year)
        {
            try
            {
                var response = await _httpClient.GetAsync($"order/yearly-orders/{year}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var yearlyOrders = JsonConvert.DeserializeObject<Dictionary<int, int>>(jsonString);

                    // Process the yearlyOrders as needed, such as drawing a chart

                    return Ok(yearlyOrders);
                }
                else
                {
                    return StatusCode((int)response.StatusCode, response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
