using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SE.GOV.MM.Integration.Package.Signing.Helper
{
    public class RecipientHelper
    {
        /// <summary>
        /// Check 'To' if it contains any invalid characters, like -.
        /// </summary>
        private string RemoveCharInRecipient(string recipient)
        {
            var invalidChars = new string[] { "-" };

            foreach (var invalidChar in invalidChars)
            {
                recipient = recipient.Replace(invalidChar, string.Empty);
            }

            return recipient;
        }

        /// <summary>
        /// Get To mailadress. Person
        /// </summary>
        public string GetRecipientAdress(string recipient)
        {
            recipient = GetRecipientWithoutDomain(recipient);
            recipient = RemoveCharInRecipient(recipient);
            return recipient;
        }

        private string GetRecipientWithoutDomain(string recipient)
        {
            if (recipient.Contains('@'))
            {
                var splitArray = new char[] { '@' };
                var substringArray = recipient.Split(splitArray);
                recipient = substringArray[0];
            }

            return recipient;
        }

        /// <summary>
        /// Validate recipient.
        /// </summary>
        public bool ValidateRecipient(string recipient, string regexPersonNo, string regexOrgNo) =>
            (validatePersonNo(recipient, regexPersonNo) || validateOrganizationNo(recipient, regexOrgNo)) ? true : false;

        /// <summary>
        /// Validate if a recipient is a personal number.
        /// </summary>
        private bool validatePersonNo(string recipient, string regexPersonNo) => new Regex(regexPersonNo).IsMatch(recipient);


        /// <summary>
        /// Validate if a recipient is a organizational number.
        /// </summary>
        private bool validateOrganizationNo(string recipient, string regexOrgNo) => new Regex(regexOrgNo).IsMatch(recipient);
        
    }
}
