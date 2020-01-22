using SE.GOV.MM.Integration.Package.BusinessLayer;
using SE.GOV.MM.Integration.Package.BusinessLayer.Helper;
using SE.GOV.MM.Integration.Package.DataTransferObjects.Request;
using SE.GOV.MM.Integration.Package.DataTransferObjects.Response;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using SE.GOV.MM.Integration.Package.BusinessLayer.BusinessObjects;
using SE.GOV.MM.Integration.Log;
using System.Diagnostics;
using SE.GOV.MM.Integration.Package.Contract;


namespace SE.GOV.MM.Integration.Package
{
    [ServiceBehavior(Namespace = "https://SE.GOV.MM.Integration/Package/2015/04")]
    public class PackageService : IPackage
    {
        private BusinessManager _businessManager;

        public PackageService()
        {
            _businessManager = new BusinessManager();

            //if (!EventLog.SourceExists(ConfigHelper.ApplicationName))
            //{
            //    EventLog.CreateEventSource(ConfigHelper.ApplicationName, "Application");
            //}
        }

        /// <summary>
        /// Incoming mailitem that should be sent to dispatcher.
        /// </summary>
        public SendPackageResponse SendPackage(SendPackageRequest request)
        {
            //add a unique id to the request for logging and return to the client.
            request.RequestId = Guid.NewGuid();
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.PackageService: incoming SendPackage with RequestId: {0}", request.RequestId));

            var distributionId = _businessManager.SendPackage(request.Mail, request.RequestId);
            var deliveryStatus = "Pending";

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.PackageService: leaving SendPackage with RequestId: {0}", request.RequestId));
            return new SendPackageResponse() { RequestId = request.RequestId, DeliveryStatus = deliveryStatus, DistributionId = distributionId };
        }

        /// <summary>
        /// Get a deliveryresult from a sent package. 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public GetPackageDeliveryResultResponse GetPackageDeliveryResult(GetPackageDeliveryResultRequest request)
        {
            request.RequestId = Guid.NewGuid();

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.PackageService: incoming GetPackageDeliveryResult with RequestId: {0}", request.RequestId));

            var listOfPackageDeliveryResult = _businessManager.GetPackageResult(request.MaxStatusMessages, request.PnrOrgNr, request.Sender, request.RequestId);

            if (listOfPackageDeliveryResult != null)
            {
                var listOfPackageResult = new List<PackageResult>();

                foreach (var packageDeliveryResult in listOfPackageDeliveryResult)
                {
                    var packageResult = new PackageResult()
                    {
                        CreatedDate = packageDeliveryResult.CreatedDate,
                        DeliveryDate = packageDeliveryResult.DeliveryDate,
                        DistributionId = packageDeliveryResult.DistributionId,
                        Status = ConvertPackageDeliveryStatusToStatus(packageDeliveryResult.Status),
                        Recipient = packageDeliveryResult.Recipient,
                        Sender = request.Sender
                    };
                    listOfPackageResult.Add(packageResult);
                }

                var response = new GetPackageDeliveryResultResponse()
                {
                    PackageResult = listOfPackageResult,
                    RequestId = request.RequestId
                };

                LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.PackageService: leaving GetPackageDeliveryResult with RequestId: {0}", request.RequestId));
                return response;
            }

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.PackageService: leaving GetPackageDeliveryResult with a null value for RequestId: {0}", request.RequestId));
            return null;
        }

        private Status ConvertPackageDeliveryStatusToStatus(PackageDeliveryStatus status)
        {
            switch (status)
            {
                case PackageDeliveryStatus.Delivered:
                    return Status.Delivered;
                case PackageDeliveryStatus.Failed:
                    return Status.Failed;
                case PackageDeliveryStatus.Pending:
                    return Status.Pending;
                default:
                    return Status.Unknown;
            }
        }

        /// <summary>
        /// Incoming mail that should be sent to mailboxoperator
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public SendPackageResponse SendPackageToMailbox(SendPackageRequest request)
        {
            //add a unique id to the request for logging and return to the client.
            request.RequestId = Guid.NewGuid();
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.PackageService: ===> incoming SendPackageToMailbox with RequestId: {0}", request.RequestId));

            var result = _businessManager.SendPackageToMailbox(request.Mail, request.MailBox, request.RequestId);

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.PackageService: <=== leaving SendPackageToMailbox with RequestId: {0}", request.RequestId));
            return result;
        }
    }
}
