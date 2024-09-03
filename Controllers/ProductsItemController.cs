using Authorization_Authentication.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authorization_Authentication.Controllers
{
    public class ProductsItemController : Controller
    {
        private readonly IProductServices _productService;

        public ProductsItemController(IProductServices _ProductService)
        {
            this._productService = _ProductService;
        }

        [Authorize(Roles ="RegularUser")]
        public IActionResult Index()
        {
            IEnumerable<Product> products =  _productService.getAllProduct();
            return View(products);
        }


    }
}
