using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Activation;
using TravelWithMe.API.Logging;
using TravelWithMe.API.Services.ServiceContract;
using TravelWithMe.API.Core.Factories;
using TravelWithMe.API.Interfaces;
using TravelWithMe.API.Services.DataContract;

namespace TravelWithMe.API.Services.ServiceImplementation
{
    [ServerMessageLogger]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class BookingService : IBookingService
    {

        public List<DataContract.BookingInformation> GetAllBookings(string authId)
        {
            List<BookingInformation> bookings = new List<BookingInformation>();
            try
            {
                IBookingProvider provider = BookingProviderFactory.GetBusProvider();
                var doBookings = provider.GetAllBookings();
                doBookings.ForEach(b => bookings.Add(b.ToDataContract()));
            }
            catch (Exception ex)
            {

            }
            return bookings;
        }
    }
}
