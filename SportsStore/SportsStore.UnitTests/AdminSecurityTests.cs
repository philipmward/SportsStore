using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Infrastructure.Abstract;
using SportsStore.WebUI.Models;
using System;
using System.Web.Mvc;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class AdminSecurityTests
    {
        [TestMethod]
        public void Can_Login_With_Valid_Credintials()
        {
            //Arrange - create a mock authentication provider
            var mock = new Mock<IAuthProvider>();
            mock.Setup(p => p.Authenticate("admin", "secret")).Returns(true);

            //arrange - create the view model
            var model = new LoginViewModel { UserName = "admin", Password = "secret" };

            //arrange - create the controller
            var controller = new AccountController(mock.Object);

            //act
            ActionResult result = controller.Login(model, "/MyURL");

            //assert
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            Assert.AreEqual("/MyURL", ((RedirectResult)result).Url);
        }

        [TestMethod]
        public void Cannot_Login_With_Invalid_Credentials()
        {
            //arrange - create mock authentication provider
            var mock = new Mock<IAuthProvider>();
            mock.Setup(p => p.Authenticate("badUser", "badPass")).Returns(false);

            //arrange - create view model
            var model = new LoginViewModel { UserName = "badUser", Password = "badPass" };

            //arrange - create the controller
            var controller = new AccountController(mock.Object);

            //act
            ActionResult result = controller.Login(model, "/MyURL");

            //assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
        }
    }
}