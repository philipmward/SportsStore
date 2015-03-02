using SportsStore.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsStore.WebUI.Controllers
{
    public class NavController : Controller
    {
        #region --fields--

        private IProductsRepository repository;

        #endregion --fields--

        #region --public methods--

        public PartialViewResult Menu(string category = null)
        {
            //better to create a view model with the list and selected category than to use view bag.
            //Just doing this way to demonstrate and for flavor in an example setting.
            ViewBag.SelectedCategory = category;

            IEnumerable<string> categories = repository.Products
                .Select(p => p.Category)
                .Distinct()
                .OrderBy(p => p);

            //changed to flex view instead of the below. Left comments and code in to show the change.
            ////if mobile return horizontal menu
            //string viewName = horizontalLayout ? "MenuHorizontal" : "Menu";

            return PartialView("FlexMenu", categories);
        }

        #endregion --public methods--

        #region --ctor--

        public NavController(IProductsRepository repo)
        {
            repository = repo;
        }

        #endregion --ctor--
    }
}