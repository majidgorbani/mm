using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SE.GOV.MM.Integration.Delivery.DataLayer.BusinessObjects;
using SE.GOV.MM.Integration.Delivery.DataLayer.Service;
using SE.GOV.MM.Integration.Log;

namespace SE.GOV.MM.Integration.Delivery.DataLayer
{
    /// <summary>
    /// Reads and write to Sql db
    /// </summary>
    public class DataManager
    {
        private const string Pending = "Pending";

        /// <summary>
        /// Checks if any messages with status pending is delivered/failed and updates status in database. 
        /// Returns sum of all pending messages.
        /// </summary>
        public void CheckAndUpdateMessageStatus()
        {
            LogManager.LogTrace("SE.GOV.MM.Integration.Delivery.DataLayer.DataManager: incoming CheckAndUpdateMessageStatus");

            // Get all DeliveryResult Pending from database.
            var listOfMessageStatusPending = GetPackageByPackageStatusPending();

            // Check for new status with CheckDistributeSecure in message webservice.
            if (listOfMessageStatusPending.Count > 0)
            {
                LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Delivery.DataLayer.DataManager: checking messages counting: {0}.", listOfMessageStatusPending.Count.ToString()));

                foreach (var message in listOfMessageStatusPending)
                {
                    DistributionStatus deliveryResult = null;

                    if (!string.IsNullOrEmpty(message.DistributionId))
                        deliveryResult = CheckDistibutionStatus(message?.DistributionId);
                    else
                        message.MessageStatus = MessageStatus.Failed;

                    if (deliveryResult != null)
                    {
                        message.MessageStatus = ConvertDeliveryResultToMessageResult(deliveryResult.DeliveryStatus, message.DistributionId);
                    }
                }

                // Update database
                UpdateMessages(listOfMessageStatusPending);
            }
            else
            {
                LogManager.LogTrace("SE.GOV.MM.Integration.Delivery.DataLayer.DataManager: No messages in database with status pending..");
            }
           
            LogManager.LogTrace("SE.GOV.MM.Integration.Delivery.DataLayer.DataManager: leaving CheckAndUpdateMessageStatus");
        }

        private DistributionStatus CheckDistibutionStatus(string distributionId) => new MessageService().CheckDistibutionStatusV3(distributionId);
        

        private MessageStatus ConvertDeliveryResultToMessageResult(DeliveryStatus deliveryStatus, string distributionId)
        {
            MessageStatus status;
            switch (deliveryStatus)
            {
                case DeliveryStatus.Delivered:
                    status = MessageStatus.Delivered;
                    break;
                case DeliveryStatus.DeliveryFailed:
                    status = MessageStatus.Failed;
                    break;
                case DeliveryStatus.Pending:
                    status = MessageStatus.Pending;
                    break;
                default:
                    throw new ArgumentException(string.Format("Cant map DeliveryStatus from MessageService DeliveryStatus: {0}, distributionId: {1}.", deliveryStatus.ToString(), distributionId));
            }

            return status;
        }

