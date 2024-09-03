using System.ComponentModel.DataAnnotations;

namespace Authorization_Authentication.Products
{
    public class Product
    {

        public int UserId { get; set; }
        public string Id { get; set; }

        [Required(ErrorMessage = "Name pf product is required")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "No Of Product is required")]
        public int NoOfProduct { get; set; }
    }
}
