using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE.GOV.MM.Integration.Delivery.DataLayer.BusinessObjects
{
    /// <summary>
    /// Status of Message
    /// </summary>
    public enum MessageStatus
    {
        Delivered = 1,
        Failed = 2,
        Pending = 3
    }
}
