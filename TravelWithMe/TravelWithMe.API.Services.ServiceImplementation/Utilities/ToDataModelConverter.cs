using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.API.Core.Model;

namespace TravelWithMe.API.Services.ServiceImplementation
{
    public static class ToDataModelConverter
    {
        public static Account ToDataModel(this DataContract.Account account)
        {
            if (account == null)
                return null;
            return new Account
            {
                AccountId = account.AccountId,
                AccountType = string.IsNullOrEmpty(account.AccountType) ? AccountType.EndUser : (AccountType)Enum.Parse(typeof(AccountType), account.AccountType),
                Email = account.Email,
                FirstName = account.FirstName,
                LastName = account.LastName,
                PhoneNumber = account.PhoneNumber,
                IsEnabled = account.IsEnabled
            };
        }

        public static List<Address> ToDataModel(this List<DataContract.Address> list)
        {
            if (list == null)
                return null;

            List<Address> drs = list.ConvertAll(x => x.ToDataModel());
            return drs;
        }

        public static Address ToDataModel(this DataContract.Address address)
        {
            if (address == null)
                return null;
            return new Address
            {
                AddressLine1 = address.AddressLine1,
                AddressLine2 = address.AddressLine2,
                City = address.City,
                Country = address.Country,
                State = address.State,
                ZipCode = address.ZipCode,
                AddressType = address.AddressType,
                Id = address.Id
            };
        }

        public static List<City> ToDataModel(this List<DataContract.City> list)
        {
            if (list == null)
                return null;

            List<City> cities = list.ConvertAll(x => x.ToDataModel());
            return cities;
        }

        public static City ToDataModel(this DataContract.City obj)
        {
            if (obj == null)
                return null;
            return new City()
            {
                Id = obj.Id,
                Name = obj.Name,
                GeoCode = obj.GeoCode,
                StateCode = obj.StateCode
            };
        }

        public static BusOperator ToDataModel(this DataContract.BusOperator obj)
        {
            if (obj == null)
                return null;
            return new BusOperator()
            {
                Addresses = obj.Addresses.ToDataModel(),
                ComapanyName = obj.CompanyName,
                BankAccount = obj.BankAccount.ToDataModel(),
                Email = obj.Email,
                PhoneNumber = obj.PhoneNumber
            };
        }

