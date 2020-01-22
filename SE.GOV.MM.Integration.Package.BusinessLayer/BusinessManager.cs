using SE.GOV.MM.Integration.Package.BusinessLayer.Helper;
using SE.GOV.MM.Integration.Package.DataTransferObjects;
using SE.GOV.MM.Integration.Package.DataTransferObjects.BusinessObjects;
using SE.GOV.MM.Integration.Package.DataLayer;
using SE.GOV.MM.Integration.Package.DataLayer.Objects;
using SE.GOV.MM.Integration.Package.BusinessLayer.BusinessObjects;
using SE.GOV.MM.Integration.Package.BusinessLayer.Handler;
using SE.GOV.MM.Integration.Log;
using System;
using System.Collections.Generic;
using SE.GOV.MM.Integration.Package.Signing.Handler;
using SE.GOV.MM.Integration.Package.Signing.Helper;
using SE.GOV.MM.Integration.Package.DataLayer.DeliveryMailbox;
using SE.GOV.MM.Integration.Package.DataTransferObjects.Response;

namespace SE.GOV.MM.Integration.Package.BusinessLayer
{
    public class BusinessManager
    {

        /// <summary>
        /// Sends a package to mailbox operator for the recipient and saves in database.
        /// </summary>
        /// <param name="mailItem"></param>
        /// <param name="mailBox"></param>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public SendPackageResponse SendPackageToMailbox(Package.DataTransferObjects.BusinessObjects.Mail mailItem, MailBox mailBox, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.BusinessLayer.BusinessManager: incoming SendPackageToMailbox with RequestId: {0}", requestId));

            SendPackageToMailboxResponse sendPackageToMailboxResponse = new SendPackageToMailboxResponse() { Delivered = false, TransId = null };
            PackageStatus packageStatus = PackageStatus.Failed;

            // Validate the mail
            if (ValidateMail(mailItem, requestId))
            {
                var dataManager = new DataLayer.DataManager();

                try
                {
                    //LOG to database
                  //  var packageHandler = new PackageHandler();
                    //Save Package in database
                   // var databaseId = packageHandler.SavePackage(mailItem.Recipient.To, mailItem.Sender, PackageStatus.Failed, requestId, mailBox.Name);
                    //Send Package to DeliveryMailboxService
                    sendPackageToMailboxResponse = dataManager.SendPackageToMailBox(mailItem, mailBox, requestId);
                    //Get status of sent Package
                    packageStatus = getPackageStatus(sendPackageToMailboxResponse.Delivered);
                    //Log to database result of sending a message.
                   // packageHandler.UpdatePackage(sendPackageToMailboxResponse.TransId, 0, packageStatus, requestId);
                    //Save statistics in database
                    DateTime? deliveredDate = null;
                    if (packageStatus == PackageStatus.Delivered)
                    {
                        deliveredDate = DateTime.Now;
                    }

                  //  packageHandler.SaveStatistics(RecipientType(mailItem.Recipient.To), deliveredDate, packageStatus.ToString(), requestId);
                }
                catch (Exception e)
                {
                    var errorMessage = string.Format("SE.GOV.MM.Integration.Package.BusinessLayer.BusinessManager: Exception thrown trying to send a Package to Mailbox. Exception: {0}, RequestId: {1}", e.ToString(), requestId);
                    LogManager.Log(new Log.Log() { EventId = EventId.GenerelizedException, Exception = e, Level = Level.Error, Message = errorMessage } );
                    throw e;
                }
            }
            else
            {
                var failureMessage = string.Format("SE.GOV.MM.Integration.Package.BusinessLayer.BusinessManager: Couldnt send message to: {0}. Didnt validate. RequestId: {1}", mailItem.Recipient.To, requestId);
                LogManager.Log(new Log.Log() { EventId = EventId.Warning, Message = failureMessage, Level = Level.Warning });
            }

            var result = new SendPackageResponse() { DeliveryStatus = ConvertPackageStatusToPackageDeliveryStatus(packageStatus).ToString(), DistributionId = sendPackageToMailboxResponse.TransId, RequestId = requestId };
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.BusinessLayer.BusinessManager: leaving SendPackageToMailbox with RequestId: {0}", requestId));
            return result;
        }

