using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;

namespace DigitalEmployment.Lekeberg
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