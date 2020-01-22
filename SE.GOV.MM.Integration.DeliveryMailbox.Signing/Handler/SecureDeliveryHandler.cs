using System;
using SE.GOV.MM.Integration.Log;
using SE.GOV.MM.Integration.DeliveryMailbox.Signing.Helper;

namespace SE.GOV.MM.Integration.DeliveryMailbox.Signing.Handler
{

    /// <summary>
    /// Handler to create a new SecureDelivery object from mina meddelanden message webservice.
    /// </summary>
    public class SecureDeliveryHandler
    {
        private ConvertHelper _converter;

        public SecureDeliveryHandler() 
        { 
            _converter = new ConvertHelper();
        }

        /// <summary>
        /// MessageHeader for a securemessage from MinaMeddelanden webservice Message, using properties from web.config to set the Supportinfo and language property.
        /// </summary>
        public MessageHeader1 getMessageHeaderV3(string messageHeaderId, string subject, string supportInfoEmailAddress,
            string supportInfoPhoneNumber, string supportInfoText, string supportInfoUri, string language, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.BusinessLayer.Handler: incoming getMessageHeader with RequestId: {0}", requestId));

            var messageHeader = new MessageHeader1()
            {
                Id = messageHeaderId,
                Subject = subject,
                Supportinfo = getSupportInfoV3(supportInfoEmailAddress, supportInfoPhoneNumber, supportInfoText, supportInfoUri),
                Language = language
            };

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.BusinessLayer.Handler: leaving getMessageHeader with RequestId: {0}", requestId));
            return messageHeader;
        }

        /// <summary>
        /// Bodytext for a securemessage from MinaMeddelanden webservice Message. Converts incoming bodytext to Base64 format and byte array, contenttype is text/html. 
        /// </summary>
        public MessageBody getMessageBody(string bodyText, string bodyContentType, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.BusinessLayer.Handler: incoming getMessageBody with RequestId: {0}", requestId));

            var messageBody = new MessageBody();
            messageBody.Body = _converter.EncodeToBase64FromStringToByteArray(bodyText);
            messageBody.ContentType = bodyContentType;

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.BusinessLayer.Handler: leaving getMessageBody with RequestId: {0}", requestId));
            return messageBody;
        }

        /// <summary>
        /// Converts and recreates incoming Attachments to MinaMeddelanden attachments from webservice Message.
        /// </summary>
        public Attachment getAttachment(byte[] body, string contentType, string fileName, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.BusinessLayer.Handler: incoming getAttachments with RequestId: {0}", requestId));

            var attachment = new Attachment();
            attachment.Body = body;
            attachment.Checksum = _converter.GetMD5ChecksumForAttachmentBody(body);
            attachment.ContentType = contentType;
            attachment.Filename = fileName;

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.BusinessLayer.Handler: leaving getAttachments with RequestId: {0}", requestId));
            return attachment;
        }

        public SecureDeliveryHeader getSecureDeliveryHeaderV2(string reference, string correalationId, string recipientNo, string senderOrgNo, string senderName, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.BusinessLayer.Handler: incoming getSecureDeliveryHeaderV2 with RequestId: {0}", requestId));

            var deliveryHeader = new SecureDeliveryHeader()
            {
                Reference = reference,
                CorrelationId = correalationId,
                Recipient = recipientNo,
                Sender = getSender(senderOrgNo, senderName) 
            };

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.BusinessLayer.Handler: leaving getSecureDeliveryHeaderV2 with RequestId: {0}", requestId));
            return deliveryHeader;
        }

        /// <summary>
        /// Get sender from Web.config values.
        /// </summary>
        private Sender getSender(string senderOrganizationNumber, string senderOrganizationName)
        {
            var sender = new Sender()
            {
                Id = senderOrganizationNumber,
                Name = senderOrganizationName
            };

            return sender;
        }


        /// <summary>
        /// Get supportinfo from web.config file.
        /// </summary>
        private SupportInfo1 getSupportInfoV3(string supportInfoEmailAddress, string supportInfoPhoneNumber, string supportInfoText, string supportInfoUri) => 
            new SupportInfo1() {
                EmailAdress = supportInfoEmailAddress,
                PhoneNumber = supportInfoPhoneNumber,
                Text = supportInfoText,
                URL = supportInfoUri
            };

    }
}
