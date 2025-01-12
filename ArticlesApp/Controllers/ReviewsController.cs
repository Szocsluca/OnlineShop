using OnlineShopApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace OnlineShopApp.Controllers
{
    public class ReviewsController : Controller
	{
		private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
        public ReviewsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
		{
			db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

		[HttpPost]
		[Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult Delete(int id)
		{
			Review rev = db.Reviews.Find(id);
            if(rev.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Reviews.Remove(rev);
                db.SaveChanges();
                return Redirect("/Products/Show/" + rev.ProductId);
            }
            else
            {
				TempData["message"] = "Nu aveti dreptul sa stergeti review-ul!";
				TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index","Products");
            }
        }

		[Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult Edit(int id)
		{
			Review rev = db.Reviews.Find(id);
			if(rev.UserId == _userManager.GetUserId(User))
            {
                return View(rev);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa editati review-ul!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Products");
            }
        }

		[HttpPost]
        [Authorize(Roles = "User,Colaborator,Admin")]
        public IActionResult Edit(int id, Review requestReview)
		{
			Review rev = db.Reviews.Find(id);
			if(rev.UserId == _userManager.GetUserId(User))
            {
                if(ModelState.IsValid)
                {
                    rev.Content = requestReview.Content;
                    rev.Score = requestReview.Score;
                    db.SaveChanges();
                    return Redirect("/Products/Show/" + rev.ProductId);
                }
                else
                {
                    return View(requestReview);
                }
                
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa editati review-ul!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Products");
            }
        }
    }
}