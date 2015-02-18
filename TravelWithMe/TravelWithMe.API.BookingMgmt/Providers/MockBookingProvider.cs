using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.API.Interfaces;
using TravelWithMe.API.Core.Model;
using System.Web;
using TravelWithMe.API.InventoryMgmt.Providers;

namespace TravelWithMe.API.BookingMgmt.Providers
{
    public class MockBookingProvider : IBookingProvider
    {
        public string _busKey = "Buses";
        public List<Core.Model.BookingInformation> GetAllBookings()
        {
            List<BookingInformation> bookings = new List<BookingInformation>();
            for (int i = 0; i < 10; i++)
            {
                BookingInformation bookingInfo = new BookingInformation();
                bookingInfo.BookingId = i + 1;
                bookingInfo.ContactNumber = "98765433" + i;
                bookingInfo.Email = "email" + i + "@gmail.com";
                bookingInfo.Passengers = new List<Passenger>();
                Passenger passenger = new Passenger() { FirstName = "Name" + i, Age = i, Gender = "Male", LastName = "LastName" + i, PassengerId = i, Title = "Mr" };
                bookingInfo.Passengers.Add(passenger);
                passenger = new Passenger() { FirstName = "_Name" + i, Age = i, Gender = "_Male", LastName = "_LastName" + i, PassengerId = (i * 100), Title = "_Mr" };
                bookingInfo.Passengers.Add(passenger);
                bookingInfo.SelectedItinerary = new BusItinerary()
                {
                    ArrivalTime = new BusTime() { Days = 1, Hours = 1, Minutes = 1 },
                    BookedSeats = new List<BookedSeat>() { new BookedSeat() { SeatNumber = i+1}},
                    BusName = "Bus" + i + 1,
                    BusOperatorId = "OperatorId" + i + 1,
                    BusTripId = "1",
                    BusType = "Semi",
                    CityPoints = new List<CityPoint>(),
                    DepartureTime = new BusTime() { Days = 2, Hours = 2, Minutes = 2 },
                    Fare = (100 * i) + 1,
                    From = "PUN",
                    To = "LTR",
                    IsAC = true,
                    JourneryTime = "12AM",
                    OperatorName = "Operator" + i + 1,
                    SeatMap = GetSeatMap((i + 1).ToString()),
                    SeatsAvailable = 10
                };
                bookingInfo.TotalAmount = 100;
                bookings.Add(bookingInfo);
            }
            return bookings;
        }

        public string GetSeatMap(string busTripId)
        {
            string seatMap = "";
            //InventoryProvider inventoryProvider = new InventoryProvider();
            //var busItinerary = inventoryProvider.GetSeatMap(busTripId, true);
            //seatMap = busItinerary.SeatMap;
            return seatMap;
        }
    }
}
