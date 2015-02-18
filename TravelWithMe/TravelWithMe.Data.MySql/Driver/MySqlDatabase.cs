using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;

namespace TravelWithMe.Data.MySql.Driver
{
    public class MySqlDatabase
    {
        public MySqlDatabase(string connectionString)
        {
            Connection = new MySqlConnection(connectionString);
        }

        public MySqlConnection Connection { get; set; }

        public DataSet ExecuteQuery(MySqlCommand command)
        {
            using (Connection)
            {
                Connection.Open();
                var adapter = new MySqlDataAdapter(command);
                var ds = new DataSet();
                adapter.Fill(ds);
                return ds;
            }
        }

        public DataSet ExecuteQuery(MySqlCommand command, string paramName, out int output)
        {
            using (Connection)
            {
                Connection.Open();
                var adapter = new MySqlDataAdapter(command);
                var ds = new DataSet();
                adapter.Fill(ds);
                output = !Convert.IsDBNull(command.Parameters[paramName].Value)
                             ? Convert.ToInt32(command.Parameters[paramName].Value)
                             : 0;
                return ds;
            }
        }

        public void ExecuteNonQuery(MySqlCommand command)
        {
            using (Connection)
            {
                command.CommandTimeout = 300000;
                Connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void ExecuteNonQuery(MySqlCommand command, string paramName, out int output)
        {
            using (Connection)
            {
                command.CommandTimeout = 300000;
                Connection.Open();
                command.ExecuteNonQuery();
                output = !Convert.IsDBNull(command.Parameters[paramName].Value)
                             ? Convert.ToInt32(command.Parameters[paramName].Value)
                             : 0;
            }
        }
    }
}
