using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace ArticlesApp.Models
{
	public class Order
	{
		public int Id { get; set; }
		public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        public DateTime Date { get; set; }
		public int TotalPrice { get; set; }
	}
}
