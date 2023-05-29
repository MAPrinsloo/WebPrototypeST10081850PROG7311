///Matthew Prinsloo ST10081850
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebPrototypeST10081850PROG7311.Models
{
    public class FarmerStatisticsModel
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
        /// Stores a list of Product types.
        /// </summary>
        public IEnumerable<SelectListItem> ProductTypesList { get; set; }
        /// <summary>
        /// Stores a list of Farm names.
        /// </summary>
        public IEnumerable<SelectListItem> FarmNamesList { get; set; }
        /// <summary>
        /// The farm name that has been selected from the drop down for.
        /// </summary>
        public string SelectedFarmName { get; set; }
        /// <summary>
        /// Stores the start date for filtering.
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// Stores the end date for filtering.
        /// </summary>
        public DateTime EndDate { get; set; }
    }
}
//===============================================================================================//