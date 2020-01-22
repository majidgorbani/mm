using SE.GOV.MM.Integration.Outlook.BusinessLayer.Helper;
using SE.GOV.MM.Integration.Outlook.BusinessLayer.Service;
using SE.GOV.MM.Integration.Outlook.Helper;
using SE.GOV.MM.Integration.Outlook.BusinessLayer.BusinessObject;
using System.Windows.Forms;
using SE.GOV.MM.Integration.Package.DataTransferObjects.BusinessObjects;

namespace SE.GOV.MM.Integration.Outlook
{
    public class MessageSender
    {      
        private string Sender = string.Empty;

        public MessageSender(string sender)
        {
            Sender = sender;
        }

        /// <summary>
        /// Sends mail to Mina meddeladen
        /// </summary>
        /// <param name="Cancel"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public SendPackageResult SendMailToPackage(Mail mail)
        {
            SendPackageResult sentMessageResult = new SendPackageResult();
            
            //Check if reachable
            var result = IsReachable(mail.Recipient.To);

            if (result != null)
            {
                //Send mail to Mina meddelanden
                sentMessageResult = sendMailToMinaMeddelanden(mail, result);
            }

            return sentMessageResult;
        }

        /// <summary>
        /// Lookup in FaR if recipient isReachable for messages from Sender.
        /// </summary>
        /// <param name="recipient"></param>
        /// <returns></returns>
        private IsReachableResult IsReachable(string recipient)
        {
            var farhelper = new FaRServiceHelper();
            return farhelper.IsReachableResult(recipient);
        }

        /// <summary>
        /// Send mail to mina meddelanden, through webservice. 
        /// </summary>
        private SendPackageResult sendMailToMinaMeddelanden(Mail mail ,IsReachableResult isReachableResult)
        {
            var recipientHelper = new RecipientHelper();
            mail.Recipient.To = recipientHelper.GetRecipientAdress(mail.Recipient.To);
            var result = sendMessage(mail, isReachableResult);
            return result;
        }

        /// <summary>
        /// Sends mail to Mina meddelanden, uses a configuration property in ConfigurationService if mail should go through a dispatcher or directly to the mailbox.
        /// </summary>
        /// <param name="mailHelper"></param>
        /// <param name="isReachableResult"></param>
        /// <returns></returns>
        private SendPackageResult sendMessage(Mail mailItem, IsReachableResult isReachableResult)
        {
            var mailHelper = new MailHelper();
            var mailBox = mailHelper.GetMailBoxFromIsReachableResult(isReachableResult);
            var messageService = new PackageService();

            SendPackageResult result = null;

            if (ConfigHelper.ConfigurationEntity.UseExternalDispatcher)
            {
                result = messageService.SendPackage(mailItem, mailBox);
            }
            else
            {
                result = messageService.SendPackageToMailBox(mailItem, mailBox);
            }
            return result;
        }  
    }
}
