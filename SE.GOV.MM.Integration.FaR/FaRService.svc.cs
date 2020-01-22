using System;
using System.ServiceModel;
using SE.GOV.MM.Integration.FaR.Helper;
using SE.GOV.MM.Integration.FaR.DataTransferObjects.Response;
using SE.GOV.MM.Integration.FaR.DataTransferObjects.Request;
using System.Diagnostics;
using SE.GOV.MM.Integration.Log;
using SE.GOV.MM.Integration.FaR.Contract;
using SE.GOV.MM.Integration.FaR.Service;

namespace SE.GOV.MM.Integration.FaR
{
    [ServiceBehavior(Namespace = "https://SE.GOV.MM.Integration/FaR/2015/04")]
    public class FaRService : IFaR
    {
        public FaRService()
        {

            if (!EventLog.SourceExists(ConfigHelper.ApplicationName))
            {
                EventLog.CreateEventSource(ConfigHelper.ApplicationName, "Application");
            }

        }    

        /// <summary>
        /// Validate incoming parameters and call recipient in FaR and check if a person got a mailbox connected.
        /// </summary>
        public IsReachableResponse IsReachable(IsReachableRequest request, string senderOrg)
        {
            //Set a new Id for this request, easier to follow request through logs.
            request.RequestId = Guid.NewGuid();
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.FaR.FaRService: incoming IsReachable with RequestId: {0}", request.RequestId));
            IsReachableResponse isReachableResponse = new IsReachableResponse() { IsReachable = false };

            try
            {
                var recipientHelper = new RecipientHelper();
              
                request.RecipientNumber = recipientHelper.GetRecipientAdress(request.RecipientNumber);               
                var validated = recipientHelper.ValidateRecipient(request.RecipientNumber, request.RequestId);              

                if (validated)
                {
                    var recipientService = new SE.GOV.MM.Integration.FaR.Service.RecipientService();
                    var response = IsUserReachableInFaR(request.RecipientNumber, request.RequestId, senderOrg);

                    if (response != null)
                    {                      
                        var reachabilityStatus = response[0];                                              
                        isReachableResponse = ConvertReachabilityStatusToIsReachableResponse(reachabilityStatus, isReachableResponse, request.RequestId);
                    }
                }               
            }
            catch (ArgumentNullException nse)
            {
                string errorMessage = string.Format("SE.GOV.MM.Integration.FaR.FaRService: ArgumentNullException RequestId: {0} ", request.RequestId);
                LogManager.Log(new Log.Log() { Message = errorMessage, EventId = EventId.ArgumentException, Exception = nse, Level = Level.Error });
                throw nse;
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("SE.GOV.MM.Integration.FaR.FaRService: Something went wrong in FaRService, RequestId: {0} ", request.RequestId);
                LogManager.Log(new Log.Log() { Message = errorMessage, EventId = EventId.GenerelizedException, Exception = ex, Level = Level.Error });
                throw ex;
            }

            return isReachableResponse;
        }

        private ReachabilityStatus[] IsUserReachableInFaR(string recipientNumber, Guid requestId, string senderOrg)
        {
            var recipientService = new RecipientService();

            return recipientService.IsUserReachableInFaRV3(recipientNumber, requestId, senderOrg);
        }

        /// <summary>
        /// Converts and copies a object from Recipient to a format supported in FaRService, ServiceSupplier.
        /// </summary>
        private IsReachableResponse ConvertReachabilityStatusToIsReachableResponse(ReachabilityStatus reachabilityStatus, IsReachableResponse isReachableResponse, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.FaR.FaRService: incoming ConvertReachabilityStatusToIsReachableResponse with RequestId: {0}", requestId));
            //Control if pending and senderaccepted is true, user/company have reactivated their mailbox and all persons havent signed, doesnt accept messages until everyone signs.
            if (reachabilityStatus.AccountStatus.Pending && reachabilityStatus.SenderAccepted)
            {
                isReachableResponse.IsReachable = false;
            }
            else
            {
                isReachableResponse.IsReachable = reachabilityStatus.SenderAccepted;
            }

            if (reachabilityStatus.AccountStatus.ServiceSupplier != null)
            {
                var serviceSupplier = new FaR.DataTransferObjects.BusinessObject.ServiceSupplier()
                {
                    Id = reachabilityStatus.AccountStatus.ServiceSupplier.Id,
                    InternalServiceAdress = reachabilityStatus.AccountStatus.ServiceSupplier.InternalServiceAdress,
                    Name = reachabilityStatus.AccountStatus.ServiceSupplier.Name,
                    ServiceAdress = reachabilityStatus.AccountStatus.ServiceSupplier.ServiceAdress,
                    UIAdress = reachabilityStatus.AccountStatus.ServiceSupplier.UIAdress
                };

                isReachableResponse.ServiceSupplier = serviceSupplier;
            }

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.FaR.FaRService: leaving ConvertReachabilityStatusToIsReachableResponse with RequestId: {0}", requestId));
            return isReachableResponse;
        }
    }
}
