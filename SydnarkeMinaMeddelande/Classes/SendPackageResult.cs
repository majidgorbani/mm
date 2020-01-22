﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitalEmployment.Lekeberg
{
    public class SendPackageResult
    {
        //Id generated by webservice, for follow up.
        public Guid RequestId { get; set; }

        //Status of newly sent Message.
        public string DeliveryStatus { get; set; }

        //If a messsage is sent to webservice.
        public bool MessageSent { get; set; }

        //OrgNo or PersonNo that should recieve a message.
        public string Recipient { get; set; }

        //OrgNo or PersonNo that should recieve a message.
        public string DistributionId { get; set; }
    }
}