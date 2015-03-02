using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.HtmlHelpers;
using SportsStore.WebUI.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// Tests if the NavController can generate the categories properly.
        /// </summary>
        [TestMethod]
        public void Can_Create_Categories()
        {
            //Arrange - create a mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(p => p.Products)
            .Returns(new List<Product>
            {
                new Product{ProductID=1, Name="P1", Category="Apples"},
                new Product{ProductID=2, Name="P2", Category="Apples"},
                new Product{ProductID=3, Name="P3", Category="Plums"},
                new Product{ProductID=4, Name="P4", Category="Oranges"}
            });

            //Arrange - create the controller.
            NavController controller = new NavController(mock.Object);

            //Act get the set of categories
            string[] results = ((IEnumerable<string>)controller.Menu().Model).ToArray();

            //Assert
            Assert.AreEqual(3, results.Length);
            Assert.AreEqual("Apples", results[0]);
            Assert.AreEqual("Oranges", results[1]);
            Assert.AreEqual("Plums", results[2]);
        }

        [TestMethod]
        public void Can_Filter_Products()
        {
            //Arrange - create mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products)
                .Returns(new Product[]
                {
                    new Product{ProductID=1, Name="P1", Category="Cat1"},
                    new Product{ProductID=2, Name="P2", Category="Cat2"},
                    new Product{ProductID=3, Name="P3", Category="Cat1"},
                    new Product{ProductID=4, Name="P4", Category="Cat2"},
                    new Product{ProductID=5, Name="P5", Category="Cat3"}
                });

            //Arrange - create a controller and make the page size 3 items
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //Action
            Product[] result = ((ProductListViewModel)controller.List("Cat2", 1).Model).Products.ToArray();

            Assert.AreEqual(2, result.Length);
            Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "P4" && result[0].Category == "Cat2");
        }

        [TestMethod]
        public void Can_Generate_Page_Links()
        {
            //Arrange - define an HTML helper - we need to do this in order to apply the extension method
            HtmlHelper myHelper = null;

            //Arrange - create PagingInfo data
            PagingInfo pagingInfo = new PagingInfo { CurrentPage = 2, TotalItems = 28, ItemsPerPage = 10 };

            //Arrange - setup the delegate using a lambda expression
            Func<int, string> pageUrlDelegate = i => "Page" + i;

            //act
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);
            string expected = @"<a class=""btn btn-default"" href=""Page1"">1</a>" + @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>" + @"<a class=""btn btn-default"" href=""Page3"">3</a>";
            string formatresult = result.ToString();
            Assert.AreEqual(expected, formatresult);
        }

        [TestMethod]
        public void Can_Paginate()
        {
            //Arrange
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products)
                .Returns(new Product[]
                {
                    new Product{ProductID=1, Name="P1"},
                    new Product{ProductID=2, Name="P2"},
                    new Product{ProductID=3, Name="P3"},
                    new Product{ProductID=4, Name="P4"},
                    new Product{ProductID=5, Name="P5"}
                });

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //act
            ProductListViewModel result = (ProductListViewModel)controller.List(null, 2).Model;

            //Assert
            Product[] prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "P4");
            Assert.AreEqual(prodArray[1].Name, "P5");
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            //Arrange
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products)
                .Returns(new Product[]
                {
                    new Product{ProductID=1, Name="P1"},
                    new Product{ProductID=2, Name="P2"},
                    new Product{ProductID=3, Name="P3"},
                    new Product{ProductID=4, Name="P4"},
                    new Product{ProductID=5, Name="P5"}
                });

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            //act
            ProductListViewModel result = (ProductListViewModel)controller.List(null, 2).Model;

            //Assert
            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalPages, 2);
        }

        /// <summary>
        /// Tests that the Product Controller Generate's the category_ specific_ product_ count.
        /// </summary>
        [TestMethod]
        public void Generate_Category_Specific_Product_Count()
        {
            //Arrange - create a mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(P => P.Products)
                .Returns(new Product[] {
                    new Product{ProductID=1, Name="P1", Category="Cat1"},
                    new Product{ProductID=2, Name="P2", Category="Cat2"},
                    new Product{ProductID=3, Name="P3", Category="Cat1"},
                    new Product{ProductID=4, Name="P4", Category="Cat2"},
                    new Product{ProductID=5, Name="P5", Category="Cat3"},
                });

            //arrange - create a controller and make the page size 3 items
            ProductController target = new ProductController(mock.Object);
            target.PageSize = 3;

            //Action - test the product counts for different categories
            int res1 = ((ProductListViewModel)target.List("Cat1").Model).PagingInfo.TotalItems;
            int res2 = ((ProductListViewModel)target.List("Cat2").Model).PagingInfo.TotalItems;
            int res3 = ((ProductListViewModel)target.List("Cat3").Model).PagingInfo.TotalItems;
            int resall = ((ProductListViewModel)target.List(null).Model).PagingInfo.TotalItems;

            //Assert
            Assert.AreEqual(2, res1);
            Assert.AreEqual(2, res2);
            Assert.AreEqual(1, res3);
            Assert.AreEqual(5, resall);
        }

        /// <summary>
        /// Tests if the NavController properly sets the category.
        /// </summary>
        [TestMethod]
        public void Indicates_Selected_Category()
        {
            //Arrange - create mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(P => P.Products)
                .Returns(new Product[] {
                    new Product{ProductID=1, Name="P1", Category="Apples"},
                    new Product{ProductID=4, Name="P2", Category="Oranges"}
                });

            //Arrange - create the controller
            NavController target = new NavController(mock.Object);

            //Arrange - define a category to select
            string categoryToSelect = "Apples";

            //Action
            string result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;

            //Assert
            Assert.AreEqual(categoryToSelect, result);
        }
    }
}