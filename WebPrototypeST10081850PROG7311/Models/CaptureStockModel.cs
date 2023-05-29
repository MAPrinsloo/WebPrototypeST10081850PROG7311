///Matthew Prinsloo ST10081850
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebPrototypeST10081850PROG7311.Models
{
    public class CaptureStockModel
    {
        /// <summary>
        /// Holds an Ienumerable for use in lists.
        /// </summary>
        public IEnumerable<WebPrototypeST10081850PROG7311.STOCK> StockList { get; set; }
        /// <summary>
        /// Used to reference the db table.
        /// </summary>
        public WebPrototypeST10081850PROG7311.STOCK tblStock { get; set; }
        /// <summary>
        /// Holds the Selected product type used for captureing stock.
        /// </summary>
        public string SelectedProductType { get; set; }
        /// <summary>
        /// Stores a list of Product types.
        /// </summary>
        public IEnumerable<SelectListItem> ProductTypesList { get; set; }
    }
}
//===============================================================================================//