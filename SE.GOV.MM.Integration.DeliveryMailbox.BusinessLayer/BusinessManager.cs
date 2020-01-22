using SE.GOV.MM.Integration.DeliveryMailbox.BusinessLayer.Helper;
using SE.GOV.MM.Integration.DeliveryMailbox.DataLayer;
using SE.GOV.MM.Integration.DeliveryMailbox.DataTransferObject.BusinessObject;
using SE.GOV.MM.Integration.DeliveryMailbox.DataTransferObject.Response;
using SE.GOV.MM.Integration.DeliveryMailbox.Signing.Handler;
using SE.GOV.MM.Integration.DeliveryMailbox.Signing.Helper;
using SE.GOV.MM.Integration.Log;
using System;
using System.Collections.Generic;

namespace SE.GOV.MM.Integration.DeliveryMailbox.BusinessLayer
{
    public class BusinessManager
    {

        /// <summary>
        /// Validates incoming Mail object. 
        /// Creates objects known to mailboxsupplier and signs the xml representation.
        /// Deliver to the recipient mailboxsupplier.  
        /// </summary>
        /// <param name="mailItem"></param>
        /// <param name="box"></param>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public SendPackageToMailboxResponse SendPackageToMailbox(Mail mailItem, Mailbox box, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.BusinessLayer.BusinessManager: incoming SendPackageToMailbox with RequestId: {0}", requestId));

            SendPackageToMailboxResponse result = null;
            if (validateMail(mailItem, requestId)) 
            { 
                result = SendPackageToMailboxV3(mailItem, box, requestId);
            }
            else
            {
                var failureMessage = string.Format("SE.GOV.MM.Integration.DeliveryMailbox.BusinessLayer.BusinessManager: Couldnt send message to: {0}. Didnt validate. RequestId: {1}", mailItem.Recipient.To, requestId);
                LogManager.Log(new Log.Log() { EventId = EventId.Warning, Message = failureMessage, Level = Level.Warning });
            }
            
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.BusinessLayer.BusinessManager: leaving SendPackageToMailbox with RequestId: {0}", requestId));
            return result;
        }

        private SendPackageToMailboxResponse SendPackageToMailboxV3(Mail mailItem, Mailbox box, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.BusinessLayer.BusinessManager: incoming SendPackageToMailboxV3 with RequestId: {0}", requestId));

            SendPackageToMailboxResponse result = null;
            if (validateMail(mailItem, requestId))
            {
                var secureDeliveryV3 = getSecureDeliveryV3(mailItem, requestId);

                //Create SignedDelivery and sign xmldocument.
                var signedDeliveryHandler = new SignedDeliveryHandler();
                var signedDeliveryV3 = signedDeliveryHandler.GetSignedDeliveryV3(secureDeliveryV3, ConfigHelper.SignDelivery, ConfigHelper.DefaultNamespaceV3, ConfigHelper.SigningCertificateSubjectName, requestId);
                //var signedDeliveryV3 = signedDeliveryHandler.GetSignedDeliveryV3(secureDeliveryV3, ConfigHelper.SignDelivery, ConfigHelper.DefaultNamespaceV3, mailItem.CertificationBySubjectName, requestId);


                //Create SealedDelivery from SignedDelivery
                var sealedDeliveryHandler = new SealedDeliveryHandler();
                var sealedDeliveryV3 = sealedDeliveryHandler.GetSealedDeliveryV3(signedDeliveryV3, ConfigHelper.SignDelivery, ConfigHelper.DefaultNamespaceV3, ConfigHelper.SigningCertificateSubjectName, requestId);
                //var sealedDeliveryV3 = sealedDeliveryHandler.GetSealedDeliveryV3(signedDeliveryV3, ConfigHelper.SignDelivery, ConfigHelper.DefaultNamespaceV3, mailItem.CertificationBySubjectName, requestId);


                //Call Recipients Mailbox operator and deliver message.
                var dataManager = new DataManager();
                //result = dataManager.SendPackageToMailboxV3(sealedDeliveryV3, box, ConfigHelper.SSLCertificate_FindByThumbprint, requestId); 
                result = dataManager.SendPackageToMailboxV3(sealedDeliveryV3, box, mailItem.CertificationBySubjectName, requestId);

            }
            else
            {
                var failureMessage = string.Format("SE.GOV.MM.Integration.DeliveryMailbox.BusinessLayer.BusinessManager: Couldnt send message to: {0}. Didnt validate. RequestId: {1}", mailItem.Recipient.To, requestId);
                LogManager.Log(new Log.Log() { EventId = EventId.Warning, Message = failureMessage, Level = Level.Warning });
            }


            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.BusinessLayer.BusinessManager: leaving SendPackageToMailboxV3 with RequestId: {0}", requestId));
            return result;
        }
        /// <summary>
        /// Validates Mail, not containing any script tag and that the recipient is a valid personnumber or organizationnumber.
        /// </summary>
        /// <param name="mailItem"></param>
        /// <param name="requestId"></param>
        /// <returns></returns>
        private bool validateMail(Mail mailItem, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.BusinessLayer.BusinessManager: incoming validateMail with RequestId: {0}", requestId));
            //Validate MailItem
            var validateMailHelper = new ValidateMailHelper();
            //Validate no script in bodytext
            validateMailHelper.ScriptTag(mailItem.Body.Text, requestId);
            //validate recipient
            var isOk = validateMailHelper.Recipient(mailItem.Recipient.To, requestId, ConfigHelper.RegexValidationPersonNo, ConfigHelper.RegexValidationOrganizationNo);

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.BusinessLayer.BusinessManager: leaving validateMail with RequestId: {0}", requestId));
            return isOk;
        }

