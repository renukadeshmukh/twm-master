using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using TravelWithMe.API.Core.Model;

namespace TravelWithMe.Data.MySql.Driver
{
    class BusCommandBuilder
    {
        #region BusOperator fuctions
        internal static MySqlCommand BuildGetBusInfoByIdCommand(int bustripid, MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spSelectbusinfo", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("pBusTripId", bustripid));

            return cmd;
        }

        internal static MySqlCommand BuildAddBusCommand(BusInfo bus, MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spAddbusinfo", connection) { CommandType = CommandType.StoredProcedure };


            cmd.Parameters.Add(new MySqlParameter("pFromLoc", bus.FromLoc.Id));
            cmd.Parameters.Add(new MySqlParameter("pToLoc", bus.ToLoc.Id));
            cmd.Parameters.Add(new MySqlParameter("pDepartureTime", GetTimeParam(bus.DepartureTime)));
            cmd.Parameters.Add(new MySqlParameter("pArrivalTime", GetTimeParam(bus.ArrivalTime)));
            cmd.Parameters.Add(new MySqlParameter("pIsAc", bus.IsAC));
            cmd.Parameters.Add(new MySqlParameter("pBusName", bus.BusName));
            cmd.Parameters.Add(new MySqlParameter("pBusOperatorId", bus.BusOperatorId));
            cmd.Parameters.Add(new MySqlParameter("pBusType", bus.BusType.ToString()));
            cmd.Parameters.Add(new MySqlParameter("pSeatMapId", bus.SeatMapId));
            cmd.Parameters.Add(new MySqlParameter("pBusTripId", MySqlDbType.Int32));
            cmd.Parameters["pBusTripId"].Direction = ParameterDirection.Output;
            return cmd;
        }

        internal static MySqlCommand BuildUpdateBusCommand(BusInfo bus, MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spUpdateBusInfo", connection) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new MySqlParameter("pBusTripId", bus.BusTripId));
            cmd.Parameters.Add(new MySqlParameter("pFromLoc", bus.FromLoc.Id));
            cmd.Parameters.Add(new MySqlParameter("pToLoc", bus.ToLoc.Id));
            cmd.Parameters.Add(new MySqlParameter("pDepartureTime", GetTimeParam(bus.DepartureTime)));
            cmd.Parameters.Add(new MySqlParameter("pArrivalTime", GetTimeParam(bus.ArrivalTime)));
            cmd.Parameters.Add(new MySqlParameter("pIsAc", bus.IsAC));
            cmd.Parameters.Add(new MySqlParameter("pBusName", bus.BusName));
            cmd.Parameters.Add(new MySqlParameter("pBusType", bus.BusType.ToString()));
            cmd.Parameters.Add(new MySqlParameter("pRowCount", MySqlDbType.Int32));
            cmd.Parameters["pRowCount"].Direction = ParameterDirection.Output;
            return cmd;
        }

        internal static MySqlCommand BuildDeleteBusInfoCommand(int bustripid, MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spDeletebusinfo", connection) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new MySqlParameter("pBusTripId", bustripid));
            cmd.Parameters.Add(new MySqlParameter("pRowCount", MySqlDbType.Int32));
            cmd.Parameters["pRowCount"].Direction = ParameterDirection.Output;
            return cmd;
        }

        internal static MySqlCommand BuildGetAllBusesForOperatorCommand(int opertorId, MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spGetAllBusesByOperator", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("pBusOperatorId", opertorId));

            return cmd;
        }

        #endregion

        #region BusRates fuctions
        internal static MySqlCommand BuildGetBusRateByIdCommand(int rateid, MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spSelectbusrate", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("pRateId", rateid));

            return cmd;
        }

        internal static MySqlCommand BuildAddBusRateCommand(BusRate rate,int busTripId, MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spAddbusrate", connection) { CommandType = CommandType.StoredProcedure };


            cmd.Parameters.Add(new MySqlParameter("pDateFrom", rate.DateFrom));
            cmd.Parameters.Add(new MySqlParameter("pDateTo", rate.DateTo));
            cmd.Parameters.Add(new MySqlParameter("pWeekDayRate", rate.WeekDayRate));
            cmd.Parameters.Add(new MySqlParameter("pWeekEndRate", rate.WeekEndRate));
            cmd.Parameters.Add(new MySqlParameter("pBusTripId", busTripId));
            cmd.Parameters.Add(new MySqlParameter("pRateId", MySqlDbType.Int32));
            cmd.Parameters["pRateId"].Direction = ParameterDirection.Output;
            return cmd;
        }

