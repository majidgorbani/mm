using SE.GOV.MM.Integration.Log;
using SE.GOV.MM.Integration.Package.DataLayer.Objects;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE.GOV.MM.Integration.Package.DataLayer
{
    public class SqlManager
    {
        public PackageStatus ConvertStringToPackageStatus(string status)
        {
            switch (status)
            {
                case "Delivered":
                    return PackageStatus.Delivered;
                case "Failed":
                    return PackageStatus.Failed;
                case "Pending":
                    return PackageStatus.Pending;
                default:
                    return PackageStatus.Unknown;
            }
        }

        /// <summary>
        /// Gets the delivery status of a package from database.
        /// </summary>
        /// <param name="distributionId"></param>
        /// <param name="sender"></param>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public List<PackageResult> GetPackageDeliveryResult(int maxStatusMessages, string pnrOrgNr, string sender, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.DataLayer.SqlManager: incoming GetPackageDeliveryResult RequestId: {0}", requestId));

            //Get package delivery result from database database.
            var sqlCommand = GetSqlCommandAsStoredProcedure("GetPackageDeliveryStatus");
            PackageResult packageResult = null;
            List<PackageResult> listOfPackageResult = null;
            try
            {
                using (sqlCommand)
                {
                    sqlCommand.Parameters.Add(GetSqlParameter(SqlDbType.Int, "MaxStatusMessages", maxStatusMessages));

                    if (!string.IsNullOrEmpty(pnrOrgNr))
                    {
                        sqlCommand.Parameters.Add(GetSqlParameter(SqlDbType.NVarChar, "PnrOrgNr", pnrOrgNr));
                    }
                    else
                    {
                        sqlCommand.Parameters.Add(GetSqlParameter(SqlDbType.NVarChar, "PnrOrgNr", DBNull.Value));
                    }

                    sqlCommand.Parameters.Add(GetSqlParameter(SqlDbType.NVarChar, "Sender", sender));

                    sqlCommand.Connection.Open();

                    var reader = sqlCommand.ExecuteReader();
                    
                    if (reader.HasRows)
                    {
                        listOfPackageResult = new List<PackageResult>();
                        while (reader.Read())
                        {
                            packageResult = new PackageResult();
                            packageResult.PackageStatus = ConvertStringToPackageStatus(reader["Status"].ToString());
                            packageResult.DistributionId = reader["DistributionId"].ToString();
                            
                            var createdDate = DateTime.MinValue;
                            DateTime.TryParse(reader["CreatedDate"].ToString(), out createdDate);
                            packageResult.CreatedDate = createdDate;

                            if (reader["Status"].ToString() == PackageStatus.Delivered.ToString())
                            {
                                var deliveryDate = DateTime.MinValue;
                                DateTime.TryParse(reader["DeliveryDate"].ToString(), out deliveryDate);
                                packageResult.DeliveryDate = deliveryDate;
                            }                                                     
                            packageResult.Recipient = reader["Recipient"].ToString();
                            listOfPackageResult.Add(packageResult);
                        }
                    }

                    sqlCommand.Connection.Close();
                }
            }
            catch (Exception e)
            {
                var errorMessage = string.Format("SE.GOV.MM.Integration.Package.DataLayer.SqlManager: Error getting Package from database. Sender: {0}, RequestId: {1}", sender, requestId);
                LogManager.Log(new Log.Log() { EventId = EventId.SqlDatabaseExceptionReading, Exception = e, Level = Level.Error, Message = errorMessage });
            }
            finally
            {
                if (sqlCommand.Connection.State != System.Data.ConnectionState.Closed)
                {
                    sqlCommand.Connection.Close();
                }
            }

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.DataLayer.SqlManager: leaving GetPackageDeliveryResult RequestId: {0}", requestId));
            return listOfPackageResult;
        }

        /// <summary>
        /// Insert statistic in database.
        /// </summary>
        public void InsertIntoPackageStatistic(RecipientType recipientType, DateTime? deliveryDate, string messageStatus, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Delivery.DataLayer.DataManager: incoming InsertIntoPackageStatistic with RequestId: {0}", requestId));
            var sqlCommand = GetSqlCommandAsStoredProcedure("InsertIntoPackageStatistic");

            try
            {
                using (sqlCommand)
                {
                    sqlCommand.Parameters.Add(GetSqlParameter(System.Data.SqlDbType.NVarChar, "RecipientType", recipientType.ToString()));
                    sqlCommand.Parameters.Add(GetSqlParameter(System.Data.SqlDbType.DateTime, "DeliveryDate", DBNull.Value));
                    if (deliveryDate != null)
                        sqlCommand.Parameters["DeliveryDate"].Value = deliveryDate;

                    sqlCommand.Parameters.Add(GetSqlParameter(System.Data.SqlDbType.NVarChar, "DeliveryStatus", messageStatus));

                    sqlCommand.Connection.Open();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Connection.Close();
                }
            }
            catch (Exception e)
            {
                var errorMessage = string.Format("SE.GOV.MM.Integration.Delivery.DataLayer.DataManager: Error saving Statistic to database. ExceptionMessage: {0}, RequestId: {1}", e.Message, requestId);
                LogManager.Log(new Log.Log() { EventId = EventId.SqlDatabaseExceptionWriting, Exception = e, Level = Level.Error, Message = errorMessage });
            }
            finally
            {
                if (sqlCommand.Connection.State != System.Data.ConnectionState.Closed)
                {
                    sqlCommand.Connection.Close();
                }
            }

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Delivery.DataLayer.DataManager: leaving InsertIntoPackageStatistic with RequestId: {0}", requestId));
        }

        /// <summary>
        /// Inserts a new Package to the database and returns the Id.
        /// </summary>
        public int InsertIntoPackage(PackageDelivery package, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.DataLayer.SqlManager: incoming InsertIntoPackage with RequestId: {0}", requestId));
            
            //Save Message to database.
            var sqlCommand = GetSqlCommandAsStoredProcedure("InsertIntoPackage");

            var returnId = 0;
            
            try
            {
                using (sqlCommand)
                {
                    sqlCommand.Parameters.Add(GetSqlParameter(System.Data.SqlDbType.NVarChar, "Sender", package.Sender));
                    sqlCommand.Parameters.Add(GetSqlParameter(System.Data.SqlDbType.NVarChar, "Recipient", package.Recipient));
                    sqlCommand.Parameters.Add(GetSqlParameter(System.Data.SqlDbType.DateTime, "CreatedDate", package.CreatedDate));
                    sqlCommand.Parameters.Add(GetSqlParameter(System.Data.SqlDbType.NVarChar, "MessageStatus", package.PackageStatus.ToString()));
                    sqlCommand.Parameters.Add(GetSqlParameter(System.Data.SqlDbType.UniqueIdentifier, "RequestId", requestId));
                    sqlCommand.Parameters.Add(GetOutputSqlParameter(SqlDbType.Int, "Id", 0));

                    sqlCommand.Connection.Open();
                    sqlCommand.ExecuteNonQuery();

                    returnId = int.Parse(sqlCommand.Parameters["Id"].Value.ToString());

                    if (returnId == 0)
                    {
                        throw new ArgumentException("SE.GOV.MM.Integration.Package.DataLayer.SqlManager: Something went wrong saving a new post in the database, no new Id was returned..");
                    }
                }
            }
            catch (Exception e)
            {
                var errorMessage = string.Format("SE.GOV.MM.Integration.Package.DataLayer.SqlManager: Error saving Message to database. Recipient: {0}, RequestId: {1}", package.Recipient, requestId);
                LogManager.Log(new Log.Log() { EventId = EventId.SqlDatabaseExceptionWriting, Exception = e, Level = Level.Error, Message = errorMessage });
                throw e;
            }
            finally
            {
                if (sqlCommand.Connection.State != System.Data.ConnectionState.Closed)
                {
                    sqlCommand.Connection.Close();
                }
            }

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.DataLayer.SqlManager: leaving InsertIntoPackage with RequestId: {0}", requestId));
            return returnId;
        }

        /// <summary>
        /// Inserts a new Package log to the database.
        /// </summary>
        public int InsertMailboxDeliveryIntoPackage(PackageDeliveryMailbox package, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.DataLayer.SqlManager: incoming InsertMailboxDeliveryIntoPackage with RequestId: {0}", requestId));

            //Save Message to database.
            var sqlCommand = GetSqlCommandAsStoredProcedure("InsertIntoPackageDeliveryMailbox");

            var returnId = 0;

            try
            {
                using (sqlCommand)
                {
                    sqlCommand.Parameters.Add(GetSqlParameter(System.Data.SqlDbType.NVarChar, "PackageStatus", package.PackageStatus.ToString()));
                    sqlCommand.Parameters.Add(GetSqlParameter(System.Data.SqlDbType.NVarChar, "Sender", package.Sender));
                    sqlCommand.Parameters.Add(GetSqlParameter(System.Data.SqlDbType.NVarChar, "Recipient", package.Recipient));
                    sqlCommand.Parameters.Add(GetSqlParameter(System.Data.SqlDbType.DateTime, "CreatedDate", package.CreatedDate));                   
                    sqlCommand.Parameters.Add(GetSqlParameter(System.Data.SqlDbType.UniqueIdentifier, "RequestId", requestId));                   
                    sqlCommand.Parameters.Add(GetSqlParameter(System.Data.SqlDbType.NVarChar, "MailBoxOperator", package.MailboxOperator));
                    sqlCommand.Parameters.Add(GetOutputSqlParameter(SqlDbType.Int, "Id", 0));

                    sqlCommand.Connection.Open();
                    sqlCommand.ExecuteNonQuery();

                    returnId = int.Parse(sqlCommand.Parameters["Id"].Value.ToString());

                    if (returnId == 0)
                    {
                        throw new ArgumentException("SE.GOV.MM.Integration.Package.DataLayer.SqlManager: Something went wrong saving InsertMailboxDeliveryIntoPackage, a new post in the database, no new Id was returned..");
                    }
                }
            }
            catch (Exception e)
            {
                var errorMessage = string.Format("SE.GOV.MM.Integration.Package.DataLayer.SqlManager: Error saving Message to database. Recipient: {0}, RequestId: {1}", package.Recipient, requestId);
                LogManager.Log(new Log.Log() { EventId = EventId.SqlDatabaseExceptionWriting, Exception = e, Level = Level.Error, Message = errorMessage });
                throw e;
            }
            finally
            {
                if (sqlCommand.Connection.State != System.Data.ConnectionState.Closed)
                {
                    sqlCommand.Connection.Close();
                }
            }

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.DataLayer.SqlManager: leaving InsertMailboxDeliveryIntoPackage with RequestId: {0}", requestId));

            return returnId;
        }

        public void UpdatePackageDeliveryMailbox(PackageDeliveryMailbox packageResult, int databaseId, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.DataLayer.SqlManager: incoming UpdatePackageDeliveryMailbox with RequestId: {0}", requestId));

            //Save MessageResult to database.
            var sqlCommand = GetSqlCommandAsStoredProcedure("UpdatePackageDeliveryMailbox");

            try
            {
                using (sqlCommand)
                {
                    sqlCommand.Parameters.Add(GetSqlParameter(System.Data.SqlDbType.NVarChar, "Id", databaseId));
                    sqlCommand.Parameters.Add(GetSqlParameter(System.Data.SqlDbType.NVarChar, "TransactionId", packageResult.TransactionId));

                    if (packageResult.PackageStatus == PackageStatus.Delivered)
                        sqlCommand.Parameters.Add(GetSqlParameter(System.Data.SqlDbType.DateTime, "DeliveryDate", DateTime.Now));

                    sqlCommand.Parameters.Add(GetSqlParameter(System.Data.SqlDbType.NVarChar, "Status", packageResult.PackageStatus.ToString()));

                    sqlCommand.Connection.Open();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Connection.Close();
                }
            }
            catch (Exception e)
            {
                var errorMessage = string.Format("SE.GOV.MM.Integration.Package.DataLayer.SqlManager: Error when updating Message in database.  PackageId: {0}, DistibutionId: {1}, RequestId: {2}", databaseId, packageResult.TransactionId, requestId);
                LogManager.Log(new Log.Log() { EventId = EventId.SqlDatabaseExceptionWriting, Exception = e, Level = Level.Error, Message = errorMessage });
            }
            finally
            {
                if (sqlCommand.Connection.State != System.Data.ConnectionState.Closed)
                {
                    sqlCommand.Connection.Close();
                }
            }

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.DataLayer.SqlManager: leaving UpdatePackage with RequestId: {0}", requestId));
        }

        public void UpdatePackage(PackageResult packageResult, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.DataLayer.SqlManager: incoming UpdatePackage with RequestId: {0}", requestId));

            //Save MessageResult to database.
            var sqlCommand = GetSqlCommandAsStoredProcedure("UpdatePackage");

            try
            {
                using (sqlCommand)
                {
                    sqlCommand.Parameters.Add(GetSqlParameter(System.Data.SqlDbType.NVarChar, "Id", packageResult.DatabaseId));
                    sqlCommand.Parameters.Add(GetSqlParameter(System.Data.SqlDbType.NVarChar, "DistributionId", packageResult.DistributionId));
                    sqlCommand.Parameters.Add(GetSqlParameter(System.Data.SqlDbType.DateTime, "DeliveryDate", packageResult.DeliveryDate));
                    sqlCommand.Parameters.Add(GetSqlParameter(System.Data.SqlDbType.NVarChar, "Status", packageResult.PackageStatus.ToString()));

                    sqlCommand.Connection.Open();

                    sqlCommand.ExecuteNonQuery();

                    sqlCommand.Connection.Close();
                }
            }
            catch (Exception e)
            {
                var errorMessage = string.Format("SE.GOV.MM.Integration.Package.DataLayer.SqlManager: Error when updating Message in database.  PackageId: {0}, DistibutionId: {1}, RequestId: {2}", packageResult.DatabaseId, packageResult.DistributionId, requestId);
                LogManager.Log(new Log.Log() { EventId = EventId.SqlDatabaseExceptionWriting, Exception = e, Level = Level.Error, Message = errorMessage });
            }
            finally
            {
                if (sqlCommand.Connection.State != System.Data.ConnectionState.Closed)
                {
                    sqlCommand.Connection.Close();
                }
            }

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.DataLayer.SqlManager: leaving UpdatePackage with RequestId: {0}", requestId));
        }

        public SqlParameter GetSqlParameter(SqlDbType type, string name, object value)
        {
            var sqlParameter = new SqlParameter(name, type);
            sqlParameter.Value = value;
            return sqlParameter;
        }

        public SqlParameter GetOutputSqlParameter(SqlDbType type, string name, object value)
        {
            var sqlParameter = new SqlParameter(name, type);
            sqlParameter.Value = value;
            sqlParameter.Direction = ParameterDirection.Output;
            return sqlParameter;
        }

        public SqlCommand GetSqlCommandAsStoredProcedure(string storedProcedureName)
        {
            var connectionString = GetConnectionString();
            var sqlConnection = new SqlConnection(connectionString);
            var sqlCommand = new SqlCommand(storedProcedureName, sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            return sqlCommand;
        }

        private string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["SqlDatabase"].ConnectionString;
        }
    }
}
