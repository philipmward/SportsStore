using SportsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportsStore.WebUI.Models
{
    public class ProductListViewModel
    {
        #region --properties--

        public string CurrentCatagory { get; set; }

        public PagingInfo PagingInfo { get; set; }

        public IEnumerable<Product> Products { get; set; }

        #endregion --properties--
    }
}