        internal static MySqlCommand BuildUpdateBusRateCommand(BusRate rate, MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spUpdatebusrate", connection) { CommandType = CommandType.StoredProcedure };


            cmd.Parameters.Add(new MySqlParameter("pDateFrom", rate.DateFrom));
            cmd.Parameters.Add(new MySqlParameter("pDateTo", rate.DateTo));
            cmd.Parameters.Add(new MySqlParameter("pWeekDayRate", rate.WeekDayRate));
            cmd.Parameters.Add(new MySqlParameter("pWeekEndRate", rate.WeekEndRate));
            cmd.Parameters.Add(new MySqlParameter("pRateId", rate.RateId));
            cmd.Parameters.Add(new MySqlParameter("pRowCount", MySqlDbType.Int32));
            cmd.Parameters["pRowCount"].Direction = ParameterDirection.Output;
            return cmd;
        }

        internal static MySqlCommand BuildDeleteBusRateCommand(int rateId, MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spDeletebusrate", connection) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new MySqlParameter("pRateId", rateId));
            cmd.Parameters.Add(new MySqlParameter("pRowCount", MySqlDbType.Int32));
            cmd.Parameters["pRowCount"].Direction = ParameterDirection.Output;
            return cmd;
        }

        internal static MySqlCommand BuildGetAllRatesForBusCommand(int busTripId, MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spGetAllRatesByBusTripId", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("pBusTripId", busTripId));

            return cmd;
        }
        #endregion

        #region CityPoint functions
        internal static MySqlCommand BuildGetCityPointByIdCommand(int rateid, MySqlConnection connection)
        {
            throw new NotImplementedException();
        }

        internal static MySqlCommand BuildAddCityPointCommand(CityPoint cityPoint, int busTripId, MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spAddbusrate", connection) { CommandType = CommandType.StoredProcedure };


            //cmd.Parameters.Add(new MySqlParameter("pDateFrom", rate.DateFrom.ToString()));
            //cmd.Parameters.Add(new MySqlParameter("pDateTo", rate.DateTo.ToString()));
            //cmd.Parameters.Add(new MySqlParameter("pWeekDayRate", rate.WeekDayRate));
            //cmd.Parameters.Add(new MySqlParameter("pWeekEndRate", rate.WeekEndRate));
            //cmd.Parameters.Add(new MySqlParameter("pBusTripId", busTripId));
            cmd.Parameters.Add(new MySqlParameter("pRateId", MySqlDbType.Int32));
            cmd.Parameters["pRateId"].Direction = ParameterDirection.Output;
            return cmd;
        }

        internal static MySqlCommand BuildUpdateCityPointCommand(CityPoint cityPoint, MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spUpdatebusrate", connection) { CommandType = CommandType.StoredProcedure };


            //cmd.Parameters.Add(new MySqlParameter("pBusTripId", rate.DateFrom.ToString()));
            //cmd.Parameters.Add(new MySqlParameter("pCPId", rate.DateTo.ToString()));
            //cmd.Parameters.Add(new MySqlParameter("pCPTime", rate.WeekDayRate));
            //cmd.Parameters.Add(new MySqlParameter("pIsDropOffPoInteger", rate.WeekEndRate));
            //cmd.Parameters.Add(new MySqlParameter("pIsPickupPoInteger", rate.RateId));
            cmd.Parameters.Add(new MySqlParameter("pRowCount", MySqlDbType.Int32));
            cmd.Parameters["pRowCount"].Direction = ParameterDirection.Output;
            return cmd;
        }

        internal static MySqlCommand BuildDeleteCityPointCommand(int cpId, int busTripId, MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spDeletebuscitypoint", connection) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new MySqlParameter("pBusTripId", busTripId));
            cmd.Parameters.Add(new MySqlParameter("pCPId", cpId));
            cmd.Parameters.Add(new MySqlParameter("pRowCount", MySqlDbType.Int32));
            cmd.Parameters["pRowCount"].Direction = ParameterDirection.Output;
            return cmd;
        }

