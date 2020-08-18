using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebMVC.Data;
using WebMVC.Models.ViewModels;

namespace WebMVC.Controllers
{
    public class StoreItemController : Controller
    {
        //dependecy injection
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnviroment;

        //use to pass al propreties
        [BindProperty]
        public StoreItemViewModel StoreItemVM { get; set; }


        public StoreItemController(ApplicationDbContext db, IWebHostEnvironment hostingEnviroment)
        {
            _db = db;
            _hostingEnviroment = hostingEnviroment;
            StoreItemVM = new StoreItemViewModel()
            {
                Category = _db.Category,
                StoreItem = new Models.StoreItem()

            };
        }
        public async Task<IActionResult> Index()
        {
            var storeItems = await _db.StoreItem.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync();

            return View(storeItems);
        }


        //GET - CREATE
        public IActionResult Create()
        {
            return View(StoreItemVM);
        }


    }

}