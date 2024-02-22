using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TingStoreClient.Models;

namespace TingStoreClient.Controllers
{
    [Route("[controller]")]
    public class SaleProductController : Controller
    {
        private readonly HttpClient client = null;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private string api;

        public SaleProductController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);

            this.api = "https://localhost:5001/api/SaleProduct";
        }

        private async Task GetProductAsync()
        {
            HttpResponseMessage response = await client.GetAsync("https://localhost:5001/api/Product");
            String data = await response.Content.ReadAsStringAsync();

            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            List<Product> products = JsonSerializer.Deserialize<List<Product>>(data, option);
            var activeProducts = products.Where(p => p.proStatus == true).ToList();
            ViewBag.proList = activeProducts;
        }
        private async Task<DiscountPercent> GetSaleProductById(int saleId)
        {
            HttpResponseMessage response = await client.GetAsync($"https://localhost:5001/api/SaleProduct/{saleId}");
            String data = await response.Content.ReadAsStringAsync();

            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            DiscountPercent disPer = JsonSerializer.Deserialize<DiscountPercent>(data, option);
            return disPer;
        }

        [HttpGet("list")]
        public async Task<IActionResult> ListSaleProduct(bool redirectToHomePage = false)
        {
            HttpResponseMessage response = await client.GetAsync(api);
            string data = await response.Content.ReadAsStringAsync();

            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            List<DiscountPercent> list = JsonSerializer.Deserialize<List<DiscountPercent>>(data, option);
            return View(list);
        }
        [HttpGet("create")]
        public async Task<IActionResult> CreateSaleProduct()
        {
            await GetProductAsync();
            return View();
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSaleProduct(DiscountPercent disPer, IFormFile proImageFile, List<int> selectedProducts)
        {
            if (proImageFile != null)
            {
                var fileName = Path.GetFileName(proImageFile.FileName);
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "assets/SaleProduct_Images", fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await proImageFile.CopyToAsync(fileStream);
                }
                disPer.discountImage = fileName;
            }

            foreach (var productId in selectedProducts)
            {
                var newDisPer = new DiscountPercent
                {
                    proId = productId,
                    discountPercentage = disPer.discountPercentage,
                    startDate = disPer.startDate,
                    endDate = disPer.endDate,
                    discountImage = disPer.discountImage,
                    isActive = true
                };

                string data = JsonSerializer.Serialize(newDisPer);
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(api, content);
                if (response.StatusCode != System.Net.HttpStatusCode.Created)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ViewBag.ErrorMessage = errorMessage;
                    await GetProductAsync();
                    return View(disPer);
                }
            }

            return RedirectToAction("ListSaleProduct");
        }
        [HttpGet("update/{id}")]
        public async Task<IActionResult> UpdateSaleProduct(int id)
        {
            HttpResponseMessage response = await client.GetAsync(api + "/" + id);
            await GetProductAsync();
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var disPer = JsonSerializer.Deserialize<DiscountPercent>(data);
                return View(disPer);
            }
            return NotFound();
        }
        [HttpPost("update/{id}")]
        public async Task<IActionResult> UpdateSaleProduct(int id, DiscountPercent disPer, IFormFile newImageFile)
        {
            disPer.discountId = id;

            if (newImageFile != null && newImageFile.Length > 0)
            {
                var fileName = Path.GetFileName(newImageFile.FileName);
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "assets/Product_Images", fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    newImageFile.CopyTo(fileStream);
                }

                disPer.discountImage = fileName;
            }
            else
            {
                var existingProduct = await GetSaleProductById(id);

                if (existingProduct != null)
                {
                    disPer.discountImage = existingProduct.discountImage;
                }
                else
                {
                    return View("Error");
                }

            }
            disPer.isActive = true;
            string data = JsonSerializer.Serialize(disPer);
            var content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync(api + "/" + id, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListSaleProduct");
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                ViewBag.ErrorMessage = errorMessage;
            }
            await GetProductAsync();
            return View(disPer);
        }

        [HttpGet("delete/{id}")]
        public async Task<IActionResult> DeleteSaleProduct(int id)
        {
            HttpResponseMessage response = await client.GetAsync(api + "/" + id);
            await GetProductAsync();
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var disPer = JsonSerializer.Deserialize<DiscountPercent>(data);
                return View(disPer);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int discountId)
        {
            HttpResponseMessage response = await client.DeleteAsync(api + "/" + discountId);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var disPer = JsonSerializer.Deserialize<Product>(data); //bóc tách dữ liệu
                if (disPer != null)
                {
                    disPer.proStatus = false;
                    var json = JsonSerializer.Serialize(disPer); // Serialize đối tượng Product đã cập nhật trở lại thành JSON.
                    var content = new StringContent(data, System.Text.Encoding.UTF8, "application/json"); // Tạo một StringContent chứa dữ liệu JSON, sử dụng UTF8 và kiểu nội dung là application/json.
                    HttpResponseMessage updateResponse = await client.PostAsync(api + "/" + discountId, content); //gửi đi
                    return RedirectToAction("ListSaleProduct");
                }
            }
            return Redirect("Home/Error");
        }
    }
}