        private void UpdateMessages(List<Message> messages)
        {
            LogManager.LogTrace("SE.GOV.MM.Integration.Delivery.DataLayer.DataManager: incoming UpdateMessages");
            LogManager.Log(new Log.Log()
            {
                Message = string.Format("SE.GOV.MM.Integration.Delivery.DataLayer.DataManager: List of messages contains: {0} messages.", messages.Count.ToString()),
                Level = Level.Info,
                EventId = EventId.Information
            });

            if (messages.Count > 0)
            {
                foreach (var message in messages)
                {
                    // If a message is older then XX minutes, it should be set as failed to deliver. How many minutes until failed is set in configfile.
                    if ((message.MessageStatus == MessageStatus.Pending) && (message.CreatedDate.AddMinutes(ConfigHelper.DeliveryTimeUntilFailed) <= DateTime.Now))
                    {
                        message.MessageStatus = MessageStatus.Failed;
                    }

                    switch (message.MessageStatus)
                    {
                        case MessageStatus.Delivered:
                            message.DeliveryDate = DateTime.Now;
                            //Update Package in database
                            UpdateMessageStatus(message);
                            //Add Statistic
                            InsertIntoPackageStatistic(message);
                            break;
                        case MessageStatus.Failed:
                            //Update Package in database
                             UpdateMessageStatus(message);
                             //Add Statistic
                             InsertIntoPackageStatistic(message);
                             LogManager.Log(new Log.Log() { Message = string.Format("SE.GOV.MM.Integration.Delivery.DataLayer.DataManager: Error delivering message with DistributionId: {0} and MessageId: {1}", message.DistributionId, message.Id), EventId = EventId.Warning, Level = Level.Warning });
                            break;
                    }
                }
            }
            LogManager.LogTrace("SE.GOV.MM.Integration.Delivery.DataLayer.DataManager: leaving UpdateMessages");
        }

        private void UpdateMessageStatus(Message message)
        {
            LogManager.LogTrace("SE.GOV.MM.Integration.Delivery.DataLayer.DataManager: incoming updateMessageStatus");
            LogManager.Log(new Log.Log() { Message = string.Format("SE.GOV.MM.Integration.Delivery.DataLayer.DataManager: DistributionId: {0}, MessageStatus: {1}", message.DistributionId, message.MessageStatus), EventId = EventId.Information, Level = Level.Info });

            var sqlCommand = GetSqlCommandAsStoredProcedure("UpdatePackageStatus");
            try
            {
                using (sqlCommand)
                {
                    sqlCommand.Parameters.Add(GetSqlParameter(System.Data.SqlDbType.NVarChar, "Id", message.Id));
                    sqlCommand.Parameters.Add(GetSqlParameter(System.Data.SqlDbType.NVarChar, "Status", message.MessageStatus));
                    sqlCommand.Parameters.Add(GetSqlParameter(System.Data.SqlDbType.DateTime, "DeliveryDate", DBNull.Value));
                    if (message.DeliveryDate != null)
                        sqlCommand.Parameters["DeliveryDate"].Value = message.DeliveryDate;               
                    
                    sqlCommand.Connection.Open();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Connection.Close();
                }
            }
            catch (Exception e)
            {
                var errorMessage = string.Format("SE.GOV.MM.Integration.Delivery.DataManager: Error updating messagestatus in database. DistributionId: {0}, ExceptionMessage: {1}", message.DistributionId, e.Message);
                LogManager.Log(new Log.Log() { EventId = EventId.SqlDatabaseExceptionWriting, Exception = e, Level = Level.Error, Message = errorMessage });
            }
            finally
            {
                if (sqlCommand.Connection.State != System.Data.ConnectionState.Closed)
                {
                    sqlCommand.Connection.Close();
                }
            }

            LogManager.LogTrace("SE.GOV.MM.Integration.Delivery.DataLayer.DataManager: leaving ExecuteUpdateMessageStatus");
        }

