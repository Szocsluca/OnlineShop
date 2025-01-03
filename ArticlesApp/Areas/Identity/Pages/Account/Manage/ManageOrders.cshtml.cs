using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArticlesApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ArticlesApp.Areas.Identity.Pages.Account.Manage
{
    public class ManageOrdersModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public ManageOrdersModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public IList<Order> Orders { get; set; }

        public async Task OnGetAsync()
        {
            // Fetch all orders and include User information
            Orders = await _db.Orders
                .Include(o => o.User)
                .ToListAsync();
        }
    }
}
