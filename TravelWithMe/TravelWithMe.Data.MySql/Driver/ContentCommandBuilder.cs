using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace TravelWithMe.Data.MySql.Driver
{
    public class ContentCommandBuilder
    {
        internal static MySqlCommand CreateGetAllCountriesCommand(MySqlConnection mySqlConnection)
        {
            var cmd = new MySqlCommand("spSelectAllCountries", mySqlConnection) { CommandType = CommandType.StoredProcedure };
            return cmd;
        }

        internal static MySqlCommand CreateGetAllCitiesCommand(MySqlConnection mySqlConnection)
        {
            var cmd = new MySqlCommand("spSelectAllCities", mySqlConnection) { CommandType = CommandType.StoredProcedure };
            return cmd;
        }

        internal static MySqlCommand CreateGetStatesByCountryCommand(MySqlConnection mySqlConnection, string countryCode)
        {
            var cmd = new MySqlCommand("spSelectStatesByCountryCode", mySqlConnection) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new MySqlParameter("pCountryCode", countryCode));
            return cmd;
        }

        internal static MySqlCommand CreateGetRecentSearchesCommand(MySqlConnection mySqlConnection)
        {
            var cmd = new MySqlCommand("spGetRecentSearches", mySqlConnection) { CommandType = CommandType.StoredProcedure };
            return cmd;
        }

        internal static MySqlCommand BuildUpdateSearchHistoryCommand(MySqlConnection mySqlConnection, int fromCityId, int toCityId)
        {
            var cmd = new MySqlCommand("spUpdateSearchHistory", mySqlConnection) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new MySqlParameter("pFromCityId", fromCityId));
            cmd.Parameters.Add(new MySqlParameter("pToCityId", toCityId));
            return cmd;
        }

        internal static MySqlCommand BuildAddCityCommand(MySqlConnection mySqlConnection, string cityName)
        {
            var cmd = new MySqlCommand("spInsertcity", mySqlConnection) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new MySqlParameter("pCityName", cityName));
            cmd.Parameters.Add(new MySqlParameter("pCityId", MySqlDbType.Int32));
            cmd.Parameters["pCityId"].Direction = ParameterDirection.Output;
            return cmd;
        }
    }
}
