using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TingStoreClient.Models;
using Newtonsoft.Json;

namespace TingStoreClient.Controllers
{
    [Route("[controller]")]
    public class TotalInformationController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public TotalInformationController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IActionResult> TotalInfor()
        {
            try
            {
                var viewModel = new TotalInforViewModel
                {
                    TotalProducts = await GetTotalProducts(),
                    TotalOrders = await GetTotalOrders(),
                    TotalRevenue = await GetTotalRevenue(),
                    TotalUser = await GetTotalUser()
                };

                return View(viewModel); // Trả về view với dữ liệu viewModel
            }
            catch (Exception ex)
            {
                // Xử lý lỗi tại đây
                Console.WriteLine($"Error in TotalInfor: {ex.Message}");
                return BadRequest(new { Message = "Error while fetching total information.", Error = ex.Message });
            }
        }


        private async Task<int> GetTotalProducts()
        {
            try
            {
                using var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync("https://localhost:5001/api/Total/GetTotalProducts");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(content);
                // Sử dụng TryParse để kiểm tra xem giá trị từ JSON có phải là số nguyên hay không
                if (int.TryParse(content, out int totalProducts))
                {
                    Console.WriteLine("gia tri content", content);
                    return totalProducts;

                }
                else
                {
                    Console.WriteLine("Error: JSON value is not a valid integer");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetTotalProducts: {ex.Message}");
                return 0;
            }
        }

        private async Task<int> GetTotalOrders()
        {
            try
            {
                using var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync("https://localhost:5001/api/Total/GetTotalOrders");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();

                if (int.TryParse(content, out int confirmedOrders))
                {
                    return confirmedOrders;
                }
                else
                {
                    Console.WriteLine("Error: JSON value is not a valid integer");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetTotalProducts: {ex.Message}");
                return 0;
            }
        }
        private async Task<decimal> GetTotalRevenue()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://localhost:5001/api/Total/GetTotalRevenue");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);
            return System.Text.Json.JsonSerializer.Deserialize<decimal>(content);
        }

        [HttpGet("TotalUser")]
        public async Task<List<int>> GetTotalUser()
        {
            try
            {
                using var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync("https://localhost:5001/api/Total/GetTotalUser");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();

                // Chuyển đổi đối tượng JSON thành danh sách số nguyên
                var numberList = JsonConvert.DeserializeObject<List<int>>(content);

                return numberList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetTotalUsers: {ex.Message}");
                return new List<int>(); // Hoặc trả về một danh sách rỗng tùy thuộc vào yêu cầu của bạn
            }
        }
        private async Task<int> GetTotalUsersRegisteredBetweenDates(DateTime startDate, DateTime endDate)
        {
            try
            {
                using var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"https://localhost:5001/api/Total/GetTotalUsersRegisteredBetweenDates?startDate={startDate}&endDate={endDate}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                if (int.TryParse(content, out int totalUsersRegistered))
                {
                    return totalUsersRegistered;
                }
                else
                {
                    Console.WriteLine("Error: JSON value is not a valid integer");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetTotalUsersRegisteredBetweenDates: {ex.Message}");
                return 0;
            }
        }


    }
}