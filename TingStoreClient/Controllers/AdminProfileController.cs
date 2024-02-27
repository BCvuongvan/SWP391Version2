using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TingStoreClient.Models;

namespace TingStoreClient.Controllers
{
    public class AdminProfileController : Controller
    {
        private readonly HttpClient client = null;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private string api;
        public AdminProfileController(IWebHostEnvironment hostEnvironment)
        {
            _hostingEnvironment = hostEnvironment;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            this.api = "https://localhost:5001/api/AdminProfile";
        }

        private async Task<User> GetUserDetailsAsync(string id)
        {
            HttpResponseMessage response = await client.GetAsync(api + "/" + id);
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    string data = await response.Content.ReadAsStringAsync();
                    var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<User>(data, option);
                }
                catch (JsonException ex)
                {
                    // Log the exception or handle it gracefully
                    Console.WriteLine($"Error deserializing JSON: {ex.Message}");
                    return null;
                }
            }
            else
            {
                // Handle error if needed
                return null;
            }
        }

        public async Task<IActionResult> Index()
        {
            string id = HttpContext.Session.GetString("_user");
            var user1 = JsonSerializer.Deserialize<User>(id);
            User user = await GetUserDetailsAsync(user1.userName);
            if (user != null)
            {
                return View(user);
            }
            return NotFound();
        }

    }
}