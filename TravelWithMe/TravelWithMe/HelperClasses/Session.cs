using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelWithMe.HelperClasses
{
    public class Session
    {
        public string AuthId { get; set; }
        public string SessionId { get; set; } //SessionId
        public string ItnIndex { get; set; } //Selected Itinerary Index
        public string FirstName { get; set; } //User First Name
        public string LastName { get; set; } //User Last Name        
        public string Email { get; set; }
        public string LastPage { get; set; }
        public string CurrentPage { get; set; }
    }
}