using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.API.Core.Model;

namespace TravelWithMe.API.Services.ServiceImplementation
{
    public static class ToDataContractConverter
    {
        public static DataContract.Account ToDataContract(this Account obj)
        {
            if (obj == null)
                return null;
            return new DataContract.Account
            {
                Email = obj.Email,
                FirstName = obj.FirstName,
                LastName = obj.LastName,
                PhoneNumber = obj.PhoneNumber,
                IsEnabled = obj.IsEnabled,
                AccountType = obj.AccountType.ToString()
            };
        }

        public static List<DataContract.BookedSeat> ToDataContract(this List<BookedSeat> list)
        {
            if (list == null)
                return null;

            List<DataContract.BookedSeat> dList = list.ConvertAll(x => x.ToDataContract());
            return dList;
        }

        public static DataContract.BookedSeat ToDataContract(this BookedSeat obj)
        {
            if (obj == null)
                return null;
            return new DataContract.BookedSeat()
            {
                SeatNumber = obj.SeatNumber,
                BookingId = obj.BookingId
            };
        }

        public static List<DataContract.CitySearch> ToDataContract(this List<CitySearch> list)
        {
            if (list == null)
                return null;

            List<DataContract.CitySearch> dList = list.ConvertAll(x => x.ToDataContract());
            return dList;
        }

        public static DataContract.CitySearch ToDataContract(this CitySearch obj)
        {
            if (obj == null)
                return null;
            return new DataContract.CitySearch
            {
                From = obj.From.ToDataContract(),
                To = obj.To.ToDataContract(),
                SearchCount = obj.SearchCount
            };
        }

        public static List<DataContract.Address> ToDataContract(this List<Address> list)
        {
            if (list == null)
                return null;

            List<DataContract.Address> buses = list.ConvertAll(x => x.ToDataContract());
            return buses;
        }

        public static DataContract.Address ToDataContract(this Address obj)
        {
            if (obj == null)
                return null;
            return new DataContract.Address
            {
                Id = obj.Id,
                AddressLine1 = obj.AddressLine1,
                AddressLine2 = obj.AddressLine2,
                City = obj.City,
                Country = obj.Country,
                State = obj.State,
                ZipCode = obj.ZipCode,
                AddressType = obj.AddressType,
            };
        }

        public static List<DataContract.City> ToDataContract(this List<City> list)
        {
            if (list == null)
                return null;

            List<DataContract.City> cities = list.ConvertAll(x => x.ToDataContract());
            return cities;
        }

        public static DataContract.City ToDataContract(this City obj)
        {
            if (obj == null)
                return null;
            return new DataContract.City()
            {
                Id = obj.Id,
                Name = obj.Name,
                GeoCode = obj.GeoCode,
                StateCode = obj.StateCode
            };
        }

        public static DataContract.BusOperator ToDataContract(this BusOperator obj)
        {
            if (obj == null)
                return null;
            return new DataContract.BusOperator()
            {
                Addresses = obj.Addresses.ToDataContract(),
                CompanyName = obj.ComapanyName,
                BankAccount = obj.BankAccount.ToDataContract(),
                Email = obj.Email,
                PhoneNumber = obj.PhoneNumber
            };
        }

        public static DataContract.BankAccount ToDataContract(this BankAccount obj)
        {
            if (obj == null)
                return null;
            return new DataContract.BankAccount()
            {
                AccountHolderName = obj.AccountHolderName,
                AccountNumber = obj.AccountNumber,
                BankName = obj.BankName,
                Branch = obj.Branch,
                IFSCCode = obj.IFSCCode,
                Id = obj.Id,
                UserId = obj.UserId
            };
        }

        public static List<DataContract.BusInfo> ToDataContract(this List<BusInfo> list)
        {
            if (list == null)
                return null;

            List<DataContract.BusInfo> buses = list.ConvertAll(x => x.ToDataContract());
            return buses;
        }

