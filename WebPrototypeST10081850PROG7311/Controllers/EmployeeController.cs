///Matthew Prinsloo ST10081850
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography.Xml;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using WebPrototypeST10081850PROG7311;
using WebPrototypeST10081850PROG7311.Models;
using WebPrototypeST10081850PROG7311.Properties;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace WebPrototypeST10081850PROG7311.Controllers
{
    public class EmployeeController : Controller
    {   /// <summary>
        /// This is the database related to the Entity Data Model.
        /// </summary>
        private FarmCentralDBEntities db = new FarmCentralDBEntities();
        //-----------------------------------------------------------------------------------------------//
        /// <summary>
        /// Populates variables in the FarmerStatisticsModel
        /// that is used to product the output view.
        /// </summary>
        /// <returns>If the user is an Employee returns FarmerStatistics View.</returns>
        public ActionResult FarmerStatistics()
        {
            //Unauthorized access check.
            if (UserIsEmployee() == false)
            {
                Session["UserLoggedIn"] = null;
                //SignOut does a check for -^ returns to login.
                return RedirectToAction("SignOut", "Home");
            }

            //Model used in the FarmerStatistics.cshtml View.
            FarmerStatisticsModel model = new FarmerStatisticsModel();

            //Get list of farm names and assign it to FarmNamesList.
            model.FarmNamesList = db.FARMs.Select(f => new SelectListItem
            {
                Text = f.FARM_NAME,
                Value = f.FARM_NAME
            }).ToList();

            //Get distinct list of product types, ProductTypesList populates DropDownListFor.
            model.ProductTypesList = db.PRODUCTs.Select(p => new SelectListItem
            {
                Text = p.PRODUCT_TYPE,
                Value = p.PRODUCT_TYPE
            }).Distinct().ToList();

            //List of all info for for the view to display.
            model.StockList = db.STOCKs
                .Include(s => s.PRODUCT)
                .Include(s => s.PRODUCT.FARM)
                .ToList();

            return View("FarmerStatistics", model);
        }
        //-----------------------------------------------------------------------------------------------//
        /// <summary>
        /// Executes when the button btnSearch is clicked on the FarmerStatistics.cshtml view.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>FarmerStatistics View.</returns>
        [HttpPost]
        public ActionResult btnSearch_Clicked(FarmerStatisticsModel model)
        {
            //The selected farm from the dropdownfor.
            string farmName = model.SelectedFarmName;
            //Assign a session so it can be called in btnFilter_Clicked.
            Session["SelectedFarmName"] = model.SelectedFarmName;

            //Filter the stocks based on the selected farm name
            model.StockList = db.STOCKs
                .Include(s => s.PRODUCT)
                .Include(s => s.PRODUCT.FARM)
                .Where(s => s.PRODUCT.FARM.FARM_NAME == farmName)
                .ToList();

            //Repopulate the farm names dropdown lists
            model.FarmNamesList = db.FARMs.Select(f => new SelectListItem
            {
                Text = f.FARM_NAME,
                Value = f.FARM_NAME
            }).ToList();

            //Repopulate the product types dropdown lists
            model.ProductTypesList = db.PRODUCTs.Select(p => new SelectListItem
            {
                Text = p.PRODUCT_TYPE,
                Value = p.PRODUCT_TYPE
            }).Distinct().ToList();
            return View("FarmerStatistics", model);
        }
        //-----------------------------------------------------------------------------------------------//
        /// <summary>
        /// Executes when the filter button btnFilter is clicked on the FarmerStatistics.cshtml View.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>FarmerStatistics.cshtml View.</returns>
        [HttpPost]
        public ActionResult btnFilter_Clicked(FarmerStatisticsModel model)
        {
            //Variables retrieved from the View
            string productType = model.tblStock.PRODUCT.PRODUCT_TYPE;
            DateTime startDate = model.StartDate;
            DateTime endDate = model.EndDate;
            //Cannot access selectedFarmName from other Html.BeginForm so we use a Session to retrieve the data.
            string selectedFarmName = Session["SelectedFarmName"] as string;

            //Filters using all variables captured.
            model.StockList = db.STOCKs
                .Include(s => s.PRODUCT)
                .Include(s => s.PRODUCT.FARM)
                .Where(s => s.PRODUCT.PRODUCT_TYPE == productType &&
                            s.PRODUCT.FARM.FARM_NAME == selectedFarmName &&
                            DateTime.Compare(s.DATE_DELIVERED, startDate) >= 0 &&
                            DateTime.Compare(s.DATE_DELIVERED, endDate) <= 0)
                .ToList();

            //Repopulate the farm names and product types dropdownfor
            model.FarmNamesList = db.FARMs.Select(f => new SelectListItem
            {
                Text = f.FARM_NAME,
                Value = f.FARM_NAME,
                Selected = (f.FARM_NAME == model.SelectedFarmName)
            }).ToList();

            model.ProductTypesList = db.PRODUCTs.Select(p => new SelectListItem
            {
                Text = p.PRODUCT_TYPE,
                Value = p.PRODUCT_TYPE
            }).Distinct().ToList();
            return View("FarmerStatistics", model);
        }
        //-----------------------------------------------------------------------------------------------//
        /// <summary>
        /// Displays ManageFarmers.cshtml after validation.
        /// </summary>
        /// <returns>If the user is an employee it will return ManageFarmer.cshtml view.</returns>
        public ActionResult ManageFarmers()
        {
            //Unauthorized access check.
            if (UserIsEmployee() == false)
            {
                Session["UserLoggedIn"] = null;
                //SignOut does a check for -^ returns to login.
                return RedirectToAction("SignOut", "Home");
            }
            return View();
        }
        //-----------------------------------------------------------------------------------------------//
        /// <summary>
        /// Creates the farmer account.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>ManageFarmer.cshtml View.</returns>
        [HttpPost]
        public ActionResult btnCreateAccount_Clicked(FARM farmModel)
        {
            //All variables that need to be retrieved
            string farmName = farmModel.FARM_NAME;
            string address = farmModel.ADDRESS;
            string name = farmModel.USER.NAME;
            string username = farmModel.USER.USERNAME;
            string password = farmModel.USER.PASSWORD.USER_PASSWORD;

            //Saving new data must be done in this order for FK references.
            try
            {
                //Checks if username is not used.
                var user = db.USERs.FirstOrDefault(u => u.USERNAME == username);
                if (user == null)
                {
                    //Creating and saving new password.
                    var newPword = new PASSWORD
                    {
                        PWORD_ID = Guid.NewGuid(),
                        USER_PASSWORD = password
                    };
                    db.PASSWORDs.Add(newPword);
                    db.SaveChanges();

                    //Creating and saving new user.
                    var newUser = new USER
                    {
                        U_ID = Guid.NewGuid(),
                        NAME = name,
                        USERNAME = username,
                        USER_TYPE = "FARMER",
                        PWORD_ID = newPword.PWORD_ID
                    };
                    db.USERs.Add(newUser);
                    db.SaveChanges();

                    //Creating and saving new farm.
                    var newFarm = new FARM
                    {
                        F_ID = Guid.NewGuid(),
                        ADDRESS = address,
                        FARM_NAME = farmName,
                        U_ID = newUser.U_ID
                    };

                    db.FARMs.Add(newFarm);
                    db.SaveChanges();

                    return View("ManageFarmers");
                }
                else
                {
                    return View("ManageFarmers");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return View("ManageFarmers");
            }
        }
        //-----------------------------------------------------------------------------------------------//
        /// <summary>
        /// Checks a session "Session["LoggedUser_Type"]" which holds
        /// the logged in users user_type. 
        /// </summary>
        /// <returns>False if user is not of user_type "EMPLOYEE"</returns>
        private bool UserIsEmployee()
        {
            if ((Session["LoggedUser_Type"] == null) || (Session["LoggedUser_Type"].Equals("FARMER")))
            {
                return false;
            }
            if (Session["LoggedUser_Type"].Equals("EMPLOYEE"))
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