using SE.GOV.MM.Integration.Log;
using System;
using System.Linq;
using System.Text.RegularExpressions;


namespace SE.GOV.MM.Integration.FaR.Helper
{
    public class RecipientHelper
    {
        public bool ValidateRecipient(string recipient, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.FaR.Helper.RecipientHelper: incoming ValidateRecipient for requestId: {0}", requestId));
            var validated = false;

            if (string.IsNullOrEmpty(recipient))
            {
                LogManager.Log(new Log.Log() { Message = string.Format("SE.GOV.MM.Integration.FaR.Helper.RecipientHelper: IsReachable with requestId: {0}, doesnt contain a recipient", requestId), EventId = EventId.Warning, Level = Level.Warning });
                throw new ArgumentNullException(string.Format("SE.GOV.MM.Integration.FaR.Helper.RecipientHelper: IsReachable with requestId: {0}, doesnt contain a recipient", requestId));
            }

            if (validatePersonalNumber(recipient, requestId))
            {
                validated = true;
            }

            if (validated || validateOrganizationNumber(recipient, requestId))
            {
                validated = true;
            }

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.FaR.Helper.RecipientHelper: leaving ValidateRecipient for requestId: {0}", requestId));
            return validated;
        }

        /// <summary>
        /// Example: 194512310015
        /// </summary>
        private bool validatePersonalNumber(string personalNumber, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.FaR.Helper.RecipientHelper: incoming validatePersonalNumber for requestId: {0}", requestId));

            var regex = new Regex(ConfigHelper.RegexValidationPersonNo);

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.FaR.Helper.RecipientHelper: leaving validatePersonalNumber for requestId: {0}", requestId));
            return regex.IsMatch(personalNumber);
        }

        /// <summary>
        /// Example: 162021005448
        /// </summary>
        /// <param name="organizationNumber"></param>
        /// <returns></returns>
        private bool validateOrganizationNumber(string organizationNumber, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.FaR.Helper.RecipientHelper: incoming validateOrganizationNumber for requestId: {0}", requestId));

            var regex = new Regex(ConfigHelper.RegexValidationOrganizationNo);

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.FaR.Helper.RecipientHelper: leaving validateOrganizationNumber for requestId: {0}", requestId));
            return regex.IsMatch(organizationNumber);
        }

        ///// <summary>
        ///// Check 'To' if it contains any invalid characters, like -.
        ///// </summary>
        private string RemoveCharInRecipient(string recipient)
        {
            var invalidChars = new string[] { "-" };

            foreach (var invalidChar in invalidChars)
            {
                recipient = recipient.Replace(invalidChar, string.Empty);
            }

            return recipient;
        }

        ///// <summary>
        ///// Get To mailadress. Person
        ///// </summary>
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
    }
}