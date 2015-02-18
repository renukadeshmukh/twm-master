using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Caching;
using TravelWithMe.API.Core.Caching.Http;
using TravelWithMe.API.Core.Model;
using TravelWithMe.API.Interfaces;
using System.Web;
using TravelWithMe.API.Logging;
using TravelWithMe.Data.Factories;
using TravelWithMe.Data.Interfaces;
using TravelWithMe.Logging.Helper;

namespace TravelWithMe.API.InventoryMgmt.Providers
{
    public class InventoryProvider : IInventoryProvider
    {
        private const string Source = "InventoryService";
        private ICacheProvider _cacheProvider = null;
        public InventoryProvider(ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }

        public List<BusItinerary> SearchInventory(DateTime travelDate, int from, int to)
        {
            try
                {
                    List<BusInfo> busList = _cacheProvider.GetBusCache().Select(keyValue => keyValue.Value).Where(bus =>
                        {
                            if(bus.FromLoc.Id == from && bus.ToLoc.Id == to && bus.IsEnabled && bus.IsPublished)
                            {
                                if (bus.BusSchedule.Frequency == BusTripFrequency.Daily)
                                {
                                    return true;
                                }
                                if (bus.BusSchedule.Frequency == BusTripFrequency.SpecificDates && bus.BusSchedule.DateRanges != null)
                                {
                                    if (bus.BusSchedule.DateRanges.Exists(range => range.FromDate <= travelDate && travelDate <= range.ToDate))
                                        return true;
                                }
                                if (bus.BusSchedule.Frequency == BusTripFrequency.SpecificWeekDays && bus.BusSchedule.Weekdays != null)
                                {
                                    if (bus.BusSchedule.Weekdays.Exists(day => day == travelDate.DayOfWeek))
                                        return true;
                                }
                            }
                            return false;
                        }).ToList();
                    List<BusItinerary> itineraries = new List<BusItinerary>();
                    foreach (BusInfo busInfo in busList)
                    {
                        BusItinerary itin = GetItineraryFromBusInfo(travelDate, busInfo);
                        itineraries.Add(itin);
                    }
                    return itineraries;
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex, Source, "SearchInventory", Severity.Trace);
                    return null;
                }
            
        }

        public BusItinerary GetBusItinerary(DateTime travelDate, string busTripId)
        {
            int id = Convert.ToInt32(busTripId);
            BusItinerary itin = GetItineraryFromBusInfo(travelDate, _cacheProvider.GetBusCache()[id]);
            return itin;
        }

        public List<BookingInformation> GetBusBookings(DateTime traveldate, int busTripId)
        {
            IInventoryDataProvider dataProvider = InventoryDataProviderFactory.CreateInventoryDataProvider();
            return dataProvider.GetBusBookings(traveldate, busTripId);
        }

        public bool UpdateBooking(BookingInformation bookingInformation, out string errorMessage)
        {
            string bookingKey = string.Format("{0}:{1:MM/dd/yyyy}", bookingInformation.SelectedItinerary.BusTripId,
                          bookingInformation.TravelDate);
            errorMessage = string.Empty;
            try
            {
                var bookingDataCache = _cacheProvider.GetBookingCache();
                IInventoryDataProvider dataProvider = InventoryDataProviderFactory.CreateInventoryDataProvider();
                if (dataProvider.UpdateBookingInfo(bookingInformation, out errorMessage))
                {
                    if (!bookingDataCache.ContainsKey(bookingKey))
                    {
                        bookingDataCache[bookingKey] = new List<BookedSeat>();
                    }
                    dataProvider.DeleteBookedSeats(bookingInformation.BookingId);
                    bookingDataCache[bookingKey].RemoveAll(b => b.BookingId == bookingInformation.BookingId);

                    // Add new seats
                    List<BookedSeat> bookedSeats = new List<BookedSeat>();
                    foreach (var seatNumber in bookingInformation.SelectedSeats)
                    {
                        BookedSeat bookedSeat = new BookedSeat()
                        {
                            BusTripId =
                                int.Parse(bookingInformation.SelectedItinerary.BusTripId),
                            BookingId = bookingInformation.BookingId,
                            SeatNumber = seatNumber,
                            TravelDate = bookingInformation.TravelDate
                        };
                        dataProvider.SaveBookedSeat(bookedSeat, out errorMessage);
                        bookedSeats.Add(bookedSeat);
                    }
                    bookingDataCache[bookingKey].AddRange(bookedSeats);
                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                Logger.LogException(ex, Source, "UpdateBooking", Severity.Trace);
                return false;
            }
            return false;
        }

        public bool DeleteBooking(long bookingId)
        {
            try
            {
                var bookingDataCache = _cacheProvider.GetBookingCache();
                string bookingKey = bookingDataCache.Keys.SingleOrDefault(key => bookingDataCache[key].Any(b => b.BookingId == bookingId));
                if (string.IsNullOrEmpty(bookingKey))
                    return false;
                IInventoryDataProvider dataProvider = InventoryDataProviderFactory.CreateInventoryDataProvider();
                if (dataProvider.DeleteBooking(bookingId))
                {
                    bookingDataCache[bookingKey].RemoveAll(b => b.BookingId == bookingId);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, Source, "DeleteBooking", Severity.Trace);
                return false;
            }
            return false;
        }

        public BookingInformation GetBooking(long bookingId)
        {
            IInventoryDataProvider dataProvider = InventoryDataProviderFactory.CreateInventoryDataProvider();
            return dataProvider.GetBooking(bookingId);
        }

        public BusItinerary GetSeatMap(string busTripId, DateTime travelDate, bool returnSeatMap)
        {
            try
            {
                BusItinerary itin = new BusItinerary();
                int id = Convert.ToInt32(busTripId);
                var busCache = _cacheProvider.GetBusCache();
                if (returnSeatMap)
                {
                    var seatMapCache = _cacheProvider.GetSeatMapCache();
                    if (busCache.ContainsKey(id))
                    {
                        itin.SeatMap = seatMapCache.ContainsKey(busCache[id].SeatMapId)
                                           ? seatMapCache[busCache[id].SeatMapId]
                                           : string.Empty;
                        itin.CityPoints = busCache[id].CityPoints;
                    }
                }

                BusRate rate = busCache[id].BusRates.Find(r => r.DateFrom <= travelDate && travelDate <= r.DateTo);
                if (rate != null)
                {
                    if (travelDate.DayOfWeek == DayOfWeek.Saturday || travelDate.DayOfWeek == DayOfWeek.Sunday)
                        itin.Fare = rate.WeekEndRate;
                    else
                        itin.Fare = rate.WeekDayRate;
                }

                itin.BookedSeats = new List<BookedSeat>();
                var bookingDataCache = _cacheProvider.GetBookingCache();
                string bookingKey = string.Format("{0}:{1:MM/dd/yyyy}", busTripId,travelDate);
                if (bookingDataCache.ContainsKey(bookingKey))
                {
                    itin.BookedSeats = bookingDataCache[bookingKey];
                }
                return itin;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, Source, "GetSeatMap", Severity.Trace);
                return null;
            }
        }

