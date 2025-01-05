using ArticlesApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArticlesApp.Controllers
{
    [Authorize]
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            db = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);

            var cart = db.Carts
                .Include(c => c.ProductCarts)
                .ThenInclude(pc => pc.Product)
                .FirstOrDefault(c => c.UserId == userId);
            ViewBag.Carts = new List<Cart> { cart };
            ViewBag.TotalPrice = cart.ProductCarts.Sum(pc => pc.Product.Price * pc.Quantity);

            return View();
        }
        //Functie care actualizeaza direct din cosul de cumparaturi cantitatea produsului ( in functie de stocul disponibil )
        [HttpPost]
        [Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult UpdateQuantity(int CartId, int ProductId, int Quantity)
        {
            var productCart = db.ProductCarts
                .Include(pc => pc.Product)
                .FirstOrDefault(pc => pc.CartId == CartId && pc.ProductId == ProductId);

            if (productCart != null)
            {
                
                if (Quantity > productCart.Product.Stock)
                {
                    TempData["message"] = $"Stock limit exceeded! Only {productCart.Product.Stock} items available.";
                    TempData["messageType"] = "alert-warning";
                }
                else if (Quantity > 0)
                {
                    productCart.Quantity = Quantity;
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }

        //Functie care sterge un produs din cosul de cumparaturi
        [HttpPost]
        [Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult RemoveProduct(int CartId, int ProductId)
        {
            var productCart = db.ProductCarts
                .FirstOrDefault(pc => pc.CartId == CartId && pc.ProductId == ProductId);

            if (productCart != null)
            {
                db.ProductCarts.Remove(productCart);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        //Functie care finalizeaza comanda. Prin finalizare se scade stocul produselor din cosul de cumparaturi si se adauga o noua comanda in baza de date
        [HttpPost]
        [Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult FinalizeOrder()
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                TempData["message"] = "Invalid UserId. Unable to process the order.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            var cart = db.Carts
                .Include(c => c.ProductCarts)
                .ThenInclude(pc => pc.Product)
                .FirstOrDefault(c => c.UserId == userId);

            if (cart == null || !cart.ProductCarts.Any())
            {
                TempData["message"] = "Cart is empty!";
                TempData["messageType"] = "alert-warning";
                return RedirectToAction("Index");
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    int totalPrice = 0;

                    foreach (var productCart in cart.ProductCarts)
                    {
                        var product = db.Products.FirstOrDefault(p => p.Id == productCart.ProductId);

                        if (product == null)
                        {
                            TempData["message"] = $"Product {productCart.Product.Title} no longer exists.";
                            TempData["messageType"] = "alert-danger";
                            return RedirectToAction("Index");
                        }

                        if (product.Stock < productCart.Quantity)
                        {
                            TempData["message"] = $"Insufficient stock for {product.Title}. Available stock: {product.Stock}.";
                            TempData["messageType"] = "alert-danger";
                            return RedirectToAction("Index");
                        }

                        product.Stock -= productCart.Quantity;
                        totalPrice += product.Price * productCart.Quantity;
                    }

                    var order = new Order
                    {
                        UserId = userId,
                        Date = DateTime.Now,
                        TotalPrice = totalPrice
                    };

                    db.Orders.Add(order);
                    cart.ProductCarts.Clear();
                    db.SaveChanges();

                    transaction.Commit();

                
                    return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    TempData["message"] = "An error occurred while finalizing the order. Please try again.";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index");
                }
            }
        }


        [Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult OrderConfirmation(int orderId)
        {
            var order = db.Orders
                .Include(o => o.User)
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                TempData["message"] = "Order not found!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            return View(order);
        }





    }
}
