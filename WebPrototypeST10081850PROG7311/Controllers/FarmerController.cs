///Matthew Prinsloo ST10081850
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using WebPrototypeST10081850PROG7311.Models;
using System.Data.Entity;
using System.Xml.Serialization;

namespace WebPrototypeST10081850PROG7311.Controllers
{
    public class FarmerController : Controller
    {
        //-----------------------------------------------------------------------------------------------//
        /// <summary>
        /// This is the database related to the Entity Data Model.
        /// </summary>
        private FarmCentralDBEntities db = new FarmCentralDBEntities();
        //-----------------------------------------------------------------------------------------------//
        /// <summary>
        /// Returns a view that only shows the products for the 
        /// logged in farmer.
        /// </summary>
        /// <returns>After authorization does returns ManageProducts.cshtml view.</returns>
        public ActionResult ManageProducts()
        {
            //Unauthorized access check.
            if (UserIsFarmer() == false)
            {
                Session["UserLoggedIn"] = null;
                //SignOut does a check for -^ returns to login.
                return RedirectToAction("SignOut", "Home");
            }
            //Model used in the cshtml.
            ManageProductsModel model = new ManageProductsModel();
            //Holds the Guid of the logged in farmer.
            Guid? farmerId = Session["LoggedFarmerId"] as Guid?;
            
            //Only shows the products that the farmer entered.
            IEnumerable<WebPrototypeST10081850PROG7311.PRODUCT> productList = db.PRODUCTs
                    .Where(p => p.F_ID == farmerId)
                    .ToList();
            model.ProductList = productList;

            return View(model);
        }
        //-----------------------------------------------------------------------------------------------//
        /// <summary>
        /// Creates a new product using some auto generated variables from the
        /// farmer.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>ManageProducts.cshtml View the newly created product.</returns>
        [HttpPost]
        public ActionResult btnCreate_Clicked(ManageProductsModel model)
        {
            try
            {
                //Gets the product type from the editorfor using the model.
                string productType = model.tblProduct.PRODUCT_TYPE;
                //Holds the Guid of the logged in farmer.
                Guid? farmerId = Session["LoggedFarmerId"] as Guid?;
                //Checks if the farm exists.
                var farmFound = db.FARMs.FirstOrDefault(u => u.F_ID == farmerId);
                var productFound = db.PRODUCTs.FirstOrDefault(p => p.PRODUCT_TYPE.Equals(productType) 
                                                           && p.F_ID == farmerId);

                if ((farmFound != null) && (productFound == null))
                {
                    //Gather values for new product entry.
                    var newProduct = new PRODUCT
                    {
                        P_ID = Guid.NewGuid(),
                        PRODUCT_TYPE = productType.ToUpper(),
                        F_ID = (Guid)farmerId,
                    };

                    db.PRODUCTs.Add(newProduct);
                    db.SaveChanges();

                    //Get the new product list, products only pertaining to the logged in farmer.
                    IEnumerable<WebPrototypeST10081850PROG7311.PRODUCT> productList = db.PRODUCTs
                        .Where(p => p.F_ID == farmerId)
                        .ToList();
                    //Update the models product list.
                    model.ProductList = productList;
                    //Update the tbl product.
                    model.tblProduct = new WebPrototypeST10081850PROG7311.PRODUCT();

                    return View("ManageProducts", model);
                }
                else
                {
                    ManageProducts();
                    return RedirectToAction("ManageProducts", "Farmer");
                }
            }
            catch (Exception)
            {
                ManageProducts();
                return RedirectToAction("ManageProducts", "Farmer");
            }
        }
        //-----------------------------------------------------------------------------------------------//
        /// <summary>
        /// shows the CaptureStock view and populates the required lists to do so.
        /// </summary>
        /// <returns>Returns the CaptureStock view</returns>
        public ActionResult CaptureStock() 
        {
            //Unauthorized access check.
            if (UserIsFarmer() == false)
            {
                Session["UserLoggedIn"] = null;
                //SignOut does a check for -^ returns to login.
                return RedirectToAction("SignOut", "Home");
            }
            //Model used in the cshtml.
            CaptureStockModel model = new CaptureStockModel();
            //Holds the Guid of the logged in farmer.
            Guid? farmerId = Session["LoggedFarmerId"] as Guid?;

            //Populates the dropdownlistfor that displays the products the farmer can capture.
            model.ProductTypesList = db.PRODUCTs.Where(p => p.F_ID == farmerId).Select(p => new SelectListItem
            {
                Text = p.PRODUCT_TYPE,
                Value = p.PRODUCT_TYPE
            }).ToList();
            //StockList being updated to include the list of product types.
            model.StockList = db.STOCKs
                .Include(s => s.PRODUCT)
                .ToList();

            return View("CaptureStock", model);
        }
        //-----------------------------------------------------------------------------------------------//
        /// <summary>
        /// When the button is clicked it gets data and records a stock entry
        /// </summary>
        /// <param name="model"></param>
        /// <returns>CaptureStock View</returns>
        public ActionResult btnCapture_Clicked(CaptureStockModel model)
        {
            //Variables needed for capture:
            //Holds the Guid of the logged in farmer-v.
            Guid? farmerId = Session["LoggedFarmerId"] as Guid?;
            int weight = model.tblStock.WEIGHT;
            string product_Type = model.SelectedProductType;
            Guid p_id = model.tblStock.P_ID;

            try
            {
                //Finds the selected product to access P_ID later.
                var selectedProduct = db.PRODUCTs.FirstOrDefault(p => p.PRODUCT_TYPE == product_Type 
                                                                   && p.F_ID == farmerId);
                if (selectedProduct != null)
                {
                    var newStock = new STOCK
                    {
                        S_ID = Guid.NewGuid(),
                        WEIGHT = weight,
                        //Farmers will be dropping things off in real time, uses DateTime.now
                        DATE_DELIVERED = DateTime.Now,
                        P_ID = selectedProduct.P_ID
                    };

                    db.STOCKs.Add(newStock);
                    db.SaveChanges();
                    //use CaptureStock to repopulate and show the CaptureStock view.
                    CaptureStock();
                    return View("CaptureStock");
                }
                else
                {
                    return View("CaptureStock");
                }
            }
            catch (Exception)
            {
                return View("CaptureStock");
            }
        }
        //-----------------------------------------------------------------------------------------------//
        /// <summary>
        /// Checks a session "Session["LoggedUser_Type"]" which holds
        /// the logged in users user_type. 
        /// </summary>
        /// <returns>False if user is not of user_type "FARMER"</returns>
        private bool UserIsFarmer() 
        {
            if ((Session["LoggedUser_Type"] == null) || (Session["LoggedUser_Type"].Equals("EMPLOYEE")))
            {
                return false;
            }
            if (Session["LoggedUser_Type"].Equals("FARMER"))
            {
                return true;
            }
            return false;
        }
    }
}
//===============================================================================================//
/// Reference: 
/// Asp.Net MVC with Drop Down List, and SelectListItem Assistance
/// https://stackoverflow.com/questions/4833652/asp-net-mvc-with-drop-down-list-and-selectlistitem-assistance
/// Accessing Your Model's Data from a New Controller
/// https://learn.microsoft.com/en-us/aspnet/mvc/overview/getting-started/introduction/accessing-your-models-data-from-a-controller
/// Actions and Functions in OData v4 Using ASP.NET Web API 2.2
/// [used for [HttpPost] more specifically]
/// https://learn.microsoft.com/en-us/aspnet/web-api/overview/odata-support-in-aspnet-web-api/odata-v4/odata-actions-and-functions
/// Simple Login Application using Sessions in ASP.NET MVC
/// https://www.c-sharpcorner.com/article/simple-login-application-using-Asp-Net-mvc/
