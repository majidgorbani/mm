using SE.GOV.MM.Integration.FaR.DataTransferObjects.BusinessObject;
using SE.GOV.MM.Integration.FaR.DataTransferObjects.Request;
using SE.GOV.MM.Integration.Outlook.BusinessLayer.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Xml.Linq;
using System.Linq;
using SE.GOV.MM.Integration.Log;
using System.Threading.Tasks;
using SE.GOV.MM.Integration.FaR.Contract;

namespace SE.GOV.MM.Integration.Outlook.BusinessLayer.Service
{
    public class ConfigurationService
    {
        /// <summary>
        /// Call ConfigurationService and get a ConfigurationEntity object and set ConfigHelper.ConfigurationEntity.
        /// </summary>
        /// <returns></returns>
        public void GetConfigurationEntity()
        {
         //   LogManager.LogTrace("SE.GOV.MM.Integration.Outlook.Service.ConfigurationService: incoming GetConfigurationEntity");
            var request = new ConfigurationEntityRequest();
            ServiceClient<IConfiguration> client = null;
            try
            {
                client = new ServiceClient<IConfiguration>("WSHttpBinding_IConfiguration");
                var response = client.Proxy.ConfigurationEntity(request);
                var result = response;

                ConfigHelper.ConfigurationEntity = result.ConfigurationEntity;
            }
            catch (CommunicationException ce)
            {
              //  LogManager.Log(new Log.Log(){ Exception = ce, Message = "SE.GOV.MM.Integration.Outlook.Service.ConfigurationService: CommunicationException error getting ConfigurationEntity from ConfigurationService.", EventId = EventId.CommunicationExceptionWithFaR, Level = Level.Error });
            }
            catch (Exception ex)
            {
               // LogManager.Log(new Log.Log(){ Exception = ex, Message = "SE.GOV.MM.Integration.Outlook.Service.ConfigurationService: Exception error getting ConfigurationEntity from ConfigurationService.", EventId = EventId.CommunicationExceptionWithFaR, Level = Level.Error });
            }
            finally
            {
                if (client.State == CommunicationState.Faulted)
                {
                    client.Abort();
                }

                client.Close();
                client = null;
            }
           // LogManager.LogTrace("SE.GOV.MM.Integration.Outlook.Service.ConfigurationService: leaving GetConfigurationEntity");
        }
    }
}
