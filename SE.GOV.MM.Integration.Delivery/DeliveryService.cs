using System;
using System.Diagnostics;
using System.ServiceProcess;
using SE.GOV.MM.Integration.Delivery.DataLayer;
using System.Timers;
using SE.GOV.MM.Integration.Log;

namespace SE.GOV.MM.Integration.Delivery
{
    public partial class DeliveryService : ServiceBase
    {

        public DeliveryService()
        {
            InitializeComponent();

            if (!EventLog.SourceExists(ConfigHelper.ApplicationName))
            {
                EventLog.CreateEventSource(ConfigHelper.ApplicationName, "Application");
            }
        }

        protected override void OnStart(string[] args)
        {
            LogManager.LogTrace("SE.GOV.MM.Integration.Delivery.DeliveryService: incoming OnStart.");
            LogManager.Log(new Log.Log() { EventId = EventId.Information, Message = "SE.GOV.MM.Integration.Delivery.DeliveryService: starting windowsservice Delivery.", Level = Level.Info });

            Init();

            LogManager.LogTrace("SE.GOV.MM.Integration.Delivery.DeliveryService: leaving OnStart.");
        }

        /// <summary>
        /// Initiating windowsservice, setting a timer (in minutes) on a app.config value. 
        /// </summary>
        private void Init()
        {
            LogManager.LogTrace("SE.GOV.MM.Integration.Delivery.DeliveryService: incoming Init.");

            //At start make a check if there is any messages that needs to be checked in database.
            checkDatabase();

            var tmrUpdateDatabase = new Timer();

            // Set timer for next check to config value.           
            tmrUpdateDatabase.Elapsed += tmrUpdateDatabase_Elapsed;
            tmrUpdateDatabase.Interval = ConfigHelper.TimerInterval;

            var timerIntervalMinutes = ConfigHelper.TimerInterval / 60000;
            var deliveryTimeUntilFailed = ConfigHelper.DeliveryTimeUntilFailed;

            LogManager.Log(new Log.Log() { Message = string.Format("SE.GOV.MM.Integration.Delivery.DeliveryService: Update timer on delivery service are set to: {0} minute.", timerIntervalMinutes), EventId = EventId.Information, Level = Level.Info });
            LogManager.Log(new Log.Log() { Message = string.Format("SE.GOV.MM.Integration.Delivery.DeliveryService: DeliveryTimeUntilFailed on delivery service are set to: {0} minute.", deliveryTimeUntilFailed), EventId = EventId.Information, Level = Level.Info });

            // Start timer.
            tmrUpdateDatabase.Start();

            LogManager.LogTrace("SE.GOV.MM.Integration.Delivery.DeliveryService: leaving Init.");
        }

        private void tmrUpdateDatabase_Elapsed(object sender, EventArgs e)
        {
            LogManager.LogTrace("SE.GOV.MM.Integration.Delivery.DeliveryService: incoming tmrUpdateDatabase_Elapsed.");

            try
            {
                checkDatabase();
            }
            catch (Exception ex)
            {
                LogManager.Log(new Log.Log() { Exception = ex, Message = string.Format("SE.GOV.MM.Integration.Delivery.DeliveryService: Something went wrong when reading/updating the database in the application. ExceptionMessage: {0}", ex.Message), EventId = EventId.SqlDatabaseExceptionReading, Level = Level.Error });
            }

            LogManager.LogTrace("SE.GOV.MM.Integration.Delivery.DeliveryService: leaving tmrUpdateDatabase_Elapsed.");
        }

        private void checkDatabase()
        {
            LogManager.LogTrace("SE.GOV.MM.Integration.Delivery.DeliveryService: incoming checkDatabase.");

            var dataManager = new DataManager();
            dataManager.CheckAndUpdateMessageStatus();

            LogManager.LogTrace("SE.GOV.MM.Integration.Delivery.DeliveryService: leaving checkDatabase.");
        }

        protected override void OnStop() => LogManager.Log(new Log.Log() { EventId = EventId.Information, Message = "SE.GOV.MM.Integration.Delivery.DeliveryService: shutting down windowsservice Delivery.", Level = Level.Info });

    }
}
