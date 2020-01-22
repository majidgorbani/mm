using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalEmployment.Lekeberg

{
    public class IsReachableResponse : ResponseBase
    {
        public bool IsReachable { get; set; }
        public ServiceSupplier ServiceSupplier { get; set; }
    }
}