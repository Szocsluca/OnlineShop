using System.ComponentModel.DataAnnotations;

namespace ArticlesApp.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Continutul este obligatoriu")]
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        public string UserId { get; set; }

        public int Score { get; set; }

        public virtual ApplicationUser User { get; set; }
    }

}
