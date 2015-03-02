using SportsStore.Domain.Abstract;
using SportsStore.WebUI.Models;
using System.Linq;
using System.Web.Mvc;

namespace SportsStore.WebUI.Controllers
{
    public class ProductController : Controller
    {
        #region --fields--

        public int PageSize = 4;
        private readonly IProductsRepository repository;

        #endregion --fields--

        #region --public methods--

        public FileContentResult GetImage(int productid)
        {
            var prod = repository.Products.FirstOrDefault(p => p.ProductID == productid);
            return prod != null ? File(prod.ImageData, prod.ImageMimeType) : null;
        }

        public ViewResult List(string category, int page = 1)
        {
            var model = new ProductListViewModel
            {
                Products = repository.Products
                    .Where(p => category == null || p.Category == category)
                    .OrderBy(p => p.ProductID)
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize),

                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = category == null ?
                        repository.Products.Count() :
                        repository.Products.Count(p => p.Category == category)
                },
                CurrentCatagory = category
            };
            return View(model);
        }

        #endregion --public methods--

        #region --ctor--

        public ProductController(IProductsRepository productRepository)
        {
            repository = productRepository;
        }

        #endregion --ctor--
    }
}