        public static BankAccount ToDataModel(this DataContract.BankAccount obj)
        {
            if (obj == null)
                return null;
            return new BankAccount()
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

        public static BusInfo ToDataModel(this DataContract.BusInfo obj)
        {
            if (obj == null)
                return null;
            return new BusInfo
            {
                BusType = (BusType)Enum.Parse(typeof(BusType), obj.BusType),
                FromLoc = ToDataModel(obj.FromLoc),
                ToLoc = ToDataModel(obj.ToLoc),
                IsAC = obj.IsAC,
                BusTripId = Convert.ToInt32(obj.BusTripId),
                BusName = obj.BusName,
                DepartureTime = obj.DepartureTime.ToDataModel(),
                ArrivalTime = obj.ArrivalTime.ToDataModel(),
                SeatMapId = obj.SeatMapId
            };
        }

        //public static BookingInformation ToDataModel(this DataContract.BookingInformation bookingInfo)
        //{
        //    if (bookingInfo == null)
        //        return null;

        //    BookingInformation modelBookingInfo = new BookingInformation()
        //    {
        //        BookingId = bookingInfo.BookingId,
        //        ContactNumber = bookingInfo.ContactNumber,
        //        Email = bookingInfo.Email,
        //        Passengers = bookingInfo.Passengers.ToDataModel(),
        //        SelectedItinerary = bookingInfo.SelectedItinerary.ToDataModel(),
        //        SelectedSeats = bookingInfo.SelectedSeats,
        //        TotalAmount = bookingInfo.TotalAmount
        //    };
        //    return modelBookingInfo;
        //}

        public static List<Passenger> ToDataModel(this List<DataContract.Passenger> list)
        {
            if (list == null)
                return null;

            List<Passenger> drs = list.ConvertAll(x => x.ToDataModel());
            return drs;
        }

        public static Passenger ToDataModel(this DataContract.Passenger obj)
        {
            if (obj == null)
                return null;
            return new Passenger()
            {
                Age = obj.Age,
                FirstName = obj.FirstName,
                LastName = obj.LastName,
                Gender = obj.Gender,
                PassengerId = obj.PassengerId == null ? 0 : int.Parse(obj.PassengerId),
                Title = obj.Title
            };
        }

        public static List<CitySearch> ToDataModel(this List<DataContract.CitySearch> list)
        {
            if (list == null)
                return null;

            List<CitySearch> dList = list.ConvertAll(x => x.ToDataModel());
            return dList;
        }

        public static CitySearch ToDataModel(this DataContract.CitySearch obj)
        {
            if (obj == null)
                return null;
            return new CitySearch
            {
                From = obj.From.ToDataModel(),
                To = obj.To.ToDataModel(),
                SearchCount = obj.SearchCount
            };
        }

        public static List<BookedSeat> ToDataModel(this List<DataContract.BookedSeat> list)
        {
            if (list == null)
                return null;

            List<BookedSeat> dList = list.ConvertAll(x => x.ToDataModel());
            return dList;
        }

        public static BookedSeat ToDataModel(this DataContract.BookedSeat obj)
        {
            if (obj == null)
                return null;
            return new BookedSeat()
            {
                SeatNumber = obj.SeatNumber,
                BookingId = obj.BookingId
            };
        }

        public static BookingInformation ToDataModel(this DataContract.BookingInformation obj)
        {
            if (obj == null)
                return null;
            return new BookingInformation()
            {
                Passengers = obj.Passengers.ToDataModel(),
                Email = obj.Email,
                BookingId = obj.BookingId,
                ContactNumber = obj.ContactNumber,
                SelectedItinerary = obj.SelectedItinerary.ToDataModel(),
                SelectedSeats = obj.SelectedSeats,
                TotalAmount = obj.TotalAmount,
                TravelDate = obj.TravelDate.ParseStringToDateTime(),
                PickupPoint = obj.PickupPoint.ToDataModel(),
                DropOffPoint = obj.DropOffPoint.ToDataModel(),
                TransactionId = obj.TransactionId,
            };
        }

        public static SessionData ToDataModel(this DataContract.SessionData obj)
        {
            if (obj == null)
                return null;
            return new SessionData()
            {
                SessionId = obj.SessionId,
                BookingInfo = obj.BookingInfo.ToDataModel()
            };
        }

        public static BusItinerary ToDataModel(this DataContract.BusItinerary obj)
        {
            if (obj == null)
                return null;
            return new BusItinerary
            {
                CityPoints = obj.CityPoints.ToDataModel().Values.ToList(),
                SeatMap = obj.SeatMap,
                BusType = obj.BusType,
                ArrivalTime = obj.ArrivalTime.ToDataModel(),
                BookedSeats = obj.BookedSeats.ToDataModel(),
                BusName = obj.BusName,
                BusOperatorId = obj.BusOperatorId,
                BusTripId = obj.BusTripId,
                DepartureTime = obj.DepartureTime.ToDataModel(),
                Fare = obj.Fare,
                IsAC = obj.IsAC,
                JourneryTime = obj.JourneryTime,
                OperatorName = obj.OperatorName,
                SeatsAvailable = obj.SeatsAvailable,
                From = obj.From,
                To = obj.To
            };
        }

        public static BusSchedule ToDataModel(this DataContract.BusSchedule schedule)
        {
            if (schedule == null)
                return null;
            return new BusSchedule()
            {
                Frequency = (BusTripFrequency)Enum.Parse(typeof(BusTripFrequency), schedule.Frequency),
                DateRanges = schedule.DateRanges.ToDataModel(),
                Weekdays = Extensions.GetWeekDaysEnum(schedule.Weekdays),
            };
        }

        public static List<DateRange> ToDataModel(this List<DataContract.DateRange> list)
        {
            if (list == null)
                return null;

            List<DateRange> drs = list.ConvertAll(x => x.ToDataModel());
            return drs;
        }

        public static DateRange ToDataModel(this DataContract.DateRange dr)
        {
            if (dr == null)
                return null;
            return new DateRange
            {
                FromDate = Convert.ToDateTime(dr.From),
                ToDate = Convert.ToDateTime(dr.To),
                RangeId = dr.RateId
            };
        }

        public static SeatArrangement ToDataModel(this DataContract.SeatArrangement seatArrangement)
        {
            if (seatArrangement == null)
                return null;
            return new SeatArrangement
            {
                Id = seatArrangement.Id,
                SeatMap = seatArrangement.SeatMap
            };
        }

        private static Location ToDataModel(DataContract.Location loc)
        {
            if (loc == null)
                return null;
            return new Location
            {
                CityName = loc.CityName,
                Latitude = loc.Latitude,
                Longitude = loc.Longitude,
                State = loc.State,
                CityCode = loc.CityCode
            };
        }

        public static Dictionary<int, CityPoint> ToDataModel(this List<DataContract.CityPoint> list)
        {
            if (list == null)
                return null;
            List<CityPoint> rates = list.ConvertAll(x => x.ToDataModel());
            return rates.ToDictionary(x => x.CPId, x => x);
        }

        public static CityPoint ToDataModel(this DataContract.CityPoint obj)
        {
            if (obj == null)
                return null;

            return new CityPoint
            {
                CityId = obj.CityId,
                CPId = obj.CPId,
                CPTime = obj.CPTime.ToDataModel(),
                Lat = obj.Lat,
                Long = obj.Long,
                CityName = obj.CityName,
                CpName = obj.CPName,
                IsDropOffPoint = obj.IsDropOffPoint,
                IsPickupPoint = obj.IsPickupPoint
            };
        }

        //public static BusTime ToDataModel(this string cpTime)
        //{
        //    //Time string format should be like 10:30AM or 10:30PM(next day) or 10:30PM(day-1)
        //    if (string.IsNullOrEmpty(cpTime)) return null;
        //    BusTime bt = new BusTime();
        //    string[] parts = cpTime.Split('(');
        //    string[] arr = parts[0].Split(':');
        //    int hrs, mins, day;
        //    if (arr[1].Contains("AM"))
        //        bt.Meridian = Meridian.AM;
        //    else if (arr[1].Contains("PM"))
        //        bt.Meridian = Meridian.PM;
        //    else return null;
        //    arr[1] = arr[1].Replace("AM", string.Empty).Replace("PM", string.Empty);
        //    bool hrsParse = int.TryParse(arr[0], out hrs);
        //    bool minsParse = int.TryParse(arr[1], out mins);
        //    if (hrsParse && minsParse && hrs < 60 && mins < 60)
        //    {
        //        bt.Hours = hrs;
        //        bt.Minutes = mins;
        //    }
        //    else return null;
        //    if (cpTime.Contains("next"))
        //        bt.Days = 1;
        //    else if (cpTime.Contains("day-"))
        //    {
        //        int index = cpTime.IndexOf('-');
        //        int len = cpTime.IndexOf(')') - index;
        //        bool dayParse = int.TryParse(cpTime.Substring(index + 1, 1), out day);
        //        if (dayParse)
        //            bt.Days = day;
        //        else return null;
        //    }
        //    return bt;
        //}

        public static BusTime ToDataModel(this DataContract.BusTime busTime)
        {
            if (busTime == null) return null;
            return new BusTime()
                       {
                           Days = busTime.Days,
                           Hours = busTime.Hours,
                           Meridian = (Meridian)Enum.Parse(typeof(Meridian),busTime.Meridian),
                           Minutes = busTime.Minutes
                       };
        }

        public static Dictionary<int, BusRate> ToDataModel(this List<DataContract.BusRate> list)
        {
            if (list == null)
                return null;
            List<BusRate> rates = list.ToDataModelList();
            return rates.ToDictionary(x => x.RateId, x => x);
        }

        public static List<BusRate> ToDataModelList(this List<DataContract.BusRate> list)
        {
            return list.ConvertAll(x => x.ToDataModel());
        }

        public static BusRate ToDataModel(this DataContract.BusRate obj)
        {
            if (obj == null)
                return null;
            return new BusRate
            {
                RateId = obj.RateId,
                WeekDayRate = obj.WeekDayRate,
                WeekEndRate = obj.WeekEndRate,
                DateFrom = Convert.ToDateTime(obj.DateFrom),
                DateTo = Convert.ToDateTime(obj.DateTo)
            };
        }
    }
}
