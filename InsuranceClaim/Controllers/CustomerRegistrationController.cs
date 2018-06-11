﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using InsuranceClaim.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Insurance.Domain;
using AutoMapper;
using System.Configuration;
using System.Globalization;

namespace InsuranceClaim.Controllers
{
    public class CustomerRegistrationController : Controller
    {
        private ApplicationUserManager _userManager;
        public CustomerRegistrationController()
        {
            // UserManager = userManager;
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: CustomerRegistration
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProductDetail()
        {
            ViewBag.Currency = InsuranceContext.Currencies.All().ToList();
            ViewBag.PoliCyStatus = InsuranceContext.PolicyStatuses.All().ToList();
            ViewBag.BusinessSource = InsuranceContext.BusinessSources.All().ToList();
            var objList = InsuranceContext.PolicyDetails.All(orderBy: "Id desc", top: 1).ToList();
            if (objList.Count > 0)
            {
                ViewBag.PolicyNumber = objList.FirstOrDefault().PolicyNumber;
            }
            else
            {
                ViewBag.PolicyNumber = ConfigurationManager.AppSettings["PolicyNumber"];
            }
            var objCustomerData = InsuranceContext.Customers.All().OrderByDescending(x => x.Id).ToList();
            if (objCustomerData.Count > 0)
            {
                ViewBag.InsurerId = objCustomerData.FirstOrDefault().Id;
                ViewBag.InsurerName = objCustomerData.FirstOrDefault().FirstName;
            }
            return View();
        }

        public ActionResult RiskDetail()
        {
            ViewBag.BusinessSource = InsuranceContext.BusinessSources.All().ToList();
            return View();
        }

        public ActionResult SummaryDetail()
        {
            return View();
        }

        public ActionResult PaymentDetail()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> SaveCustomerData(CustomerModel model)
        {
            decimal custId = 0;
            if (ModelState.IsValid)
            {
                try
                {
                    var user = new ApplicationUser { UserName = model.EmailAddress, Email = model.EmailAddress, PhoneNumber = model.PhoneNumber };
                    var result = await UserManager.CreateAsync(user, "Kindle@123");
                    if (result.Succeeded)
                    {
                        var objCustomer = InsuranceContext.Customers.All(top: 1).OrderByDescending(x => x.Id).ToList();
                        if (objCustomer.Count > 0)
                        {
                            custId = objCustomer.FirstOrDefault().CustomerId + 1;
                        }
                        else
                        {
                            custId = Convert.ToDecimal(ConfigurationManager.AppSettings["CustomerId"]);
                        }

                        model.UserID = user.Id;
                        model.CustomerId = custId;
                        var customer = Mapper.Map<CustomerModel, Customer>(model);
                        InsuranceContext.Customers.Insert(customer);
                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {

                    return Json(false, JsonRequestBehavior.AllowGet);
                }

            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SavePolicyData(PolicyDetailModel model)
        {
            try
            {
                DateTimeFormatInfo usDtfi = new CultureInfo("en-US", false).DateTimeFormat;
                var startDate = Request.Form["StartDate"];
                var endDate = Request.Form["EndDate"];
                var renewDate = Request.Form["RenewalDate"];
                var transactionDate = Request.Form["TransactionDate"];
                if (startDate != null)
                {
                    model.StartDate = Convert.ToDateTime(startDate, usDtfi);
                }
                if (endDate != null)
                {
                    model.EndDate = Convert.ToDateTime(endDate, usDtfi);
                }
                if (renewDate != null)
                {
                    model.RenewalDate = Convert.ToDateTime(renewDate, usDtfi);
                }
                if (transactionDate != null)
                {
                    model.TransactionDate = Convert.ToDateTime(transactionDate, usDtfi);
                }

                var policy = Mapper.Map<PolicyDetailModel, PolicyDetail>(model);
                InsuranceContext.PolicyDetails.Insert(policy);

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(false, JsonRequestBehavior.AllowGet);
            }
            
        }
    }
}