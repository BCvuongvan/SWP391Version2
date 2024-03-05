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
    public class ProductController : Controller
    {
        private readonly HttpClient client = null;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private string api;

        public ProductController(IWebHostEnvironment hostingEnvironment)
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

        private async Task<Category> GetCategoryById(int cateId)
        {
            HttpResponseMessage response = await client.GetAsync($"https://localhost:5001/api/Category/{cateId}");
            String data = await response.Content.ReadAsStringAsync();

            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            Category category = JsonSerializer.Deserialize<Category>(data, option);

            return category;
        }

        private async Task<Product> GetProductById(int proId)
        {
            HttpResponseMessage response = await client.GetAsync($"https://localhost:5001/api/Product/{proId}");
            String data = await response.Content.ReadAsStringAsync();

            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            Product product = JsonSerializer.Deserialize<Product>(data, option);
            return product;
        }
        [HttpGet("ProductsByCategoryID/{cateId}")]
        public async Task<IActionResult> GetProductsByCategory(int cateId)
        {
            HttpResponseMessage response = await client.GetAsync($"{this.api}/ProductsByCategoryID/{cateId}");

            string data = await response.Content.ReadAsStringAsync();
            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            List<Product> list = JsonSerializer.Deserialize<List<Product>>(data, option);
            await GetCategoriesAsync();
            return View("ListProduct", list);
        }

        [HttpGet("list")]
        public async Task<IActionResult> ListProduct(bool redirectToHomePage = false)
        {
            HttpResponseMessage response = await client.GetAsync(api);
            string data = await response.Content.ReadAsStringAsync();

            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            List<Product> list = JsonSerializer.Deserialize<List<Product>>(data, option);
            await GetCategoriesAsync();
            ViewBag.Categories = ViewBag.cateList;
            return View(list);
        }

        [HttpGet("create")]
        public async Task<IActionResult> CreateProduct()
        {
            await GetCategoriesAsync();
            return View();
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct(Product product, IFormFile proImageFile, string productInfo, string highlightFeatures, List<string> specNames, List<string> specValues)
        {
            // Xử lý file ảnh
            if (proImageFile != null && proImageFile.Length > 0)
            {
                var fileName = Path.GetFileName(proImageFile.FileName);
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "assets/Product_Images", fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await proImageFile.CopyToAsync(fileStream);
                }

                product.proImage = fileName; // Giả sử bạn có trường ProImage trong model Product của mình
            }

            // Lưu thông tin sản phẩm và tính năng nổi bật vào file
            var detailFilesPath = Path.Combine(_hostingEnvironment.WebRootPath, "assets/Product_Details");
            if (!string.IsNullOrEmpty(productInfo))
            {
                SaveToFile(Path.Combine(detailFilesPath, $"{product.proName}_Info.txt"), productInfo);
            }

            if (!string.IsNullOrEmpty(highlightFeatures))
            {
                SaveToFile(Path.Combine(detailFilesPath, $"{product.proName}_Features.txt"), highlightFeatures);
            }

            // Xử lý và lưu Specifications
            if (specNames != null && specValues != null && specNames.Count == specValues.Count)
            {
                var specsList = new List<string>();
                for (int i = 0; i < specNames.Count; i++)
                {
                    specsList.Add($"{specNames[i]}: {specValues[i]}");
                }
                var technicalSpecs = string.Join("\n", specsList);
                if (!string.IsNullOrEmpty(technicalSpecs))
                {
                    SaveToFile(Path.Combine(detailFilesPath, $"{product.proName}_Specs.txt"), technicalSpecs);
                }
            }

            // Gửi dữ liệu sản phẩm qua API
            var data = JsonSerializer.Serialize(product);
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(api, content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListProduct");
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                ViewBag.ErrorMessage = errorMessage;
                return View(product);
            }
        }

        [HttpPost("addImages")]
        public async Task<IActionResult> UploadImage(IFormFile file, int productId)
        {
            if (file != null && file.Length > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "assets/Product_Images", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }

                var apiProductImage = "https://localhost:5001/api/Product/addImages";

                var productImage = new ProductImage
                {
                    proId = productId,
                    imageUrl = fileName,
                    imageStatus = true
                };


                var json = JsonSerializer.Serialize(productImage);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiProductImage, content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("ManagementProductDetail", new { id = productId });
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ViewBag.ErrorMessage = errorMessage;
                    return View();
                }
            }

            return View();
        }


        [HttpGet("Picture/{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            HttpResponseMessage response = await client.GetAsync($"{this.api}/Picture/{id}");
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var proImg = JsonSerializer.Deserialize<ProductImage>(data);
                return View(proImg);
            }
            return NotFound();
        }
        [HttpPost("DeleteImageProduct/{imageId}")]
        public async Task<IActionResult> DeleteImageProductDetail(int imageId)
        {
            HttpResponseMessage response = await client.DeleteAsync($"{this.api}/DeleteImageProduct/{imageId}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var imgProduct = JsonSerializer.Deserialize<ProductImage>(data);
                if (imgProduct != null)
                {
                    imgProduct.imageStatus = false;
                    var json = JsonSerializer.Serialize(imgProduct);
                    var content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
                    HttpResponseMessage updateResponse = await client.PostAsync($"{this.api}/DeleteImageProduct/{imageId}", content);
                    int productId = imgProduct.proId;
                    return RedirectToAction("ManagementProductDetail", new { id = productId });
                }
            }
            return Redirect("Home/Error");
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

        [HttpGet("update/{id}")]
        public async Task<IActionResult> UpdateProduct(int id)
        {
            HttpResponseMessage response = await client.GetAsync(api + "/" + id);
            await GetCategoriesAsync();
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var product = JsonSerializer.Deserialize<Product>(data);
                return View(product);
            }
            return NotFound();
        }

        [HttpPost("update/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product pro, IFormFile newImageFile, string productInfo, string highlightFeatures, string technicalSpecs)
        {
            pro.proId = id;

            if (newImageFile != null && newImageFile.Length > 0)
            {
                var fileName = Path.GetFileName(newImageFile.FileName);
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "assets/Product_Images", fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    newImageFile.CopyTo(fileStream);
                }

                pro.proImage = fileName;
            }
            else
            {
                var existingProduct = await GetProductById(id);

                if (existingProduct != null)
                {
                    pro.proImage = existingProduct.proImage;
                }
                else
                {
                    return View("Error");
                }

            }
            pro.proStatus = true;
            string data = JsonSerializer.Serialize(pro);
            var content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync(api + "/" + id, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListProduct");
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                ViewBag.ErrorMessage = errorMessage;
                await GetCategoriesAsync();
            }
            await GetCategoriesAsync();
            return View(pro);
        }

        [HttpGet("delete/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            HttpResponseMessage response = await client.GetAsync(api + "/" + id);
            await GetCategoriesAsync();
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var product = JsonSerializer.Deserialize<Product>(data);

                if (product != null)
                {
                    product.category = await GetCategoryById(product.cateId);
                }
                return View(product);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int proId)
        {
            HttpResponseMessage response = await client.DeleteAsync(api + "/" + proId);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<Product>(data); //bóc tách dữ liệu
                if (product != null)
                {
                    product.proStatus = false;
                    var json = JsonSerializer.Serialize(product); // Serialize đối tượng Product đã cập nhật trở lại thành JSON.
                    var content = new StringContent(data, System.Text.Encoding.UTF8, "application/json"); // Tạo một StringContent chứa dữ liệu JSON, sử dụng UTF8 và kiểu nội dung là application/json.
                    HttpResponseMessage updateResponse = await client.PostAsync(api + "/" + proId, content); //gửi đi
                    return RedirectToAction("ListProduct");
                }
            }
            return Redirect("Home/Error");
        }

        [HttpGet("productdetail/{id}")]
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
                return View(product);
            }
            return NotFound();
        }

        [HttpGet("updateproductinformation/{id}")]
        public async Task<IActionResult> UpdateProductInfomationDetail(int id)
        {
            HttpResponseMessage response = await client.GetAsync(api + "/" + id);
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var product = JsonSerializer.Deserialize<Product>(data);

                var detailFilesPath = Path.Combine(_hostingEnvironment.WebRootPath, "assets/Product_Details");
                var productInfoPath = Path.Combine(detailFilesPath, $"{product.proName}_Info.txt");

                var productInfo = ReadFromFile(productInfoPath);

                ViewBag.ProductInfo = productInfo;
                return View(product);
            }
            return NotFound();
        }

        [HttpPost("updateproductinformation/{id}")]
        public async Task<IActionResult> UpdateProductInfomationDetail(int id, Product pro, string productInfo)
        {
            pro.proId = id;
            string data = JsonSerializer.Serialize(pro);
            var content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync(api + "/" + id, content);
            if (response == null)
            {
                return NotFound();
            }
            var detailFilesPath = Path.Combine(_hostingEnvironment.WebRootPath, "assets/Product_Details");
            SaveToFile(Path.Combine(detailFilesPath, $"{pro.proName}_Info.txt"), productInfo);
            return RedirectToAction("ManagementProductDetail", new { id = id });
        }

        [HttpGet("updateproducthighlightfeatures/{id}")]
        public async Task<IActionResult> UpdateProductHighlightFeaturesDetail(int id)
        {
            HttpResponseMessage response = await client.GetAsync(api + "/" + id);
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var product = JsonSerializer.Deserialize<Product>(data);

                var detailFilesPath = Path.Combine(_hostingEnvironment.WebRootPath, "assets/Product_Details");
                var highlightFeaturesPath = Path.Combine(detailFilesPath, $"{product.proName}_Features.txt");

                var highlightFeatures = ReadFromFile(highlightFeaturesPath);

                ViewBag.HighlightFeatures = highlightFeatures;
                return View(product);
            }
            return NotFound();
        }

        [HttpPost("updateproducthighlightfeatures/{id}")]
        public async Task<IActionResult> UpdateProductHighlightFeaturesDetail(int id, Product pro, string highlightFeatures)
        {
            pro.proId = id;
            string data = JsonSerializer.Serialize(pro);
            var content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync(api + "/" + id, content);
            if (response == null)
            {
                return NotFound();
            }
            var detailFilesPath = Path.Combine(_hostingEnvironment.WebRootPath, "assets/Product_Details");
            SaveToFile(Path.Combine(detailFilesPath, $"{pro.proName}_Features.txt"), highlightFeatures);
            return RedirectToAction("ManagementProductDetail", new { id = id });
        }

        [HttpGet("updateproducttechnicalspecs/{id}")]
        public async Task<IActionResult> UpdateProductTechnicalSpecsDetail(int id)
        {
            HttpResponseMessage response = await client.GetAsync(api + "/" + id);
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var product = JsonSerializer.Deserialize<Product>(data);

                var detailFilesPath = Path.Combine(_hostingEnvironment.WebRootPath, "assets/Product_Details");
                var technicalSpecsPath = Path.Combine(detailFilesPath, $"{product.proName}_Specs.txt");

                var technicalSpecs = ReadFromFile(technicalSpecsPath);

                ViewBag.TechnicalSpecs = technicalSpecs;
                return View(product);
            }
            return NotFound();
        }

        [HttpPost("updateproducttechnicalspecs/{id}")]
        public async Task<IActionResult> UpdateProductTechnicalSpecsDetail(int id, Product pro, string technicalSpecs)
        {
            pro.proId = id;
            string data = JsonSerializer.Serialize(pro);
            var content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync(api + "/" + id, content);
            if (response == null)
            {
                return NotFound();
            }
            var detailFilesPath = Path.Combine(_hostingEnvironment.WebRootPath, "assets/Product_Details");
            SaveToFile(Path.Combine(detailFilesPath, $"{pro.proName}_Specs.txt"), technicalSpecs);
            return RedirectToAction("ManagementProductDetail", new { id = id });
        }
    }
}