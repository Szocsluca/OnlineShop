using System.ComponentModel.DataAnnotations;

namespace OnlineShopApp.Models
{
    [AtLeastOneRequired]
    public class Review
    {
        [Key]
        public int Id { get; set; }
        public string? Content { get; set; }
        public DateTime Date { get; set; }
        public int ProductId { get; set; }

        public virtual Product? Product { get; set; }

        public string? UserId { get; set; }

        public int? Score { get; set; }

        public virtual ApplicationUser? User { get; set; }
    }

}
