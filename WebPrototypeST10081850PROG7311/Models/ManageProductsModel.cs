///Matthew Prinsloo ST10081850
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebPrototypeST10081850PROG7311.Models
{
    public class ManageProductsModel
    {
        /// <summary>
        /// Holds an Ienumerable for use in lists.
        /// </summary>
        public IEnumerable<WebPrototypeST10081850PROG7311.PRODUCT> ProductList { get; set; }
        /// <summary>
        /// Used to reference the db table.
        /// </summary>
        public WebPrototypeST10081850PROG7311.PRODUCT tblProduct { get; set; }
    }
}
//===============================================================================================//