        /// <summary>
        /// Insert statistic in database.
        /// </summary>
        public void InsertIntoPackageStatistic(Message message)
        {
            LogManager.LogTrace("SE.GOV.MM.Integration.Delivery.DataLayer.DataManager: incoming InsertIntoPackageStatistic");
            var sqlCommand = GetSqlCommandAsStoredProcedure("InsertIntoPackageStatistic");

            try
            {               
                using (sqlCommand)
                {
                    sqlCommand.Parameters.Add(GetSqlParameter(System.Data.SqlDbType.NVarChar, "RecipientType", message.RecipientType.ToString()));
                    sqlCommand.Parameters.Add(GetSqlParameter(System.Data.SqlDbType.DateTime, "DeliveryDate", DBNull.Value));
                    if (message.DeliveryDate != null)
                        sqlCommand.Parameters["DeliveryDate"].Value = message.DeliveryDate;

                    sqlCommand.Parameters.Add(GetSqlParameter(System.Data.SqlDbType.NVarChar, "DeliveryStatus", message.MessageStatus));

                    sqlCommand.Connection.Open();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Connection.Close();
                }
            }
            catch (Exception e)
            {
                var errorMessage = string.Format("SE.GOV.MM.Integration.Delivery.DataLayer.DataManager: Error saving Statistic to database. ExceptionMessage: {0}", e.Message);
                LogManager.Log(new Log.Log() { EventId = EventId.SqlDatabaseExceptionWriting, Exception = e, Level = Level.Error, Message = errorMessage });
            }
            finally
            {
                if (sqlCommand.Connection.State != System.Data.ConnectionState.Closed)
                {
                    sqlCommand.Connection.Close();
                }
            }

            LogManager.LogTrace("SE.GOV.MM.Integration.Delivery.DataLayer.DataManager: leaving InsertIntoPackageStatistic");
        }

        /// <summary>
        /// Get Messages with status pending from database.
        /// </summary>
        private List<Message> GetPackageByPackageStatusPending()
        {
            LogManager.LogTrace("SE.GOV.MM.Integration.Delivery.DataLayer.DataManager: incoming GetMessageByMessageStatusPending");

            var listOfMessage = new List<Message>();
            var sqlCommand = GetSqlCommandAsStoredProcedure("GetPackageByStatus");

            try
            {
                using (sqlCommand)
                {
                    sqlCommand.Parameters.Add(GetSqlParameter(System.Data.SqlDbType.NVarChar, "Status", Pending));
                    
                    sqlCommand.Connection.Open();

                    var reader = sqlCommand.ExecuteReader();

                    if (reader.HasRows)
                    {
                        Message message;

                        while (reader.Read())
                        {
                            message = new Message();
                            message.Id = int.Parse(reader["Id"].ToString());
                            message.DistributionId = reader["DistributionId"].ToString();
                            message.MessageStatus = MessageStatus.Pending;
                            var createdDate = DateTime.MinValue;
                            DateTime.TryParse(reader["CreatedDate"].ToString(), out createdDate);
                            message.CreatedDate = createdDate;
                            message.Recipient = reader["Recipient"].ToString();
                            message.RecipientType = GetRecipientType(reader["Recipient"].ToString());

                            listOfMessage.Add(message);
                        }
                    }

                    sqlCommand.Connection.Close();
                }
            }
            catch (Exception e)
            {
                var errorMessage = string.Format("SE.GOV.MM.Integration.Delivery: Error getting messages with status pending from database. ExceptionMessage: {0}", e.Message);
                LogManager.Log(new Log.Log() { EventId = EventId.SqlDatabaseExceptionReading, Exception = e, Level = Level.Error, Message = errorMessage });
            }
            finally
            {
                if (sqlCommand.Connection.State != System.Data.ConnectionState.Closed)
                {
                    sqlCommand.Connection.Close();
                }
            }

            LogManager.LogTrace("SE.GOV.MM.Integration.Delivery.DataLayer.DataManager: leaving GetMessageByMessageStatusPending");

            return listOfMessage;
        }

        private SqlCommand GetSqlCommandAsStoredProcedure(string storedProcedureName)
        {
            var connectionString = ConfigHelper.SqlDatabaseConnectionString;
            var sqlConnection = new SqlConnection(connectionString);
            var sqlCommand = new SqlCommand(storedProcedureName, sqlConnection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            return sqlCommand;
        }

        private SqlParameter GetSqlParameter(SqlDbType type, string name, object value)
        {
            var sqlParameter = new SqlParameter(name, type);
            sqlParameter.Value = value;
            return sqlParameter;
        }

        /// <summary>
        /// Gets the recipient type, Organization always start with '16' or Person.
        /// </summary>
        private RecipientType GetRecipientType(string recipient) => recipient.StartsWith("16") ? RecipientType.Organization : RecipientType.Private;

    }
}