        //Save booking information in the database and cache.
        public bool BookSeats(BookingInformation bookingInformation, out string errorMessage)
        {
            string bookingKey = string.Format("{0}:{1:MM/dd/yyyy}", bookingInformation.SelectedItinerary.BusTripId,
                          bookingInformation.TravelDate);
            errorMessage = string.Empty;
            try
            {
                var bookingDataCache = _cacheProvider.GetBookingCache();
                IInventoryDataProvider dataProvider = InventoryDataProviderFactory.CreateInventoryDataProvider();
                if(dataProvider.SaveBookingInfo(bookingInformation, out errorMessage))
                {
                    List<BookedSeat> bookedSeats = new List<BookedSeat>();
                    foreach (var seatNumber in bookingInformation.SelectedSeats)
                    {
                        BookedSeat bookedSeat = new BookedSeat()
                                                    {
                                                        BusTripId =
                                                            int.Parse(bookingInformation.SelectedItinerary.BusTripId),
                                                        BookingId = bookingInformation.BookingId,
                                                        SeatNumber = seatNumber,
                                                        TravelDate = bookingInformation.TravelDate
                                                    };
                        dataProvider.SaveBookedSeat(bookedSeat, out errorMessage);
                        bookedSeats.Add(bookedSeat);
                    }
                    string busTripId = bookingInformation.SelectedItinerary.BusTripId;
                    if (!bookingDataCache.ContainsKey(bookingKey))
                    {
                        bookingDataCache[bookingKey] = new List<BookedSeat>();
                    }
                    bookingDataCache[bookingKey].AddRange(bookedSeats);
                    return true;
                }
            }
            catch(Exception ex)
            {
                errorMessage = ex.Message;
                Logger.LogException(ex, Source, "BookSeats", Severity.Trace);
                return false;
            }
            return false;
        }

        private BusItinerary GetItineraryFromBusInfo(DateTime travelDate, BusInfo busInfo)
        {
            BusItinerary itin = new BusItinerary();
            itin.ArrivalTime = busInfo.ArrivalTime;
            itin.BusName = busInfo.BusName;
            itin.BusOperatorId = busInfo.BusOperatorId;
            itin.BusTripId = busInfo.BusTripId.ToString();
            itin.BusType = busInfo.BusType.ToString();
            itin.CityPoints = busInfo.CityPoints;
            itin.DepartureTime = busInfo.DepartureTime;
            itin.From = busInfo.FromLoc.Name;
            itin.To = busInfo.ToLoc.Name;
            BusRate rate = busInfo.BusRates.Find(r => r.DateFrom <= travelDate && travelDate <= r.DateTo);
            if (rate != null)
            {
                if (travelDate.DayOfWeek == DayOfWeek.Saturday || travelDate.DayOfWeek == DayOfWeek.Sunday)
                    itin.Fare = rate.WeekEndRate;
                else
                    itin.Fare = rate.WeekDayRate;
            }
            itin.IsAC = busInfo.IsAC;
            itin.OperatorName = busInfo.OperatorName;
            itin.JourneryTime = BusTime.GetJourneyTime(busInfo.DepartureTime, busInfo.ArrivalTime);
            return itin;
        }
    }
}
