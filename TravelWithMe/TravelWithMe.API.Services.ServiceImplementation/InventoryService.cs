using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using TravelWithMe.API.Core.Infra;
using TravelWithMe.API.Interfaces;
using TravelWithMe.API.Logging;
using TravelWithMe.API.Services.DataContract.Messages;
using TravelWithMe.API.Services.ServiceContract;
using TravelWithMe.API.Core.Factories;
using TravelWithMe.Logging.Helper;
using Model = TravelWithMe.API.Core.Model;
using TravelWithMe.API.Services.DataContract;
using System.Threading.Tasks;
using SessionData = TravelWithMe.API.Core.Model.SessionData;

namespace TravelWithMe.API.Services.ServiceImplementation
{
    [ServerMessageLogger]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class InventoryService : IInventoryService
    {
        private const string Source = "InventoryService";

        public SearchBusesRS SearchBuses(string sessionId, string travelDate, string from, string to)
        {
            SearchBusesRS response = new SearchBusesRS() { IsSuccess = true };
            int toCityId, fromCityId;
            string errorMessage = string.Empty;
            if (ValidateSearchInput(travelDate, from, to, out toCityId, out fromCityId, out errorMessage))
            {
                using (new ApplicationContextScope(new ApplicationContext()))
                {
                    try
                    {
                        IBusProvider busProvider = BusProviderFactory.GetBusProvider(); //TODO: This needs to be removed after businfo DAL implementation
                        IInventoryProvider inventoryProvider = InventoryProviderFactory.GetInventoryProvider();
                        List<Model.BusItinerary> itineraries =
                            inventoryProvider.SearchInventory(travelDate.ParseStringToDateTime(), fromCityId, toCityId);
                        response.BusItineraries = itineraries.ToDataContract();
                        ICacheProvider cacheProvider = CacheProviderFactory.GetCacheProvider();
                        Task.Factory.StartNew(() => cacheProvider.UpdateSearchHistory(new Model.City() { Id = fromCityId, Name = from }, new Model.City() { Id = toCityId, Name = to }));
                        if (response.BusItineraries == null)
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "Please try again.";
                        }
                    }
                    catch (Exception e)
                    {
                        response.IsSuccess = false;
                        response.ErrorMessage = e.Message;
                        Logger.LogException(e, Source, "SearchBuses", Severity.Normal);
                    }
                }
            }
            else
            {
                response.IsSuccess = true;
                response.ErrorMessage = errorMessage;
            }
            return response;
        }

        public GetBusBookingsResponse GetBusBookings(string sessionId, string authId, string travelDate, string busTripId)
        {
            GetBusBookingsResponse response = new GetBusBookingsResponse() { IsSuccess = true };
            try
            {
                string busOperatorId = string.Empty;
                if (ValidateAccount(response, busTripId, authId, out busOperatorId, sessionId))
                {
                    IInventoryProvider inventoryProvider = InventoryProviderFactory.GetInventoryProvider();
                    List<Model.BookingInformation> bookings =
                        inventoryProvider.GetBusBookings(travelDate.ParseStringToDateTime(), int.Parse(busTripId));
                    response.Bookings = bookings.ToDataContract();
                    if (response.Bookings == null)
                    {
                        response.IsSuccess = false;
                        response.ErrorMessage = "Please try again.";
                    }
                }
            }
            catch (Exception exception)
            {
                response.IsSuccess = false;
                response.ErrorMessage = exception.Message;
                Logger.LogException(exception, Source, "GetBusBookings", Severity.Trace);
            }
            return response;
        }

