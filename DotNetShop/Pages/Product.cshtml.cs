using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotNetShop.Pages
{
    public class ProductModel : PageModel
    {
        readonly IProductRepository _productRepository;

        public Product? Product { get; set; }

        public ProductModel(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IActionResult> OnGet(int id)
        {
            var product = Product = await _productRepository.Get(id);

            return product switch
            {
                null => NotFound(),
                _ => Page(),
            };
        }
    }
}
