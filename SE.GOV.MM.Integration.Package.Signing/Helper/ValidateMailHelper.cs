using SE.GOV.MM.Integration.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace SE.GOV.MM.Integration.Package.Signing.Helper
{

    /// <summary>
    /// Validate incoming parameters for mail item.
    /// </summary>
    public class ValidateMailHelper
    {
        public ValidateMailHelper() { }
      
        /// <summary>
        /// Validate recipient.
        /// </summary>
        public bool Recipient(string recipient, Guid requestId, string regexPersonNo, string regexOrgNo)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.BusinessLayer.Helper: incoming ValidateMailHelper Recipient with RequestId: {0}", requestId));
            
            bool isOk = false;
            var recipientHelper = new RecipientHelper();

            recipient = recipientHelper.GetRecipientAdress(recipient);

            if (recipientHelper.ValidateRecipient(recipient, regexPersonNo, regexOrgNo))
            {
                isOk = true;
            }

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.BusinessLayer.Helper: leaving ValidateMailHelper Recipient with RequestId: {0}", requestId));
            return isOk;
        }

        /// <summary>
        /// Validate that no script is included in body text.
        /// </summary>
        public void ScriptTag(string bodyText, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.BusinessLayer.Helper: incoming ValidateMailHelper ScriptTag with RequestId: {0}", requestId));

            if (bodyText.Contains("script"))
            {
                throw new Exception("Brödtexten är inte giltig, innehåller illegala taggar.");
            }

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.BusinessLayer.Helper: leaving ValidateMailHelper ScriptTag with RequestId: {0}", requestId));
        }

        
    }
}