using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TingStoreClient.Models;
using TingStoreClient.Filters;

namespace TingStoreClient.Controllers
{
    [AuthenticationRedirect]
    public class CartManagerController : Controller
    {
        private readonly HttpClient client = null;
        private string cartApi;
        private string handleOrderApi;
        private string productApi;
        private string userApi;
        private string authAPI;
        public CartManagerController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            this.cartApi = "https://localhost:5001/api/CartManager";
            this.handleOrderApi = "https://localhost:5001/api/HandlingCart";
            this.productApi = "https://localhost:5001/api/Product";
            this.userApi = "https://localhost:5001/api/UserManager";
            this.authAPI = "https://localhost:5001/api/Auth";
        }

        public async Task<IActionResult> Index()
        {
            string userJson = HttpContext.Session.GetString("_user");
            var user = JsonSerializer.Deserialize<User>(userJson);

            HttpResponseMessage response = await client.GetAsync(cartApi + "/" + user.userName);
            String data = await response.Content.ReadAsStringAsync();

            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            List<Cart> list = JsonSerializer.Deserialize<List<Cart>>(data, option);
            return View(list);
        }

        public async Task<IActionResult> AddToCart(int id)
        {
            string userJson = HttpContext.Session.GetString("_user");
            var user = JsonSerializer.Deserialize<User>(userJson);
            HttpResponseMessage response = await client.GetAsync(cartApi + "/AddToCart/" + user.userName + "/" + id);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");

            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> DecreaseQuantityProduct(int id)
        {
            string userJson = HttpContext.Session.GetString("_user");
            var user = JsonSerializer.Deserialize<User>(userJson);
            HttpResponseMessage response = await client.GetAsync(cartApi + "/DecreaseQuantity/" + user.userName + "/" + id);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");

            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> HandlingOrder(decimal id)
        {
            // get session get object
            string userJson = HttpContext.Session.GetString("_user");
            var user = JsonSerializer.Deserialize<User>(userJson);

            // get List Of Cart
            HttpResponseMessage response = await client.GetAsync(cartApi + "/" + user.userName);
            String data = await response.Content.ReadAsStringAsync();
            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            List<Cart> listCart = JsonSerializer.Deserialize<List<Cart>>(data, option);

            // Create Order
            Order order = new Order(user.userName, DateTime.Now, id, 1);
            String dataOrder = JsonSerializer.Serialize(order);
            var content = new StringContent(dataOrder, System.Text.Encoding.UTF8, "application/json");
            response = await client.PostAsync(handleOrderApi + "/CreateOrder", content);

            string responseData = await response.Content.ReadAsStringAsync();


            foreach (var item in listCart)
            {
                response = await client.GetAsync(handleOrderApi + "/UpdateProduct/" + item.proId + "/" + item.quantity);
                response = await client.DeleteAsync(cartApi + "/RemoveItem/" + user.userName + "/" + item.proId);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> RemoveItem(int id)
        {
            // get session get object
            string userJson = HttpContext.Session.GetString("_user");
            var user = JsonSerializer.Deserialize<User>(userJson);

            HttpResponseMessage response = await client.DeleteAsync(cartApi + "/RemoveItem/" + user.userName + "/" + id);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Order()
        {
            string userJson = HttpContext.Session.GetString("_user");
            var user = JsonSerializer.Deserialize<User>(userJson);

            HttpResponseMessage response = await client.GetAsync(cartApi + "/" + user.userName);
            String data = await response.Content.ReadAsStringAsync();

            var option = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            List<Cart> list = JsonSerializer.Deserialize<List<Cart>>(data, option);
            string error = "";
            int count = 0;
            foreach (var item in list)
            {
                response = await client.GetAsync($"https://localhost:5001/api/Product/{item.proId}");
                var dataProduct = await response.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<Product>(dataProduct, option);
                if (item.quantity > product.proQuantity)
                {
                    count++;
                    error += (count == 1) ? product.proName + "Sorry, product is out of stock" : ", " + product.proName + " product is out of stock";
                }
            }
            if (count != 0)
            {
                TempData["SystemNotification"] = error;
                return RedirectToAction("Index");
            }
            return View(list);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeAddress(string address)
        {
            if (address == null)
            {
                return NotFound();
            }

            string userJson = HttpContext.Session.GetString("_user");
            var user = JsonSerializer.Deserialize<User>(userJson);

            HttpResponseMessage response = await client.GetAsync(userApi + "/ChangeAddress/" + user.userName + "/" + address);

            if (response.IsSuccessStatusCode)
            {
                TempData["SystemNotification"] = "Change Address sucessfully";
                HttpResponseMessage responseSession = await client.GetAsync(authAPI + "/GetUser/" + user.userName + "/" + user.password);
                string data = await responseSession.Content.ReadAsStringAsync();
                const string _user = "_user";
                // string userDataJson = JsonConvert.SerializeObject(user);
                HttpContext.Session.SetString(_user, data);
                return RedirectToAction("Order");
            }
            TempData["SystemNotificationError"] = "There was an error when you changed your address, please try again later";
            return RedirectToAction("Order");
        }

    }
}