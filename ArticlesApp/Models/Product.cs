using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace ArticlesApp.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Titlul este obligatoriu")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Continutul articolului este obligatoriu")]
        public string Description { get; set; }
        //public Image Image { get; set; }
        public int Price { get; set; }
        public int Stock { get; set; }

        public bool IsVisible { get; set; }
        public int Rating { get; set; }
		[Required(ErrorMessage = "Categoria este obligatorie")]
        public int CategoryId { get; set; }

        public string? UserId { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem> Categ { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<ProductCart>? ProductCarts { get; set; }
    }


}
