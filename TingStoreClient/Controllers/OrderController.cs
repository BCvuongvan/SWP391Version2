using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TingStoreClient.Models;

namespace TingStoreClient.Controllers
{
   public class OrderController : Controller
    {
        private readonly HttpClient client = null;

        private string orderApi;
        public OrderController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            this.orderApi = "https://localhost:5001/api/Order";
        }

        [HttpGet("Admin/GetAllOrder/{statusId?}")]
        public async Task<IActionResult> GetAllOrder(int? statusId)
        {
            var response = await client.GetAsync(orderApi); // Gửi yêu cầu GET đến orderApi và lưu trữ kết quả trong response
            var data = await response.Content.ReadAsStringAsync(); // Đọc nội dung của phản hồi dưới dạng chuỗi
            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true }; // Tạo tùy chọn cho việc Deserialize JSON
            var orderList = JsonSerializer.Deserialize<List<Order>>(data, option) ?? new List<Order>(); // Deserialize dữ liệu JSON thành danh sách đơn hàng

            foreach (var item in orderList) // Duyệt qua từng đơn hàng trong danh sách
            {
                item.orderStatus = await GetOrderStatusById(item.orderStatusId); // Gán trạng thái của đơn hàng bằng cách gọi phương thức GetOrderStatusById
            }

            if (statusId.HasValue) // Kiểm tra xem statusId có giá trị không
            {
                orderList = orderList.Where(x => x.orderStatusId == statusId.Value).ToList(); // Lọc danh sách đơn hàng theo statusId
            }

            // Đếm số lượng đơn hàng với mỗi trạng thái và gán vào các thuộc tính ViewBag tương ứng
            ViewBag.countAllOrder = orderList.Count(x => x.orderStatusId == 1);
            ViewBag.countOrderSuccessful = orderList.Count(x => x.orderStatusId == 2);
            ViewBag.countOrderReject = orderList.Count(x => x.orderStatusId == 3);
            ViewBag.countOrderCancel = orderList.Count(x => x.orderStatusId == 4);
            ViewBag.SelectedStatusId = statusId; // Gán giá trị statusId vào ViewBag

            return View(orderList);
        }

        [HttpGet("Admin/GetAllOrder/OrderDetailById/{orderStatusId}")]
        private async Task<OrderStatus> GetOrderStatusById(int orderStatusId)
        {
            HttpResponseMessage response = await client.GetAsync($"https://localhost:5001/api/OrderStatus/{orderStatusId}");
            String data = await response.Content.ReadAsStringAsync();

            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            OrderStatus orderStatus = JsonSerializer.Deserialize<OrderStatus>(data, option);

            return orderStatus;
        }

        [HttpGet("Admin/GetAllOrder/OrderDetailById/{id}")]
        public async Task<IActionResult> OrderDetail(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid order id.");
            }

            HttpResponseMessage response = await client.GetAsync(orderApi + "/" + id);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var orderDetails = JsonSerializer.Deserialize<Order>(data, option);
                return View(orderDetails);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ChangeOrderStatusConfirm(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid order id.");
            }

            HttpResponseMessage response = await client.GetAsync(orderApi + "/" + id);

            string data = await response.Content.ReadAsStringAsync();
            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var order = JsonSerializer.Deserialize<Order>(data, option);
            if (order == null)
            {
                return NotFound();
            }
            order.orderStatusId = 2;
            string responseData = JsonSerializer.Serialize(order);

            var content = new StringContent(responseData, System.Text.Encoding.UTF8, "application/json");
            response = await client.PutAsync(orderApi + "/" + id, content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("GetAllOrder");
            }
            return Redirect("Home/Error");
        }

        [HttpPost("ChangeOrderStatusReject/{id}")]
        public async Task<IActionResult> ChangeOrderStatusReject(int id)
        {
            HttpResponseMessage response = await client.GetAsync(orderApi + "/" + id);

            string data = await response.Content.ReadAsStringAsync();
            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var order = JsonSerializer.Deserialize<Order>(data, option);
            if (order == null)
            {
                return NotFound();
            }
            order.orderStatusId = 3;
            string responseData = JsonSerializer.Serialize(order);

            var content = new StringContent(responseData, System.Text.Encoding.UTF8, "application/json");
            response = await client.PutAsync(orderApi + "/" + id, content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("GetAllOrder");
            }
            return Redirect("Home/Error");
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderFiltered(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return NotFound("Invalid customer name");
            }

            // Call the API with the constructed query string
            HttpResponseMessage response = await client.GetAsync($"https://localhost:5001/api/Order/search/{userName}");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();

                // Deserialize the response data to a list of orders
                var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var orderList = JsonSerializer.Deserialize<List<Order>>(data, option);
                return View(orderList);
            }
            return NotFound();
        }
    }
}