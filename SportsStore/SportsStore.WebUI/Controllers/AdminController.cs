using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsStore.WebUI.Controllers
{
    //The authorize filter can be added to methods or classes as a whole. When added to the class like this it applies to every method in the class automatically.
    [Authorize]
    public class AdminController : Controller
    {
        #region --fields--

        private IProductsRepository repository;

        #endregion --fields--

        #region --public methods--

        public ActionResult Create()
        {
            return View("Edit", new Product());
        }

        [HttpPost]
        public ActionResult Delete(int productId)
        {
            var deletedProduct = repository.DeleteProduct(productId);
            if (deletedProduct != null)
            {
                TempData["message"] = string.Format("{0} was deleted", deletedProduct.Name);
            }
            return RedirectToAction("Index");
        }

        public ViewResult Edit(int productId)
        {
            Product product = repository.Products.FirstOrDefault(p => p.ProductID == productId);
            return View(product);
        }

        [HttpPost]
        public ActionResult Edit(Product product, HttpPostedFileBase image = null)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    product.ImageMimeType = image.ContentType;
                    product.ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(product.ImageData, 0, image.ContentLength);
                }
                repository.SaveProduct(product);
                TempData["message"] = string.Format("{0} has been saved", product.Name);
                return RedirectToAction("Index");
            }

            //else - there is something wrong with the data values
            return View(product);
        }

        // GET: Admin
        public ActionResult Index()
        {
            return View(repository.Products);
        }

        #endregion --public methods--

        #region --ctor--

        public AdminController(IProductsRepository repo)
        {
            repository = repo;
        }

        #endregion --ctor--
    }
}