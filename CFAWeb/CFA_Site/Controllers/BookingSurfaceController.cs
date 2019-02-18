using CustomExtensions;
using CFA_Site.Models;
using CFA_Site.App_Code.BookingSection;
using System;
using System.Configuration;
using System.Web.Mvc;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Web.Mvc;
using System.Collections.Generic;

namespace CFA_Site.Controllers
{
    public class BookingSurfaceController : SurfaceController
    {
        [ActionName("RenderBookingForm")]
        public ActionResult RenderBookingForm()
        {
            BookingViewModel booking = new BookingViewModel();

            if (TempData["nodeid"] != null)
            {
                try
                {
                    string nodeId = TempData["nodeid"].ToString();

                    IPublishedContent node = Umbraco.TypedContent(nodeId);
                    booking.EventDetails.Id = Convert.ToInt32(nodeId);
                    booking.EventDetails.Name = node.Name;
                    DateTime date = Convert.ToDateTime(node.GetProperty("date").Value);
                    booking.EventDetails.Date_DateTime = date;
                    booking.EventDetails.Date = Extension.Ordinal(Convert.ToInt32(date.ToString("%d"))) + " " + date.ToString("MMMM");
                    booking.EventDetails.Time = node.GetProperty("time").Value.ToString();
                    booking.EventDetails.Category = node.GetProperty("category").Value.ToString();

                    IPublishedContent location = Umbraco.TypedContent(node.GetProperty("locationPicker").Value.ToString());
                    booking.EventDetails.Address = location.GetProperty("address").Value.ToString();

                    booking.EventDetails.AvailableTickets = Convert.ToInt32(node.GetProperty("availableTickets").Value);
                    booking.EventDetails.AllocatedTickets = Convert.ToInt32(node.GetProperty("allocatedTickets").Value);

                    List<SelectListItem> sourceSelectList = new List<SelectListItem>();
                    foreach (var item in Umbraco.ContentAtXPath("//Events").First().GetPropertyValue("sourceDropDownList"))
                        sourceSelectList.Add(new SelectListItem() { Value = item.value, Text = item.value });

                    booking.SourceItems = sourceSelectList;

                    return PartialView("BookingForm", booking);
                }
                catch (Exception ex)
                {
                    LogHelper.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "Looking up Event with Id: - " + TempData["nodeid"].ToString(), new Exception(ex.Message));
                    return PartialView("BookingForm", booking);
                }
            }
            else
            {
                return PartialView("BookingForm", booking);
            }
        }

        [HttpPost]
        public ActionResult HandleBookingSubmit(BookingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            ReCaptcha.CaptchaResponse response = new ReCaptcha.CaptchaResponse();
            if (ConfigurationManager.AppSettings["Development"] != "True")
            {
                response = ReCaptcha.ValidateCaptcha(Request["g-recaptcha-response"].ToString());
            }

            //Validate reCaptcha
            if (response.Success == "True" || ConfigurationManager.AppSettings["Development"] == "True")
            {
                try
                {
                    //Insert Request into Db
                    Booking bookingDetails = PopulateBooking(model);
                    var db = UmbracoContext.Application.DatabaseContext.Database;
                    var res = db.Insert(bookingDetails);

                    //Send Email and Redirect to Thank You page.
                    string link = "http://" + Request.Url.Host + "/umbraco#/BookingSection/BookingSectionTree/edit/" + res;
                    EmailHelper email = new EmailHelper();
                    email.SendBookingInternalEmail(model, link);

                    //IPublishedContent formNode = Umbraco.TypedContent(UmbracoContext.PageId.Value);
                    //email.SendCustomerFacingEmail(
                    //                                bookingDetails.Email, 
                    //                                formNode.GetProperty("emailSubject").Value.ToString(),
                    //                                formNode.GetProperty("emailBody").Value.ToString().Replace("src=\"/","src=\"http://" + Request.Url.Authority + "/")
                    //                             );

                    return this.Redirect("thank-you");
                }
                catch (Exception ex)
                {
                    LogHelper.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "failed Submitting booking form", new Exception(ex.Message));
                    TempData["Status"] = "Failed submitting event booking";
                    return CurrentUmbracoPage();
                }
            }
            else
            {
                TempData["reCapthcaMessage"] = response.Message;
                return CurrentUmbracoPage();
            }
        }

        public Booking PopulateBooking(BookingViewModel model)
        {
            return new Booking()
            {
                EventId = model.EventDetails.Id,
                EventName = model.EventDetails.Name,
                Spaces = model.Spaces,
                Name = model.Name,
                DelegateNames = model.DelegateNames,
                Position = model.Position,
                Organisation = model.Organisation,
                Telephone = model.Telephone,
                Email = model.Email,
                Address = model.Address,
                Source = model.SelectedSource,
                DietaryRequirements = model.DietaryRequirements,
                Comments = model.Comments,
                RequestedDate = DateTime.Now
            };
        }
    }
}