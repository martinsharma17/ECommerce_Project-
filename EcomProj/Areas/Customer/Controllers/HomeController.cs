using EcomProj.DataAccess.Data;
using EcomProj.DataAccess.Repository.IRepository;
using EcomProj.Model;
using EcomProj.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace EcomProj.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ApplicationDbContext _db;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _db = db;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {

            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category,SubCategory");
            return View(productList);

        }
        public IActionResult Details(int productId)
        {

            ShoppingCart cartObj = new()
            {
                Count = 1,
                ProductId = productId,
                //Product = _db.Products.FirstOrDefault(u => u.Id == productId)
               Product= _unitOfWork.Product.GetFirstorDefault(u => u.Id == productId, includeProperties: "Category,SubCategory")

        };
            return View(cartObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            //this claim helps to find out whether user is login or not
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.UserId = claim.Value;

            ShoppingCart cartFromDB = _unitOfWork.ShoppingCart.GetFirstorDefault(
              u => u.UserId == claim.Value && u.ProductId == shoppingCart.ProductId);

            if (cartFromDB == null)
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
            }
            else
            {
                _unitOfWork.ShoppingCart.IncrementCount(cartFromDB, shoppingCart.Count);
            }

            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }


        public IActionResult Privacy()
        {
            return View();
        }

        //search here


        public ActionResult SearchFunc(string search)
        {
            IEnumerable<Product> fineList = _unitOfWork.Product.GetAll(includeProperties: "Category,SubCategory");

            if (fineList == null)
            {
                return NotFound();
            }

            return View(fineList.Where(x => x.Name.Contains(search) || search == null).ToList());
        }



        //search ends here


        public IActionResult About()
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