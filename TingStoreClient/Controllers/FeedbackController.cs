using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TingStoreClient.Models;

namespace TingStoreClient.Controllers
{
    public class FeedbackController : Controller
    {
        // private readonly IHttpClientFactory _httpClientFactory;
        // private readonly string _apiBaseUrl = "https://localhost:5001/api/Feedback/";
        // private readonly string ProductAPI = "https://localhost:5001/api/Product";

        // public FeedbackController(IHttpClientFactory httpClientFactory)
        // {
        //     _httpClientFactory = httpClientFactory;
        // }

        // private async Task<IEnumerable<Product>> GetProductsBoughtByUser()
        // {
        //     try
        //     {
        //         // Lấy username từ session
        //         var userSession = HttpContext.Session.GetString("_user");
        //         var userObject = JsonConvert.DeserializeObject<User>(userSession);
        //         var username = userObject.userName;

        //         System.Console.WriteLine(username);
        //         if (username == null)
        //         {
        //             // Nếu không tìm thấy username trong session, bạn có thể xử lý theo ý định của mình, ví dụ: quay lại trang đăng nhập
        //             return Enumerable.Empty<Product>();
        //         }

        //         var client = _httpClientFactory.CreateClient();
        //         var response = await client.GetAsync($"{_apiBaseUrl}products-bought/{username}"); // Thay đổi địa chỉ endpoint
        //         response.EnsureSuccessStatusCode();
        //         var productsResponse = await response.Content.ReadFromJsonAsync<IEnumerable<object>>();
        //         if (productsResponse != null)
        //         {
        //             var products = productsResponse.Select(pr =>
        //             {
        //                 var proId = pr.GetType().GetProperty("ProId")?.GetValue(pr);
        //                 var proName = pr.GetType().GetProperty("ProName")?.GetValue(pr)?.ToString();
        //                 var proImage = pr.GetType().GetProperty("ProImage")?.GetValue(pr)?.ToString();

        //                 if (proId != null && proName != null && proImage != null)
        //                 {
        //                     return new Product
        //                     {
        //                         proId = Convert.ToInt32(proId),
        //                         proName = proName,
        //                         proImage = proImage
        //                     };
        //                 }
        //                 else
        //                 {
        //                     // Trả về null nếu có lỗi khi truy cập các thuộc tính của sản phẩm
        //                     return null;
        //                 }
        //             }).Where(product => product != null).ToList();
        //             System.Console.WriteLine(products);
        //             return products;
        //         }
        //         else
        //         {
        //             return Enumerable.Empty<Product>();
        //         }
        //     }
        //     catch (HttpRequestException ex)
        //     {
        //         System.Console.WriteLine($"Error get product bought by user: {ex.Message}");
        //         return Enumerable.Empty<Product>();
        //     }
        // }

        // public async Task<IActionResult> AddNew(int id)
        // {
        //     HttpResponseMessage response = await client.GetAsync(ProductAPI+"/"+id);
        //     if (response.IsSuccessStatusCode)
        //     {
        //         var data = response.Content.ReadAsStringAsync().Result;
        //         var customer = JsonSerializer.Deserialize<Product>(data);
        //         return View(customer);
        //     }
        //     return NotFound();
        // }


        // [HttpPost("AddFeedback")]
        // public async Task<IActionResult> AddFeedback(string userName, int proId, string comment, int rating)
        // {
        //     try
        //     {
        //         var feedback = new Feedback
        //         {
        //             userName = userName,
        //             proId = proId,
        //             comment = comment,
        //             rating = rating
        //         };
        //         var client = _httpClientFactory.CreateClient();
        //         var response = await client.PostAsJsonAsync($"{_apiBaseUrl}AddFeedback", feedback); // Sử dụng PostAsJsonAsync để gửi dữ liệu
        //         response.EnsureSuccessStatusCode();
        //         return RedirectToAction("AddFeedbackGet");

        //     }
        //     catch (HttpRequestException ex)
        //     {

        //         System.Console.WriteLine($"Error add feedback: {ex.Message}");
        //         return BadRequest("Error add feedback");
        //     }
        // }


        // public async Task<IActionResult> AddFeedbackGet()
        // {
        //     // Lấy danh sách sản phẩm mà người dùng đã mua
        //     var productsBought = await GetProductsBoughtByUser();

        //     // Lưu userName vào ViewBag để sử dụng trong AddFeedbackGet.cshtm
        //     var userSession = HttpContext.Session.GetString("_user");
        //     var userObject = JsonConvert.DeserializeObject<User>(userSession);
        //     var username = userObject.userName;
        //     ViewBag.username = username;
        //     // Trả về trang AddFeedbackGet.cshtml với danh sách sản phẩm mà người dùng đã mua
        //     return View("AddFeedbackGet", productsBought);
        // }

    }
}
