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
    public class HomeController : Controller
    {
        private readonly HttpClient client = null;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private string api;

        public HomeController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);

            this.api = "https://localhost:5001/api/Product";
        }

        private async Task GetCategoriesAsync()
        {
            HttpResponseMessage response = await client.GetAsync("https://localhost:5001/api/Category");
            String data = await response.Content.ReadAsStringAsync();

            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            List<Category> list = JsonSerializer.Deserialize<List<Category>>(data, option);
            ViewBag.cateList = list;
        }
        private async Task<Product> GetProductById(int proId)
        {
            HttpResponseMessage response = await client.GetAsync($"https://localhost:5001/api/Product/{proId}");
            String data = await response.Content.ReadAsStringAsync();

            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            Product product = JsonSerializer.Deserialize<Product>(data, option);
            return product;
        }
        
        private async Task GetDiscountProductAsync()
        {
            HttpResponseMessage response = await client.GetAsync("https://localhost:5001/api/DiscountProduct");
            String data = await response.Content.ReadAsStringAsync();

            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            List<DiscountPercent> list = JsonSerializer.Deserialize<List<DiscountPercent>>(data, option);
            ViewBag.discountproductList = list;
        }
        private async Task<DateTime?> GetLatestDiscountEndTimeAsync()
        {
            HttpResponseMessage response = await client.GetAsync("https://localhost:5001/api/SaleProduct/DiscountProduct/Latest");
            if (response.IsSuccessStatusCode)
            {
                String data = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<DateTime>(data);
            }
            return null;
        }
        public async Task<List<Product>> GetSimilarProduct(int cateId, int currentProductId){
            HttpResponseMessage response = await client.GetAsync(api);
            if(!response.IsSuccessStatusCode) return new List<Product>();
            string data = await response.Content.ReadAsStringAsync();
            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            List<Product> listSimilarProduct = JsonSerializer.Deserialize<List<Product>>(data, option);
            List<Product> listSimilarProductByCategory = listSimilarProduct.Where(p => p.cateId == cateId && p.proId != currentProductId).ToList();
            return listSimilarProductByCategory;
        }

        public async Task<IActionResult> ProductsByCategoryID(int cateId, string sortOrder)
        {
            HttpResponseMessage response = await client.GetAsync(api);

            string data = await response.Content.ReadAsStringAsync();
            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            List<Product> list = JsonSerializer.Deserialize<List<Product>>(data, option);
            List<Product> listProductByCategory = list.Where(p => p.cateId == cateId).ToList();
            switch (sortOrder)
            {
                case "asc":
                    listProductByCategory = listProductByCategory.OrderBy(p => p.proPrice).ToList();
                    break;
                case "desc":
                    listProductByCategory = listProductByCategory.OrderByDescending(p => p.proPrice).ToList();
                    break;
                default: break;
            }
            await GetCategoriesAsync();
            ViewBag.CurrentSortOrder = sortOrder;
            return View("Index", listProductByCategory);
        }
        public async Task<IActionResult> ProductsByPrice(decimal minPrice, decimal maxPrice, string sortOrder)
        {
            HttpResponseMessage response = await client.GetAsync(api);

            string data = await response.Content.ReadAsStringAsync();
            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            List<Product> list = JsonSerializer.Deserialize<List<Product>>(data, option);
            List<Product> listProductByCategory = list.Where(p => p.proPrice >= minPrice && p.proPrice <= maxPrice).ToList();
            switch (sortOrder)
            {
                case "asc":
                    listProductByCategory = listProductByCategory.OrderBy(p => p.proPrice).ToList();
                    break;
                case "desc":
                    listProductByCategory = listProductByCategory.OrderByDescending(p => p.proPrice).ToList();
                    break;
                default: break;
            }
            await GetCategoriesAsync();
            ViewBag.CurrentSortOrder = sortOrder;
            return View("Index", listProductByCategory);
        }

        private async Task GetHotProducts()
        {
            HttpResponseMessage response = await client.GetAsync("https://localhost:5001/api/Product/Hot");
            String data = await response.Content.ReadAsStringAsync();

            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            List<Product> list = JsonSerializer.Deserialize<List<Product>>(data, option);
            ViewBag.proList = list;
        }
        public async Task<IActionResult> Index(string sortOrder, bool redirectToHomePage = false)
        {
            HttpResponseMessage response = await client.GetAsync(api);
            string data = await response.Content.ReadAsStringAsync();

            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            List<Product> productList = JsonSerializer.Deserialize<List<Product>>(data, option);
            switch (sortOrder)
            {
                case "asc":
                    productList = productList.OrderBy(p => p.proPrice).ToList();
                    break;
                case "desc":
                    productList = productList.OrderByDescending(p => p.proPrice).ToList();
                    break;
                default: break;
            }
            var endDate = await GetLatestDiscountEndTimeAsync();
            Console.WriteLine($"End Date: {endDate}"); // Thêm dòng này để debug
            ViewBag.LatestDiscountEndDate = endDate?.ToString("o") ?? string.Empty;

            await GetCategoriesAsync();
            await GetHotProducts();
            await GetDiscountProductAsync();
            if (ViewBag.proList.Count != 4)
            {
                throw new Exception("Số lượng sản phẩm bán chạy không đúng");
            }
            var techNews = ListTechNews();
            ViewBag.TechNews = techNews;
            ViewBag.Categories = ViewBag.cateList;
            ViewBag.CurrentSortOrder = sortOrder;
            return View(productList);
        }
        private void SaveToFile(string fileName, string content)
        {
            var path = Path.Combine(_hostingEnvironment.WebRootPath, "assets/Product_Details", fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(path)); // Đảm bảo thư mục tồn tại
            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                var bytes = Encoding.UTF8.GetBytes(content);
                fileStream.Write(bytes, 0, bytes.Length);
            }
        }

        private string ReadFromFile(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                return System.IO.File.ReadAllText(filePath);
            }
            return "Không có thông tin.";
        }
        [HttpGet]
        public async Task<IActionResult> ManagementProductDetail(int id)
        {
            HttpResponseMessage response = await client.GetAsync(api + "/" + id);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<Product>(data);

                var detailFilesPath = Path.Combine(_hostingEnvironment.WebRootPath, "assets/Product_Details");
                var productInfoPath = Path.Combine(detailFilesPath, $"{product.proName}_Info.txt");
                var highlightFeaturesPath = Path.Combine(detailFilesPath, $"{product.proName}_Features.txt");
                var technicalSpecsPath = Path.Combine(detailFilesPath, $"{product.proName}_Specs.txt");

                // Đọc nội dung từ file
                var productInfo = ReadFromFile(productInfoPath);
                var highlightFeatures = ReadFromFile(highlightFeaturesPath);
                var technicalSpecs = ReadFromFile(technicalSpecsPath);

                // Gửi dữ liệu đến View
                ViewBag.ProductInfo = productInfo;
                ViewBag.HighlightFeatures = highlightFeatures;
                ViewBag.TechnicalSpecs = technicalSpecs;
                var similarProducts = await GetSimilarProduct(product.cateId, product.proId);
                ViewBag.SimilarProducts = similarProducts;
                return View(product);
            }
            return NotFound();
        }
        private async Task SaveTechNew(string title, IFormFile image, string newsId, string link)
        {
            var newsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/TechNews", newsId);
            Directory.CreateDirectory(newsDir);

            var titlePath = Path.Combine(newsDir, "title.txt");
            var imagePath = Path.Combine(newsDir, "image.jpg");
            var linkPath = Path.Combine(newsDir, "link.txt");
            System.IO.File.WriteAllText(titlePath, title);
            await System.IO.File.WriteAllTextAsync(linkPath, link);

            using (var fileStream = new FileStream(imagePath, FileMode.Create, FileAccess.Write))
            {
                image.CopyTo(fileStream);
            }
        }

        private IEnumerable<(string Title, string ImageUrl, string NewsId, string Link)> ListTechNews()
        {
            var newsList = new List<(string, string, string, string)>();
            var newsDirs = Directory.GetDirectories(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/TechNews"));

            foreach (var technew in newsDirs)
            {
                var newsId = new DirectoryInfo(technew).Name;
                var titlePath = Path.Combine(technew, "title.txt");
                var imagePath = Path.Combine(technew, "image.jpg");
                var linkPath = Path.Combine(technew, "link.txt");

                if (System.IO.File.Exists(titlePath))
                {
                    var title = System.IO.File.ReadAllText(titlePath);
                    var imageUrl = $"/assets/TechNews/{newsId}/image.jpg";
                    var link = System.IO.File.Exists(linkPath) ? System.IO.File.ReadAllText(linkPath) : "";
                    newsList.Add((title, imageUrl, newsId, link));
                }
            }
            return newsList;
        }
    }
}
