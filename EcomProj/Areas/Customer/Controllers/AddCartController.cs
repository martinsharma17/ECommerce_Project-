using Ecom.Utils;
using EcomProj.DataAccess.Repository.IRepository;
using EcomProj.Model;
using EcomProj.Model.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace EcomProj.Areas.Customer.Controllers
{
    [Area("Customer")]

    public class AddCartController : Controller
    {


        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]

        public ShoppingCartVM ShoppingCartVM { get; set; }

        public int OrderTotal { get; set; }

        public AddCartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.UserId == claim.Value, includeProperties:
                "Product"),
                OrderHeader = new()
            };

            foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.FixedPrice,
                    cart.Product.DiscountPrice);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }


        //for summary get
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.UserId == claim.Value, includeProperties:
                "Product"),
                OrderHeader = new()
            };


            ShoppingCartVM.OrderHeader.UserModel = _unitOfWork.UserModel.GetFirstorDefault(
                u => u.Id == claim.Value
                );

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.UserModel.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.UserModel.PhoneNumber;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.UserModel.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.UserModel.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.UserModel.PostalCode;


            foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.FixedPrice,
                    cart.Product.DiscountPrice);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }




        //for post
        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public IActionResult SummaryPOST()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.UserId == claim.Value, includeProperties:
                "Product");

            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            ShoppingCartVM.OrderHeader.PaymentStatus = SD.StatusPending;
            ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartVM.OrderHeader.UserId = claim.Value;


            foreach (var cart in ShoppingCartVM.ListCart)
            {

                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.FixedPrice,
                    cart.Product.DiscountPrice);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();

            foreach (var cart in ShoppingCartVM.ListCart)
            {
                OrderDetails orderDetails = new()
                {
                    ProductId = cart.ProductId,
                    OrderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count,
                };
                _unitOfWork.OrderDetail.Add(orderDetails);
                _unitOfWork.Save();


            }

            //stripe setting starts here
            var domain = "https://localhost:7270/";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                LineItems = new List<SessionLineItemOptions>(),

                Mode = "payment",
                SuccessUrl = domain + $"customer/AddCart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                CancelUrl = domain + $"customer/AddCart/index",
            };

            foreach (var item in ShoppingCartVM.ListCart)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                        },
                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionLineItem);

            }

            var service = new SessionService();
            Session session = service.Create(options);
            _unitOfWork.OrderHeader.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

            //stripe setting ends here

            //_unitOfWork.ShoppingCart.RemoveRange(ShoppingCartVM.ListCart);
            //_unitOfWork.Save();
            // return RedirectToAction("Index", "Home");

        }


        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstorDefault(u => u.Id == id);
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);

            //check the stripe status
            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                _unitOfWork.Save();
            }
            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.UserId ==
            orderHeader.UserId).ToList();

            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();
            return View(id);


        }




        public IActionResult Plus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstorDefault(u => u.Id == cartId);
            _unitOfWork.ShoppingCart.IncrementCount(cart, 1);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }








        public IActionResult Minus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstorDefault(u => u.Id == cartId);
            if (cart.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(cart);

            }
            else
            {
                _unitOfWork.ShoppingCart.DecrementCount(cart, 1);

            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }




        public IActionResult Remove(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstorDefault(u => u.Id == cartId);
            _unitOfWork.ShoppingCart.Remove(cart);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }


        private double GetPriceBasedOnQuantity(double quantity, double fixedPrice,double discountPrice)
        {
            if (quantity <= 5)
            {
                return fixedPrice;
            }
            else
            {
                if (quantity <= 10)
                {
                    return discountPrice;
                }
                return discountPrice;
            }

        }
    }
}
