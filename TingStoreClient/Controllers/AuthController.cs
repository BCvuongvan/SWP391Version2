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
        private string ResetPassapi;

        public AuthController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            this.api = "https://localhost:5001/api/Auth";
            this.ResetPassapi = "https://localhost:5001/api/ForgotPassword";
        }

        public IActionResult Login()
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
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.errorMessage = ("User doesn't exit");
                return View("Login");
            }
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
            if (user.userType == 1 || user.userType == 2)
            {
                HttpContext.Session.SetString(_user, data);
                return RedirectToAction("Index", "HomeAdmin");
            }
            else if (user.userType == 3)
            {
                HttpContext.Session.SetString(_user, data);
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

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult SignUp()
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

        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string username)
        {
            if (username == null)
            {
                TempData["SystemNotificationError"] = "Invalid Input, Please try again!";
                return View("ForgotPassword");
            }
            HttpResponseMessage response = await client.GetAsync(api);
            string data = await response.Content.ReadAsStringAsync();

            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var Listuser = JsonSerializer.Deserialize<List<User>>(data, option);
            User user = null;
            foreach (var item in Listuser)
            {
                if (item.userName.Equals(username))
                {
                    user = item;
                }
            }

            // User user = list.FirstOrDefault(u => u.userName.Equals(username) && u.password.Equals(password));
            if (user == null)
            {
                TempData["SystemNotificationError"] = "Your username does not exist";
                return View("ForgotPassword");
            }
            System.Console.WriteLine(user.email);
            response = await client.GetAsync(ResetPassapi + "/SendMail/" + user.email);
            if (response.IsSuccessStatusCode)
            {
                const string _user = "username";
                // string userDataJson = JsonConvert.SerializeObject(user);
                HttpContext.Session.SetString(_user, user.userName);

                var dataCode = response.Content.ReadAsStringAsync().Result;
                var requestCode = JsonSerializer.Deserialize<string>(dataCode);
                TempData["RequestCode"] = requestCode;
            }
            return View("ConfirmEmail");
        }

        private IActionResult ConfirmEmail()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(string code)
        {
            string requestCode = TempData["RequestCode"] as string;
            if (requestCode.Equals(code))
            {
                return View("ResetPassword");
            }
            return RedirectToAction("Login");
        }

        private IActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string pass)
        {
            string username = HttpContext.Session.GetString("username");

            HttpResponseMessage response = await client.GetAsync(ResetPassapi + "/ResetPass/" + username + "/" + pass);
            if (response.IsSuccessStatusCode)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }
            HttpContext.Session.Clear();
            return View();
        }
    }
}