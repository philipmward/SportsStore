using Ninject;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Concrete;
using SportsStore.WebUI.Infrastructure.Abstract;
using SportsStore.WebUI.Infrastructure.Concrete;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;

namespace SportsStore.WebUI.Infrastructure
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        #region --fields--

        private IKernel kernel;

        #endregion --fields--

        #region --public methods--

        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }

        #endregion --public methods--

        #region --private methods--

        private void AddBindings()
        {
            kernel.Bind<IProductsRepository>().To<EFProductRepository>();
            //Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            //mock.Setup(P => P.Products)
            //    .Returns(new List<Product>
            //    {
            //        new Product{Name="Football", Price=25},
            //        new Product{Name="Surf board", Price=179},
            //        new Product{Name="Running shoes", Price=95}
            //    });

            //kernel.Bind<IProductsRepository>().ToConstant(mock.Object);

            EmailSettings emailSettings = new EmailSettings { WriteAsFile = bool.Parse(ConfigurationManager.AppSettings["Email.WriteAsFile"] ?? "false") };
            kernel.Bind<IOrderProcessor>().To<EmailOrderProcessor>()
                .WithConstructorArgument("settings", emailSettings);

            kernel.Bind<IAuthProvider>().To<FormsAuthProvider>();
        }

        #endregion --private methods--

        #region --ctor--

        public NinjectDependencyResolver(IKernel kernelParam)
        {
            kernel = kernelParam;
            AddBindings();
        }

        #endregion --ctor--
    }
}