        public static DataContract.BusInfo ToDataContract(this BusInfo obj)
        {
            if (obj == null)
                return null;
            DataContract.BusTime depTime = obj.DepartureTime.ToDataContract(), arrTime = obj.ArrivalTime.ToDataContract();
            return new DataContract.BusInfo
            {
                BusType = obj.BusType.ToString(),
                FromLoc = ToDataContract(obj.FromLoc),
                ToLoc = ToDataContract(obj.ToLoc),
                IsAC = obj.IsAC,
                BusTripId = obj.BusTripId,
                DepartureTime = depTime,
                ArrivalTime = arrTime,
                BusName = obj.BusName,
                SeatMapId = obj.SeatMapId,
                IsEnabled = obj.IsEnabled,
                IsPublished = obj.IsPublished
            };
        }

        public static List<DataContract.BusItinerary> ToDataContract(this List<BusItinerary> list)
        {
            if (list == null)
                return null;

            List<DataContract.BusItinerary> itineraries = list.ConvertAll(x => x.ToDataContract());
            return itineraries;
        }

        public static DataContract.BusItinerary ToDataContract(this BusItinerary obj)
        {
            if (obj == null)
                return null;
            return new DataContract.BusItinerary
            {
                CityPoints = obj.CityPoints.ToDataContract(),
                SeatMap = obj.SeatMap,
                BusType = obj.BusType,
                ArrivalTime = obj.ArrivalTime.ToDataContract(),
                BookedSeats = obj.BookedSeats.ToDataContract(),
                BusName = obj.BusName,
                BusOperatorId = obj.BusOperatorId,
                BusTripId = obj.BusTripId,
                DepartureTime = obj.DepartureTime.ToDataContract(),
                Fare = obj.Fare,
                IsAC = obj.IsAC,
                JourneryTime = obj.JourneryTime,
                OperatorName = obj.OperatorName,
                SeatsAvailable = obj.SeatsAvailable,
                From = obj.From,
                To = obj.To
            };
        }

        public static List<DataContract.Passenger> ToDataContract(this List<Passenger> list)
        {
            if (list == null)
                return null;

            List<DataContract.Passenger> drs = list.ConvertAll(x => x.ToDataContract());
            return drs;
        }

        public static DataContract.Passenger ToDataContract(this Passenger obj)
        {
            if (obj == null)
                return null;
            return new DataContract.Passenger()
            {
                Age = obj.Age,
                FirstName = obj.FirstName,
                LastName = obj.LastName,
                Gender = obj.Gender,
                PassengerId = obj.PassengerId.ToString(),
                Title = obj.Title
            };
        }

        public static List<DataContract.BookingInformation> ToDataContract(this List<BookingInformation> list)
        {
            if (list == null)
                return null;

            List<DataContract.BookingInformation> drs = list.ConvertAll(x => x.ToDataContract());
            return drs;
        }

        public static DataContract.BookingInformation ToDataContract(this BookingInformation obj)
        {
            if (obj == null)
                return null;
            return new DataContract.BookingInformation()
            {
                Passengers = obj.Passengers.ToDataContract(),
                Email = obj.Email,
                BookingId = obj.BookingId,
                ContactNumber = obj.ContactNumber,
                SelectedItinerary = obj.SelectedItinerary.ToDataContract(),
                SelectedSeats = obj.SelectedSeats,
                TotalAmount = obj.TotalAmount,
                TravelDate = obj.TravelDate.ToShortDateString(),
                PickupPoint = obj.PickupPoint.ToDataContract(),
                DropOffPoint = obj.DropOffPoint.ToDataContract(),
                TransactionId = obj.TransactionId,
            };
        }

        public static DataContract.SessionData ToDataContract(this SessionData obj)
        {
            if (obj == null)
                return null;
            return new DataContract.SessionData()
            {
                SessionId = obj.SessionId,
                UserAccount = obj.UserAccount.ToDataContract(),
                BookingInfo = obj.BookingInfo.ToDataContract()
            };
        }

        public static DataContract.BusSchedule ToDataContract(this BusSchedule schedule)
        {
            if (schedule == null)
                return null;
            return new DataContract.BusSchedule()
            {
                Frequency = schedule.Frequency.ToString(),
                DateRanges = schedule.DateRanges.ToDataContract(),
                Weekdays = Extensions.GetWeekDays(schedule.Weekdays),

            };
        }

