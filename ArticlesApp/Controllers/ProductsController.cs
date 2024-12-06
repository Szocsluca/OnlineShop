using ArticlesApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ArticlesApp.Controllers
{
    public class ProductsController : Controller
	{
		private readonly ApplicationDbContext db;
		public ProductsController(ApplicationDbContext context)
		{
			db = context;
		}

		public IActionResult Index()
		{
			var products = db.Products.Include("Category");
			ViewBag.Products = products;
			if (TempData.ContainsKey("message"))
			{
				ViewBag.Message = TempData["message"];
			}
			return View();
		}

		public IActionResult Show(int id)
		{
			Product product = db.Products.Include("Category").Include("Reviews")
							  .Where(art => art.Id == id)
							  .First();
			return View(product);
		}

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