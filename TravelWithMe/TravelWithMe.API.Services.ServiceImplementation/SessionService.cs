using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using TravelWithMe.API.Core.Factories;
using TravelWithMe.API.Core.Infra;
using TravelWithMe.API.Interfaces;
using TravelWithMe.API.Logging;
using TravelWithMe.API.Services.DataContract;
using TravelWithMe.API.Services.ServiceContract;
using TravelWithMe.Logging.Helper;
using Model = TravelWithMe.API.Core.Model;

namespace TravelWithMe.API.Services.ServiceImplementation
{
    [ServerMessageLogger]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SessionService : ISessionService
    {
        private const string Source = "SessionService";

        public CreateSessionResponse CreateSession(string sessionId)
        {
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                ApplicationContext.SetSessionId(sessionId);
                try
                {
                    ISessionProvider sessionProvider = SessionProviderFactory.GetSessionProvider();

                    return new CreateSessionResponse() {SessionId = sessionProvider.CreateSession(sessionId)};
                }
                catch (Exception exception)
                {
                    Logger.LogException(exception, Source, "CreateSession", Severity.Trace);
                    return null;
                }
            }
        }

        public SaveBookingInfoResponse SaveBookingInfo(string sessionId, string section, BookingInformation bookingInfo)
        {
            SaveBookingInfoResponse response = new SaveBookingInfoResponse() {IsSuccess = true};
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                ApplicationContext.SetSessionId(sessionId);
                try
                {
                    ISessionProvider sessionProvider = SessionProviderFactory.GetSessionProvider();
                    Model.SessionData sessionData = sessionProvider.GetSession(sessionId);
                    Model.BookingInformation objBookInfo = bookingInfo.ToDataModel();
                    sessionData.BookingInfo = sessionData.BookingInfo ?? new Model.BookingInformation();
                    if(string.IsNullOrEmpty(section) || string.Equals(section, "Bus"))
                    {
                        sessionData.BookingInfo.TravelDate = objBookInfo.TravelDate;
                        sessionData.BookingInfo.SelectedItinerary = objBookInfo.SelectedItinerary;
                        sessionData.BookingInfo.SelectedSeats = objBookInfo.SelectedSeats;
                        sessionData.BookingInfo.TotalAmount = objBookInfo.TotalAmount;
                    }
                    else
                    {
                        sessionData.BookingInfo.ContactNumber = objBookInfo.ContactNumber;
                        sessionData.BookingInfo.Email = objBookInfo.Email;
                        sessionData.BookingInfo.Passengers = objBookInfo.Passengers;
                    }
                    response.IsSuccess = sessionProvider.SaveSession(sessionData);
                    if (!response.IsSuccess)
                        response.ErrorMessage = "Failed to save booking information in to the session! Please try again.";
                }
                catch (Exception exception)
                {
                    response.IsSuccess = false;
                    Logger.LogException(exception, Source, "SaveBookingInfo", Severity.Trace);
                }
            }
            return response;
        }

        public GetSessionResponse GetSessionData(string sessionId)
        {
            GetSessionResponse response = new GetSessionResponse() { IsSuccess = true };
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                ApplicationContext.SetSessionId(sessionId);
                try
                {
                    ISessionProvider sessionProvider = SessionProviderFactory.GetSessionProvider();
                    Model.SessionData sessionData = sessionProvider.GetSession(sessionId);
                    response.Session = sessionData.ToDataContract();
                }
                catch (Exception exception)
                {
                    response.IsSuccess = false;
                    Logger.LogException(exception, Source, "GetSessionData", Severity.Trace);
                }
            }
            return response;
        }

        public BookingInfoResponse GetBookingInfo(string sessionId)
        {
            BookingInfoResponse response = new BookingInfoResponse() { IsSuccess = true };
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                ApplicationContext.SetSessionId(sessionId);
                try
                {
                    ISessionProvider sessionProvider = SessionProviderFactory.GetSessionProvider();
                    Model.SessionData sessionData = sessionProvider.GetSession(sessionId);
                    if (sessionData != null && sessionData.BookingInfo != null)
                    {
                        response.BookingInfo = sessionData.BookingInfo.ToDataContract();
                        IInventoryProvider inventoryProvider = InventoryProviderFactory.GetInventoryProvider();
                        Model.BusItinerary busItinerary = inventoryProvider.GetSeatMap(response.BookingInfo.SelectedItinerary.BusTripId, response.BookingInfo.TravelDate.ParseStringToDateTime(), false);
                        response.BookedSeats = busItinerary.BookedSeats.ToDataContract();
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.ErrorMessage = "Not able to find booking information in the response!";
                    }
                }
                catch (Exception exception)
                {
                    response.IsSuccess = false;
                    Logger.LogException(exception, Source, "GetBookingInfo", Severity.Trace);
                }
            }
            return response;
        }
    }
}
