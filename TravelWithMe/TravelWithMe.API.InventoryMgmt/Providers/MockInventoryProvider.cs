using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using TravelWithMe.API.Core.Caching.Http;
using TravelWithMe.API.Core.Model;
using TravelWithMe.API.Interfaces;
using TravelWithMe.API.Logging;
using TravelWithMe.Logging.Helper;
using System.Collections.Concurrent;

namespace TravelWithMe.API.InventoryMgmt.Providers
{
    public class MockInventoryProvider : IInventoryProvider
    {
        private const string Source = "InventoryService";
        public ConcurrentDictionary<int, BusInfo> _busCache = null;
        private ConcurrentDictionary<string, List<BookedSeat>> _bookingDataCache = null;
        private ConcurrentDictionary<int, string> _seatMapCache = null;
        public MockInventoryProvider()
        {
            string _busKey = "Buses";
            string _bookingDataKey = "BookingDataCache";
            string _seatMapCacheKey = "SeatMapCache";
            _busCache = ApplicationCache.GetValue(_busKey) as ConcurrentDictionary<int, BusInfo>;
            if(_busCache == null)
            {
                _busCache = new ConcurrentDictionary<int, BusInfo>();
                ApplicationCache.Insert(_busKey, _busCache, null, Cache.NoAbsoluteExpiration,
                                         DateTime.Now.AddHours(1) - DateTime.Now,
                                         CacheItemPriority.Normal,
                                         null);
            }
            _seatMapCache = ApplicationCache.GetValue(_seatMapCacheKey) as ConcurrentDictionary<int, string>;
            if (_seatMapCache == null)
            {
                _seatMapCache = new ConcurrentDictionary<int, string>();
                ApplicationCache.Insert(_seatMapCacheKey, _seatMapCache, null, Cache.NoAbsoluteExpiration,
                                         DateTime.Now.AddHours(1) - DateTime.Now,
                                         CacheItemPriority.Normal,
                                         null);
            }
            _bookingDataCache = ApplicationCache.GetValue(_bookingDataKey) as ConcurrentDictionary<string, List<BookedSeat>>;
            if (_bookingDataCache == null)
            {
                _bookingDataCache = new ConcurrentDictionary<string, List<BookedSeat>>();
                ApplicationCache.Insert(_bookingDataKey, _bookingDataCache, null, Cache.NoAbsoluteExpiration,
                                         DateTime.Now.AddHours(1) - DateTime.Now, CacheItemPriority.Normal,
                                         null);
            }
        }

        public List<BusItinerary> SearchInventory(DateTime travelDate, int from, int to)
        {
            try
            {
                string searchKey = string.Format("{0}|{1}|{2}", String.Format("{0:MM/dd/yyyy}", travelDate), from, to);
                List<int> busTrips = ApplicationCache.GetValue(searchKey) as List<int>;
                if (busTrips == null)
                {
                    //TODO: Get bus info from database and cache it.
                    busTrips = SearchBusTrips(travelDate, from, to);
                    ApplicationCache.Insert(searchKey, busTrips, null, Cache.NoAbsoluteExpiration,
                                        DateTime.Now.AddHours(1) - DateTime.Now, CacheItemPriority.Normal,
                                        null);
                }
                List<BusItinerary> itineraries = new List<BusItinerary>();
                foreach (int busTripid in busTrips)
                {
                    BusItinerary itin = GetItineraryFromBusInfo(travelDate, busTripid);
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

        public BusItinerary GetSeatMap(string busTripId, DateTime dateTime, bool returnSeatMap)
        {
            try
            {
                BusItinerary itin = new BusItinerary();
                int id = Convert.ToInt32(busTripId);
                if (returnSeatMap)
                {
                    if (_busCache.ContainsKey(id))
                    {
                        itin.SeatMap = _seatMapCache.ContainsKey(_busCache[id].SeatMapId)
                                           ? _seatMapCache[_busCache[id].SeatMapId]
                                           : string.Empty;
                    }
                }
                List<BookedSeat> bookedSeats = null;
                if (_bookingDataCache.ContainsKey(busTripId))
                {
                    bookedSeats = _bookingDataCache[busTripId];
                }
                else
                {
                    bookedSeats = new List<BookedSeat>();
                    _bookingDataCache[busTripId] = bookedSeats;
                    //TODO: Get booking info from database and cache it
                }
                itin.BookedSeats = bookedSeats ?? new List<BookedSeat>();
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
                    bookedSeats.Add(bookedSeat);
                }
                string busTripId = bookingInformation.SelectedItinerary.BusTripId;
                if (!_bookingDataCache.ContainsKey(bookingKey))
                {
                    _bookingDataCache[bookingKey] = new List<BookedSeat>();
                }
                _bookingDataCache[bookingKey].AddRange(bookedSeats);
                return true;
            }
            catch(Exception ex)
            {
                errorMessage = ex.Message;
                Logger.LogException(ex, Source, "BookSeats", Severity.Trace);
                return false;
            }
        }

        private BusItinerary GetItineraryFromBusInfo(DateTime travelDate, int busTripid)
        {
            BusInfo busInfo = null;
            int id = Convert.ToInt32(busTripid);
            if (_busCache.ContainsKey(id))
            {
                busInfo = _busCache[id];
            }
            else
            {
                //TODO: Get bus info from database and cache it
            }
            BusItinerary itin = new BusItinerary();
            itin.ArrivalTime = busInfo.ArrivalTime;
            itin.BusName = busInfo.BusName;
            itin.BusOperatorId = busInfo.BusOperatorId;
            itin.BusTripId = busTripid.ToString();
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

        //TODO: this needs to be moved in the Inventorydataprovider which searches in the database
        private List<int> SearchBusTrips(DateTime travelDate, int from, int to)
        {
            List<int> busTrips = new List<int>();
            BusSchedule schedule = null;
            foreach (int id in _busCache.Keys)
            {
                schedule = _busCache[id].BusSchedule;
                if (schedule == null) continue;
                switch (schedule.Frequency)
                {
                    case BusTripFrequency.Daily:
                        busTrips.Add(id);
                        break;
                    case BusTripFrequency.SpecificWeekDays:
                        if (schedule.Weekdays != null && schedule.Weekdays.Exists(day => day == travelDate.DayOfWeek))
                        {
                            busTrips.Add(id);
                        }
                        break;
                    case BusTripFrequency.SpecificDates:
                        if (schedule.DateRanges != null
                            && schedule.DateRanges.Exists(range => range.FromDate <= travelDate && travelDate <= range.ToDate))
                        {
                            busTrips.Add(id);
                        }
                        break;
                }
            }
            return busTrips;
        }


        public BusItinerary GetBusItinerary(DateTime travelDate, string busTripId)
        {
            int id = Convert.ToInt32(busTripId);
            BusItinerary itin = GetItineraryFromBusInfo(travelDate, id);
            return itin;
        }

        public List<BookingInformation> GetBusBookings(DateTime dateTime, int busTripId)
        {
            throw new NotImplementedException();
        }

        public bool UpdateBooking(BookingInformation objBookInfo, out string errorMessage)
        {
            throw new NotImplementedException();
        }

        public bool DeleteBooking(long bookingId)
        {
            throw new NotImplementedException();
        }

        public BookingInformation GetBooking(long bookingId)
        {
            throw new NotImplementedException();
        }
    }
}
