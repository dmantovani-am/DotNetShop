namespace DotNetShop.Pages
{
    public class IndexModel : PageModel
    {
        readonly IProductRepository _productRepository;

        public IEnumerable<Product> Products { get; set; }

        public IndexModel(IProductRepository productRepository)
        {
            _productRepository = productRepository;

            Products = Enumerable.Empty<Product>();
        }

        public async Task OnGet(int n = 9)
        {
            Products = await _productRepository.GetTopProducts(n);
        }
    }
}
