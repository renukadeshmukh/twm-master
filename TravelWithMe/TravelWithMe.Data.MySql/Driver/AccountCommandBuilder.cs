using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using TravelWithMe.API.Core.Model;

namespace TravelWithMe.Data.MySql.Driver
{
    public class AccountCommandBuilder
    {
        public static MySqlCommand BuildSaveBusOperatorCommand(MySqlConnection connection, BusOperator busOperator)
        {
            var cmd = new MySqlCommand("spSaveOrUpdateBusOperator", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("inOperatorId", busOperator.OperatorId));
            cmd.Parameters.Add(new MySqlParameter("inAgencyName", busOperator.ComapanyName));
            cmd.Parameters.Add(new MySqlParameter("inEmail", busOperator.Email[0]));
            cmd.Parameters.Add(new MySqlParameter("inPhoneNumber", busOperator.PhoneNumber[0]));
            return cmd;
        }

        public static MySqlCommand BuildSaveBankAccountCommand(MySqlConnection connection, BankAccount bankAccount)
        {
            var cmd = new MySqlCommand("spSaveOrUpdateBankAccount", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("inUserId", bankAccount.UserId));
            cmd.Parameters.Add(new MySqlParameter("inBankName", bankAccount.BankName));
            cmd.Parameters.Add(new MySqlParameter("inIFSCCode", bankAccount.IFSCCode));
            cmd.Parameters.Add(new MySqlParameter("inBranch", bankAccount.BankName));
            cmd.Parameters.Add(new MySqlParameter("inAccountHolderName", bankAccount.AccountHolderName));
            cmd.Parameters.Add(new MySqlParameter("inAccountNumber", bankAccount.AccountNumber));
            cmd.Parameters.Add(new MySqlParameter("outId", MySqlDbType.Int32));
            cmd.Parameters["outId"].Direction = ParameterDirection.Output;
            return cmd;
        }

        internal static MySqlCommand BuildSaveUserAddressCommand(MySqlConnection connection, int accountId, Address address)
        {
            var cmd = new MySqlCommand("spSaveOrUpdateAddress", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("inAccountId", accountId));
            cmd.Parameters.Add(new MySqlParameter("inAddressType", address.AddressType));
            cmd.Parameters.Add(new MySqlParameter("inAddressLine1", address.AddressLine1));
            cmd.Parameters.Add(new MySqlParameter("inAddressLine2", address.AddressLine2));
            cmd.Parameters.Add(new MySqlParameter("inCity", address.City));
            cmd.Parameters.Add(new MySqlParameter("inState", address.State));
            cmd.Parameters.Add(new MySqlParameter("inZipCode", address.ZipCode));
            cmd.Parameters.Add(new MySqlParameter("inCountry", address.Country));
            cmd.Parameters.Add(new MySqlParameter("outId", MySqlDbType.Int32));
            cmd.Parameters["outId"].Direction = ParameterDirection.Output;
            return cmd;
        }

        internal static MySqlCommand BuildGetOperatorInfoByIdCommand(MySqlConnection connection, int busOperatorId)
        {
            var cmd = new MySqlCommand("spGetBusOperator", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("inOperatorId", busOperatorId));
            return cmd;
        }

        internal static MySqlCommand BuildUserBankAccountCommand(MySqlConnection connection, int operatorId)
        {
            var cmd = new MySqlCommand("spGetUserBankAccounts", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("inUserId", operatorId));
            return cmd;
        }

        internal static MySqlCommand BuildUserAddressesCommand(MySqlConnection connection, int operatorId)
        {
            var cmd = new MySqlCommand("spGetUserAddress", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("inAccountId", operatorId));
            return cmd;
        }
    }
}
