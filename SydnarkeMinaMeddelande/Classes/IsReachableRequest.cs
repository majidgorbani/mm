using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalEmployment.Lekeberg
{
    public class IsReachableRequest : RequestBase
    {
        public string RecipientNumber { get; set; }
    }
}