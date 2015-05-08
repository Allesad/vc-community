﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VirtoCommerce.Web.Models;
using VirtoCommerce.Web.Models.FormModels;

namespace VirtoCommerce.Web.Controllers
{
    public class CheckoutController : StoreControllerBase
    {
        //
        // GET: /Checkout
        [HttpGet]
        public ActionResult Index()
        {
            return RedirectToAction("Step1", "Checkout");
        }

        //
        // GET: /Checkout/Step1
        [HttpGet]
        [Route("checkout/step-1")]
        public async Task<ActionResult> Step1()
        {
            if (Context.Cart.ItemCount == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            var checkout = await Service.GetCheckoutAsync(SiteContext.Current);
            Context.Checkout = checkout;

            return View("checkout-step-1");
        }

        //
        // POST: /Checkout/Step1
        [HttpPost]
        public async Task<ActionResult> Step1(CheckoutFirstStepFormModel formModel)
        {
            var form = GetForm(formModel.form_type);

            var checkout = await Service.GetCheckoutAsync(SiteContext.Current);

            if (form != null)
            {
                var formErrors = GetFormErrors(ModelState);

                if (formErrors == null)
                {
                    form.PostedSuccessfully = true;

                    var shippingAddress = new CustomerAddress
                    {
                        Address1 = formModel.Address1,
                        Address2 = !string.IsNullOrEmpty(formModel.Address2) ? formModel.Address2 : null,
                        City = formModel.City,
                        Company = !string.IsNullOrEmpty(formModel.Company) ? formModel.Company : null,
                        Country = formModel.Country,
                        CountryCode = "RUS",
                        FirstName = formModel.FirstName,
                        LastName = formModel.LastName,
                        Phone = !string.IsNullOrEmpty(formModel.Phone) ? formModel.Phone : null,
                        Province = formModel.Province,
                        Zip = formModel.Zip
                    };

                    checkout.Currency = Context.Shop.Currency;
                    checkout.Email = formModel.Email;
                    checkout.ShippingAddress = shippingAddress;

                    if (User.Identity.IsAuthenticated)
                    {
                        var customer = await this.CustomerService.GetCustomerAsync(formModel.Email, Context.Shop.StoreId);
                        if (customer != null)
                        {
                            customer.Addresses.Add(shippingAddress);
                            await CustomerService.UpdateCustomerAsync(customer);
                        }
                    }

                    await Service.UpdateCheckoutAsync(SiteContext.Current, checkout);

                    return RedirectToAction("Step2", "Checkout");
                }
                else
                {
                    form.Errors = formErrors;
                    form.PostedSuccessfully = false;

                    return RedirectToAction("Step1", "Checkout");
                }
            }

            Context.ErrorMessage = "Liquid error: Form context was not found.";

            return View("error");
        }

        //
        // GET: /Checkout/Step2
        [HttpGet]
        [Route("checkout/step-2")]
        public async Task<ActionResult> Step2()
        {
            var checkout = await Service.GetCheckoutAsync(SiteContext.Current);

            if (checkout.ShippingAddress == null || !checkout.ShippingAddress.IsFilledCorrectly)
            {
                return RedirectToAction("Step1", "Checkout");
            }

            Context.Checkout = checkout;

            return View("checkout-step-2");
        }

        //
        // POST: /Checkout/Step2
        [HttpPost]
        public async Task<ActionResult> Step2(CheckoutSecondStepFormModel formModel)
        {
            var form = GetForm(formModel.form_type);

            if (form != null)
            {
                var formErrors = GetFormErrors(ModelState);

                if (formErrors == null)
                {
                    var checkout = await Service.GetCheckoutAsync(SiteContext.Current);

                    var billingAddress = new CustomerAddress
                    {
                        Address1 = formModel.Address1,
                        Address2 = !string.IsNullOrEmpty(formModel.Address2) ? formModel.Address2 : null,
                        City = formModel.City,
                        Company = !string.IsNullOrEmpty(formModel.Company) ? formModel.Company : null,
                        Country = formModel.Country,
                        CountryCode = "RUS",
                        FirstName = formModel.FirstName,
                        LastName = formModel.LastName,
                        Phone = !string.IsNullOrEmpty(formModel.Phone) ? formModel.Phone : null,
                        Province = formModel.Province,
                        Zip = formModel.Zip
                    };

                    checkout.BillingAddress = billingAddress;
                    checkout.ShippingMethod = checkout.ShippingMethods.FirstOrDefault(sm => sm.Handle == formModel.ShippingMethodId);
                    checkout.PaymentMethod = checkout.PaymentMethods.FirstOrDefault(pm => pm.Handle == formModel.PaymentMethodId);

                    if (User.Identity.IsAuthenticated)
                    {
                        var customer = await CustomerService.GetCustomerAsync(checkout.Email, Context.Shop.StoreId);
                        if (customer != null)
                        {
                            customer.Addresses.Add(billingAddress);
                            await this.CustomerService.UpdateCustomerAsync(customer);
                        }
                    }

                    checkout.Order = await Service.CreateOrderAsync(SiteContext.Current, checkout);

                    Context.Checkout = checkout;

                    Session.Remove("Forms");

                    return View("thanks_page");
                }
                else
                {
                    form.Errors = formErrors;
                    form.PostedSuccessfully = false;

                    return View("checkout-step-2");
                }
            }

            return View("error");
        }
    }
}