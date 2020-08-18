using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebMVC.Data;
using WebMVC.Models.ViewModels;
using WebMVC.Utility;

namespace WebMVC.Controllers
{
    public class StoreItemController : Controller
    {
        //dependecy injection
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnviroment;

        //use to pass all propreties of the storeitemVM whithout need to pass paramethers in every method
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

        //POST - CREATE
        [HttpPost,ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
            //get the missing data, one populate from js
            StoreItemVM.StoreItem.SubCategoryId = Convert.ToInt32(Request.Form["SubcategoryId"].ToString());

            if (!ModelState.IsValid) 
            {
                return View(StoreItemVM);
            }

            _db.StoreItem.Add(StoreItemVM.StoreItem);
            await _db.SaveChangesAsync();


            //Work on the image save section
            //the image name need to be unique

            string webRootPath = _hostingEnviroment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var StoreItemFromDb = await _db.StoreItem.FindAsync(StoreItemVM.StoreItem.Id);

            if (files.Count > 0)
            {
                //files upload
                var uploads = Path.Combine(webRootPath, "images/StoreImages");
                var extension = Path.GetExtension(files[0].FileName);

                using (var filesStream = new FileStream(Path.Combine(uploads,StoreItemVM.StoreItem.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);

                }
                StoreItemFromDb.Image = @"\images\StoreImages\" + StoreItemVM.StoreItem.Id + extension;

            }
            else
            {
                //nofiles was upload
                var uploads = Path.Combine(webRootPath, @"images\StoreImages\" + SD.DefaultItemImage);
                System.IO.File.Copy(uploads, webRootPath + @"\images\StoreImages\" + StoreItemVM.StoreItem.Id + ".jpg");
                StoreItemFromDb.Image = @"\images\StoreImages" + StoreItemVM.StoreItem.Id + ".jpg";

            }
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }

}