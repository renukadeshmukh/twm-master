using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.Data.MySql.Driver;
using TravelWithMe.Data.MySql.Entities;
using TravelWithMe.Logging.Helper;
using TravelWithMe.Data.Interfaces;
using System.Data;

namespace TravelWithMe.Data.MySql
{
    public class CodeVerificationDataProvider : ICodeVerificationDataProvider
    {
        private string Source = "CodeVerificationDataProvider";

        #region ICodeVerificationDataProvider Members

        public void SaveNewCode(string key, string value, string type)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseWrite);
                db.ExecuteNonQuery(CommandBuilder.BuildSaveVerificationCodeCommand(key, value, type, db.Connection));
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "SaveNewCode", Severity.Critical);
            }
        }

        public bool IsValid(string key, string value, string type)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseRead);

                DataSet dataSet =
                    db.ExecuteQuery(CommandBuilder.BuildVerifyVerificationCodeCommand(key, value, type, db.Connection));

                if (dataSet != null && dataSet.Tables != null &&
                    dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count > 0)
                {
                    if (!Convert.IsDBNull(dataSet.Tables[0].Rows[0]["outresult"]))
                    {
                        return Convert.ToBoolean(dataSet.Tables[0].Rows[0]["outresult"]);
                    }
                }
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "IsValid", Severity.Critical);
            }
            return false;
        }

        public bool Remove(string key, string type)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseWrite);

                db.ExecuteNonQuery(CommandBuilder.BuildRemoveVerificationCodeCommand(key, type, db.Connection));
                return true;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "Remove", Severity.Critical);
            }
            return false;
        }

        #endregion
    }
}
