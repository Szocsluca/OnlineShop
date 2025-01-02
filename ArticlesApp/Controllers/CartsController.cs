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

        private readonly RoleManager<IdentityRole> _roleManager;

        public CartsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }

        [Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            SetAccessRights();

            var carts = from cart in db.Carts.Include("User")
                               .Where(b => b.UserId == _userManager.GetUserId(User))
                            select cart;

            foreach (var cart in carts)
            {
                cart.TotalPrice = GetTotalPrice(cart.Id);
            }

            ViewBag.Carts = carts;

            return View();
        }

        [Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult Show(int id)
        {
            SetAccessRights();

            var carts = db.Carts
                                  .Include("ProductCarts.Product.Category")
                                  .Include("ProductCarts.Product.User")
                                  .Include("User")
                                  .Where(b => b.Id == id)
                                  .Where(b => b.UserId == _userManager.GetUserId(User))
                                  .FirstOrDefault();
            carts.TotalPrice = GetTotalPrice(carts.Id);
            if (carts == null)
            {
                TempData["message"] = "Resursa cautata nu poate fi gasita";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Articles");
            }

            return View(carts);
        }

        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult New(Cart c)
        {
            c.UserId = _userManager.GetUserId(User);
            c.TotalPrice = 0;

            db.Carts.Add(c);
            db.SaveChanges();
            TempData["message"] = "Cosul a fost adaugat";
            TempData["messageType"] = "alert-success";
            return RedirectToAction("Index");
        }

        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            if (User.IsInRole("Editor") || User.IsInRole("User"))
            {
                ViewBag.AfisareButoane = true;
            }

            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }

        private int GetTotalPrice(int cartId)
        {
            var totalPrice = 0;
            var productCarts = db.ProductCarts
                .Where(b => b.CartId == cartId)
                .Include("Product")
                .ToList();
            foreach (var productCart in productCarts)
            {
                totalPrice += productCart.Product.Price * productCart.Quantity;
            }
            return totalPrice;
        }
    }
}
