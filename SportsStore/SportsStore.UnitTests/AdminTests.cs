using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class AdminTests
    {
        [TestMethod]
        public void Can_Edit_Product()
        {
            //arrange - create repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(P => P.Products)
                .Returns(new Product[]{
                    new Product{ProductID=1, Name="P1"},
                    new Product{ProductID=2, Name="P2"},
                    new Product{ProductID=3, Name="P3"}
                });

            //arrange - create controller
            AdminController controller = new AdminController(mock.Object);

            //act
            Product p1 = controller.Edit(1).ViewData.Model as Product;
            Product p2 = controller.Edit(2).ViewData.Model as Product;
            Product p3 = controller.Edit(3).ViewData.Model as Product;

            //assert
            Assert.AreEqual(1, p1.ProductID);
            Assert.AreEqual(2, p2.ProductID);
            Assert.AreEqual(3, p3.ProductID);
        }

        [TestMethod]
        public void Cannot_Edit_Nonexistent_Product()
        {
            //arrange - create a new repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(P => P.Products)
                .Returns(new Product[]
                {
                    new Product{ProductID=1, Name="P1"},
                    new Product{ProductID=2, Name="P2"},
                    new Product{ProductID=3, Name="P3"}
                });

            //arrange - create a controller
            AdminController controller = new AdminController(mock.Object);

            //act
            Product result = (Product)controller.Edit(4).ViewData.Model;

            //assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Index_Contains_All_Products()
        {
            //arrange - create a new repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(P => P.Products)
                .Returns(new Product[]
                {
                    new Product{ProductID=1, Name="P1"},
                    new Product{ProductID=2, Name="P2"},
                    new Product{ProductID=3, Name="P3"}
                });

            //arrange - create a controller
            AdminController controller = new AdminController(mock.Object);

            //action
            Product[] result = ((IEnumerable<Product>)((ViewResult)controller.Index()).ViewData.Model).ToArray();

            //assert
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual("P1", result[0].Name);
            Assert.AreEqual("P2", result[1].Name);
            Assert.AreEqual("P3", result[2].Name);
        }

        [TestMethod]
        public void Can_Save_Valid_Changes()
        {
            //arrange
            var repo = new Mock<IProductsRepository>();
            var controller = new AdminController(repo.Object);

            //arrange - create a product
            var product = new Product {Name = "Test"};

            //act
            ActionResult result = controller.Edit(product);

            //assert - check that repository was called
            repo.Verify(p=>p.SaveProduct(product));
            //Assert - check the method result type
            Assert.IsNotInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Cannot_Save_Invalid_Changes()
        {
            //arrange
            var repo = new Mock<IProductsRepository>();
            var controller = new AdminController(repo.Object);

            //arrange - create a product
            var product = new Product { Name = "Test" };

            //arrange - add an error to the model state
            controller.ModelState.AddModelError("error", "error");

            //act
            ActionResult result = controller.Edit(product);

            //assert
            repo.Verify(p=>p.SaveProduct(It.IsAny<Product>()), Times.Never);
            //assert - check return type
            Assert.IsInstanceOfType(result, typeof(ViewResult));

        }

        [TestMethod]
        public void Can_Delete_Valid_Products()
        {
            //Arrange
            var prod = new Product {ProductID = 2, Name = "Test"};

            var repo = new Mock<IProductsRepository>();
            repo.Setup(m => m.Products)
                .Returns(new Product[]
                {
                    new Product {ProductID = 1, Name = "P1"},
                    prod,
                    new Product {ProductID = 3, Name = "P3"}
                });

            var controller = new AdminController(repo.Object);

            //act
            var result = controller.Delete(prod.ProductID);

            //assert
            repo.Verify(p=>p.DeleteProduct(prod.ProductID));
        }
    }
}