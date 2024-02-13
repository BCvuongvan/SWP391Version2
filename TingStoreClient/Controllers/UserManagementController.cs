using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TingStoreClient.Models;

namespace TingStoreClient.Controllers
{

    public class UserManagementController : Controller
    {
        private readonly HttpClient client = null;
        private string userApi;
        public UserManagementController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            this.userApi = "https://localhost:5001/api/UserManager";
        }

        public async Task<ActionResult> Index()
        {
            HttpResponseMessage response = await client.GetAsync(userApi);
            String data = await response.Content.ReadAsStringAsync();

            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var UserList = JsonSerializer.Deserialize<List<User>>(data, option);
            return View(UserList);
        }


    }
}