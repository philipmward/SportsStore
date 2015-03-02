using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using System;
using System.Linq;
using System.Web.Mvc;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class CartTests
    {
        #region --CartController tests--

        [TestMethod]
        public void Adding_Product_To_Cart_Goes_To_Cart_Screen()
        {
            //arrange - create mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(P => P.Products)
            .Returns(new Product[]
            {
                new Product{ProductID=1, Name="P1", Category="Apples"}
            }.AsQueryable());

            //arrange create a cart
            Cart cart = new Cart();

            //arrange - create a cartcontroller
            CartController target = new CartController(mock.Object, null);

            //act - add a product to the cart
            RedirectToRouteResult result = target.AddToCart(cart, 2, "myUrl");

            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("myUrl", result.RouteValues["returnUrl"]);
        }

        [TestMethod]
        public void Can_Add_To_Cart()
        {
            //arrange - create mock repository
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(P => P.Products)
            .Returns(new Product[]
            {
                new Product{ProductID=1, Name="P1", Category="Apples"}
            }.AsQueryable());

            //arrange create a cart
            Cart cart = new Cart();

            //arrange - create a cartcontroller
            CartController target = new CartController(mock.Object, null);

            //act
            target.AddToCart(cart, 1, null);

            Assert.AreEqual(1, cart.Lines.Count());
            Assert.AreEqual(1, cart.Lines.ToArray()[0].Product.ProductID);
        }

        [TestMethod]
        public void Can_Checkout_And_Submit_Order()
        {
            //arrange - create a mock order processor.
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            //Arrange - create a cart with an item
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            //arrange - create an instance of the controller
            CartController target = new CartController(null, mock.Object);

            //act - try to checkout
            ViewResult result = target.Checkout(cart, new ShippingDetails());

            //assert - check that the order has passed on to the processor
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Once);

            //assert - check that the completed view is returning
            Assert.AreEqual("Completed", result.ViewName);

            //assert - check that I am passing a valid model to the view
            Assert.AreEqual(true, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Can_View_Cart_Contents()
        {
            //arrange - create a cart
            Cart cart = new Cart();

            //arrange create a controller
            CartController target = new CartController(null, null);

            //act - call index action method
            CartIndexViewModel result = (CartIndexViewModel)target.Index(cart, "myUrl").Model;

            //assert
            Assert.AreEqual(cart, result.Cart);
            Assert.AreEqual("myUrl", result.ReturnUrl);
        }

        [TestMethod]
        public void Cannot_Checkout_Invalid_ShippingDetails()
        {
            //arrange - create a mock order processor
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            //arrange create a cart with an item
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            //arrange - create an instance of the controller
            CartController target = new CartController(null, mock.Object);
            //arrange - add an error to the model
            target.ModelState.AddModelError("error", "error");

            //act - try to checkout
            ViewResult result = target.Checkout(cart, new ShippingDetails());

            //assert - check that the order hasn't been passed on to the processor
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never);
            //assert - check that the method is returning the default view
            Assert.AreEqual(string.Empty, result.ViewName);
            //assert - check that I am passing an invalid model to the view
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Checkout_Empty_Cart()
        {
            //Arrange
            Mock<IOrderProcessor> proc = new Mock<IOrderProcessor>();
            Cart cart = new Cart();
            ShippingDetails shippingDetails = new ShippingDetails();
            CartController target = new CartController(null, proc.Object);

            //act
            ViewResult result = target.Checkout(cart, shippingDetails);

            //assert -  check that the order hasn't been passed on to the processor.
            proc.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never);
            //assert - check that the method is returning the default view
            Assert.AreEqual(string.Empty, result.ViewName);
            //assert - check that I am passing an invalid model back to the view
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        #endregion --CartController tests--

        #region --Cart tests--

        [TestMethod]
        public void Calculate_Cart_Total()
        {
            //arrange - create some products
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };

            //arrange - create a new cart
            Cart target = new Cart();

            //act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 3);
            decimal result = target.ComputeTotalValue();

            //assert
            Assert.AreEqual(450M, result);
        }

        [TestMethod]
        public void Can_Add_New_Lines()
        {
            //arrange - create some test products
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            //arrange - create a new cart
            Cart cart = new Cart();

            //act
            cart.AddItem(p1, 1);
            cart.AddItem(p2, 1);
            CartLine[] result = cart.Lines.ToArray();

            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(result[0].Product, p1);
            Assert.AreEqual(result[1].Product, p2);
        }

        [TestMethod]
        public void Can_Add_Quantity_For_Existing_Lines()
        {
            //arrange - create some test products
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            //arrange - create a new cart
            Cart cart = new Cart();

            //act
            cart.AddItem(p1, 1);
            cart.AddItem(p2, 1);
            cart.AddItem(p1, 10);
            CartLine[] result = cart.Lines.ToArray();

            //assert
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(result[0].Quantity, 11);
            Assert.AreEqual(result[1].Quantity, 1);
        }

        [TestMethod]
        public void Can_Clear_Cart()
        {
            //arrange - create some products
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };

            //arrange - create a new cart
            Cart target = new Cart();

            //act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 3);
            target.Clear();

            //assert
            Assert.AreEqual(0, target.Lines.Count());
        }

        [TestMethod]
        public void Can_Remove_Line()
        {
            //arrange - create some test products
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            Product p3 = new Product { ProductID = 3, Name = "P3" };

            //arrange create a new cart
            Cart cart = new Cart();

            //arrange add some products to the cart
            cart.AddItem(p1, 1);
            cart.AddItem(p2, 3);
            cart.AddItem(p3, 5);
            cart.AddItem(p2, 1);

            //act
            cart.RemoveLine(p2);

            //assert
            Assert.AreEqual(0, cart.Lines.Where(P => P.Product == p2).Count());
            Assert.AreEqual(2, cart.Lines.Count());
        }

        #endregion --Cart tests--
    }
}