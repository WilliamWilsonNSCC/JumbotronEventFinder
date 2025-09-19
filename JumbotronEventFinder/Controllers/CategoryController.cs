using JumbotronEventFinder.Models;
using Microsoft.AspNetCore.Mvc;

namespace JumbotronEventFinder.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            List<Category> categories = new List<Category>
            {
                new() //Concerts
                {
                   CategoryId = 1,
                   Title = "Concerts",
                },
                new()//Family Events
                {
                   CategoryId = 2,
                   Title = "Family Events",
                },
                new() //Sports
                {
                   CategoryId = 3,
                   Title = "Sports",
                },
            };
            return View(categories);
        }
    }
}
