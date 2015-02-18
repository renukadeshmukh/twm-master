using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Transactions;
using MySql.Data.MySqlClient;
using TravelWithMe.API.Core.Model;
using TravelWithMe.Data.Interfaces;
using TravelWithMe.Data.MySql.Driver;
using TravelWithMe.Data.MySql.Entities;
using TravelWithMe.Logging.Helper;

namespace TravelWithMe.Data.MySql
{
    public class InventoryDataProvider : IInventoryDataProvider
    {
        private string Source = "InventoryDataProvider";

        public List<BookedSeat> GetAllBookedSeats()
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.BookingDB);
                MySqlCommand command = InventoryCommandBuilder.BuildGetAllBookedSeatsCommand(db.Connection);
                DataSet dataSet = db.ExecuteQuery(command);
                List<BookedSeat> bookedSeats = new List<BookedSeat>();
                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    if (dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in dataSet.Tables[0].Rows)
                        {
                            var bookedSeat = new BookedSeat()
                            {
                                BusTripId = row["BusTripId"].GetInt(),
                                BookingId = row["BookingId"].GetInt(),
                                SeatNumber = row["SeatNumber"].GetInt(),
                                TravelDate = row["TravelDate"].GetDate(),
                            };
                            bookedSeats.Add(bookedSeat);
                        }
                    }
                }
                return bookedSeats;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "GetAllBookedSeats", Severity.Critical);
                return null;
            }
        }

        public bool SaveBookedSeat(BookedSeat bookedSeat, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                var db = new MySqlDatabase(DbConfiguration.BookingDB);
                MySqlCommand cmd = InventoryCommandBuilder.BuildSaveBookedSeatCommand(db.Connection, bookedSeat);
                db.ExecuteNonQuery(cmd);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                DBExceptionLogger.LogException(ex, Source, "SaveBookedSeat", Severity.Critical);
                return false;
            }
        }

        public bool DeleteBookedSeats(long bookingId)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.BookingDB);
                MySqlCommand cmd = InventoryCommandBuilder.BuildDeleteBookedSeatsCommand(db.Connection, bookingId);
                db.ExecuteNonQuery(cmd);
                return true;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "DeleteBookedSeats", Severity.Critical);
                return false;
            }
        }

        public bool SaveBookingInfo(BookingInformation bookingInfo, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    int bookingId = 0;
                    var db = new MySqlDatabase(DbConfiguration.BookingDB);

                    //Save Booking information.
                    MySqlCommand cmd = InventoryCommandBuilder.BuildSaveBookingInfoCommand(db.Connection, bookingInfo);
                    db.ExecuteNonQuery(cmd, "outBookingId", out bookingId);
                    bookingInfo.BookingId = bookingId;

                    //Check itinerary information is present or not
                    cmd = InventoryCommandBuilder.BuildCheckItineraryCommand(db.Connection,
                                                                             bookingInfo.SelectedItinerary.BusTripId);
                    DataSet dataSet = db.ExecuteQuery(cmd);
                    if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows == null || dataSet.Tables[0].Rows.Count == 0)
                    {
                        //If itinerary information is not present then save it.
                        cmd = InventoryCommandBuilder.BuildSaveItineraryCommand(db.Connection, bookingInfo);
                        db.ExecuteNonQuery(cmd);
                    }

                    //Insert booked itineray information
                    cmd = InventoryCommandBuilder.BuildSaveBookedItineraryCommand(db.Connection, bookingInfo);
                    db.ExecuteNonQuery(cmd);


                    //Save passenger information
                    foreach (Passenger passenger in bookingInfo.Passengers)
                    {
                        SavePassenger(bookingId, bookingInfo.AccountId, passenger, out errorMessage);
                    }
                    scope.Complete();
                }
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                DBExceptionLogger.LogException(ex, Source, "SaveBookingInfo", Severity.Critical);
                return false;
            }
        }

        public bool UpdateBookingInfo(BookingInformation bookingInfo, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var db = new MySqlDatabase(DbConfiguration.BookingDB);
                    MySqlCommand cmd = InventoryCommandBuilder.BuildUpdateBookingInfoCommand(db.Connection, bookingInfo);
                    db.ExecuteNonQuery(cmd);

                    //Delete booking passengers
                    cmd = InventoryCommandBuilder.BuildDeletePassengerInfoCommand(db.Connection, bookingInfo.BookingId);
                    db.ExecuteNonQuery(cmd);

                    //Save passenger information
                    foreach (Passenger passenger in bookingInfo.Passengers)
                    {
                        SavePassenger(bookingInfo.BookingId, bookingInfo.AccountId, passenger, out errorMessage);
                    }
                    scope.Complete();
                }
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                DBExceptionLogger.LogException(ex, Source, "UpdateBookingInfo", Severity.Critical);
                return false;
            }
        }

        public bool UpdatePassenger(Passenger passenger)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.BookingDB);
                int passengerId;
                MySqlCommand cmd = InventoryCommandBuilder.BuildUpdatePassengerCommand(db.Connection, passenger);
                db.ExecuteNonQuery(cmd);
                return true;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "UpdatePassenger", Severity.Critical);
                return false;
            }
        }

        public bool SavePassenger(long bookingId, int accountId, Passenger passenger, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                var db = new MySqlDatabase(DbConfiguration.BookingDB);
                int passengerId;
                MySqlCommand cmd = InventoryCommandBuilder.BuildSavePassengerCommand(db.Connection, bookingId, accountId, passenger);
                db.ExecuteNonQuery(cmd, "passengerId", out passengerId);
                passenger.PassengerId = passengerId;
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                DBExceptionLogger.LogException(ex, Source, "SavePassenger", Severity.Critical);
                return false;
            }
        }

        public List<BookingInformation> GetBusBookings(DateTime traveldate, int busTripId)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.BookingDB);
                MySqlCommand cmd = InventoryCommandBuilder.BuildGetBusBookingsCommand(db.Connection, traveldate, busTripId);
                DataSet dataSet = db.ExecuteQuery(cmd);
                List<BookingInformation> bookings = ParseBookings(dataSet);
                return bookings;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "GetBusBookings", Severity.Critical);
                return null;
            }
        }

        public bool DeleteBooking(long bookingId)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.BookingDB);
                MySqlCommand cmd = InventoryCommandBuilder.BuildDeleteBookingCommand(db.Connection, bookingId);
                db.ExecuteNonQuery(cmd);
                return true;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "DeleteBooking", Severity.Critical);
                return false;
            }
        }

        public BookingInformation GetBooking(long bookingId)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.BookingDB);
                MySqlCommand cmd = InventoryCommandBuilder.BuildGetBookingCommand(db.Connection, bookingId);
                DataSet dataSet = db.ExecuteQuery(cmd);
                List<BookingInformation> bookings = ParseBookings(dataSet);
                return bookings[0];
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "GetBooking", Severity.Critical);
                return null;
            }
        }

        private List<BookingInformation> ParseBookings(DataSet dataSet)
        {
            List<BookingInformation> bookings = new List<BookingInformation>();
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                if (dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count >= 1)
                {
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        BookingInformation booking = ParseBooking(row);
                        if (booking != null)
                            bookings.Add(booking);
                    }
                    return bookings;
                }
            }
            return null;
        }

        private BookingInformation ParseBooking(DataRow row)
        {
            if (Convert.IsDBNull(row["BookingId"]))
                return null;
            var booking = new BookingInformation()
            {
                BookingId = row["BookingId"].GetInt(),
                SelectedItinerary = new BusItinerary() { BusTripId = row["BusTripId"].GetString() },
                AccountId = row["AccountId"].GetInt(),
                Email = row["Email"].GetString(),
                ContactNumber = row["ContactNumber"].GetString(),
                DropOffPoint = new CityPoint() { CpName = row["DropOffPoint"].GetString() },
                PickupPoint = new CityPoint() { CpName = row["PickupPoint"].GetString() },
                SelectedSeats = ParseSeatNumbers(row["SelectedSeats"].GetString()),
                TotalAmount = row["TotalAmount"].GetDecimal(),
                TransactionId = row["TransactionId"].GetString(),
                TravelDate = row["TravelDate"].GetDate(),
            };
            booking.Passengers = GetPassengers(booking.BookingId);
            return booking;
        }

        private List<int> ParseSeatNumbers(string seats)
        {
            string[] seatParts = seats.Split(',');
            List<int> seatNumbers = new List<int>();
            foreach (string seatPart in seatParts)
            {
                seatNumbers.Add(int.Parse(seatPart));
            }
            return seatNumbers;
        }

        private List<Passenger> GetPassengers(long bookingId)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.BookingDB);
                MySqlCommand cmd = InventoryCommandBuilder.BuildGetPassengersByBookingIdCommand(db.Connection, bookingId);
                DataSet dataSet = db.ExecuteQuery(cmd);
                List<Passenger> passengers = ParsePassengers(dataSet);
                return passengers;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "GetPassengers", Severity.Critical);
                return null;
            }
        }

        private List<Passenger> ParsePassengers(DataSet dataSet)
        {
            List<Passenger> passengers = new List<Passenger>();
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                if (dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count >= 1)
                {
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        Passenger passenger = ParsePassenger(row);
                        if (passenger != null)
                            passengers.Add(passenger);
                    }
                    return passengers;
                }
            }
            return null;
        }

        private Passenger ParsePassenger(DataRow row)
        {
            var passenger = new Passenger()
            {
                PassengerId = row["PassengerId"].GetInt(),
                FirstName = row["FirstName"].GetString(),
                LastName = row["LastName"].GetString(),
                Gender = row["Gender"].GetString(),
                Age = row["Age"].GetInt(),
            };
            return passenger;
        }
    }
}
