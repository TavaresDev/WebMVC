using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebMVC.Data;
using WebMVC.Models;
using WebMVC.Models.ViewModels;

namespace WebMVC.Controllers
{
    public class HomeController : Controller
    {

        //Dependency Injection
        private readonly ApplicationDbContext _db;
        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }


        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        public async Task<IActionResult> Index()
        {
            //query the data
            IndexViewModel IndexVM = new IndexViewModel()
            {
                StoreItem = await _db.StoreItem.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync(),
                Category = await _db.Category.ToListAsync(),
                Coupon = await _db.Coupon.Where(c => c.IsActive == true).ToListAsync()
            };
            //Pass data to view
            return View(IndexVM);
        }

        // API call Get: /Shop
        [Route("api/Categories")]
        public IActionResult GetCategories()
        {
            var categories = _db.Category.OrderBy(c => c.Name).ToList();
            return Json(new { categories });
        }

        //Api cal to change the products with js
        [Route("api/Categories/{Id:int}")]
        public IActionResult GetProductsByCategory(int Id)
        {
            //store the selctes category name in the viewbag so we can display in the view heading
            //ViewBag.Category = catId;

            // get the list of products for the slected category and pass the list to the view
            var products = _db.StoreItem.Include(m => m.Category).Where(p => p.Category.Id == Id).OrderBy(p => p.Name).ToList();
            return Json(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
