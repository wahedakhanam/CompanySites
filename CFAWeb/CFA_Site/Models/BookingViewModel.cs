using CustomExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CFA_Site.Models
{
    public class BookingViewModel
    {
        [Required]
        public int Spaces { get; set; }

        [Required]
        public string Name { get; set; }

        [Required(ErrorMessage = "The Delegate Names field is required.")]
        public string DelegateNames { get; set; }

        [Required]
        public string Position { get; set; }

        [Required]
        public string Organisation { get; set; }

        [Required]
        public string Telephone { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Source { get; set; }

        public string SelectedSource { get; set; }

        public IEnumerable<SelectListItem> SourceItems { get; set; }

        //{
        //    get
        //    {
        //        return new List<SelectListItem>()
        //        {
        //            new SelectListItem(){ Value="1", Text="E-mail"},
        //            new SelectListItem(){ Value="2", Text="Website"},
        //            new SelectListItem(){ Value="3", Text="Newsletter"},
        //            new SelectListItem(){ Value="4", Text="Assessor or Consultant"},
        //            new SelectListItem(){ Value="5", Text="Partner"},
        //            new SelectListItem(){ Value="6", Text="Other"}
        //        };
        //    }
        //}

        public string DietaryRequirements { get; set; }

        public string Comments { get; set; }

        [BooleanRequired(ErrorMessage = "You must accept the terms and conditions.")]
        public bool AcceptTCs { get; set; }

        public Event EventDetails { get; set; }

        public BookingViewModel()
        {
            EventDetails = new Event();
        }
    }

    public class Event
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime Date_DateTime { get; set; }

        public string Date { get; set; }

        public string Category { get; set; }

        public string Time { get; set; }

        public string Address { get; set; }

        public int AvailableTickets { get; set; }

        public int AllocatedTickets { get; set; }

    }
}