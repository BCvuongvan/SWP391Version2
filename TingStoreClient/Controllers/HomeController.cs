﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
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

        public async Task<IActionResult> Index(string sortOrder)
        {
            HttpResponseMessage response = await client.GetAsync(api);
            string data = await response.Content.ReadAsStringAsync();

            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            List<Product> productList = JsonSerializer.Deserialize<List<Product>>(data, option);
            switch(sortOrder){
                case "asc" : productList = productList.OrderBy(p => p.proPrice).ToList();
                break;
                case "desc" : productList = productList.OrderByDescending(p => p.proPrice).ToList();
                break;
                default: break;
            }
            ViewBag.CurrentSortOrder = sortOrder;
            return View(productList);
        }

        private string ReadFromFile(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                return System.IO.File.ReadAllText(filePath);
            }
            return "File not found";
        }
        [HttpGet]
        public async Task<IActionResult> ManagementProductDetail(int id)
        {
            HttpResponseMessage response = await client.GetAsync(api + "/" + id);
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result; //kq phản hồi từ phía server, lấy dữ liệu đó lưu vào data
                var product = JsonSerializer.Deserialize<Product>(data); //bóc tách dữ liệu

                var detailFilesPath = Path.Combine(_hostingEnvironment.WebRootPath, "assets/Product_Details");
                var productInfoPath = Path.Combine(detailFilesPath, $"{product.proName}_Info.txt");
                var highlightFeaturesPath = Path.Combine(detailFilesPath, $"{product.proName}_Features.txt");
                var technicalSpecsPath = Path.Combine(detailFilesPath, $"{product.proName}_Specs.txt");

                // Đọc nội dung từ file
                var productInfo = ReadFromFile(productInfoPath);
                var highlightFeatures = ReadFromFile(highlightFeaturesPath);
                var technicalSpecs = ReadFromFile(technicalSpecsPath);

                // Tạo và gửi ViewModel hoặc ViewBag đến View
                ViewBag.ProductInfo = productInfo;
                ViewBag.HighlightFeatures = highlightFeatures;
                ViewBag.TechnicalSpecs = technicalSpecs;
                return View(product);
            }
            return NotFound();
        }


    }
}
