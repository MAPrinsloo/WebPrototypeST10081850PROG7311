///Matthew Prinsloo ST10081850
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPrototypeST10081850PROG7311.Models;

namespace WebPrototypeST10081850PROG7311.Controllers
{
    public class HomeController : Controller
    {
        //-----------------------------------------------------------------------------------------------//
        /// <summary>
        /// When application requests the Login View
        /// </summary>
        /// <returns>Login View</returns>
        public ActionResult Login()
        {
            return View();
        }
        /// <summary>
        /// Handles the click event on button named btnLogin
        /// </summary>
        /// <returns>If successful returns SignOut View</returns>
        public ActionResult btnLogin_Clicked(USER model)
        {
            string username = model.USERNAME;
            string password = model.PASSWORD.USER_PASSWORD;
            bool userFound = false;
            bool passwordMatch = false;
            try
            {
                using (FarmCentralDBEntities db = new FarmCentralDBEntities())
                {
                    //Checks if the username is in the db.
                    USER user = db.USERs.FirstOrDefault(u => u.USERNAME == username);
                    Guid pword_id;
                    //Used to find and determine if the password is found.
                    PASSWORD findPassword = null;
                    if (user != null)
                    {
                        userFound = true;
                        //Now that user is found we instantiate the pword_id.
                        pword_id = user.PWORD_ID;
                        //Find the record that matches the Password ID: PWORD_ID
                        findPassword = db.PASSWORDs.FirstOrDefault(p => p.PWORD_ID.Equals(pword_id));
                    }
                    //Checks if the record's password matches the one the user entered.
                    if ((userFound == true) && (findPassword != null) && (findPassword.USER_PASSWORD.Equals(password)))
                    {
                        passwordMatch = true;
                    }
                    //Checks if user is a FARMER and does necessary prep.
                    //!NB use Trim(), db outputs with spaces even when entered with sql.
                    if ((passwordMatch == true) && (user.USER_TYPE.Trim() == ("FARMER")))
                    {
                        FARM findF_ID = db.FARMs.FirstOrDefault(f => f.U_ID == user.U_ID);
                        Session["LoggedFarmerId"] = findF_ID.F_ID;
                        Session["LoggedUser_Type"] = "FARMER";
                        Session["UserLoggedIn"] = "UserLoggedIn";
                        return RedirectToAction("ManageProducts", "Farmer");
                    }
                    //Checks if user is a EMPLOYEE and does necessary prep.
                    if ((passwordMatch == true) && (user.USER_TYPE.Trim() == ("EMPLOYEE")))
                    {
                        Session["LoggedUser_Type"] = "EMPLOYEE";
                        Session["UserLoggedIn"] = "UserLoggedIn";
                        return RedirectToAction("FarmerStatistics", "Employee");
                    }
                    //Use SignOut to reset progress.
                    SignOut();
                    return View("Login");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return View("Login");
            }
        }
        //-----------------------------------------------------------------------------------------------//
        /// <summary>
        /// When application requests the SignOut View,
        /// does a check to see if user has logged in and processes the
        /// outcome.
        /// </summary>
        /// <returns>SignOut View</returns>
        public ActionResult SignOut() 
        {
            if (Session["UserLoggedIn"] == null)
            {
                ClearSessionVariables();
                return RedirectToAction("Login", "Home");
            }
            return View();
        }
        //-----------------------------------------------------------------------------------------------//
        /// <summary>
        /// Handles the click event on button named btnSignOut
        /// </summary>
        /// <returns>Login View</returns>
        public ActionResult btnSignOut_Clicked() 
        {
            ClearSessionVariables();
            return RedirectToAction("Login", "Home");
        }
        //-----------------------------------------------------------------------------------------------//
        /// <summary>
        /// Clears all the Sessions Variables used throughout the web app
        /// </summary>
        private void ClearSessionVariables()
        {
            Session["UserLoggedIn"] = null;
            Session["LoggedUser_Type"] = "";
            Session["LoggedFarmerId"] = "";
            Session["SelectedFarmName"] = "";
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