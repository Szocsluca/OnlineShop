using ArticlesApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ArticlesApp.Controllers
{
    public class ProductsController : Controller
	{
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
		{
            int unconfirmedCount =db.Products.Count(p => !p.IsVisible);
            ViewBag.UnconfirmedCount = unconfirmedCount;
            var products = db.Products.Include("Category");
			ViewBag.Products = products;
			if (TempData.ContainsKey("message"))
			{
				ViewBag.Message = TempData["message"];
			}
			return View();
		}
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Confirm()
        {
            var products = db.Products.Include("Category");
            ViewBag.Products = products;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }
        [HttpPost]
        public IActionResult ConfirmAddProduct(int id)
        {
            var product = db.Products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound(); 
            }
            product.IsVisible = true;
            db.SaveChanges();
            ViewBag.Message = "Product visibility confirmed successfully!";
            return RedirectToAction("Index"); 
        }

		[HttpGet]
        public IActionResult Show(int id)
		{
			Product product = db.Products.Include("Category").Include("Reviews")
				.Include("User").Include("Reviews.User")
                              .Where(art => art.Id == id)
							  .First();
			SetAccessRights();
			if(TempData.ContainsKey("message"))
			{
                ViewBag.Message = TempData["message"];
				ViewBag.Alert = TempData["messageType"];
            }
                return View(product);
		}

		[HttpPost]
		[Authorize(Roles ="User,Colaborator,Admin")]
        public IActionResult Show([FromForm] Review review)
		{
			review.Date = DateTime.Now;
            review.UserId = _userManager.GetUserId(User);
			var existingReview = db.Reviews.Where(r => r.ProductId == review.ProductId && r.UserId == review.UserId).FirstOrDefault();

            if (existingReview != null)
			{
				TempData["message"] = "Ai lasat deja un review pentru acest produs!";
                TempData["messageType"] = "alert-danger";
				return Redirect("/Products/Show/" + review.ProductId);
            }

             if (existingReview == null)
			{
                db.Reviews.Add(review);
                db.SaveChanges();
				return Redirect("/Products/Show/" + review.ProductId);
            }
            else
			{
				Product prod = db.Products.Include("Category")
					.Include("Reviews").Include("Reviews.User")
                    .Where(p => p.Id == review.ProductId).First();

                SetAccessRights();
                return View(prod);

            }
        }

        [Authorize(Policy = "ColaboratorOnly")]
        public IActionResult New()
		{
			Product product = new Product();
			product.Categ = GetAllCategories();
			return View(product);
		}

		[HttpPost]
		public IActionResult New(Product product)
		{
			product.Categ = GetAllCategories();
			try
			{
				db.Products.Add(product);
				db.SaveChanges();
				TempData["message"] = "Produsul a fost adaugat";
				return RedirectToAction("Index");
			}
			catch (Exception)
			{
				return View(product);
			}
		}

		public IActionResult Edit(int id)
		{

			Product product = db.Products.Include("Category")
										 .Where(art => art.Id == id)
										 .First();
			product.Categ = GetAllCategories();
			return View(product);
		}

		[HttpPost]
		public IActionResult Edit(int id, Product requestProduct)
		{
			Product product = db.Products.Find(id);
			try
			{
				product.Title = requestProduct.Title;
				product.Description = requestProduct.Description;
				product.Price = requestProduct.Price;
				product.Stock = requestProduct.Stock;
				product.Rating = requestProduct.Rating;
				//product.Image = requestProduct.Image;
				product.CategoryId = requestProduct.CategoryId;
				db.SaveChanges();
				TempData["message"] = "Produsul a fost modificat";
				return RedirectToAction("Index");
			}
			catch (Exception e)
			{
				requestProduct.Categ = GetAllCategories();
				return View(requestProduct);
			}
		}

		[HttpPost]
		public ActionResult Delete(int id)
		{
			Product product = db.Products.Find(id);
			db.Products.Remove(product);
			db.SaveChanges();
			TempData["message"] = "Produsul a fost sters";
			return RedirectToAction("Index");
		}

		private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;
            ViewBag.UserCurent = _userManager.GetUserId(User);
			ViewBag.EsteAdmin = User.IsInRole("Admin");
        }

        [NonAction]
		public IEnumerable<SelectListItem> GetAllCategories()
		{
			// generam o lista de tipul SelectListItem fara elemente
			var selectList = new List<SelectListItem>();

			// extragem toate categoriile din baza de date
			var categories = from cat in db.Categories
							 select cat;

			// iteram prin categorii
			foreach (var category in categories)
			{
				// adaugam in lista elementele necesare pentru dropdown
				// id-ul categoriei si denumirea acesteia
				selectList.Add(new SelectListItem
				{
					Value = category.Id.ToString(),
					Text = category.CategoryName
				});
			}

			// returnam lista de categorii
			return selectList;
		}
	}
}