        private RecipientType RecipientType(string recipient) => recipient.StartsWith("16") ? DataLayer.Objects.RecipientType.Organization : DataLayer.Objects.RecipientType.Private;


        private PackageStatus getPackageStatus(bool delivered) => delivered ? PackageStatus.Delivered : PackageStatus.Failed;
        
        

        /// <summary>
        /// Validates recipient and signs message, sends it to Mina meddelanden. 
        /// </summary>
        public string SendPackage(Package.DataTransferObjects.BusinessObjects.Mail mailItem, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.BusinessLayer.BusinessManager: incoming SendPackage with RequestId: {0}", requestId));

            var distributionId = string.Empty;
            // Validate the mail
            if (ValidateMail(mailItem, requestId)) 
            {
                distributionId = SendPackageV3(mailItem, requestId);                       
            }
            else
            {
                var failureMessage = string.Format("SE.GOV.MM.Integration.Package.BusinessLayer.BusinessManager: Couldnt send package to: {0}. Didnt validate. RequestId: {1}", mailItem.Recipient.To, requestId);
                LogManager.Log(new Log.Log() { EventId = EventId.Warning, Message = failureMessage, Level = Level.Warning });
            }

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.BusinessLayer.BusinessManager: leaving SendPackage with RequestId: {0}", requestId));
            return distributionId;
        }


        public string SendPackageV3(DataTransferObjects.BusinessObjects.Mail mailItem, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.BusinessLayer.BusinessManager: incoming SendPackageV3 with RequestId: {0}", requestId));

            //Transform MailItem to object of type specified by MinaMeddelandens Message webservice. 
            var secureDelivery = GetSecureDeliveryV3(mailItem, requestId);

            //Create SignedDelivery and sign xmldocument and create a secureDelivery object specified in Mina meddelanden Message webservice.
            var signedDeliveryHandler = new SignedDeliveryHandler();
            var signedDelivery = signedDeliveryHandler.GetSignedDeliveryV3(secureDelivery, ConfigHelper.SignDelivery, ConfigHelper.DefaultNamespaceV3, ConfigHelper.SigningCertificateSubjectName, requestId);

            //LOG to database before sending a message.
            var packageHandler = new PackageHandler();

            //Save message in database
          //  var databaseId = packageHandler.SavePackage(mailItem.Recipient.To, mailItem.Sender, PackageStatus.Pending, requestId);

            var dataManager = new DataManager();

            var distributionId = dataManager.SendPackageV3(signedDelivery, requestId);

            //Log to database result of sending a message.
            DateTime? deliveryDate = null;
            packageHandler.UpdatePackage(distributionId, 0, PackageStatus.Pending, deliveryDate, requestId);

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.BusinessLayer.BusinessManager: leaving SendPackageV3 with RequestId: {0}", requestId));

            return distributionId;
        }

        private bool ValidateMail(DataTransferObjects.BusinessObjects.Mail mailItem, Guid requestId)
        {
            //Validate MailItem
            var validateMailHelper = new ValidateMailHelper();
            //Validate no script in bodytext
            validateMailHelper.ScriptTag(mailItem.Body.Text, requestId);
            //validate recipient
            return validateMailHelper.Recipient(mailItem.Recipient.To, requestId, ConfigHelper.RegexValidationPersonNo, ConfigHelper.RegexValidationOrganizationNo);
        }

        public List<PackageDeliveryResult> GetPackageResult(int maxStatusMessages, string pnrOrgNr, string sender, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.BusinessLayer.BusinessManager: incoming GetPackageResult with RequestId: {0}", requestId));

            var sqlManager = new SqlManager();
            var result = sqlManager.GetPackageDeliveryResult(maxStatusMessages, pnrOrgNr, sender, requestId);

            if (result != null) 
            {
                var listOfPackageDeliveryResult = new List<PackageDeliveryResult>();
                foreach (var pResult in result)
                {
                    var packageDeliveryResult = new PackageDeliveryResult()
                    {
                        CreatedDate = pResult.CreatedDate,
                        DeliveryDate = pResult.DeliveryDate,
                        DistributionId = pResult.DistributionId,
                        Status = ConvertPackageStatusToPackageDeliveryStatus(pResult.PackageStatus),
                        Recipient = pResult.Recipient
                    };

                    listOfPackageDeliveryResult.Add(packageDeliveryResult);
                }
                

                LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.BusinessLayer.BusinessManager: leaving GetPackageResult with RequestId: {0}", requestId));
                return listOfPackageDeliveryResult;
            }

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.BusinessLayer.BusinessManager: leaving GetPackageResult with a null for RequestId: {0}", requestId));
            return null;
        }

