using EcomProj.DataAccess.Data;
using EcomProj.DataAccess.Repository.IRepository;
using EcomProj.Model;
using EcomProj.Model.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace EcomProj.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductController(ApplicationDbContext db, IWebHostEnvironment hostEnvironment,IUnitOfWork unitOfWork)
        {
            _db = db;
            _hostEnvironment = hostEnvironment;
            _unitOfWork = unitOfWork;
        }



        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                Product = new(),
                CategoryList = _db.Categories.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()

                }),
                SubCategoryList = _db.SubCategories.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            if(id== null || id == 0)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = _db.Products.FirstOrDefault(x=>x.Id==id);
                return View(productVM);

            }
        }


        //POST
        [HttpPost]                 //used to handle the http request
        [ValidateAntiForgeryToken] //used to prevent the cross site request forgery attack
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {
          
                //for creating firstproduct
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\products");
                    var extention = Path.GetExtension(file.FileName);

                    if (obj.Product.Image != null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Product.Image.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extention), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    obj.Product.Image = @"\images\products\" + fileName + extention;
                }
                if (obj.Product.Id == 0)
                {
                    _db.Products.Add(obj.Product);

                }
                else
                {
                    _db.Products.Update(obj.Product);


                }   //
                //for creating firstproduct
                _db.SaveChanges();        //    
                TempData["success"] = "Product Created Successfully";

                return RedirectToAction("Index");
              }



        #region API CALLS
        [HttpGet]

        public IActionResult GetAll()
        {
            //var productList = _db.Products;
            //return Json(new { data = productList });

            var productList = _unitOfWork.Product.GetAll(includeProperties: "Category,SubCategory");
            return Json(new { data = productList });


        }
        //delete
        [HttpDelete]                 //used to handle the http request
        public IActionResult Delete(int? id)
        {

            var obj = _unitOfWork.Product.GetFirstorDefault(u => u.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.Image.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _unitOfWork.Product.Remove(obj);    //
            _unitOfWork.Save();         //
            return Json(new { success = true, message = "Successfully  deleted" });

        }
        #endregion


    }

}

