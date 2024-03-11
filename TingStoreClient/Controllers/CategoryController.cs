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
    public class CategoryController : Controller
    {
        private readonly HttpClient client = null;
        private string api;

        public CategoryController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            this.api = "https://localhost:5001/api/Category";
        }

        public async Task<IActionResult> GetAllCategory()
        {
            HttpResponseMessage response = await client.GetAsync(api);
            string data = await response.Content.ReadAsStringAsync();

            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            List<Category> list = JsonSerializer.Deserialize<List<Category>>(data, option);
            return View(list);
        }

        // [HttpPost]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                string data = JsonSerializer.Serialize(category);
                var content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(api, content);
                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    // Redirect to the GetAllCategory action
                    TempData["SystemNotification"] = "Add category sucessfully!";
                    return RedirectToAction("GetAllCategory");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ViewBag.ErrorMessage = errorMessage;
                    return View(category);
                }
            }

            // If creation fails or ModelState is invalid, return the same view with the provided category
            return View(category);
        }

        public async Task<IActionResult> UpdateCategory(int id)
        {
            HttpResponseMessage response = await client.GetAsync(api + "/" + id);
            await GetAllCategory();
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result; //kq phản hồi từ phía server, lấy dữ liệu đó lưu vào data
                var category = JsonSerializer.Deserialize<Category>(data); //bóc tách dữ liệu
                return View(category);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateCategory(int id, Category category)
        {
            category.cateId = id;
            string data = JsonSerializer.Serialize(category);
            var content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");//định nghĩa nội dung gửi đi
            HttpResponseMessage response = await client.PutAsync(api + "/" + id, content);//nhận kết quả phản hồi về
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("GetAllCategory");
            }
            return View(category);
        }

        public async Task<IActionResult> DeleteCategory(int id)
        {
            HttpResponseMessage response = await client.GetAsync(api + "/" + id);
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var category = JsonSerializer.Deserialize<Category>(data);
                return View(category);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int cateId)
        {
            HttpResponseMessage response = await client.DeleteAsync(api + "/" + cateId);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("GetAllCategory");
            }
            // Trả về lại trang DeleteCategory với model nếu không thành công
            var data = await response.Content.ReadAsStringAsync();
            var category = JsonSerializer.Deserialize<Category>(data);
            return View("DeleteCategory", category);
        }


    }
}