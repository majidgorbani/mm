using SE.GOV.MM.Integration.Outlook.BusinessLayer.BusinessObject;
using SE.GOV.MM.Integration.Outlook.BusinessLayer.Helper;
using SE.GOV.MM.Integration.Outlook.BusinessLayer.Service;
using System.Collections.Generic;

namespace SE.GOV.MM.Integration.Outlook
{
    public class MessageDeliveryResult
    {
        public string Sender { get; set; }

        public MessageDeliveryResult(string sender)
        {
            Sender = sender;
        }

        /// <summary>
        /// Requests PackageService for a status of delivered package.
        /// </summary>
        /// <returns></returns>
        public List<DeliveryResult> GetDeliveryStatusResult(string pnrOrgNr)
        {
            var recipientHelper = new RecipientHelper();
            var packageService = new PackageService();
            var deliveryResult = packageService.GetPackageDelivery(Sender, pnrOrgNr, ConfigHelper.ConfigurationEntity.MaxAllowedStatusMessages);
            return deliveryResult;
        }
    }
}
