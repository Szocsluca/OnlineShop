using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace ArticlesApp.Models
{
	public class Order
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public DateTime Date { get; set; }
		public int TotalPrice { get; set; }
	}
}