        internal static MySqlCommand BuildGetAllCityPointForBusCommand(int busTripId, MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spGetBusCityPoints", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("pBusTripId", busTripId));

            return cmd;
        }
        #endregion

        #region helper methods
        private static string GetTimeParam(BusTime bustime)
        {
            string time = string.Concat(bustime.Hours, '|',
                                        bustime.Minutes, '|',
                                        bustime.Meridian, '|',
                                        bustime.Days);
            return time;
        }

        #endregion

        internal static MySqlCommand BuildGetAllBusesCommand(MySqlConnection mySqlConnection)
        {
            var cmd = new MySqlCommand("spGetAllBuses", mySqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            return cmd;
        }

        internal static MySqlCommand BuildGetBusFrequencyCommand(MySqlConnection mySqlConnection, int busTripId)
        {
            var cmd = new MySqlCommand("spGetBusFrequencyByBusTripId", mySqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("pBusTripId", busTripId));
            return cmd;
        }

        internal static MySqlCommand BuildGetScheduleWeekDaysCommand(MySqlConnection mySqlConnection, int busTripId)
        {
            var cmd = new MySqlCommand("spGetScheduleDaysByBusTripId", mySqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("pBusTripId", busTripId));
            return cmd;
        }

        internal static MySqlCommand BuildGetScheduleDatesCommand(MySqlConnection mySqlConnection, int busTripId)
        {
            var cmd = new MySqlCommand("spGetScheduleDatesByBusTripId", mySqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("pBusTripId", busTripId));
            return cmd;
        }

        internal static MySqlCommand BuildGetAllSeatMapsCommand(MySqlConnection mySqlConnection)
        {
            var cmd = new MySqlCommand("spGetAllSeatMaps", mySqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            return cmd;
        }

        internal static MySqlCommand BuildPublishBusCommand(MySqlConnection mySqlConnection, int busTripId, bool isPublished)
        {
            var cmd = new MySqlCommand("spSetBusPublishStatus", mySqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("pBusTripId", busTripId));
            cmd.Parameters.Add(new MySqlParameter("pIsPublished", isPublished));
            cmd.Parameters.Add(new MySqlParameter("pRowCount", MySqlDbType.Int32));
            cmd.Parameters["pRowCount"].Direction = ParameterDirection.Output;
            return cmd;
        }

        internal static MySqlCommand BuildEnableBusCommand(MySqlConnection mySqlConnection, int busTripId, bool isEnabled)
        {
            var cmd = new MySqlCommand("spSetBusStatus", mySqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("pBusTripId", busTripId));
            cmd.Parameters.Add(new MySqlParameter("pIsEnabled", isEnabled));
            cmd.Parameters.Add(new MySqlParameter("pRowCount", MySqlDbType.Int32));
            cmd.Parameters["pRowCount"].Direction = ParameterDirection.Output;
            return cmd;
        }

        internal static MySqlCommand BuildAddSeatMapCommand(MySqlConnection mySqlConnection, string seatMap)
        {
            var cmd = new MySqlCommand("spSaveSeatMap", mySqlConnection) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new MySqlParameter("pSeatMap", seatMap));
            cmd.Parameters.Add(new MySqlParameter("pSeatMapId", MySqlDbType.Int32));
            cmd.Parameters["pSeatMapId"].Direction = ParameterDirection.Output;
            return cmd;
        }

        internal static MySqlCommand BuildUpdateBusSeatMapCommand(MySqlConnection mySqlConnection, int busTripId, int seatMapId)
        {
            var cmd = new MySqlCommand("spUpdateBusSeatMap", mySqlConnection) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new MySqlParameter("pBusTripId", busTripId));
            cmd.Parameters.Add(new MySqlParameter("pSeatMapId", seatMapId));
            return cmd;
        }

        internal static MySqlCommand BuildSaveBusFrequencyCommand(MySqlConnection mySqlConnection, int busTripId, BusSchedule schedule)
        {
            var cmd = new MySqlCommand("spSaveBusFrequency", mySqlConnection) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new MySqlParameter("pBusTripId", busTripId));
            cmd.Parameters.Add(new MySqlParameter("pFrequency", schedule.Frequency.ToString()));
            return cmd;
        }

