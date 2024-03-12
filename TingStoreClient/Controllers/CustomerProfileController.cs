using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using TingStoreClient.Util;
using TingStoreClient.Filters;


namespace TingStoreClient.Controllers
{
    [AuthenticationRedirect]
    public class CustomerProfileController : Controller
    {
        private readonly HttpClient client = null;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private string userApi;
        private string customerOrderApi;
        public CustomerProfileController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            this.userApi = "https://localhost:5001/api/CustomerProfile";
            this.customerOrderApi = "https://localhost:5001/api/CustomerOrder";
        }

        public async Task<IActionResult> CustomerProfile()
        {
            // Lấy id của người dùng từ Session
            string id = HttpContext.Session.GetString("_user");
            // Chuyển chuổi id lấy được thành đối tượng User
            var user1 = JsonSerializer.Deserialize<User>(id);
            // Lấy thông tin của người dùng dựa vào Username
            User user = await GetUserDetailsAsync(user1.userName);
            if (user != null)
            {
                return View(user);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CustomerProfile(User user, IFormFile UserPicture)
        {
            // Lấy id của người dùng từ Session
            string id = HttpContext.Session.GetString("_user");
            // Chuyển chuổi id lấy được thành đối tượng User
            var user1 = JsonSerializer.Deserialize<User>(id);
            if (UserPicture != null && UserPicture.Length > 0)
            {
                // Lấy tên file ảnh
                var fileName = Path.GetFileName(UserPicture.FileName);
                // Xác định đường dẫn lưu file ảnh
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "assets/User_Picture", fileName);
                // Lưu file ảnh vào đường dẫn đã xác định
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    UserPicture.CopyTo(fileStream);
                }
                // Cập nhật đường dẫn vào user
                user.picture = fileName;
            }
            else
            {
                user.picture = user1.picture;
            }
            // Cập nhật userName từ session vào object user
            user.userName = user1.userName;
            user.updateAt = DateTime.Now;
            // Chuyển đổi user thành chuỗi Json
            string data = JsonSerializer.Serialize(user);
            // Tạo http để gửi đi
            var content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
            // Gửi yêu cầu PUT đến API để cập nhật thông tin
            HttpResponseMessage response = await client.PutAsync(userApi + "/" + user1.userName, content);
            if (response.IsSuccessStatusCode)
            {
                TempData["SystemNotification"] = "Your changes have been saved successfully!";
                return RedirectToAction("CustomerProfile");
            }
            return RedirectToAction("Index", "Home");
        }

        private async Task<User> GetUserDetailsAsync(string id)
        {
            // Gửi yêu cầu GET đến API để lấy thông tin user
            HttpResponseMessage response = await client.GetAsync(userApi + "/" + id);
            // Nếu yêu cầu thành công, đọc và chuyển đổi dữ liệu JSON thành đối tượng User
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
                    Console.WriteLine($"Error deserializing JSON: {ex.Message}");
                    return null;
                }
            }
            else
            {
                return null;
            }
        }


        [HttpGet("{orderStatusId}")]
        private async Task<OrderStatus> GetOrderStatusById(int orderStatusId)
        {
            HttpResponseMessage response = await client.GetAsync($"https://localhost:5001/api/OrderStatus/{orderStatusId}");
            String data = await response.Content.ReadAsStringAsync();

            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            OrderStatus orderStatus = JsonSerializer.Deserialize<OrderStatus>(data, option);

            return orderStatus;
        }

        [HttpGet("CustomerProfile/HistoryOrder/Orderdetail/{id}")]
        public async Task<IActionResult> OrderDetailByCustomer(int id)
        {
            if (id <= 0)
            {
                return NotFound("Invalid order id.");
            }

            HttpResponseMessage response = await client.GetAsync(customerOrderApi + "/" + id);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var orderDetails = JsonSerializer.Deserialize<Order>(data, option);
                return View(orderDetails);
            }
            return NotFound();
        }

        public async Task<IActionResult> CustomerCancelOrder(int id)
        {
            HttpResponseMessage response = await client.GetAsync(customerOrderApi + "/CancelOrder/" + id);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("CustomerProfile");
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
