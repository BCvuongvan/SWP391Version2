using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
// using Newtonsoft.Json;
using TingStoreClient.Models;

namespace TingStoreClient.Controllers
{
    public class AuthController : Controller
    {
        private readonly HttpClient client = null;
        private string api;

        public AuthController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            this.api = "https://localhost:5001/api/Auth";
        }

        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (username == null || password == null)
            {
                ViewBag.errorMessage = ("User doesn't enter");
                return View("Login");
            }
            HttpResponseMessage response = await client.GetAsync(api + "/GetUser/" + username + "/" + password);
            string data = await response.Content.ReadAsStringAsync();

            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var user = JsonSerializer.Deserialize<User>(data, option);

            // User user = list.FirstOrDefault(u => u.userName.Equals(username) && u.password.Equals(password));
            if (user == null)
            {
                ViewBag.errorMessage = ("User doesn't exit");
                return View("Login");
            }
            const string _user = "_user";
            // string userDataJson = JsonConvert.SerializeObject(user);
            HttpContext.Session.SetString(_user, data);
            if (user.userType == 1 || user.userType == 2)
            {
                return RedirectToAction("Index", "HomeAdmin");
            }
            else if (user.userType == 3)
            {
                return RedirectToAction("Index", "Home");
            }
            else if (user.userType == 0)
            {
                ViewBag.errorMessage = ("Your Account is ban");
                return View("Login");
            }
            else
            {
                ViewBag.errorMessage = ("Your Account does not exit");
                return View("Login");
            }

        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> SignUp()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> SignUp(User user)
        {
            user.fullName = user.userName;
            user.address = "none";
            user.picture = "l60Hf-150x150.png";
            user.createdAt = DateTime.Now;
            user.updateAt = DateTime.Now;
            user.userType = 3;
            if (ModelState.IsValid)
            {
                String data = JsonSerializer.Serialize(user);
                var content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(api, content);

                return RedirectToAction("Login");

            }
            return View(user);
        }







    }
}