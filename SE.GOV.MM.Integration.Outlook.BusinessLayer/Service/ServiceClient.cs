using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SE.GOV.MM.Integration.Outlook.BusinessLayer.Service
{
    public class ServiceClient<T> : ClientBase<T> where T : class
    {
        public ServiceClient()
            : base(typeof(T).FullName)
        {
        }
        public ServiceClient(string endpointConfigurationName)
            : base(endpointConfigurationName)
        {
        }
        public T Proxy
        {
            get { return this.Channel; }
        }
    }
}
