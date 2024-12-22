using System.ComponentModel.DataAnnotations.Schema;

namespace ArticlesApp.Models
{
    public class ProductCart
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public int? CartId { get; set; }
        public virtual Cart? Cart { get; set; }
        public virtual Product? Product { get; set; }
        public int Quantity { get; set; }
    }
}
