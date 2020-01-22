﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SE.GOV.MM.Integration.DeliveryMailbox.DataTransferObject.BusinessObject
{
    [DataContract(Namespace = "https://SE.GOV.MM.Integration.DeliveryMailbox/2015/04")]
    public class Recipient
    {
        [DataMember]
        public string To { get; set; }
    }
}