        /// <summary>
        /// Converts a Mail to a SecureDelivery.
        /// </summary>
        private SecureDelivery2 getSecureDeliveryV3(Mail mailItem, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.BusinessLayer.BusinessManager: incoming getSecureDeliveryV3 with RequestId: {0}", requestId));

            var secureDeliveryHandler = new SecureDeliveryHandler();
            var secureDelivery = new SecureDelivery2();

            secureDelivery.Header = secureDeliveryHandler.getSecureDeliveryHeaderV2(
                mailItem.Reference, mailItem.CorrealationId, mailItem.Recipient.To,
                                    //ConfigHelper.SenderOrganizationNumber, 
                                    mailItem.SenderOrgNumber, 
                                    //ConfigHelper.SenderOrganizationName
                                    mailItem.SenderOrgName, 
                                    requestId);

            secureDelivery.Message = getSecureDeliveryMessageV3(mailItem, requestId);

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.BusinessLayer.BusinessManager: leaving getSecureDeliveryV3 with RequestId: {0}", requestId));
            return secureDelivery;
        }

        /// <summary>
        /// Creates a secure message, can only contain one message!
        /// </summary>
        private SecureMessage1[] getSecureDeliveryMessageV3(Mail mailItem, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.BusinessLayer.BusinessManager: incoming getSecureDeliveryMessageV3 with RequestId: {0}", requestId));

            var secureDeliveryHandler = new SecureDeliveryHandler();
            var secureMessageArray = new SecureMessage1[1];
            var secureMessage = new SecureMessage1();

            if (mailItem.Attachments != null && mailItem.Attachments.Count > 0)
            {
                var attachments = new List<Attachment>();
                Attachment attachment;

                foreach (var a in mailItem.Attachments)
                {
                    attachment = secureDeliveryHandler.getAttachment(a.Body, a.ContentType, a.Filename, requestId);
                    attachments.Add(attachment);
                }
                secureMessage.Attachment = attachments.ToArray();
            }

            secureMessage.Body = secureDeliveryHandler.getMessageBody(mailItem.Body.Text, mailItem.Body.ContentType, requestId);
            secureMessage.Header = secureDeliveryHandler.getMessageHeaderV3(mailItem.MessageHeaderId, mailItem.Subject, 
                //ConfigHelper.SupportInfoEmailAddress, 
                mailItem.SupInfoEmailAddress,
                //ConfigHelper.SupportInfoPhoneNumber,
                mailItem.SupInfoPhoneNumber,
                //ConfigHelper.SupportInfoText, 
                mailItem.SupInfoText,
                //ConfigHelper.SupportInfoUri, 
                mailItem.SupInfoUrI,
                ConfigHelper.Language, 
                requestId);

            secureMessageArray[0] = secureMessage;

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.BusinessLayer.BusinessManager: leaving getSecureDeliveryMessageV3 with RequestId: {0}", requestId));
            return secureMessageArray;
        }
    }
}
