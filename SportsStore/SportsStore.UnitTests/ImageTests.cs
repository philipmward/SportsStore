using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace SportsStore.UnitTests
{
    /// <summary>
    /// Summary description for ImageTests
    /// </summary>
    [TestClass]
    public class ImageTests
    {
        [TestMethod]
        public void Can_Retrieve_Image_Data()
        {
            //Arrange = create a product with image data
            var prod = new Product
            {
                ProductID = 2,
                Name = "Test",
                ImageData = new byte[] { },
                ImageMimeType = "image/png"
            };

            //Arrange - create the mock repository
            var mock = new Mock<IProductsRepository>();
            mock.Setup(p => p.Products)
                .Returns(new[]
                {
                    new Product{ProductID = 1, Name = "P1"},
                    prod,
                    new Product{ProductID = 3, Name = "P3"}
                }.AsQueryable());

            //arrange - create the controller
            var controller = new ProductController(mock.Object);

            //act - call GetImage action method
            ActionResult result = controller.GetImage(2);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(FileResult));
            Assert.AreEqual(prod.ImageMimeType, ((FileResult)result).ContentType);
        }

        [TestMethod]
        public void Cannot_Retrieve_Image_Data_For_Invalid_ID()
        {
            //Arrange - create the mock repository
            var mock = new Mock<IProductsRepository>();
            mock.Setup(p => p.Products)
                .Returns(new[]
                {
                    new Product {ProductID = 1, Name = "P1"},
                    new Product {ProductID = 2, Name = "P2"}
                }.AsQueryable());

            //arrange - create the controller
            var controller = new ProductController(mock.Object);

            //act
            ActionResult result = controller.GetImage(100);

            //assert
            Assert.IsNull(result);
        }
    }
}