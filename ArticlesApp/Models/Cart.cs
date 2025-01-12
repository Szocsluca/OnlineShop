using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace OnlineShopApp.Models
{
	public class Cart
    {
        [Key] 
		public int Id { get; set; }
        public string Name { get; set; }
        public string? UserId { get; set; }
        public int TotalPrice { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public virtual ICollection<ProductCart>? ProductCarts { get; set; }
    }
}
