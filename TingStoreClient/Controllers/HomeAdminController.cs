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
using TingStoreClient.Filters;

namespace TingStoreClient.Controllers
{
    [StaffAuthenticationRedirect]
    public class HomeAdminController : Controller
    {

        private readonly HttpClient client = null;

        private string orderApi;

        public HomeAdminController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            this.orderApi = "https://localhost:5001/api/Order";
        }

        public async Task<IActionResult> Index()
        {
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
        }
    }
}