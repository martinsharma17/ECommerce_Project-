using EcomProj.DataAccess.Data;
using EcomProj.Model;
using Microsoft.AspNetCore.Mvc;

namespace EcomProj.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        //just the index and return all category
        public IActionResult Index()
        {
            IEnumerable<Category> categories = _db.Categories;
            return View(categories);
        }

        //get method

        public IActionResult Create()
        {
            return View();

        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (ModelState.IsValid)
            {
                _db.Categories.Add(obj);
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
            var category = _db.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }


        [HttpPost]

        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _db.Categories.Update(obj);
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
            var category = _db.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);

        }

        [HttpPost]
        public IActionResult DeletePost(int? id)
        {
            var obj = _db.Categories.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.Categories.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
