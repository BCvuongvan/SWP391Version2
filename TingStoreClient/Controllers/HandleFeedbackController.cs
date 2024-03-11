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

namespace TingStoreClient.Controllers
{
    public class HandleFeedbackController : Controller
    {
        private readonly HttpClient client = null;
        private string productAPI;
        private string api;

        public HandleFeedbackController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            this.productAPI = "https://localhost:5001/api/Product";
            this.api = "https://localhost:5001/api/Feedback";
        }

        public async Task<IActionResult> GetListProduct()
        {
            string userJson = HttpContext.Session.GetString("_user");
            var user = JsonSerializer.Deserialize<User>(userJson);

            HttpResponseMessage response = await client.GetAsync(api + "/" + user.userName);
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                var user1 = JsonSerializer.Deserialize<User>(data);
                return View(user1);
            }
            return NotFound();
        }


        public async Task<IActionResult> CreateFeedback(int id)
        {
            string userJson = HttpContext.Session.GetString("_user");
            var user = JsonSerializer.Deserialize<User>(userJson);

            HttpResponseMessage response = await client.GetAsync(api + "/GetFeedbackByProId/" + user.userName + "/" + id);
            System.Console.WriteLine(productAPI + "/GetFeedbackByProId/" + user.userName + "/" + id);
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                System.Console.WriteLine(data);
                var feedback = JsonSerializer.Deserialize<Feedback>(data);
                return View(feedback);
            }
            Feedback f = new Feedback(user.userName, id, null, 0);
            return View(f);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFeedback(Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                string userJson = HttpContext.Session.GetString("_user");
                var user = JsonSerializer.Deserialize<User>(userJson);
                feedback.userName = user.userName;

                String data = JsonSerializer.Serialize(feedback);//chuyển dữ liệu custom dạng Json
                var content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");// định nghĩa nội dung gửi đi có : dữ liệu, định dạng mã utf-8, loại dữ liệu gửi đi
                HttpResponseMessage response = await client.PostAsync(api, content); // gửi dữ liệu đi
                if (response.IsSuccessStatusCode)// nhận phản hồi về sau khi gửi dữ liệu đi
                {
                    return RedirectToAction("GetListProduct");
                }
            }
            return View(feedback);
        }
    }
}