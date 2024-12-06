using ArticlesApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace ArticlesApp.Controllers
{
    public class ReviewsController : Controller
	{
		private readonly ApplicationDbContext db;
		public ReviewsController(ApplicationDbContext context)
		{
			db = context;
		}

		[HttpPost]
		public IActionResult New(Review rev)
		{
			rev.Date = DateTime.Now;

			try
			{
				db.Reviews.Add(rev);
				db.SaveChanges();
				return Redirect("/Products/Show/" + rev.ProductId);
			}

			catch (Exception)
			{
				return Redirect("/Products/Show/" + rev.ProductId);
			}

		}

		[HttpPost]
		public IActionResult Delete(int id)
		{
			Review rev = db.Reviews.Find(id);
			db.Reviews.Remove(rev);
			db.SaveChanges();
			return Redirect("/Products/Show/" + rev.ProductId);
		}

		public IActionResult Edit(int id)
		{
			Review rev = db.Reviews.Find(id);
			ViewBag.Review = rev;
			return View();
		}

		[HttpPost]
		public IActionResult Edit(int id, Review requestReview)
		{
			Review rev = db.Reviews.Find(id);
			try
			{
				rev.Content = requestReview.Content;
				db.SaveChanges();
				return Redirect("/Products/Show/" + rev.ProductId);
			}
			catch (Exception e)
			{
				return Redirect("/Products/Show/" + rev.ProductId);
			}
		}
	}
}