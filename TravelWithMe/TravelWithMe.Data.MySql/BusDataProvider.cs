using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TravelWithMe.API.Core.Model;
using TravelWithMe.Data.Interfaces;
using TravelWithMe.Data.MySql.Driver;
using MySql.Data.MySqlClient;
using TravelWithMe.Data.MySql.Entities;
using TravelWithMe.Logging.Helper;

namespace TravelWithMe.Data.MySql
{
    public class BusDataProvider : IBusDataProvider
    {
        private string Source = "BusDataProvider";

        #region BusInfo operations
        public BusInfo GetBusInfo(int busTripId)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseRead);

                MySqlCommand command;
                command = BusCommandBuilder.BuildGetBusInfoByIdCommand(busTripId, db.Connection);
                DataSet dataSet = db.ExecuteQuery(command);
                if (dataSet != null && dataSet.Tables[0] != null && dataSet.Tables[0].Rows.Count > 0)
                {
                    BusInfo bus = ParseBusInfo(dataSet.Tables[0].Rows[0]);
                    return bus;
                }
                return null;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "GetAccount", Severity.Critical);
                return null;
            }
        }

        public int AddBusInfo(BusInfo busInfo)
        {
            try
            {
                int busTripid;
                var db = new MySqlDatabase(DbConfiguration.DatabaseWrite);
                MySqlCommand cmd = BusCommandBuilder.BuildAddBusCommand(busInfo, db.Connection);
                db.ExecuteNonQuery(cmd, "pBusTripId", out busTripid);

                if (busTripid != 0)
                {
                    busInfo.BusTripId = busTripid;
                    return busTripid;
                }
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "AddBusInfo", Severity.Critical);
                return -1;
            }
            return -1;
        }

        public bool UpdateBusinfo(BusInfo busInfo)
        {
            try
            {
                int rowcount;
                var db = new MySqlDatabase(DbConfiguration.DatabaseWrite);
                MySqlCommand cmd = BusCommandBuilder.BuildUpdateBusCommand(busInfo, db.Connection);
                db.ExecuteNonQuery(cmd, "pRowCount", out rowcount);

                if (rowcount == 1)
                    return true;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "UpdateBusinfo", Severity.Critical);
                return false;
            }
            return false;
        }

        public bool DeleteBusInfo(int busTripId)
        {
            try
            {
                int rowcount;
                var db = new MySqlDatabase(DbConfiguration.DatabaseWrite);
                MySqlCommand cmd = BusCommandBuilder.BuildDeleteBusInfoCommand(busTripId, db.Connection);
                db.ExecuteNonQuery(cmd, "pRowCount", out rowcount);

                if (rowcount == 1)
                    return true;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "DeleteBusInfo", Severity.Critical);
                return false;
            }
            return false;
        }

        public List<BusInfo> GetAllBusesForOperator(int operatorId)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseRead);

                MySqlCommand command;
                command = BusCommandBuilder.BuildGetAllBusesForOperatorCommand(operatorId, db.Connection);
                DataSet dataSet = db.ExecuteQuery(command);

                List<BusInfo> buses = ParseBuses(dataSet);
                return buses;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "GetAllBusesForOperator", Severity.Critical);
                return null;
            }
        }

        public List<BusInfo> GetAllBuses()
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseRead);
                MySqlCommand command;
                command = BusCommandBuilder.BuildGetAllBusesCommand(db.Connection);
                DataSet dataSet = db.ExecuteQuery(command);
                List<BusInfo> buses = ParseBuses(dataSet);
                return buses;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "GetAllBuses", Severity.Critical);
                return null;
            }
        }

        public BusSchedule GetBusSchedule(int busTripId)
        {
            try
            {
                BusSchedule busSchedule = new BusSchedule();
                var db = new MySqlDatabase(DbConfiguration.DatabaseRead);
                MySqlCommand command;
                command = BusCommandBuilder.BuildGetBusFrequencyCommand(db.Connection, busTripId);
                DataSet dataSet = db.ExecuteQuery(command);
                if (dataSet != null && dataSet.Tables[0] != null && dataSet.Tables[0].Rows.Count == 1)
                {
                    busSchedule.Frequency = dataSet.Tables[0].Rows[0]["Frequency"].GetEnum(BusTripFrequency.SpecificDates);
                    if (busSchedule.Frequency == BusTripFrequency.SpecificWeekDays)
                    {
                        command = BusCommandBuilder.BuildGetScheduleWeekDaysCommand(db.Connection, busTripId);
                        dataSet = db.ExecuteQuery(command);
                        if (dataSet != null && dataSet.Tables[0] != null && dataSet.Tables[0].Rows.Count > 0)
                        {
                            busSchedule.Weekdays = new List<DayOfWeek>();
                            foreach (DataRow row in dataSet.Tables[0].Rows)
                            {
                                busSchedule.Weekdays.Add(row["DayOfWeek"].GetEnum(DayOfWeek.Monday));
                            }
                        }
                    }
                    if (busSchedule.Frequency == BusTripFrequency.SpecificDates)
                    {
                        command = BusCommandBuilder.BuildGetScheduleDatesCommand(db.Connection, busTripId);
                        dataSet = db.ExecuteQuery(command);
                        if (dataSet != null && dataSet.Tables[0] != null && dataSet.Tables[0].Rows.Count > 0)
                        {
                            busSchedule.DateRanges = new List<DateRange>();
                            foreach (DataRow row in dataSet.Tables[0].Rows)
                            {
                                DateRange range = new DateRange();
                                range.RangeId = row["RangeId"].GetInt();
                                range.FromDate = row["FromDate"].GetDate();
                                range.ToDate = row["ToDate"].GetDate();
                                busSchedule.DateRanges.Add(range);
                            }
                        }
                    }
                }
                else
                {
                    return null;
                }
                return busSchedule;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "GetBusSchedule", Severity.Critical);
                return null;
            }
        }

        public bool SetBusPublishStatus(int busTripId, bool isPublished)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseWrite);
                MySqlCommand cmd = BusCommandBuilder.BuildPublishBusCommand(db.Connection, busTripId, isPublished);
                int rowcount;
                db.ExecuteNonQuery(cmd, "pRowCount", out rowcount);
                if (rowcount == 1)
                    return true;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "SetBusPublishStatus", Severity.Critical);
                return false;
            }
            return false;
        }

        public bool SetBusStatus(int busTripId, bool isEnabled)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseWrite);
                MySqlCommand cmd = BusCommandBuilder.BuildEnableBusCommand(db.Connection, busTripId, isEnabled);
                int rowcount;
                db.ExecuteNonQuery(cmd, "pRowCount", out rowcount);
                if (rowcount == 1)
                    return true;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "SetBusStatus", Severity.Critical);
                return false;
            }
            return false;
        }
        #endregion

        #region Busrate operations
        public List<BusRate> GetAllBusRatesForBus(int bustripid)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseRead);
                MySqlCommand command;
                command = BusCommandBuilder.BuildGetAllRatesForBusCommand(bustripid, db.Connection);
                DataSet dataSet = db.ExecuteQuery(command);

                List<BusRate> rates = ParseRates(dataSet) ?? new List<BusRate>();
                return rates;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "AllGetBusRatesForBus", Severity.Critical);
                return null;
            }
        }

        public BusRate GetBusRate(int rateId)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseRead);

                MySqlCommand command;
                command = BusCommandBuilder.BuildGetBusRateByIdCommand(rateId, db.Connection);
                DataSet dataSet = db.ExecuteQuery(command);

                BusRate rate = ParseRate(dataSet);
                return rate;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "GetBusRate", Severity.Critical);
                return null;
            }
        }

        public bool UpdateBusRate(BusRate busRate)
        {
            try
            {
                int rowcount;
                var db = new MySqlDatabase(DbConfiguration.DatabaseWrite);
                MySqlCommand cmd = BusCommandBuilder.BuildUpdateBusRateCommand(busRate, db.Connection);
                db.ExecuteNonQuery(cmd, "pRowCount", out rowcount);

                if (rowcount == 1)
                    return true;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "UpdateBusRate", Severity.Critical);
                return false;
            }
            return false;
        }

        public bool DeleteBusRate(int rateId)
        {
            try
            {
                int rowcount;
                var db = new MySqlDatabase(DbConfiguration.DatabaseWrite);
                MySqlCommand cmd = BusCommandBuilder.BuildDeleteBusRateCommand(rateId, db.Connection);
                db.ExecuteNonQuery(cmd, "pRowCount", out rowcount);

                if (rowcount == 1)
                    return true;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "DeleteBusRate", Severity.Critical);
                return false;
            }
            return false;
        }

        public bool DeleteAllRatesForBus(int bustripid)
        {
            throw new NotImplementedException();
        }

        public int AddBusRate(int bustripid, BusRate rate)
        {
            try
            {
                int rateId;
                var db = new MySqlDatabase(DbConfiguration.DatabaseWrite);
                MySqlCommand cmd = BusCommandBuilder.BuildAddBusRateCommand(rate, bustripid, db.Connection);
                db.ExecuteNonQuery(cmd, "pRateId", out rateId);

                if (rateId != 0)
                {
                    rate.RateId = rateId;
                    return rateId;
                }
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "AddBusRate", Severity.Critical);
                return -1;
            }
            return -1;
        }


        #endregion

        #region seatmap
        public int AddSeatMap(string seatMap)
        {
            try
            {
                int seatMapId;
                var db = new MySqlDatabase(DbConfiguration.DatabaseWrite);
                MySqlCommand cmd = BusCommandBuilder.BuildAddSeatMapCommand(db.Connection, seatMap);
                db.ExecuteNonQuery(cmd, "pSeatMapId", out seatMapId);
                return seatMapId;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "AddSeatMap", Severity.Critical);
                return 0;
            }
        }

        public void UpdateBusSeatMap(int busTripId, int seatMapId)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseWrite);
                MySqlCommand cmd = BusCommandBuilder.BuildUpdateBusSeatMapCommand(db.Connection, busTripId, seatMapId);
                db.ExecuteNonQuery(cmd);
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "UpdateBusSeatMap", Severity.Critical);
            }
        }
        
        #endregion

        #region helper methods
        private BusInfo ParseBusInfo(DataRow row)
        {
            if (Convert.IsDBNull(row["BusTripId"]))
                return null;
            var bus = new BusInfo()
            {
                BusTripId = row["BusTripId"].GetInt(),
                FromLoc = GetCity(row["FromCityId"].GetInt(), row["FromCity"].GetString()),
                ToLoc = GetCity(row["ToCityId"].GetInt(), row["ToCity"].GetString()),
                BusOperatorId = row["busoperatorId"].GetString(),
                BusName = row["busname"].GetString(),
                BusType = (BusType)Enum.Parse(typeof(BusType), row["bustype"].GetString()),
                IsAC = row["isAc"].GetBool(),
                OperatorName = row["AgencyName"].GetString(),
                ArrivalTime = row["arrivaltime"].GetBusTime(),
                DepartureTime = row["departuretime"].GetBusTime(),
                SeatMapId = row["SeatMapId"].GetInt(),
                IsEnabled = row["IsEnabled"].GetBool(),
                IsPublished = row["IsPublished"].GetBool()
            };
            bus.BusRates = GetAllBusRatesForBus(bus.BusTripId);
            bus.BusSchedule = GetBusSchedule(bus.BusTripId);
            bus.CityPoints = GetAllCityPointsForBus(bus.BusTripId);
            return bus;
        }

        private static City GetCity(int cityId, string cityname)
        {
            return new City()
                       {
                           Name = cityname,
                           Id = cityId
                       };
        }

        private List<BusInfo> ParseBuses(DataSet dataSet)
        {
            List<BusInfo> buses = new List<BusInfo>();
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                if (dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count >= 1)
                {
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        BusInfo bus = ParseBusInfo(row);
                        if(bus!=null)
                            buses.Add(bus);
                    }
                    return buses;
                }
            }
            return null;
        }

        private static BusRate ParseRate(DataSet dataSet)
        {
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                if (dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count == 1)
                {
                    DataRow row = dataSet.Tables[0].Rows[0];
                    if (Convert.IsDBNull(row["RateId"]))
                        return null;
                    var busRate = new BusRate()
                    {
                        DateFrom = row["DateFrom"].GetDate(),
                        DateTo = row["DateTo"].GetDate(),
                        WeekDayRate = row["WeekDayRate"].GetDecimal(),
                        WeekEndRate = row["WeekEndRate"].GetDecimal(),
                        RateId = row["RateId"].GetInt(),
                    };
                    return busRate;
                }
            }
            return null;
        }

        private static List<BusRate> ParseRates(DataSet dataSet)
        {
            List<BusRate> rates = new List<BusRate>();
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                if (dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count >= 1)
                {
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        if (Convert.IsDBNull(row["RateId"]))
                            return null;
                        var bus = new BusRate()
                        {
                            DateFrom = row["DateFrom"].GetDate(),
                            DateTo = row["DateTo"].GetDate(),
                            WeekDayRate = row["WeekDayRate"].GetDecimal(),
                            WeekEndRate = row["WeekEndRate"].GetDecimal(),
                            RateId = row["RateId"].GetInt(),
                        };
                        rates.Add(bus);

                    }
                    return rates;
                }
            }
            return rates;
        }
        #endregion

        #region CityPoint operations
        public List<CityPoint> GetAllCityPointsForBus(int bustripid)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseRead);
                MySqlCommand command;
                command = BusCommandBuilder.BuildGetAllCityPointForBusCommand(bustripid, db.Connection);
                DataSet dataSet = db.ExecuteQuery(command);

                List<CityPoint> cityPoints = new List<CityPoint>();
                if (dataSet != null && dataSet.Tables[0] != null && dataSet.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        CityPoint cityPoint = ParseBusCityPoint(row);
                        cityPoints.Add(cityPoint);
                    }
                }
                return cityPoints;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "GetAllCityPointsForBus", Severity.Critical);
                return null;
            }
        }

        private CityPoint ParseBusCityPoint(DataRow row)
        {
            CityPoint cityPoint = new CityPoint();
            cityPoint.CPId = row["CPId"].GetInt();
            cityPoint.CPTime = row["CPTime"].GetBusTime();
            cityPoint.CityId = row["CityId"].GetInt();
            cityPoint.CityName = row["CityName"].GetString();
            cityPoint.CpName = row["CPName"].GetString();
            cityPoint.IsDropOffPoint = row["IsDropOffPoint"].GetBool();
            cityPoint.IsPickupPoint = row["IsPickupPoint"].GetBool();
            return cityPoint;
        }

        public List<CityPoint> GetAllCityPoint()
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseRead);
                MySqlCommand command;
                command = BusCommandBuilder.BuildGetAllCityPointsCommand(db.Connection);
                DataSet dataSet = db.ExecuteQuery(command);

                List<CityPoint> cityPoints = new List<CityPoint>();
                if (dataSet != null && dataSet.Tables[0] != null && dataSet.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        CityPoint cityPoint = ParseCityPoint(row);
                        cityPoints.Add(cityPoint);
                    }
                }
                return cityPoints;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "GetAllCityPoint", Severity.Critical);
                return null;
            }
        }

        private CityPoint ParseCityPoint(DataRow row)
        {
            CityPoint cityPoint = new CityPoint();
            cityPoint.CPId = row["CPId"].GetInt();
            cityPoint.CityId = row["CityId"].GetInt();
            cityPoint.CpName = row["CPName"].GetString();
            return cityPoint;
        }

        public int AddCityPoint(CityPoint cityPoint)
        {
            try
            {
                int cityPointId;
                var db = new MySqlDatabase(DbConfiguration.DatabaseWrite);
                MySqlCommand cmd = BusCommandBuilder.BuildAddCityPointCommand(db.Connection, cityPoint);
                db.ExecuteNonQuery(cmd, "pCityPointId", out cityPointId);
                return cityPointId;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "AddCityPoint", Severity.Critical);
                return 0;
            }
        }

        public bool SaveBusCityPoint(int busTripId, CityPoint cityPoint)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseWrite);
                MySqlCommand cmd = BusCommandBuilder.BuildSaveBusCityPointCommand(db.Connection, busTripId, cityPoint);
                db.ExecuteNonQuery(cmd);
                return true;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "SaveBusCityPoint", Severity.Critical);
                return false;
            }
        }

        public bool DeleteBusCityPoint(int busTripId, int cpId)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseWrite);
                MySqlCommand cmd = BusCommandBuilder.BuildDeleteBusCityPointCommand(db.Connection, busTripId, cpId);
                db.ExecuteNonQuery(cmd);
                return true;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "SaveBusCityPoint", Severity.Critical);
                return false;
            }
        }
        #endregion


        public List<SeatArrangement> GetAllSeatMaps()
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseRead);
                MySqlCommand command;
                command = BusCommandBuilder.BuildGetAllSeatMapsCommand(db.Connection);
                DataSet dataSet = db.ExecuteQuery(command);
                List<SeatArrangement> seatMaps = new List<SeatArrangement>();
                if(dataSet!=null && dataSet.Tables[0] != null && dataSet.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        SeatArrangement seatMap = new SeatArrangement();
                        seatMap.Id = row["SeatMapId"].GetInt();
                        seatMap.SeatMap = row["SeatMap"].GetString();
                        seatMaps.Add(seatMap);
                    }
                }
                return seatMaps;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "GetAllSeatMaps", Severity.Critical);
                return null;
            }
        }

        public bool SaveBusFrequency(int busTripId, BusSchedule schedule)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseWrite);
                MySqlCommand cmd = BusCommandBuilder.BuildSaveBusFrequencyCommand(db.Connection, busTripId, schedule);
                db.ExecuteNonQuery(cmd);
                return true;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "SaveBusFrequency", Severity.Critical);
                return false;
            }
        }

        public void DeleteScheduleDayOfWeeks(int busTripId)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseWrite);
                MySqlCommand cmd = BusCommandBuilder.BuildDeleteDayOfWeeksCommand(db.Connection, busTripId);
                db.ExecuteNonQuery(cmd);
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "DeleteScheduleDayOfWeeks", Severity.Critical);
            }
        }

        public bool SaveScheduleDayOfWeek(int busTripId, DayOfWeek dayOfWeek)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseWrite);
                MySqlCommand cmd = BusCommandBuilder.BuildSaveDayOfWeeksCommand(db.Connection, busTripId, dayOfWeek);
                db.ExecuteNonQuery(cmd);
                return true;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "SaveScheduleDayOfWeek", Severity.Critical);
                return false;
            }
        }

        public void DeleteScheduleDates(int busTripId)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseWrite);
                MySqlCommand cmd = BusCommandBuilder.BuildDeleteScheduleDatesCommand(db.Connection, busTripId);
                db.ExecuteNonQuery(cmd);
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "DeleteScheduleDates", Severity.Critical);
            }
        }

        public int SaveScheduleDate(int busTripId, DateRange dateRange)
        {
            try
            {
                int rangeId;
                var db = new MySqlDatabase(DbConfiguration.DatabaseWrite);
                MySqlCommand cmd = BusCommandBuilder.BuildSaveScheduleDateCommand(db.Connection, busTripId, dateRange);
                db.ExecuteNonQuery(cmd, "pRangeId", out rangeId);
                return rangeId;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "SaveScheduleDate", Severity.Critical);
                return 0;
            }
        }
    }
}
