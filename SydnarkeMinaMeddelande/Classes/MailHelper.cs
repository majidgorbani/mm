using SE.GOV.MM.Integration.Outlook.BusinessLayer.BusinessObject;
using SE.GOV.MM.Integration.Outlook.BusinessLayer.Helper;
using SE.GOV.MM.Integration.Package.DataTransferObjects.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinaMeddelanden.Sydnarke
{
    public class MailHelper
    {

        /// <summary>
        /// Convert outlook Mailitem to object specified in Message service.
        /// </summary>
        public Mail CreateMailItem(string sender, string recipient, string subject, string bodyText, string reference, 
            string supInfoEmailAddress, 
            string supInfoPhoneNumber, string supInfoText, string supInfoUrI, string senderOrgNumber,string senderOrgName, string certificationBySubjectName)
        {
            Mail mailItem = new Mail()
            {
                CorrealationId = string.Empty,
                MessageHeaderId = "1",
                Reference = reference,
                Sender = sender,
                Subject = subject,

                SupInfoEmailAddress = supInfoEmailAddress,
                SupInfoPhoneNumber = supInfoPhoneNumber,
                SupInfoText = supInfoText,
                SupInfoUrI = supInfoUrI,
                SenderOrgName = senderOrgName,
                SenderOrgNumber = senderOrgNumber,
                CertificationBySubjectName = certificationBySubjectName

                
            };

            mailItem.Attachments = new List<SE.GOV.MM.Integration.Package.DataTransferObjects.BusinessObjects.Attachment>();

            mailItem.Recipient = new Recipient()
            {
                To = recipient
            };

            // Set body
            var body = new Body();
            body.Text = bodyText;
            body.ContentType = "text/plain";
            mailItem.Body = body;

            return mailItem;
        }

        public void Validate(string recipient, string subject)
        {
            var rHelper = new RecipientHelper();
            if (!rHelper.ValidRecipient(recipient))
            {
                throw new ArgumentException(ResourceHelper.Resources[3].Text);
            }

            if (string.IsNullOrEmpty(subject))
            {
                throw new ArgumentException(ResourceHelper.Resources[5].Text);
            }

        }

        public MailBox GetMailBoxFromIsReachableResult(IsReachableResult isReachableResult)
        {
            return new MailBox()
            {
                Id = isReachableResult.Id,
                Name = isReachableResult.Name,
                ServiceAdress = isReachableResult.ServiceAdress,
                UIAdress = isReachableResult.UIAdress
            };
        }
    }
}
