using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineShopApp.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace OnlineShopApp.Areas.Identity.Pages.Account.Manage
{
    public class ConfirmProductsModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public ConfirmProductsModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public IList<Product> UnconfirmedProducts { get; set; }

        public async Task OnGetAsync()
        {
            UnconfirmedProducts = await _db.Products
                .Include(p => p.Category)
                .Where(p => !p.IsVisible)
                .ToListAsync();
        }
    }
}
