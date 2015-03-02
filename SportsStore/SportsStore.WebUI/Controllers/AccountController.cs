using SportsStore.WebUI.Infrastructure.Abstract;
using SportsStore.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsStore.WebUI.Controllers
{
    public class AccountController : Controller
    {
        #region --fields--

        private IAuthProvider authProvider;

        #endregion --fields--

        #region --methods--

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (authProvider.Authenticate(model.UserName, model.Password))
                {
                    return Redirect(returnUrl ?? Url.Action("Index", "Admin"));
                }

                ModelState.AddModelError("", "Incorrect username or password");
                return View();
            }

            return View();
        }

        #endregion --methods--

        #region --ctor--

        public AccountController(IAuthProvider auth)
        {
            authProvider = auth;
        }

        #endregion --ctor--
    }
}