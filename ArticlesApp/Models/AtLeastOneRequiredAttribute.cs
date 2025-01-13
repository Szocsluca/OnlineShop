using System.ComponentModel.DataAnnotations;
namespace OnlineShopApp.Models
{
    public class AtLeastOneRequiredAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var review = (Review)validationContext.ObjectInstance;
            if (string.IsNullOrEmpty(review.Content) && review.Score == null)
            {
                return new ValidationResult("Cel putin un camp este obligatoriu");
            }
            return ValidationResult.Success;
        }
    }
}
