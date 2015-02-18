using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace TravelWithMe.Data.MySql.Driver
{
    public class InventoryCommandBuilder
    {

        internal static MySqlCommand BuildGetAllBookedSeatsCommand(MySqlConnection mySqlConnection)
        {
            var cmd = new MySqlCommand("spGetBookedSeats", mySqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            return cmd;
        }

        internal static MySqlCommand BuildSaveBookedSeatCommand(MySqlConnection mySqlConnection, API.Core.Model.BookedSeat bookedSeat)
        {
            var cmd = new MySqlCommand("spSaveBookedSeat", mySqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("inBusTripId", bookedSeat.BusTripId));
            cmd.Parameters.Add(new MySqlParameter("inTravelDate", bookedSeat.TravelDate));
            cmd.Parameters.Add(new MySqlParameter("inSeatNumber", bookedSeat.SeatNumber));
            cmd.Parameters.Add(new MySqlParameter("inBookingId", bookedSeat.BookingId));
            return cmd;
        }

        internal static MySqlCommand BuildDeleteBookedSeatsCommand(MySqlConnection mySqlConnection, long bookingId)
        {
            var cmd = new MySqlCommand("spDeleteBookedSeats", mySqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("inBookingId", bookingId));
            return cmd;
        }

        internal static MySqlCommand BuildSaveBookingInfoCommand(MySqlConnection mySqlConnection, API.Core.Model.BookingInformation bookingInfo)
        {
            var cmd = new MySqlCommand("spSaveBookingInfo", mySqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("inAccountId", bookingInfo.AccountId));
            cmd.Parameters.Add(new MySqlParameter("inBusOperatorId", bookingInfo.SelectedItinerary.BusOperatorId));
            cmd.Parameters.Add(new MySqlParameter("inBusTripId", bookingInfo.SelectedItinerary.BusTripId));
            cmd.Parameters.Add(new MySqlParameter("inContactNumber", bookingInfo.ContactNumber));
            cmd.Parameters.Add(new MySqlParameter("inDropOffPoint", bookingInfo.DropOffPoint.CpName));
            cmd.Parameters.Add(new MySqlParameter("inEmail", bookingInfo.Email));
            cmd.Parameters.Add(new MySqlParameter("inPickupPoint", bookingInfo.PickupPoint.CpName));
            cmd.Parameters.Add(new MySqlParameter("inSelectedSeats", string.Join(",", bookingInfo.SelectedSeats)));
            cmd.Parameters.Add(new MySqlParameter("inTotalAmount", bookingInfo.TotalAmount));
            cmd.Parameters.Add(new MySqlParameter("inTransactionId", bookingInfo.TransactionId));
            cmd.Parameters.Add(new MySqlParameter("inTravelDate", bookingInfo.TravelDate));
            cmd.Parameters.Add(new MySqlParameter("outBookingId", MySqlDbType.Int32));
            cmd.Parameters["outBookingId"].Direction = ParameterDirection.Output;
            return cmd;
        }

        internal static MySqlCommand BuildCheckItineraryCommand(MySqlConnection mySqlConnection, string busTripid)
        {
            var cmd = new MySqlCommand("spCheckItinerary", mySqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("pBusTripId", busTripid));
            return cmd;
        }

        internal static MySqlCommand BuildSaveItineraryCommand(MySqlConnection mySqlConnection, API.Core.Model.BookingInformation bookingInfo)
        {
            var cmd = new MySqlCommand("spSaveItinerary", mySqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("pBusTripId", bookingInfo.SelectedItinerary.BusTripId));
            cmd.Parameters.Add(new MySqlParameter("pBusOperatorId", bookingInfo.SelectedItinerary.BusOperatorId));
            cmd.Parameters.Add(new MySqlParameter("pOperatorName", bookingInfo.SelectedItinerary.BusOperatorId));
            cmd.Parameters.Add(new MySqlParameter("pJourneryTime", bookingInfo.SelectedItinerary.JourneryTime));
            cmd.Parameters.Add(new MySqlParameter("pBusName", bookingInfo.SelectedItinerary.BusName));
            cmd.Parameters.Add(new MySqlParameter("pFrom", bookingInfo.SelectedItinerary.From));
            cmd.Parameters.Add(new MySqlParameter("pTo", bookingInfo.SelectedItinerary.To));
            cmd.Parameters.Add(new MySqlParameter("pDepartureTime", bookingInfo.SelectedItinerary.DepartureTime.ToString()));
            cmd.Parameters.Add(new MySqlParameter("pArrivalTime", bookingInfo.SelectedItinerary.ArrivalTime.ToString()));
            cmd.Parameters.Add(new MySqlParameter("pIsAC", bookingInfo.SelectedItinerary.IsAC));
            cmd.Parameters.Add(new MySqlParameter("pSeatMap", bookingInfo.SelectedItinerary.SeatMap));
            return cmd;
        }

        internal static MySqlCommand BuildSaveBookedItineraryCommand(MySqlConnection mySqlConnection, API.Core.Model.BookingInformation bookingInfo)
        {
            var cmd = new MySqlCommand("spSaveBookedItinerary", mySqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("pBookingId", bookingInfo.BookingId));
            cmd.Parameters.Add(new MySqlParameter("pBusTripId", bookingInfo.SelectedItinerary.BusTripId));
            cmd.Parameters.Add(new MySqlParameter("pFare", bookingInfo.SelectedItinerary.Fare));
            return cmd;
        }

        internal static MySqlCommand BuildSavePassengerCommand(MySqlConnection mySqlConnection, long bookingId, int accountId, API.Core.Model.Passenger passenger)
        {
            var cmd = new MySqlCommand("spSavePassenger", mySqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("pBookingId", bookingId));
            cmd.Parameters.Add(new MySqlParameter("pAccountId", accountId));
            cmd.Parameters.Add(new MySqlParameter("pFirstName", passenger.FirstName));
            cmd.Parameters.Add(new MySqlParameter("pLastName", passenger.LastName));
            cmd.Parameters.Add(new MySqlParameter("pGender", passenger.Gender));
            cmd.Parameters.Add(new MySqlParameter("pAge", passenger.Age));
            cmd.Parameters.Add(new MySqlParameter("passengerId", MySqlDbType.Int32));
            cmd.Parameters["passengerId"].Direction = ParameterDirection.Output;
            return cmd;
        }

        internal static MySqlCommand BuildUpdateBookingInfoCommand(MySqlConnection mySqlConnection, API.Core.Model.BookingInformation bookingInfo)
        {
            var cmd = new MySqlCommand("spUpdateBookingInfo", mySqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("inBookingId", bookingInfo.BookingId));
            cmd.Parameters.Add(new MySqlParameter("inContactNumber", bookingInfo.ContactNumber));
            cmd.Parameters.Add(new MySqlParameter("inDropOffPoint", bookingInfo.DropOffPoint.CpName));
            cmd.Parameters.Add(new MySqlParameter("inEmail", bookingInfo.Email));
            cmd.Parameters.Add(new MySqlParameter("inPickupPoint", bookingInfo.PickupPoint.CpName));
            cmd.Parameters.Add(new MySqlParameter("inSelectedSeats", string.Join(",", bookingInfo.SelectedSeats)));
            cmd.Parameters.Add(new MySqlParameter("inTotalAmount", bookingInfo.TotalAmount));
            cmd.Parameters.Add(new MySqlParameter("inTransactionId", bookingInfo.TransactionId));
            cmd.Parameters.Add(new MySqlParameter("inTravelDate", bookingInfo.TravelDate));
            return cmd;
        }

        internal static MySqlCommand BuildUpdatePassengerCommand(MySqlConnection mySqlConnection, API.Core.Model.Passenger passenger)
        {
            var cmd = new MySqlCommand("spUpdatePassenger", mySqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("pPassengerId", passenger.PassengerId));
            cmd.Parameters.Add(new MySqlParameter("pFirstName", passenger.FirstName));
            cmd.Parameters.Add(new MySqlParameter("pLastName", passenger.LastName));
            cmd.Parameters.Add(new MySqlParameter("pGender", passenger.Gender));
            cmd.Parameters.Add(new MySqlParameter("pAge", passenger.Age));
            return cmd;
        }

        internal static MySqlCommand BuildDeletePassengerInfoCommand(MySqlConnection mySqlConnection, long bookingId)
        {
            var cmd = new MySqlCommand("spDeletePassengers", mySqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("inBookingId", bookingId));
            return cmd;
        }

        internal static MySqlCommand BuildGetBusBookingsCommand(MySqlConnection mySqlConnection, DateTime traveldate, int busTripId)
        {
            var cmd = new MySqlCommand("spGetBusBookings", mySqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("pBusTripId", busTripId));
            cmd.Parameters.Add(new MySqlParameter("pTravelDate", traveldate));
            return cmd;
        }

        internal static MySqlCommand BuildGetPassengersByBookingIdCommand(MySqlConnection mySqlConnection, long bookingId)
        {
            var cmd = new MySqlCommand("spGetPassengersByBookingId", mySqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("pBookingId", bookingId));
            return cmd;
        }

        internal static MySqlCommand BuildDeleteBookingCommand(MySqlConnection mySqlConnection, long bookingId)
        {
            var cmd = new MySqlCommand("spDeleteBooking", mySqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("inBookingId", bookingId));
            return cmd;
        }

        internal static MySqlCommand BuildGetBookingCommand(MySqlConnection mySqlConnection, long bookingId)
        {
            var cmd = new MySqlCommand("spGetBookingInfo", mySqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("pBookingId", bookingId));
            return cmd;
        }
    }
}
