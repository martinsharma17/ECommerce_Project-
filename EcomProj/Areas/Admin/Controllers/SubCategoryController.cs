using EcomProj.DataAccess.Data;
using EcomProj.Model;
using Microsoft.AspNetCore.Mvc;

namespace EcomProj.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        //just the index and return all category
        public IActionResult Index()
        {
            IEnumerable<SubCategory> subCategories = _db.SubCategories;
            return View(subCategories);
        }

        //get method

        public IActionResult Create()
        {
            return View();

        }

        [HttpPost]
        public IActionResult Create(SubCategory obj)
        {
            if (ModelState.IsValid)
            {
                _db.SubCategories.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }


        //get method
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subCategory = _db.SubCategories.Find(id);
            if (subCategory == null)
            {
                return NotFound();
            }
            return View(subCategory);
        }


        [HttpPost]

        public IActionResult Edit(SubCategory obj)
        {
            if (ModelState.IsValid)
            {
                _db.SubCategories.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }


        //get method for delete

        public IActionResult Delete(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            var subCategory = _db.SubCategories.Find(id);
            if (subCategory == null)
            {
                return NotFound();
            }
            return View(subCategory);

        }

        [HttpPost]
        public IActionResult DeletePost(int? id)
        {
            var obj = _db.SubCategories.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.SubCategories.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
