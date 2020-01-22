
using SE.GOV.MM.Integration.Outlook.BusinessLayer.Helper;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace MinaMeddelanden.Sydnarke
{
    //public class OutlookMailItemValidator
    //{
    //    private MailItem OutlookMailItem;

    //    public OutlookMailItemValidator(MailItem item)
    //    {
    //        OutlookMailItem = item;
    //    }

    //    /// <summary>
    //    /// Validate email adress, Recipients only contain one adress.
    //    /// Check if the user exists in FaR. Change Mailobjects 'To' too appropriate email adress.
    //    /// </summary>
    //    public bool ValidateMail(ref bool Cancel)
    //    {
    //        if (!string.IsNullOrEmpty(OutlookMailItem.To))
    //        {
    //            if (ConfigHelper.ConfigurationEntity != null)
    //            {
    //                //Only one recipient, should be valid personNo/OrgNo
    //                if (ValidateRecipientInMailItem(ref Cancel))
    //                {
    //                    //Validate Subject
    //                    if (ValidateSubject(ref Cancel))
    //                    {
    //                        //Validate attachments
    //                        if (ValidateAttachments(ref Cancel))
    //                        {
    //                            //Validate sender, authorized to use 'mina meddelanden' service.
    //                            if (ValidateSender(ref Cancel))
    //                            {
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        var ok = !Cancel;
    //        return ok;
    //    }

    //    /// <summary>
    //    /// Validates subject. If empty shows an error message.
    //    /// </summary>
    //    public bool ValidateSubject(ref bool Cancel)
    //    {
    //        if (string.IsNullOrEmpty(OutlookMailItem.Subject))
    //        {
    //           // var result = MessageBox.Show(ResourceHelper.Resources[5].Text, ResourceHelper.Resources[1].Text, MessageBoxButtons.OK, MessageBoxIcon.Error);

    //            Cancel = true;
    //            return false;              
    //        }
    //        return true;
    //    }

    //    /// <summary>
    //    /// Validate Recipient in mailitem, 
    //    /// Only contains one recipient, valid personNo or OrgNo.
    //    /// </summary>
    //    public bool ValidateRecipientInMailItem(ref bool Cancel)
    //    {
    //        if (OutlookMailItem.Recipients.Count > 1)
    //        {
    //          //  MessageBox.Show(ResourceHelper.Resources[4].Text, ResourceHelper.Resources[1].Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
    //            Cancel = true;
    //            return false;
    //        }

    //        var recipientHelper = new RecipientHelper();
    //        if (!recipientHelper.ValidRecipient(OutlookMailItem.To))
    //        {
    //            //MessageBox.Show(ResourceHelper.Resources[3].Text, ResourceHelper.Resources[1].Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
    //            Cancel = true;
    //            return false;
    //        }
    //        return true;
    //    }     

    //    /// <summary>
    //    /// Validation if mail contains images, Mina Meddelanden doesnt support images at this time.
    //    /// </summary>
    //    public bool ValidateImagesInMailItem()
    //    {
    //        var mailHelper = new MailHelper();
    //        if (mailHelper.ContainsImageTag(OutlookMailItem.HTMLBody))
    //        {
    //            return true;
    //        }
    //        return false;
    //    }

    //    /// <summary>
    //    /// Validates attachments and checks if attachment extension isnt allowed.
    //    /// </summary>
    //    public bool ValidateAttachments(ref bool Cancel)
    //    {
    //        try
    //        {
    //            var mailHelper = new MailHelper();
    //            var listOfImageTags = mailHelper.GetImageTagFromOutlookMailBody(OutlookMailItem.HTMLBody);

    //            var attachmentHelper = new AttachmentHelper();
    //            return attachmentHelper.ValidateAttachments(OutlookMailItem.Attachments, listOfImageTags);
    //        }
    //        catch (NotSupportedException nse)
    //        {
    //            MessageBox.Show(nse.Message, ResourceHelper.Resources[1].Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
    //            Cancel = true;
    //            return false;
    //        }
    //        catch (ApplicationException ae)
    //        {
    //            MessageBox.Show(ae.Message, ResourceHelper.Resources[1].Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
    //            Cancel = true;
    //            return false;
    //        }
    //    }

    //    /// <summary>
    //    /// Validate if a user is authorized to send email in 'Mina meddelanden'. Using Windows authentication setup on the webserver.
    //    /// </summary>
    //    public bool ValidateSender(ref bool Cancel)
    //    {
    //        return true;
    //    }
    //}
}
