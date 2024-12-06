using Microsoft.AspNetCore.Identity;

namespace ArticlesApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Review>? Reviews{ get; set; }
        public virtual ICollection<Product>? Products { get; set; }

        public virtual Cart? Carts { get; set; }

    }
}
