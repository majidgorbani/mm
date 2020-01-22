using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE.GOV.MM.Integration.Delivery.DataLayer.BusinessObjects
{
    /// <summary>
    /// Object used when reading and updating database.
    /// </summary>
    public class Message
    {
        public int Id { get; set; }
        public string DistributionId { get; set; }
        public string Recipient { get; set; }
        public MessageStatus MessageStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public RecipientType RecipientType { get; set; }
    }
}
