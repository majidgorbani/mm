using SE.GOV.MM.Integration.Log;
using SE.GOV.MM.Integration.Package.DataLayer;
using SE.GOV.MM.Integration.Package.DataLayer.Objects;
using System;

namespace SE.GOV.MM.Integration.Package.BusinessLayer.Handler
{
    /// <summary>
    /// Handles packages, builds objects to send to sqlManager. 
    /// </summary>
    public class PackageHandler
    {
        private SqlManager _sqlManager;

        public PackageHandler()
        {
            _sqlManager = new SqlManager();
        }

        /// <summary>
        /// Creates a PackageResult and updates database.
        /// </summary>
        public void UpdatePackage(string distributionId, int databaseId, PackageStatus status, DateTime? deliveryDate, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.BusinessLayer.Handler.PackageHandler: incoming UpdateMessage with RequestId: {0}", requestId));

            var packageResult = new PackageResult()
            {
                DeliveryDate = deliveryDate,
                DistributionId = distributionId,
                PackageStatus = status,
                DatabaseId = databaseId
            };

            _sqlManager.UpdatePackage(packageResult, requestId);

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.BusinessLayer.Handler.PackageHandler: leaving UpdateMessage with RequestId: {0}", requestId));
        }

        /// <summary>
        /// Creates a PackageResult and updates database.
        /// </summary>
        public void UpdatePackage(string transactionId, int databaseId, PackageStatus status, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.BusinessLayer.Handler.PackageHandler: incoming UpdateMessage with RequestId: {0}", requestId));

            var packageDeliveryMailbox = new PackageDeliveryMailbox()
            {
                TransactionId = transactionId,
                PackageStatus = status,
            };

            _sqlManager.UpdatePackageDeliveryMailbox(packageDeliveryMailbox, databaseId, requestId);

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.BusinessLayer.Handler.PackageHandler: leaving UpdateMessage with RequestId: {0}", requestId));
        }

        /// <summary>
        /// Creates and saves a PackageDelivery in database. 
        /// </summary>
        public int SavePackage(string recipient, string sender, PackageStatus status, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.BusinessLayer.Handler.PackageHandler: incoming SavePackage with RequestId: {0}", requestId));

            var package = new PackageDelivery()
            {
                Recipient = recipient,
                CreatedDate = DateTime.Now,
                Sender = sender,
                PackageStatus = status
            };

            var id = _sqlManager.InsertIntoPackage(package, requestId);

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.BusinessLayer.Handler.PackageHandler: leaving SavePackage with RequestId: {0}", requestId));
            return id;
        }

        /// <summary>
        /// Creates and saves a PackageDeliveryMailbox in database. 
        /// </summary>
        public int SavePackage(string recipient, string sender, PackageStatus status, Guid requestId, string mailboxOperator)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.BusinessLayer.Handler.PackageHandler: incoming SavePackage with RequestId: {0}", requestId));

            var package = new PackageDeliveryMailbox()
            {
                Recipient = recipient,
                CreatedDate = DateTime.Now,
                Sender = sender,
                PackageStatus = status,
                MailboxOperator = mailboxOperator
            };

            var databaseId = _sqlManager.InsertMailboxDeliveryIntoPackage(package, requestId);

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.BusinessLayer.Handler.PackageHandler: leaving SavePackage with RequestId: {0}", requestId));

            return databaseId;
        }

        /// <summary>
        /// Saves statistics in database.
        /// </summary>
        /// <param name="recipientType"></param>
        /// <param name="deliveryDate"></param>
        /// <param name="messageStatus"></param>
        /// <param name="requestId"></param>
        public void SaveStatistics(RecipientType recipientType, DateTime? deliveryDate, string messageStatus, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.BusinessLayer.Handler.PackageHandler: incoming SavePackage with RequestId: {0}", requestId));

            _sqlManager.InsertIntoPackageStatistic(recipientType, deliveryDate, messageStatus, requestId);

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.BusinessLayer.Handler.PackageHandler: leaving SavePackage with RequestId: {0}", requestId));
        }
    }
}
