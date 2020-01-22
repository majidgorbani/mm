using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SE.GOV.MM.Integration.Log;

using SE.GOV.MM.Integration.Outlook.BusinessLayer.BusinessObject;
using SE.GOV.MM.Integration.Outlook.BusinessLayer.Helper;
using SE.GOV.MM.Integration.Outlook.BusinessLayer.Service;

namespace MinaMeddelanden.Sydnarke
{
    public class FaRServiceHelper
    {

        /// <summary>
        /// returns a object containing ServiceSupplier and if the recipient isReachable.
        /// </summary>
        /// <param name="recipient"></param>
        /// <returns></returns>
        public IsReachableResult IsReachableResult(string recipient, string senderOrg)
        {
            var isReachableResult = new IsReachableResult();
            try
            {
                //Validate recipient
                var recipientHelper = new RecipientHelper();
                var recipientService = new RecipientService();

               // recipient = recipientHelper.GetRecipientAdress(recipient); //MG Not sure if we need this

                if (recipientHelper.ValidRecipient(recipient))
                {
                    isReachableResult = recipientService.IsReachable(recipient,  senderOrg);
                }
            }
            catch (System.Exception e)
            {
               // LogManager.Log(new Log.Log() { EventId = EventId.GenerelizedException, Exception = e, Level = Level.Error, Message = "Something went wrong checking recipient against FaRService." });
                return null;
            }
            return isReachableResult;
        }


        /// <summary>
        /// Dialog with error information if something goes wrong calling FaRService to check if a recipient IsReachable.
        /// Contains message configured in Resource.xml, Id = 19. 
        /// </summary>
        /// <param name="recipient"></param>
        public void ExceptionWhenCheckingReachableInFaRDialog(string recipient)
        {
            var errorMessage = ResourceHelper.Resources[17].Text;
            var recipientHelper = new RecipientHelper();
            if (errorMessage.Contains("[placeholder]"))
                errorMessage = errorMessage.Replace("[placeholder]", recipientHelper.GetRecipientAdress(recipient));

           // MessageBox.Show(errorMessage, ResourceHelper.Resources[1].Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public string ReachableInFaRText(string recipient)
        {
            var reachableInFaRText = ResourceHelper.Resources[12].Text;
            return reachableInFaRText;
        }

        /// <summary>
        /// Text shown when recipient is not reachable
        /// </summary>
        /// <returns></returns>
        public string NotReachableInFaRText(string recipient)
        {
            var notReachableInFaRDialog = ResourceHelper.Resources[13].Text;
            return notReachableInFaRDialog;
        }


        /// <summary>
        /// Use internal webservice to check if recipient exists and isreachable in FaR.
        /// </summary>
        public bool IsReachable(string recipient, string senderOrg)
        {
            var recipientHelper = new RecipientHelper();
            var recipientNumber = recipientHelper.GetRecipientAdress(recipient);

            // Call FaR, ask if recipient is reachable
            var service = new RecipientService();
            var isReachableResult = service.IsReachable(recipientNumber, senderOrg);

            return isReachableResult.IsReachable;
        }
    }
}