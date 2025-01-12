using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace OnlineShopApp.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Titlul este obligatoriu")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Descrierea produsuui este obligatorie")]
        [MinLength(10, ErrorMessage = "Descrierea trebuie sa aiba cel putin 10 caractere")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Pretul este obligatoriu")]
        [Range(1, int.MaxValue, ErrorMessage = "Pretul trebuie sa fie mai mare decat 0")]
        public int Price { get; set; }
        [Required(ErrorMessage = "Stocul este obligatoriu")]
        [Range(1, int.MaxValue, ErrorMessage = "Stocul trebuie sa fie mai mare decat 0")]
        public int Stock { get; set; }
        [Required(ErrorMessage = "Imaginea este obligatorie")]
        public string? Image { get; set; }
        public bool IsVisible { get; set; }
        public double Rating { get; set; }
		[Required(ErrorMessage = "Categoria este obligatorie")]
        public int? CategoryId { get; set; }

        public string? UserId { get; set; }
        public virtual Category? Category { get; set; }
        public virtual ICollection<Review>? Reviews { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem>? Categ { get; set; }

        public virtual ApplicationUser? User { get; set; }
        public virtual ICollection<ProductCart>? ProductCarts { get; set; }
    }


}