        public GetSeatMapRS GetSeatMap(string sessionId, string busTripId, string travelDate, bool returnSeatMap)
        {
            GetSeatMapRS response = new GetSeatMapRS() { IsSuccess = true };
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                ApplicationContext.SetSessionId(sessionId);
                try
                {
                    DateTime tDate = travelDate.ParseStringToDateTime();
                    if (IsBusAvailable(int.Parse(busTripId), tDate))
                    {
                        IInventoryProvider inventoryProvider = InventoryProviderFactory.GetInventoryProvider();
                        Model.BusItinerary busItinerary =
                            inventoryProvider.GetSeatMap(busTripId, tDate, returnSeatMap);
                        if (busItinerary == null)
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = "Please try again.";
                        }
                        else
                        {
                            response.SeatMap = busItinerary.SeatMap;
                            response.BookedSeats = busItinerary.BookedSeats.ToDataContract();
                            response.Fare = busItinerary.Fare;
                            response.CityPoints = busItinerary.CityPoints.ToDataContract();
                        }
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.ErrorMessage = "Bus is not scheduled on " + travelDate;
                    }
                }
                catch (Exception e)
                {
                    response.IsSuccess = false;
                    response.ErrorMessage = e.Message;
                    Logger.LogException(e, Source, "GetSeatMap", Severity.Normal);
                }
            }
            return response;
        }

        public BookSeatsResponse BookSeats(string sessionId, string authId, BookingInformation bookingInfo)
        {
            BookSeatsResponse response = new BookSeatsResponse() { IsSuccess = true };
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                ApplicationContext.SetSessionId(sessionId);
                try
                {
                    string busOperatorId = string.Empty;
                    if (ValidateAccount(response, bookingInfo.SelectedItinerary.BusTripId, authId, out busOperatorId, sessionId))
                    {
                        IInventoryProvider inventoryProvider = InventoryProviderFactory.GetInventoryProvider();
                        if (!string.IsNullOrEmpty(bookingInfo.TravelDate))
                        {
                            bookingInfo.SelectedItinerary =
                                inventoryProvider.GetBusItinerary(bookingInfo.TravelDate.ParseStringToDateTime(),
                                                                  bookingInfo.SelectedItinerary.
                                                                      BusTripId).ToDataContract();
                        }
                        string validationError = string.Empty;
                        if (ValidBookingRequest(bookingInfo, out validationError))
                        {
                            
                            Model.BookingInformation objBookInfo = bookingInfo.ToDataModel();
                            //TODO:assing proper transaction id
                            objBookInfo.TransactionId = Guid.NewGuid().ToString();
                            objBookInfo.AccountId = Int32.Parse(busOperatorId);
                            var sessionData = SaveBookingData(sessionId, response, objBookInfo);
                            if (!response.IsSuccess)
                            {
                                response.ErrorMessage =
                                    "Failed to save booking information into the session! Please try again.";
                                return response;
                            }

                            List<Model.BookedSeat> bookedSeats =
                                inventoryProvider.GetSeatMap(bookingInfo.SelectedItinerary.BusTripId,
                                                             bookingInfo.TravelDate.ParseStringToDateTime(), false).
                                    BookedSeats.ToList();
                            if (bookedSeats.Select(b => b.SeatNumber).Intersect(bookingInfo.SelectedSeats).Any()) // Check availability of seats
                            {
                                response.BookedSeats = bookedSeats.ToDataContract();
                                response.SelectedSeats = bookingInfo.SelectedSeats.Except(bookedSeats.Select(b => b.SeatNumber)).ToList();
                                response.IsSuccess = false;
                                response.ErrorMessage =
                                    "Some of the selected seats are not available. Please select another seats.";
                            }
                            else // if seats available then mark them as booked.
                            {
                                string errorMessage = string.Empty;
                                if (!inventoryProvider.BookSeats(objBookInfo, out errorMessage) ||
                                    !string.IsNullOrEmpty(errorMessage))
                                {
                                    response.ErrorMessage = errorMessage;
                                    response.IsSuccess = false;
                                }
                                else
                                {
                                    response.BookingId = objBookInfo.BookingId;
                                    sessionData.BookingInfo.BookingId = objBookInfo.BookingId;
                                }
                            }
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = validationError;
                        }
                    }
                }
                catch (Exception exception)
                {
                    response.IsSuccess = false;
                    response.ErrorMessage = exception.Message;
                    Logger.LogException(exception, Source, "BookSeats", Severity.Trace);
                }
            }
            return response;
        }

        public UpdateBookingResponse UpdateBooking(string sessionId, string authId, BookingInformation bookingInfo)
        {
            UpdateBookingResponse response = new UpdateBookingResponse() { IsSuccess = true };
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                ApplicationContext.SetSessionId(sessionId);
                try
                {
                    string busOperatorId = string.Empty;
                    if (ValidateAccount(response, bookingInfo.SelectedItinerary.BusTripId, authId, out busOperatorId, sessionId))
                    {
                        IInventoryProvider inventoryProvider = InventoryProviderFactory.GetInventoryProvider();
                        if (!string.IsNullOrEmpty(bookingInfo.TravelDate))
                        {
                            bookingInfo.SelectedItinerary =
                                inventoryProvider.GetBusItinerary(bookingInfo.TravelDate.ParseStringToDateTime(),
                                                                  bookingInfo.SelectedItinerary.
                                                                      BusTripId).ToDataContract();
                        }
                        string validationError = string.Empty;
                        if (ValidBookingRequest(bookingInfo, out validationError))
                        {

                            Model.BookingInformation objBookInfo = bookingInfo.ToDataModel();
                            //TODO:assing proper transaction id
                            objBookInfo.AccountId = Int32.Parse(busOperatorId);
                            var sessionData = SaveBookingData(sessionId, response, objBookInfo);
                            if (!response.IsSuccess)
                            {
                                response.ErrorMessage =
                                    "Failed to save booking information into the session! Please try again.";
                                return response;
                            }

                            List<Model.BookedSeat> bookedSeats =
                                inventoryProvider.GetSeatMap(bookingInfo.SelectedItinerary.BusTripId,
                                                             bookingInfo.TravelDate.ParseStringToDateTime(), false).BookedSeats.ToList();
                            List<int> previousSeats =
                                bookedSeats.Where(s => s.BookingId == objBookInfo.BookingId).Select(s => s.SeatNumber).
                                    ToList();
                            List<int> newSeats = objBookInfo.SelectedSeats.Except(previousSeats).ToList();
                            if (bookedSeats.Select(b => b.SeatNumber).Intersect(newSeats).Any()) // Check availability of seats
                            {
                                response.BookedSeats = bookedSeats.ToDataContract();
                                response.SelectedSeats = newSeats.Except(bookedSeats.Select(b => b.SeatNumber)).ToList();
                                response.IsSuccess = false;
                                response.ErrorMessage =
                                    "Some of the selected seats are not available. Please select another seats.";
                            }
                            else // if seats available then mark them as booked.
                            {
                                string errorMessage = string.Empty;
                                if (!inventoryProvider.UpdateBooking(objBookInfo,  out errorMessage) ||
                                    !string.IsNullOrEmpty(errorMessage))
                                {
                                    response.ErrorMessage = errorMessage;
                                    response.IsSuccess = false;
                                }
                                else
                                {
                                    sessionData.BookingInfo.BookingId = objBookInfo.BookingId;
                                }
                            }
                        }
                        else
                        {
                            response.IsSuccess = false;
                            response.ErrorMessage = validationError;
                        }
                    }
                }
                catch (Exception exception)
                {
                    response.IsSuccess = false;
                    response.ErrorMessage = exception.Message;
                    Logger.LogException(exception, Source, "UpdateBooking", Severity.Trace);
                }
            }
            return response;
        }

        public DeleteBookingResponse DeleteBooking(string sessionId, string authId, string busTripId, string bookingId)
        {
            DeleteBookingResponse response = new DeleteBookingResponse() { IsSuccess = true };
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                ApplicationContext.SetSessionId(sessionId);
                try
                {
                    string busOperatorId = string.Empty;
                    if (!string.IsNullOrEmpty(busTripId) && !string.IsNullOrEmpty(bookingId) && ValidateAccount(response, busTripId, authId, out busOperatorId, sessionId))
                    {
                        IInventoryProvider inventoryProvider = InventoryProviderFactory.GetInventoryProvider();
                        if(inventoryProvider.DeleteBooking(long.Parse(bookingId)))
                        {
                            return response;
                        }
                        response.ErrorMessage = "Faild to delete booking!";
                    }
                    response.IsSuccess = false;
                    return response;
                }
                catch (Exception exception)
                {
                    response.IsSuccess = false;
                    response.ErrorMessage = exception.Message;
                    Logger.LogException(exception, Source, "DeleteBooking", Severity.Trace);
                }
            }
            return response;
        }

        public GetBookingResponse GetBooking(string sessionId, string authId, string busTripId, string bookingId)
        {
            GetBookingResponse response = new GetBookingResponse() { IsSuccess = true };
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                ApplicationContext.SetSessionId(sessionId);
                try
                {
                    string busOperatorId = string.Empty;
                    if (!string.IsNullOrEmpty(busTripId) && !string.IsNullOrEmpty(bookingId) && ValidateAccount(response, busTripId, authId, out busOperatorId, sessionId))
                    {
                        IInventoryProvider inventoryProvider = InventoryProviderFactory.GetInventoryProvider();
                        Model.BookingInformation booking = inventoryProvider.GetBooking(long.Parse(bookingId));
                        
                        if (booking!=null)
                        {
                            Model.BusItinerary itin =
                                inventoryProvider.GetBusItinerary(booking.TravelDate, booking.SelectedItinerary.BusTripId);
                            response.Booking = booking.ToDataContract();
                            response.Booking.SelectedItinerary = itin.ToDataContract();
                            response.Booking.SelectedItinerary.SeatMap = "";
                            return response;
                        }
                        response.ErrorMessage = "Faild to get booking!";
                    }
                    response.IsSuccess = false;
                    return response;
                }
                catch (Exception exception)
                {
                    response.IsSuccess = false;
                    response.ErrorMessage = exception.Message;
                    Logger.LogException(exception, Source, "GetBooking", Severity.Trace);
                }
            }
            return response;
        }

        #region private helper functions
        private bool ValidBookingRequest(BookingInformation bookingInfo, out string validationError)
        {
            validationError = string.Empty;
            if (bookingInfo == null)
            {
                validationError = "No booking information found!!";
                return false;
            }
            if (bookingInfo.SelectedSeats == null || bookingInfo.SelectedSeats.Count == 0)
            {
                validationError = "Please select atleast one seat!!";
                return false;
            }
            if (bookingInfo.SelectedSeats.Count > 5)
            {
                validationError = "At most 5 seats can be selected in one booking!!";
                return false;
            }
            if (bookingInfo.Passengers == null || bookingInfo.Passengers.Count == 0)
            {
                validationError = "No passenger information found!!";
                return false;
            }
            if (bookingInfo.Passengers.Count != bookingInfo.SelectedSeats.Count)
            {
                validationError = string.Format("{0} passengers information required.", bookingInfo.SelectedSeats.Count);
                return false;
            }
            if (string.IsNullOrEmpty(bookingInfo.TravelDate))
            {
                validationError = "Travel date missing from the booking request!!";
                return false;
            }
            if (bookingInfo.SelectedItinerary == null || string.IsNullOrEmpty(bookingInfo.SelectedItinerary.BusTripId))
            {
                validationError = "Selected bus itinerary not found!! Please search again.";
                return false;
            }
            if (bookingInfo.TotalAmount <= 0)
            {
                validationError = "Amount cannot be zero.";
                return false;
            }
            decimal expectedAmount = (bookingInfo.SelectedItinerary.Fare * bookingInfo.SelectedSeats.Count);
            if (bookingInfo.TotalAmount < expectedAmount)
            {
                validationError = "Price mismatch found!!!";
                return false;
            }
            return true;
        }

        private bool ValidateSearchInput(string travelDate, string from, string to, out int toCityId, out int fromCityId, out string errorMessage)
        {
            errorMessage = string.Empty;
            toCityId = 0;
            fromCityId = 0;
            if (string.IsNullOrEmpty(travelDate) || string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
            {
                errorMessage = "Validation failed! One of the search paramerter missing";
                return false;
            }
            ICacheProvider cacheProvider = CacheProviderFactory.GetCacheProvider();
            fromCityId = cacheProvider.GetCityId(from);
            if (fromCityId == 0)
            {
                errorMessage = "Validation failed! Invalid Departure city.";
                return false;
            }
            toCityId = cacheProvider.GetCityId(to);
            if (fromCityId == 0)
            {
                errorMessage = "Validation failed! Invalid Arrival city.";
                return false;
            }
            return true;
        }

        private bool ValidateAccount(BaseResponse response, string busTripId, string authId, out string accountID, string sessionId)
        {
            IAuthenticationProvider authProvider = AuthenticationProviderFactory.GetAuthenticationProvider();
            if (!authProvider.Validate(authId))
            {
                response.IsSuccess = false;
                response.ErrorMessage = "Validation failed! Not a valid user, Please login and try again!!";
                accountID = null;
                return false;
            }
            accountID = authProvider.GetAccountId(authId);
            if (string.IsNullOrEmpty(accountID))
            {
                response.IsSuccess = false;
                response.ErrorMessage = "Validation failed! User not found, Please loging and try again!!";
                return false;
            }
            ISessionProvider sessionProvider = SessionProviderFactory.GetSessionProvider();
            bool isBusOperator = sessionProvider.IsBusOperator(sessionId, accountID);
            if (!isBusOperator)
            {
                response.IsSuccess = false;
                response.ErrorMessage = "Validation failed! your are not a bus operator!!";
                return false;
            }
            if (isBusOperator)
            {
                Model.BusInfo cachedBus =
                    CacheProviderFactory.GetCacheProvider().GetBusCache()[int.Parse(busTripId)];
                if (!string.Equals(cachedBus.BusOperatorId, accountID, StringComparison.OrdinalIgnoreCase))
                {
                    response.IsSuccess = false;
                    response.ErrorMessage = "Validation failed! Invalid bus!";
                }
            }
            return true;
        }

        private static SessionData SaveBookingData(string sessionId, BaseResponse response, Model.BookingInformation objBookInfo)
        {
            ISessionProvider sessionProvider = SessionProviderFactory.GetSessionProvider();
            Model.SessionData sessionData = sessionProvider.GetSession(sessionId);

            sessionData.BookingInfo = sessionData.BookingInfo ?? new Model.BookingInformation();
            sessionData.BookingInfo.TravelDate = objBookInfo.TravelDate;
            sessionData.BookingInfo.SelectedItinerary = objBookInfo.SelectedItinerary;
            sessionData.BookingInfo.SelectedSeats = objBookInfo.SelectedSeats;
            sessionData.BookingInfo.TotalAmount = objBookInfo.TotalAmount;
            sessionData.BookingInfo.ContactNumber = objBookInfo.ContactNumber;
            sessionData.BookingInfo.Email = objBookInfo.Email;
            sessionData.BookingInfo.Passengers = objBookInfo.Passengers;
            sessionData.BookingInfo.PickupPoint = objBookInfo.PickupPoint;
            sessionData.BookingInfo.DropOffPoint = objBookInfo.DropOffPoint;
            sessionData.BookingInfo.TransactionId = objBookInfo.TransactionId;
            sessionData.BookingInfo.AccountId = objBookInfo.AccountId;
            response.IsSuccess = sessionProvider.SaveSession(sessionData);
            return sessionData;
        }

        private bool IsBusAvailable(int busTripId, DateTime tDate)
        {
            ICacheProvider cacheProvider = CacheProviderFactory.GetCacheProvider();
            Model.BusInfo bus = cacheProvider.GetBusCache()[busTripId];
            if (bus.BusSchedule.Frequency == Model.BusTripFrequency.Daily)
            {
                return true;
            }
            if (bus.BusSchedule.Frequency == Model.BusTripFrequency.SpecificDates && bus.BusSchedule.DateRanges != null)
            {
                if (bus.BusSchedule.DateRanges.Exists(range => range.FromDate <= tDate && tDate <= range.ToDate))
                    return true;
            }
            if (bus.BusSchedule.Frequency == Model.BusTripFrequency.SpecificWeekDays && bus.BusSchedule.Weekdays != null)
            {
                if (bus.BusSchedule.Weekdays.Exists(day => day == tDate.DayOfWeek))
                    return true;
            }
            return false;
        }
        #endregion
    }
}
