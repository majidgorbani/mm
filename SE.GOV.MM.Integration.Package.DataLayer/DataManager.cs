using SE.GOV.MM.Integration.Log;
using SE.GOV.MM.Integration.Package.DataLayer.Service;
using System;
using System.Collections.Generic;
using SE.GOV.MM.Integration.Package.DataTransferObjects.BusinessObjects;
using SE.GOV.MM.Integration.Package.DataLayer.DeliveryMailbox;

namespace SE.GOV.MM.Integration.Package.DataLayer
{
    public class DataManager
    {
        /// <summary>
        /// Send a message.
        /// </summary>
        public string SendPackageV3(SignedDelivery1 signedDelivery, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.DataLayer.DataManager: incoming SendPackageV3 with RequestId: {0}", requestId));

            var service = new MessageService();
            var id = service.DistributeSecureV3(signedDelivery, requestId);

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.DataLayer.DataManager: leaving SendPackageV3 with RequestId: {0}", requestId));
            return id;
        }

        /// <summary>
        /// Sends a mail directly to the mailboxoperator.
        /// </summary>
        /// <param name="mailItem"></param>
        /// <param name="mailBox"></param>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public SendPackageToMailboxResponse SendPackageToMailBox(DataTransferObjects.BusinessObjects.Mail mailItem, MailBox mailBox, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.DataLayer.DataManager: incoming SendPackageToMailBox with RequestId: {0}", requestId));

            var sendMessageToMailboxRequest = new SendPackageToMailboxRequest()
            {
                Mail = ConvertPackageMailItemToDeliveryMailItem(mailItem),
                Mailbox = ConvertPackageMailboxToDeliveryMailbox(mailBox)
            };

            var service = new DeliveryMailboxService();
            var result = service.SendPackageToMailBox(sendMessageToMailboxRequest, requestId);

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.DataLayer.DataManager: leaving SendPackageToMailBox with RequestId: {0}", requestId));
            return result;
        }

        private DeliveryMailbox.Mail ConvertPackageMailItemToDeliveryMailItem(DataTransferObjects.BusinessObjects.Mail mailItem)
        {
            var mail = new DeliveryMailbox.Mail()
            {
                Attachments = CreateDeliveryAttachment(mailItem.Attachments),
                Body = new DeliveryMailbox.Body() { ContentType = mailItem.Body.ContentType, Text = mailItem.Body.Text },
                CorrealationId = mailItem.CorrealationId,
                MessageHeaderId = mailItem.MessageHeaderId,
                MetaData = CreateDeliveryMetaData(mailItem.MetaData),
                Recipient = new DeliveryMailbox.Recipient() { To = mailItem.Recipient.To },
                Reference = mailItem.Reference,
                Sender = mailItem.Sender,
                Subject = mailItem.Subject, 
                SenderOrgNumber = mailItem.SenderOrgNumber,
                SenderOrgName = mailItem.SenderOrgName,
                SupInfoEmailAddress= mailItem.SupInfoEmailAddress,
                SupInfoPhoneNumber = mailItem.SupInfoPhoneNumber,
                SupInfoText = mailItem.SupInfoText,
                SupInfoUrI = mailItem.SupInfoUrI,
                CertificationBySubjectName =mailItem.CertificationBySubjectName
                




            };

            return mail;
        }

        private DeliveryMailbox.MetaData[] CreateDeliveryMetaData(List<DataTransferObjects.BusinessObjects.MetaData> list)
        {
            if (list != null)
            {
                var DeliveryMetaDataList = new List<DeliveryMailbox.MetaData>();
                DeliveryMailbox.MetaData metaData;

                foreach (DataTransferObjects.BusinessObjects.MetaData metadata in list)
                {
                    metaData = new DeliveryMailbox.MetaData();
                    metaData.Key = metadata.Key;
                    metaData.Value = metadata.Value;

                    DeliveryMetaDataList.Add(metaData);
                }
                return DeliveryMetaDataList.ToArray();
            }
            return new DeliveryMailbox.MetaData[1];
        }

        private DeliveryMailbox.Attachment[] CreateDeliveryAttachment(List<DataTransferObjects.BusinessObjects.Attachment> list)
        {
            if (list != null)
            {
                var DeliveryAttachmentList = new List<DeliveryMailbox.Attachment>();
                DeliveryMailbox.Attachment attachment;

                foreach (DataTransferObjects.BusinessObjects.Attachment attach in list)
                {
                    attachment = new DeliveryMailbox.Attachment();
                    attachment.Body = attach.Body;
                    attachment.ContentType = attach.ContentType;
                    attachment.Filename = attach.Filename;

                    DeliveryAttachmentList.Add(attachment);
                }
                return DeliveryAttachmentList.ToArray();
            }
            return new DeliveryMailbox.Attachment[1];
        }

        private Mailbox ConvertPackageMailboxToDeliveryMailbox(MailBox box) => new Mailbox()
        {
            Id = box.Id,
            Name = box.Name,
            ServiceAdress = box.ServiceAdress,
            UIAdress = box.UIAdress
        };
    }
}
