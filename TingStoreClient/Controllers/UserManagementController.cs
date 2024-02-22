using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TingStoreClient.Models;

namespace TingStoreClient.Controllers
{

    public class UserManagementController : Controller
    {
        private readonly HttpClient client = null;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private string userApi;
        public UserManagementController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            this.userApi = "https://localhost:5001/api/UserManager";
        }

        public async Task<ActionResult> Index(int id)
        {
            switch (id)
            {
                case -1:
                    ViewBag.nametable = "Delete account";
                    ViewBag.userType = -1;
                    break;
                case 0:
                    ViewBag.nametable = "Ban account";
                    ViewBag.userType = 0;
                    break;
                case 1:
                    ViewBag.nametable = "Admin account";
                    ViewBag.userType = 1;
                    break;
                case 2:
                    ViewBag.nametable = "Staff account";
                    ViewBag.userType = 2;
                    break;
                case 3:
                    ViewBag.nametable = "Customer account";
                    ViewBag.userType = 3;
                    break;
                default:
                    ViewBag.nametable = "Customer account";
                    ViewBag.userType = 3;
                    break;
            }
            var UserList = await GetUserListAsync();
            List<string> UserListName = new List<string>();
            foreach (var item in UserList)
            {
                UserListName.Add(item.userName);
            }
            ViewBag.UserListName = UserListName;
            return View(UserList);
        }

        public async Task<IActionResult> UserDetails(string id)
        {
            User user = await GetUserDetailsAsync(id);
            if (user != null)
            {
                return View(user);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> UserDetails(string id, User user, IFormFile UserPicture)
        {
            if (UserPicture != null && UserPicture.Length > 0)
            {
                var fileName = Path.GetFileName(UserPicture.FileName);
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "assets/User_Picture", fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    UserPicture.CopyTo(fileStream);
                }
                user.picture = fileName;
            }
            user.userName = id;
            user.updateAt = DateTime.Now;
            String data = JsonSerializer.Serialize(user);
            var content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync(userApi + "/" + id, content);
            if (response.IsSuccessStatusCode)
            {
                TempData["SystemNotification"] = "Edited the object successfully";
                return RedirectToAction("Index", new { id = 3 });
            }
            return Redirect("Home/Error");
        }

        [HttpGet]
        public async Task<IActionResult> BanAccount(string id, int uStyle)
        {
            User user = await GetUserDetailsAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            user.userType = uStyle;
            String data = JsonSerializer.Serialize(user);
            var content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync(userApi + "/" + id, content);
            if (response.IsSuccessStatusCode)
            {
                ViewBag.SystemNotification = "Edit successful";
                var userList = await GetUserListAsync();
                return Json("User has been banned.");
            }
            return Json("Failed to ban user.");
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            user.picture = "l60Hf-150x150.png";
            user.createdAt = DateTime.Now;
            user.updateAt = DateTime.Now;
            user.userType = 2;
            if (ModelState.IsValid)
            {
                String data = JsonSerializer.Serialize(user);
                var content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(userApi, content);

                return RedirectToAction("Index");

            }
            return View(user);
        }

        private async Task<List<User>> GetUserListAsync()
        {
            HttpResponseMessage response = await client.GetAsync(userApi);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<User>>(data, option);
            }
            else
            {
                // Handle error if needed
                return new List<User>();
            }
        }

        // private async Task<User> GetUserDetailsAsync(string id)
        // {
        //     HttpResponseMessage response = await client.GetAsync(userApi + "/" + id);
        //     if (response.IsSuccessStatusCode)
        //     {
        //         string data = await response.Content.ReadAsStringAsync();
        //         var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        //         return JsonSerializer.Deserialize<User>(data, option);
        //     }
        //     else
        //     {
        //         // Handle error if needed
        //         return null;
        //     }
        // }
        private async Task<User> GetUserDetailsAsync(string id)
        {
            HttpResponseMessage response = await client.GetAsync(userApi + "/" + id);
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

        [HttpPost]
        public async Task<IActionResult> AutoCompleteSearch(string userName)
        {
            User user = await GetUserDetailsAsync(userName);
            if (user != null)
            {
                return View("UserDetails", user);
            }
            else
            {
                TempData["SystemNotificationError"] = "Error to find";
                return RedirectToAction("Index", new { id = 3 });
            }
        }
    }
}