using ArticlesApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace ArticlesApp.Controllers
{
    public class ProductsController : Controller
	{
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _env;
        public ProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, IWebHostEnvironment env)
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _env = env;
        }

        public IActionResult Index(List<int> categoryIds)
        {
            ViewBag.Categories = db.Categories.ToList();

   
            IQueryable<Product> products = db.Products.Include("Category");
            if (categoryIds != null && categoryIds.Count > 0)
            {
                products = products.Where(p => categoryIds.Contains(p.CategoryId));
            }

            ViewBag.Products = products.ToList();

   
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ProductListPartial", ViewBag.Products);
            }
            int unconfirmedCount = db.Products.Count(p => !p.IsVisible);
            ViewBag.UnconfirmedCount = unconfirmedCount;

            var pproducts = db.Products.ToList();
            ViewBag.Products = pproducts;


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
        [HttpPost]
        public IActionResult DeleteProductRequest(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            TempData["message"] = "Cererea a fost respinsa";
            return RedirectToAction("Confirm");
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
        public async Task<IActionResult> New(Product product, IFormFile Image)
		{
			if (Image != null && Image.Length > 0)
			{
				var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
				var fileExtension = Path.GetExtension(Image.FileName).ToLower();
				if (!allowedExtensions.Contains(fileExtension))
				{
					ModelState.AddModelError("ProductImage", "Fisierul trebuie sa fie o imagine (jpg, jpeg, png).");
					return View(product);
				}
				var storagePath = Path.Combine(_env.WebRootPath, "images", Image.FileName);
				var databaseFileName = "/images/" + Image.FileName;
                using (var stream = new FileStream(storagePath, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }

                ModelState.Remove(nameof(product.Image));
				product.Image = databaseFileName;
			}
			Console.WriteLine(product.Title);
            if (TryValidateModel(product))
            {    
				db.Products.Add(product);
				db.SaveChanges();
                TempData["message"] = "Produsul a fost trimis la confirmare";
                return RedirectToAction("Index");
            }
            product.Categ = GetAllCategories();
            return View(product);
        }
        [Authorize(Policy = "ColaboratorOnly")]
        public IActionResult Edit(int id)
		{

			Product product = db.Products.Include("Category")
										 .Where(art => art.Id == id)
										 .First();
			product.Categ = GetAllCategories();
			return View(product);
		}


        //--------------------------------------------------------------------------------
        [HttpPost]
        [Authorize(Policy = "ColaboratorOnly")]
        public IActionResult Edit(int id, Product requestProduct, IFormFile uploadedImage)
        {
            Product product = db.Products.Find(id);
            try
            {
                // Update product properties
                product.Title = requestProduct.Title;
                product.Description = requestProduct.Description;
                product.Price = requestProduct.Price;
                product.Stock = requestProduct.Stock;
                product.Rating = requestProduct.Rating;
                product.CategoryId = requestProduct.CategoryId;
                product.IsVisible = false;


                if (uploadedImage != null && uploadedImage.Length > 0)
                {
             
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadedImage.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        uploadedImage.CopyTo(stream);
                    }
                    product.Image = "/images/" + fileName;
                }
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