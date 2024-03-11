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
        private string OrderApi;

        public SearchProductController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            this.SearchApi = "https://localhost:5001/api/SearchProduct";
            this.OrderApi = "https://localhost:5001/api/Order";
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

        public async Task<List<Order>> GetListPendingOrder()
        {
            HttpResponseMessage response = await client.GetAsync(OrderApi);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var ListProduct = JsonSerializer.Deserialize<List<Order>>(data, option);
                List<Order> ListPendingOrder = new List<Order>();
                foreach (var item in ListProduct)
                {
                    if(item.orderStatusId == 1){
                        ListPendingOrder.Add(item);
                    }
                }
                return ListPendingOrder;
            }
            else
            {
                // Handle error if needed
                return new List<Order>();
            }
        }
        
    }
}