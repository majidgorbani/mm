using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
using System.ServiceModel;
using SE.GOV.MM.Integration.Log;
using SE.GOV.MM.Integration.FaR.Contract;
using SE.GOV.MM.Integration.FaR;


namespace DigitalEmployment.Lekeberg
{
    public class RecipientService
    {
        /// <summary>
        /// Check if a user IsReachable in FaR
        /// </summary>
        public IsReachableResult IsReachable(string recipientNumber)
        {
           // LogManager.LogTrace("SE.GOV.MM.Integration.Outlook.Service.RecipientService: incoming IsReachable");

            var request = new IsReachableRequest() { RecipientNumber = recipientNumber };
            var result = new IsReachableResponse();
            IsReachableResult isReachableResult;
            ServiceClient<IFaR> client = null;
            try
            {
                client = new ServiceClient<IFaR>("WSHttpBinding_IFaR");
                var response = client.Proxy.IsReachable(request);
                result = response;

                isReachableResult = new IsReachableResult()
                {
                    IsReachable = result.IsReachable
                };

                if (result.ServiceSupplier != null)
                {
                    isReachableResult.Id = result.ServiceSupplier.Id;
                    isReachableResult.Name = result.ServiceSupplier.Name;
                    isReachableResult.ServiceAdress = result.ServiceSupplier.ServiceAdress;
                    isReachableResult.UIAdress = result.ServiceSupplier.UIAdress;
                }
            }
            catch (CommunicationException ce)
            {
                //LogManager.Log(new Log.Log() { Exception = ce, Message = "SE.GOV.MM.Integration.Outlook.Service.RecipientService: Communication error, getting a response from FaRService.", EventId = EventId.CommunicationExceptionWithFaR, Level = Level.Error });
                throw ce;
            }
            catch (Exception ex)
            {
                //LogManager.Log(new Log.Log() { Exception = ex, Message = "SE.GOV.MM.Integration.Outlook.Service.RecipientService: Error getting a response from RecipientService.", EventId = EventId.CommunicationExceptionWithFaR, Level = Level.Error });
                throw ex;
            }
            finally
            {
                if (client.State != CommunicationState.Faulted)
                {
                    client.Abort();
                }
                client.Close();
                client = null;
            }

            LogManager.LogTrace("SE.GOV.MM.Integration.Outlook.Service.RecipientService: leaving IsReachable");
            return isReachableResult;
        }
    }
}