        public static List<DataContract.DateRange> ToDataContract(this List<DateRange> list)
        {
            if (list == null)
                return null;

            List<DataContract.DateRange> drs = list.ConvertAll(x => x.ToDataContract());
            return drs;
        }

        public static DataContract.DateRange ToDataContract(this DateRange dr)
        {
            if (dr == null)
                return null;
            return new DataContract.DateRange
            {
                From = dr.FromDate.ToString("MMM dd, yyyy"),
                To = dr.ToDate.ToString("MMM dd, yyyy"),
                RateId = dr.RangeId
            };
        }

        public static DataContract.SeatArrangement ToDataContract(this SeatArrangement seatArngmnt)
        {
            if (seatArngmnt == null)
                return null;
            string seatArrngmentType = string.Empty;
            return new DataContract.SeatArrangement
            {
                Id = seatArngmnt.Id,
                SeatMap = seatArngmnt.SeatMap
            };
        }

        public static DataContract.Location ToDataContract(this Location loc)
        {
            if (loc == null)
                return null;
            return new DataContract.Location
            {
                CityName = loc.CityName,
                Latitude = loc.Latitude,
                Longitude = loc.Longitude,
                State = loc.State,
                CityCode = loc.CityCode
            };
        }

        public static List<DataContract.CityPoint> ToDataContract(this List<CityPoint> list)
        {
            if (list == null)
                return null;
            //var sortedDict = obj.CityPoints.OrderBy(x => x.Value.CPTime.MinutesOfJourney).ToDictionary(pair => pair.Key, pair => pair.Value);
            List<CityPoint> cps = list.OrderBy(x => x.CPTime.MinutesSinceMidNight).ToList();
            return cps.ConvertAll(x => x.ToDataContract());
        }

        public static DataContract.CityPoint ToDataContract(this CityPoint obj)
        {
            if (obj == null)
                return null;

            return new DataContract.CityPoint
            {
                CityId = obj.CityId,
                CPId = obj.CPId,
                CPTime = obj.CPTime.ToDataContract(),
                Lat = obj.Lat,
                Long = obj.Long,
                CityName = obj.CityName,
                CPName = obj.CpName,
                IsDropOffPoint = obj.IsDropOffPoint,
                IsPickupPoint = obj.IsPickupPoint
            };
        }

        public static DataContract.BusTime ToDataContract(this BusTime obj)
        {
            if (obj == null)
                return null;

            return new DataContract.BusTime()
                       {
                           Days = obj.Days,
                           Hours = obj.Hours,
                           Meridian = obj.Meridian.ToString(),
                           Minutes = obj.Minutes,
                       };
        }

        public static List<DataContract.BusRate> ToDataContract(this List<BusRate> list)
        {
            if (list == null)
                return null;
            return list.ConvertAll(x => x.ToDataContract());
        }

        public static DataContract.BusRate ToDataContract(this BusRate obj)
        {
            if (obj == null)
                return null;

            return new DataContract.BusRate
            {
                DateFrom = obj.DateFrom.ToString("MMM dd, yyyy"),
                DateTo = obj.DateTo.ToString("MMM dd, yyyy"),
                RateId = obj.RateId,
                WeekDayRate = obj.WeekDayRate,
                WeekEndRate = obj.WeekEndRate
            };
        }

        //public static DataContract.BookingInformation ToDataContract(this BookingInformation bookingInfo)
        //{
        //    if (bookingInfo == null)
        //        return null;

        //    DataContract.BookingInformation modelBookingInfo = new DataContract.BookingInformation()
        //    {
        //        BookingId = bookingInfo.BookingId,
        //        ContactNumber = bookingInfo.ContactNumber,
        //        Email = bookingInfo.Email,
        //        Passengers = bookingInfo.Passengers.ToDataContract(),
        //        SelectedItinerary = bookingInfo.SelectedItinerary.ToDataContract(),
        //        SelectedSeats = bookingInfo.SelectedSeats,
        //        TotalAmount = bookingInfo.TotalAmount
        //    };
        //    return modelBookingInfo;
        //}
    }
}
