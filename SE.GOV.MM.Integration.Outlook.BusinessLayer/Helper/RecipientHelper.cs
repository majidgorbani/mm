using System;
using System.Text.RegularExpressions;
using System.Configuration;

namespace SE.GOV.MM.Integration.Outlook.BusinessLayer.Helper
{
    public class RecipientHelper
    {
        /// <summary>
        /// Check if recipient contains any invalid characters, like -,+.
        /// </summary>
        private string RemoveCharInRecipient(string recipient)
        {
            var invalidChars = new string[] { "-", "+" };
            foreach (var invalidChar in invalidChars)
            {
                recipient = recipient.Replace(invalidChar, string.Empty);
            }

            return recipient;
        }

        /// <summary>
        /// Checks recipient to not contain any invalid characters
        /// </summary>
        public string GetRecipientAdress(string recipient)
        {
            recipient = RemoveCharInRecipient(recipient);
            if (IsOrganization(recipient) && recipient.Length < 12)
            {
                return AddOrganizationNumber(recipient);
            }
            return RemoveCharInRecipient(recipient);
        }

        /// <summary>
        /// valid personNo or OrgNo
        /// </summary>
        public bool ValidRecipient(string recipient)
        {
            var validRecipient = false;
            recipient = RemoveCharInRecipient(recipient);
            validRecipient = ValidateRecipientPersonNo(recipient);

            if (validRecipient)
            {
                return true;
            }

            validRecipient = ValidateRecipientOrganizationNo(recipient);

            if (validRecipient)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Validate valid PersonNo
        /// </summary>
        private bool ValidateRecipientPersonNo(string recipient)
        {
            if (IsValidCheckNumber(recipient))
            {
                
                return  new Regex(ConfigurationManager.AppSettings["RegexValidationPersonNo"]).IsMatch(recipient); /*TODO: Fix it*/
               // return  new Regex(ConfigHelper.ConfigurationEntity.RegexValidationPersonNo).IsMatch(recipient); //TODO: Fix it
            }
            return false;
        }

        /// <summary>
        /// Validate valid OrganizationNo
        /// </summary>
        private bool ValidateRecipientOrganizationNo(string recipient)
        {
            recipient = RemoveCharInRecipient(recipient);
            if (IsValidCheckNumber(recipient))
            {
                if (IsOrganization(recipient) && recipient.Length < 12)
                {
                    recipient = AddOrganizationNumber(recipient);
                }

                return new Regex(@"(16[2-9]\d{9})").IsMatch(recipient);
            }
            return false;
        }

        /// <summary>
        /// Add 16 to a organization number.
        /// https://sv.wikipedia.org/wiki/Organisationsnummer
        /// </summary>
        private string AddOrganizationNumber(string recipient)
        {
            return string.Format("16{0}", recipient);
        }

        /// <summary>
        /// Check last digit for a recipient
        /// </summary>
        /// <param name="recipient"></param>
        /// <returns></returns>
        private bool IsValidCheckNumber(string recipient)
        {
            var pnrOrgNr = RemoveCharInRecipient(recipient);

            if (pnrOrgNr.Length == 12)
            {
                pnrOrgNr = pnrOrgNr.Substring(2, 10);
            }

            var check = int.Parse(pnrOrgNr.Substring(9, 1));
            var sValue = pnrOrgNr.Substring(0, 9);

            var result = 0;

            for (int i = 0; i < sValue.Length; i++)
            {
                var tmp = int.Parse(sValue.Substring(i, 1));

                //if i is a uneven number multiply with 1
                //if i is a even number multiply with 2
                if ((i % 2) == 0)
                {
                    tmp = (tmp * 2);
                }

                //if number bigger than 9 add 1 + tmp
                if (tmp > 9)
                {
                    result += (1 + (tmp % 10));
                }
                else //add tmp
                {
                    result += tmp;
                }
            }

            var isValid = (((check + result) % 10) == 0);

            return isValid;   
        }

        /// <summary>
        /// Checks if a recipient is organization, (month) is bigger or equal 20. 
        /// </summary>
        private bool IsOrganization(string recipient)
        {
            if (recipient.Length == 12)
            {
                recipient = recipient.Substring(2, 10);
            }

            var isCompany = int.Parse(recipient.Substring(2, 2)) >= 20;
            return isCompany;
        }

        /// <summary>
        /// Valid characters.
        /// </summary>
        private char[] _validChars = new char[] { '0','1','2','3','4','5','6','7','8','9','-','+' };

        /// <summary>
        /// check invalid characters in recipient.
        /// Throws ArgumentException if any invalid chars.
        /// </summary>
        public bool CheckInvalidChars(string recipient)
        {
            var valid = false;
            var tempRecipient = recipient.Trim();
            foreach (char i in tempRecipient)
            {
                foreach (char y in _validChars)
                {
                    if (i == y)
                    {
                        valid = true;
                    }
                }

                if (!valid)
                {
                    return false;
                }

                valid = false;
            }

            return true;
        }

        /// <summary>
        /// Add century numbers to recipient (19XXXXXX-XXXX, 20XXXXXXXXXX)
        /// </summary>
        /// <param name="recipient"></param>
        /// <returns></returns>
        public string AddCenturyNumbers(string recipient)
        {
            // Checks if the recipient is less than 12 chars (we need a century number)
            // And if the recipient is NOT an organization
            if (recipient.Length < 12 && !(int.Parse(recipient.Substring(2, 2)) >= 20))
            {
                // A + 
                if (recipient.Substring(6, 1) == "+")
                {
                    return "19" + recipient.Remove(6, 1);
                }
                var year = DateTimeOffset.UtcNow.Year;
                var century = int.Parse(year.ToString().Substring(0, 2) + "00");
                var recipientBirthYear = int.Parse(recipient.Substring(0, 2));
                // Try to guess century,
                // this isn't bulletproof, but it works in most cases
                if (century + recipientBirthYear > year)
                {
                    return "19" + recipient;
                }
                else
                {
                    return "20" + recipient;
                }
            }
            return recipient;
        }
    }
}
