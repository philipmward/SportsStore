using SportsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsStore.Domain.Entities
{
    public class Cart
    {
        #region --fields--

        public List<CartLine> lineCollection = new List<CartLine>();

        #endregion --fields--

        #region --properties--

        public IEnumerable<CartLine> Lines { get { return lineCollection; } }

        #endregion --properties--

        #region --public methods--

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="product">The product.</param>
        /// <param name="quantity">The quantity.</param>
        public void AddItem(Product product, int quantity)
        {
            CartLine line = lineCollection
                .FirstOrDefault(P => P.Product.ProductID == product.ProductID);

            if (line == null)
            {
                lineCollection.Add(new CartLine { Product = product, Quantity = quantity });
            }
            else
            {
                line.Quantity += quantity;
            }
        }

        /// <summary>
        /// Clears all lines.
        /// </summary>
        public void Clear()
        {
            lineCollection.Clear();
        }

        /// <summary>
        /// Computes the total value of lines.
        /// </summary>
        /// <returns></returns>
        public decimal ComputeTotalValue()
        {
            return lineCollection.Sum(P => P.Product.Price * P.Quantity);
        }

        /// <summary>
        /// Removes all matching lines.
        /// </summary>
        /// <param name="product">The product.</param>
        public void RemoveLine(Product product)
        {
            lineCollection.RemoveAll(P => P.Product.ProductID == product.ProductID);
        }

        #endregion --public methods--
    }
}

public class CartLine
{
    #region --properties--

    public Product Product { get; set; }

    public int Quantity { get; set; }

    #endregion --properties--
}