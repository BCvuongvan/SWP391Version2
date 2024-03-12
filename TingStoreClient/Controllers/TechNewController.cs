using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TingStoreClient.Filters;

namespace TingStoreClient.Controllers
{
    [StaffAuthenticationRedirect]
    [Route("[controller]")]
    public class TechNewController : Controller
    {
        [HttpGet("create")]
        public IActionResult CreateTechNew()
        {
            return View();
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTechNew(string title, IFormFile image, string link)
        {
            if (image != null && image.Length > 0)
            {
                var newsId = Guid.NewGuid().ToString();
                await SaveTechNew(title, image, newsId, link);
            }
            return RedirectToAction("ListTechNew");
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
        [HttpGet("list")]
        public IActionResult ListTechNew()
        {
            var newListTechNew = ListTechNews();
            return View(newListTechNew);
        }

        [HttpGet("updatetechnew/{NewsId}")]
        public IActionResult UpdateTechNew(string NewsId)
        {
            var newsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/TechNews", NewsId);
            var titlePath = Path.Combine(newsDir, "title.txt");
            var linkPath = Path.Combine(newsDir, "link.txt");
            if (!System.IO.File.Exists(titlePath) || !System.IO.File.Exists(linkPath))
            {
                return NotFound();
            }

            var title = System.IO.File.ReadAllText(titlePath);
            var link = System.IO.File.ReadAllText(linkPath);

            ViewBag.TitleTechNew = title;
            ViewBag.LinkTechNew = link;
            ViewBag.NewsId = NewsId;
            return View();
        }

        [HttpPost("updatetechnew/{NewsId}")]
        public async Task<IActionResult> UpdateTechNew(string newsId, string title, string link, IFormFile newImageTechNew)
        {
            var newsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/TechNews", newsId);
            if (!Directory.Exists(newsDir))
            {
                NotFound();
            }
            if (newImageTechNew != null && newImageTechNew.Length > 0)
            {
                var imagePath = Path.Combine(newsDir, "image.jpg");
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                using (var fileStream = new FileStream(imagePath, FileMode.Create, FileAccess.Write))
                {

                    await newImageTechNew.CopyToAsync(fileStream);
                }
            }

            var titlePath = Path.Combine(newsDir, "title.txt");
            var linkPath = Path.Combine(newsDir, "link.txt");
            System.IO.File.WriteAllText(titlePath, title);
            await System.IO.File.WriteAllTextAsync(linkPath, link);
            return RedirectToAction("ListTechNew");
        }

        [HttpGet("deletetechnew/{NewsId}")]
        public IActionResult DeleteTechNew(string NewsId)
        {
            var newsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/TechNews", NewsId);
            var titlePath = Path.Combine(newsDir, "title.txt");
            var linkPath = Path.Combine(newsDir, "link.txt");
            if (!System.IO.File.Exists(titlePath) || !System.IO.File.Exists(linkPath))
            {
                return NotFound();
            }

            var title = System.IO.File.ReadAllText(titlePath);
            var link = System.IO.File.ReadAllText(linkPath);

            ViewBag.TitleTechNew = title;
            ViewBag.LinkTechNew = link;
            ViewBag.NewsId = NewsId;
            return View();
        }

        [HttpPost("deletetechnew")]
        public IActionResult Delete(string NewsId)
        {
            var newsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/TechNews", NewsId);
            if (Directory.Exists(newsDir))
            {
                Directory.Delete(newsDir, true);
            }

            return RedirectToAction("ListTechNew");
        }

    }
}
