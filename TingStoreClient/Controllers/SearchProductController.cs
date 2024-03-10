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
    [Route("[controller]")]
    public class SearchProductController : Controller
    {
        private readonly HttpClient client = null;
        private string SearchApi;

        public SearchProductController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            this.SearchApi = "https://localhost:5001/api/SearchProduct";
        }

        public async Task<List<string>> getListProductbyProName()
        {
            HttpResponseMessage response = await client.GetAsync(SearchApi);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var ListProduct = JsonSerializer.Deserialize<List<Product>>(data, option);
                List<string> ListProductByName = new List<string>();
                foreach (var item in ListProduct)
                {
                    ListProductByName.Add(item.proName);
                }
                return ListProductByName;
            }
            else
            {
                // Handle error if needed
                return new List<string>();
            }
        }
}
}