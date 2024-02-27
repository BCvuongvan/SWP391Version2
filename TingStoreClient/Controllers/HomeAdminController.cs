using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TingStoreClient.Models;

namespace TingStoreClient.Controllers
{
    public class HomeAdminController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly HttpClient client = null;

        private string orderApi;

        public HomeAdminController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            this.orderApi = "https://localhost:5001/api/Order";
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var viewModel = new TotalInforViewModel
                {

                    TotalRevenue = await GetTotalRevenue(),
                };

                 // Trả về view với dữ liệu viewModel
            
            int countOrder = 0;
            HttpResponseMessage response = await client.GetAsync(orderApi);
            string data = await response.Content.ReadAsStringAsync();

            // var option = new JsonSerializerOptions{PropertyNameCaseInsensitive = true};
            var options = new JsonSerializerOptions()
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString |
    JsonNumberHandling.WriteAsString
            };
            List<Order> listOrder = JsonSerializer.Deserialize<List<Order>>(data, options);
            foreach (var item in listOrder)
            {
                if (item.orderStatusId == 1)
                {
                    countOrder++;
                }
            }
            ViewBag.countOrder = countOrder;
            return View();
        //    return View(viewModel);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi tại đây
                Console.WriteLine($"Error in TotalInfor: {ex.Message}");
                return BadRequest(new { Message = "Error while fetching total information.", Error = ex.Message });
            }
        }
        private async Task<decimal> GetTotalRevenue()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://localhost:5001/api/Total/GetTotalRevenue");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);
            ViewBag.content = content;
            return System.Text.Json.JsonSerializer.Deserialize<decimal>(content);
        }
    }
}