        internal static MySqlCommand BuildDeleteDayOfWeeksCommand(MySqlConnection mySqlConnection, int busTripId)
        {
            var cmd = new MySqlCommand("spDeleteScheduleDayOfWeeks", mySqlConnection) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new MySqlParameter("pBusTripId", busTripId));
            return cmd;
        }

        internal static MySqlCommand BuildSaveDayOfWeeksCommand(MySqlConnection mySqlConnection, int busTripId, DayOfWeek dayOfWeek)
        {
            var cmd = new MySqlCommand("spSaveScheduleDayOfWeek", mySqlConnection) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new MySqlParameter("pBusTripId", busTripId));
            cmd.Parameters.Add(new MySqlParameter("pDayOfWeek", dayOfWeek.ToString()));
            return cmd;
        }

        internal static MySqlCommand BuildDeleteScheduleDatesCommand(MySqlConnection mySqlConnection, int busTripId)
        {
            var cmd = new MySqlCommand("spDeleteScheduleDates", mySqlConnection) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new MySqlParameter("pBusTripId", busTripId));
            return cmd;
        }

        internal static MySqlCommand BuildSaveScheduleDateCommand(MySqlConnection mySqlConnection, int busTripId, DateRange dateRange)
        {
            var cmd = new MySqlCommand("spSaveScheduleDate", mySqlConnection) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new MySqlParameter("pBusTripId", busTripId));
            cmd.Parameters.Add(new MySqlParameter("pFromDate", dateRange.FromDate));
            cmd.Parameters.Add(new MySqlParameter("pToDate", dateRange.ToDate));
            cmd.Parameters.Add(new MySqlParameter("pRangeId", MySqlDbType.Int32));
            cmd.Parameters["pRangeId"].Direction = ParameterDirection.Output;
            return cmd;
        }

        internal static MySqlCommand BuildDeleteBusCityPointCommand(MySqlConnection mySqlConnection, int busTripId, int cpId)
        {
            var cmd = new MySqlCommand("spDeletebuscitypoint", mySqlConnection) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new MySqlParameter("pBusTripId", busTripId));
            cmd.Parameters.Add(new MySqlParameter("pCPId", cpId));
            return cmd;
        }

        internal static MySqlCommand BuildGetAllCityPointsCommand(MySqlConnection mySqlConnection)
        {
            var cmd = new MySqlCommand("spGetAllCityPoints", mySqlConnection) { CommandType = CommandType.StoredProcedure };
            return cmd;
        }

        internal static MySqlCommand BuildAddCityPointCommand(MySqlConnection mySqlConnection, CityPoint cityPoint)
        {
            var cmd = new MySqlCommand("spSaveCityPoint", mySqlConnection) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new MySqlParameter("pCPName", cityPoint.CpName));
            cmd.Parameters.Add(new MySqlParameter("pCityId", cityPoint.CityId));
            cmd.Parameters.Add(new MySqlParameter("pGeoCode", ""));
            cmd.Parameters.Add(new MySqlParameter("pCityPointId", MySqlDbType.Int32));
            cmd.Parameters["pCityPointId"].Direction = ParameterDirection.Output;
            return cmd;
        }

        internal static MySqlCommand BuildSaveBusCityPointCommand(MySqlConnection mySqlConnection, int busTripId, CityPoint cityPoint)
        {
            var cmd = new MySqlCommand("spSaveBusCityPoint", mySqlConnection) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new MySqlParameter("pBusTripId", busTripId));
            cmd.Parameters.Add(new MySqlParameter("pCPId", cityPoint.CPId));
            cmd.Parameters.Add(new MySqlParameter("pCPTime", GetTimeParam(cityPoint.CPTime)));
            cmd.Parameters.Add(new MySqlParameter("pIsDropOffPoint", cityPoint.IsDropOffPoint));
            cmd.Parameters.Add(new MySqlParameter("pIsPickupPoint", cityPoint.IsPickupPoint));
            return cmd;
        }
    }
}
