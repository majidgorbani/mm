using BO=SE.GOV.MM.Integration.Package.DataTransferObjects.BusinessObjects;
using SE.GOV.MM.Integration.FaR.DataTransferObjects.BusinessObject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SE.GOV.MM.Integration.Outlook.BusinessLayer.Helper;
using System.Configuration;

namespace MinaMeddelanden.Sydnarke
{
    public class AttachmentHelper
    {
        public AttachmentHelper() { }

        /// <summary>
        /// Get filter for showfiledialog in gui.
        /// "Mina meddelanden |*.pdf;*.doc;*.docx;*.ics";
        /// </summary>
        public string GetAttachmentFilter()
        {
            var sBuilder = new StringBuilder("Mina meddelanden |");

            for (int i = 0; i < ConfigHelper.ConfigurationEntity.AttachmentExtensions.Count; i++)
            {
                sBuilder.Append(string.Format("*{0};", ConfigHelper.ConfigurationEntity.AttachmentExtensions[i].Extension));
            }

            return sBuilder.ToString();
        }

        /// <summary>
        /// Creates a attachment object.
        /// </summary>
        public BO.Attachment GetAttachment(FileInfo attachment)
        {
            return getAttachment(attachment);
        }

        /// <summary>
        /// Checks if Attachments are allowed to be sent 
        /// </summary>
        public bool ValidateAttachment(List<BO.Attachment> attachments)
        {
            var maxSize = Convert.ToInt32(ConfigurationManager.AppSettings["MaxTotalAttachmentSize"]); // ConfigHelper.ConfigurationEntity.MaxTotalAttachmentSize;
            var totalSize = 0;

            foreach (BO.Attachment attachment in attachments)
            {
                totalSize += attachment.Size;
                if (totalSize > maxSize)
                {
                    throw new ArgumentException(ResourceHelper.Resources[26].Text);
                }

               // if (!checkAttachmentExtension(attachment.Filename, ConfigHelper.ConfigurationEntity.AttachmentExtensions))
               //// if (!checkAttachmentExtension(attachment.Filename,))
               // {
               //     throw new ArgumentException(ResourceHelper.Resources[25].Text);
               // }
            }

            return true;
        }

        /// <summary>
        /// Check if file extension is valid as an attachment.
        /// </summary>
        private bool checkAttachmentExtension(string filename, List<AttachmentExtension> allowed)
        {
            //Create substring of extension.
            string extension = getAttachmentExtension(filename);
            foreach (AttachmentExtension attachmentExtension in allowed)
            {
                if (attachmentExtension.Extension == extension)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets a attachments file extension
        /// </summary>
        private string getAttachmentExtension(string fileName)
        {
            string returnValue = string.Empty;
            char[] splitter = new char[] { '.' };
            string[] splittedFilename = fileName.Split(splitter);
            string extension = string.Format(".{0}", splittedFilename[splittedFilename.Length - 1]);

            /*  

              ".pdf"  ContentType="application/pdf" Filter="pdf files (*.pdf)|*.pdf" />
              ".doc"  ContentType="application/msword" Filter="doc files (*.doc)|*.doc"/>
              ".docx" ContentType="application/vnd.openxmlformats-officedocument.wordprocessingml.document" Filter="docx files (*.docx)|*.docx"/>
              ".dotx" ContentType="application/vnd.openxmlformats-officedocument.wordprocessingml.template" Filter="dotx files (*.dotx)|*.dotx"/>
              ".ics"  ContentType="text/calendar" Filter="ics files (*.ics)|*.ics"/>

             * 
             * */

            if (extension == ".pdf")
            {
                returnValue = "application/pdf";
            }
            else if(extension == ".doc")
            {
                returnValue = "application/msword";
            }
            //".docx" ContentType="application/vnd.openxmlformats-officedocument.wordprocessingml.document" 
            else if (extension == ".docx")
            {
                returnValue = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            }
            //".dotx" ContentType="application/vnd.openxmlformats-officedocument.wordprocessingml.template"

            else if (extension == ".dotx")
            {
                returnValue = "application/vnd.openxmlformats-officedocument.wordprocessingml.template";
            }
            //".ics"  ContentType="text/calendar" Filter="ics files (*.ics)|*.ics"/>
            else if (extension == ".ics")
            {
                returnValue = "text/calendar";
            }


            return returnValue;
        }

        /// <summary>
        /// Create a new Attachment
        /// </summary>
        private BO.Attachment getAttachment(FileInfo info)
        {
            if (info.Exists)
            {
                var attachment = new BO.Attachment()
                {
                    Filename = info.Name,
                    ContentType = getAttachmentExtension(info.Name), // getContentType(info),
                    Body = getFileAsBase64ByteArray(info),
                    Size = (int)info.Length
                };
                return attachment;
            }

            throw new ArgumentException(string.Format("Hittar inte filen: {0}. Kontrollera att den inte är borttagen.", info.Name));
        }

        /// <summary>
        /// Gets a file as base64byteArray
        /// </summary>
        private byte[] getFileAsBase64ByteArray(FileInfo fileInfo)
        {
            byte[] base64encoded = new byte[0];
            try
            {
                if (fileInfo != null)
                {
                    using (FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        byte[] filebytes = new byte[fs.Length];
                        fs.Read(filebytes, 0, Convert.ToInt32(fs.Length));
                        base64encoded = filebytes;
                    }
                }

            }
            catch (Exception)
            { 

                throw;
            }
            
            return base64encoded;
        }

        /// <summary>
        /// Gets the content type for an attachment.
        /// </summary>
        private string getContentType(FileInfo info)
        {
            var test = ConfigurationManager.AppSettings["AttachmentExtensions"];
            //foreach (AttachmentExtension extension in ConfigurationManager.AppSettings["AttachmentExtensions"])
            //{
            //    if (info.Extension == extension.Extension)
            //        return extension.ContentType;
            //}

            throw new ArgumentException(string.Format("{0} : {1}", ResourceHelper.Resources[25].Text, info.Name));
        }
    }
}