        private PackageDeliveryStatus ConvertPackageStatusToPackageDeliveryStatus(PackageStatus status)
        {
            switch (status)
            {
                case PackageStatus.Delivered:
                    return PackageDeliveryStatus.Delivered;
                case PackageStatus.Failed:
                    return PackageDeliveryStatus.Failed;
                case PackageStatus.Pending:
                    return PackageDeliveryStatus.Pending;
                default:
                    return PackageDeliveryStatus.Unknown;
            }
        }

        /// <summary>
        /// Converts a Mail to a SecureDelivery.
        /// </summary>
        private SecureDelivery1 GetSecureDeliveryV3(DataTransferObjects.BusinessObjects.Mail mailItem, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.BusinessLayer: incoming SecureDeliveryHandler GetSecureDelivery with RequestId: {0}", requestId));

            var secureDeliveryHandler = new SecureDeliveryHandler();
            var secureDelivery = new SecureDelivery1();

            secureDelivery.Header = secureDeliveryHandler.GetSecureDeliveryHeader(mailItem.Reference, mailItem.CorrealationId,
                mailItem.Recipient.To,
                //ConfigHelper.SenderOrganizationNumber, 
                mailItem.SenderOrgNumber, 
                //ConfigHelper.SenderOrganizationName, 
                mailItem.SenderOrgName,
                requestId);

            secureDelivery.Message = getSecureDeliveryMessageV3(mailItem, requestId);

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.BusinessLayer: leaving SecureDeliveryHandler GetSecureDelivery with RequestId: {0}", requestId));
            return secureDelivery;
        }

        /// <summary>
        /// Creates a secure message, can only contain one message!
        /// </summary>
        private SecureMessage1[] getSecureDeliveryMessageV3(Package.DataTransferObjects.BusinessObjects.Mail mailItem, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.BusinessLayer: incoming getSecureDeliveryMessage with RequestId: {0}", requestId));

            var secureDeliveryHandler = new SecureDeliveryHandler();
            var secureMessageArray = new SecureMessage1[1];
            var secureMessage = new SecureMessage1();

            if (mailItem.Attachments != null && mailItem.Attachments.Count > 0)
            {
                var attachments = new List<Attachment>();

                foreach (DataTransferObjects.BusinessObjects.Attachment att in mailItem.Attachments)
                {
                    attachments.Add(secureDeliveryHandler.GetAttachment(att.Body, att.ContentType, att.Filename, requestId));
                }
                secureMessage.Attachment = attachments.ToArray();
            }

            secureMessage.Body = secureDeliveryHandler.GetMessageBody(mailItem.Body.Text, mailItem.Body.ContentType, requestId);
            //secureMessage.Header = secureDeliveryHandler.GetMessageHeader(mailItem.MessageHeaderId, mailItem.Subject, ConfigHelper.SupportInfoEmailAddress, ConfigHelper.SupportInfoPhoneNumber,
            //    ConfigHelper.SupportInfoText, ConfigHelper.SupportInfoUri, ConfigHelper.Language, requestId); ==MG==
            secureMessage.Header = secureDeliveryHandler.GetMessageHeader(mailItem.MessageHeaderId, mailItem.Subject, 
                //ConfigHelper.SupportInfoEmailAddress, 
                mailItem.SupInfoEmailAddress,
                //ConfigHelper.SupportInfoPhoneNumber,
                mailItem.SupInfoPhoneNumber,
                //ConfigHelper.SupportInfoText, 
                mailItem.SupInfoText,
                //ConfigHelper.SupportInfoUri, 
                mailItem.SupInfoUrI,
                ConfigHelper.Language, requestId);

            secureMessageArray[0] = secureMessage;

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.BusinessLayer: leaving getSecureDeliveryMessage with RequestId: {0}", requestId));
            return secureMessageArray;
        }
    }
}
