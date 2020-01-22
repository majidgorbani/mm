using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE.GOV.MM.Integration.Delivery.DataLayer.BusinessObjects
{
    /// <summary>
    /// If the recipient is a Person or Organization
    /// </summary>
    public enum RecipientType
    {
        Private = 1,
        Organization = 2
    }
}
