using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebMVC.Data;
using WebMVC.Models;
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

            //Work on the image save section - the image name need to be unique

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
                StoreItemFromDb.Image = @"\images\StoreImages\" + StoreItemVM.StoreItem.Id + ".jpg";

            }
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        //GET - EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            StoreItemVM.StoreItem = await _db.StoreItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);
            StoreItemVM.SubCategory = await _db.SubCategory.Where(s => s.CategoryId == StoreItemVM.StoreItem.CategoryId).ToListAsync();

            if(StoreItemVM.StoreItem == null)
            {
                return NotFound();
            }
            return View(StoreItemVM);
        }

        //POST - EDIT
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            //get the missing data, one populate from js
            StoreItemVM.StoreItem.SubCategoryId = Convert.ToInt32(Request.Form["SubcategoryId"].ToString());

            if (!ModelState.IsValid)
            {
                //re-populate the subcategory info
                StoreItemVM.SubCategory = await _db.SubCategory.Where(s => s.CategoryId == StoreItemVM.StoreItem.CategoryId).ToListAsync();
                return View(StoreItemVM);
            }

            //Work on the image save section -   //the image name need to be unique

            string webRootPath = _hostingEnviroment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var storeItemFromDb = await _db.StoreItem.FindAsync(StoreItemVM.StoreItem.Id);

            if (files.Count > 0)
            {
                //New Image has been Uploaded
                var uploads = Path.Combine(webRootPath, "images/StoreImages");
                var extension_new = Path.GetExtension(files[0].FileName);


                //Delete the OG file
                if(storeItemFromDb.Image != null)
                {
                    var imagePath = Path.Combine(webRootPath, storeItemFromDb.Image.TrimStart('\\'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
                //we will Upload the new file
                using (var filesStream = new FileStream(Path.Combine(uploads, StoreItemVM.StoreItem.Id + extension_new), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);

                }
                storeItemFromDb.Image = @"\images\StoreImages\" + StoreItemVM.StoreItem.Id + extension_new;

            }

            storeItemFromDb.Name = StoreItemVM.StoreItem.Name;
            storeItemFromDb.Description = StoreItemVM.StoreItem.Description;
            storeItemFromDb.Price = StoreItemVM.StoreItem.Price;
            storeItemFromDb.Quantity = StoreItemVM.StoreItem.Quantity;
            storeItemFromDb.CategoryId = StoreItemVM.StoreItem.CategoryId;
            storeItemFromDb.SubCategoryId = StoreItemVM.StoreItem.SubCategoryId;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        //GET - Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            StoreItemVM.StoreItem = await _db.StoreItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);
           
            if (StoreItemVM.StoreItem == null)
            {
                return NotFound();
            }
            return View(StoreItemVM);
        }

        //GET - DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            StoreItemVM.StoreItem = await _db.StoreItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);
            
            if (StoreItemVM.StoreItem == null)
            {
                return NotFound();
            }
            return View(StoreItemVM);
        }

        //POST - DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {

            string webRootPath = _hostingEnviroment.WebRootPath;
            StoreItem storeItem = await _db.StoreItem.FindAsync(id);

            if(storeItem != null)
            {
                var imagePath = Path.Combine(webRootPath, storeItem.Image.TrimStart('\\'));
                //Delete the file
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                _db.StoreItem.Remove(storeItem);
                await _db.SaveChangesAsync();
            }        

            return RedirectToAction(nameof(Index));
        }

    }

}