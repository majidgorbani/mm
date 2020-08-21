using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using BO = SE.GOV.MM.Integration.Package.DataTransferObjects.BusinessObjects;
using SE.GOV.MM.Integration.Log;
using System.Text;
using System.Xml.Linq;
using SE.GOV.MM.Integration.Package.DataTransferObjects.BusinessObjects;
using SE.GOV.MM.Integration.Outlook.BusinessLayer.Helper;

namespace MinaMeddelanden.Sydnarke
{
    /// <summary>
    /// Summary description for DigitalMessage
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class DigitalMessage : System.Web.Services.WebService
    {

        public string rootPath = HttpContext.Current.Server.MapPath("~/");

       


        /// <summary>
        /// 
        /// </summary>
        /// <param name="recepientId">Personal number</param>
        /// <param name="sender">Sender ex: "Lekeberg Kommun"</param>
        /// <param name="subject"></param>
        /// <param name="message">Body</param>
        /// <param name="filesinXml">xml string</param>
        [WebMethod]
        public string SendMessage(  string recepientId, string sender, string subject, string messageBody,  string filesinXml,
                                    string senderOrgNumber, string senderOrgName, string reference, string certificationBySubjectName,
                                    string supInfoText, string supInfoEmailAddress, string supInfoPhoneNumber, string supInfoUrI
                                    )
        {
            string stats;
            //LogManager.Log(new Log { Message = string.Format(Environment.NewLine), Level = Level.Trace });
            LogManager.Log(new Log { Message = string.Format("######################### Starting Send Message Proccess ###################################" +Environment.NewLine )  , Level = Level.Trace });

            stats = SendMessageToMinaMeddelande( recepientId, sender, subject, messageBody, filesinXml,
                                                senderOrgNumber, senderOrgName, reference, certificationBySubjectName,
                                                supInfoText,  supInfoEmailAddress, supInfoPhoneNumber, supInfoUrI);
            return stats;
        }  


        /// <summary>
        /// skickar meddelande till mina meddelande for avsluta anställning
        /// </summary>
        /// <param name="sender">avsändare </param>
        /// <param name="recepientId">oersonnummer</param>
        /// <param name="subject"></param>
        /// <param name="messageBody"></param>
        /// <param name="stringBaseFile"></param>
        private string  SendMessageToMinaMeddelande(
            string recepientId , string sender, string subject , string messageBody , string stringBaseFile,
            string senderOrgNumber, string senderOrgName, string reference, string certificationBySubjectName,
            string supInfoText, string supInfoEmailAddress, string supInfoPhoneNumber, 
            string supInfoUrI)
        {

            
            List<string> filenNameList = new List<string>();
            string tempDirectoryPath = string.Empty;
            string result = string.Empty;
            HelpMethod f = new HelpMethod();           



            string logMessage = string.Format("**** Send Message: for recepientId: {0} sender: {1} subject: {2} *****", recepientId, sender, subject);
            LogManager.Log(new Log { Message = logMessage, Level = Level.Info }); //write to log

            try
            {
                tempDirectoryPath = downloadFilesFromBase64(rootPath, stringBaseFile); // returns list of file path 
                var mailHelper = new MailHelper();           

                
                Mail mailItem = mailHelper.CreateMailItem(sender, recepientId, subject, messageBody, reference, supInfoEmailAddress, supInfoPhoneNumber, supInfoText, supInfoUrI, senderOrgNumber, senderOrgName, certificationBySubjectName);
                
                getAttachmentToMailItem(ref mailItem, tempDirectoryPath); //Attaching file to mailitem object
                var messageSender = new MessageSender(sender); // set sender for exp "Lekebergs Kommun"
                var sentResult = messageSender.SendMailToPackage(mailItem); //send message as a package

                result = sentResult.DeliveryStatus;
               

            }
            catch (Exception ex)
            {
                LogManager.Log(new Log { Message = "Error message: " + ex, Level = Level.Error });
            }
            finally
            {
                if (Directory.Exists(tempDirectoryPath))
                {
                    Directory.Delete(tempDirectoryPath, true); // delete directory recursive
                    LogManager.Log(new Log { Message = string.Format("folder {0} have been deleted successfully.", tempDirectoryPath), Level = Level.Info });

                }
               
            }
            LogManager.Log(new Log { Message = string.Format("####### MinaMeddelanden.Sydnarke: The message sending process has been done with the following status: {0} #########" + Environment.NewLine, result), Level = Level.Info });

            return result;
        }

        private void getAttachmentToMailItem(ref Mail mailItem, string folderPath)
        {
           // folderPath = @"C:\Dev\Projects\MinMeddelanden\SydnarkeMinaMeddelande\UploadsFolder";
            string[] files = Directory.GetFiles(folderPath);
            mailItem.Attachments = new List<BO.Attachment>(); //BO BusinessObject project
  
            AttachmentHelper attachmentHelper = new AttachmentHelper();
            if (files.Length >= 0) // controll for filenNameList is not null
            {
                foreach (var filepath in files)
                {
                   
                        FileInfo fileAttachment = new FileInfo(filepath);
                        BO.Attachment attac = attachmentHelper.GetAttachment(fileAttachment); // create Attachment object
                        mailItem.Attachments.Add(attac);
                        if (attachmentHelper.ValidateAttachment(mailItem.Attachments)) // check if the attachment is valid
                        {
                            LogManager.Log(new Log { Message = string.Format("Attached file: {0} succesfully", fileAttachment.Name), Level = Level.Info });
                        }
                

            
                   

                }

            }
        }


        /// <summary>
        /// parse string base64 to valid xml and store data in destination path 
        /// </summary>
        /// <param name="ApplicationRootPath"></param>
        /// <param name="stringBaseFile"></param>
        /// <returns>List of urls of uplpoaded files</returns>
        public string downloadFilesFromBase64(string ApplicationRootPath, string stringBaseFile)
        {

            string filePath = string.Empty;
            DirectoryInfo directory = null;
            string name = string.Empty;
            string content = string.Empty;
            // generating temp folder direcrory name with following format TempFolder-18-12_13-24 => TempFolder-day-mounth_hour-minute
            string tempFolderName =
                    @"TempFolder-" +
                    String.Format("{0:dd-M}", DateTime.Today)
                    + "_" +
                    String.Format("{0:HH-mm}", DateTime.Now);
            string tempFolderPathString = Path.Combine(rootPath, tempFolderName); //get the folder path

            try
            {
                directory = Directory.CreateDirectory(tempFolderPathString);
                LogManager.Log(new Log { Message = "Temp directory: " + directory.Name + " have been created.", Level = Level.Info });
            }
            catch (Exception ex)
            {

                LogManager.Log(new Log { Message = "Error message: " + ex, Level = Level.Warning });
            }
            if (stringBaseFile == string.Empty) // for d
            {
                stringBaseFile = System.IO.File.ReadAllText(@"C:\Temp\xmlOutput.txt");

            }

            if (stringBaseFile != string.Empty)
            {
                /*
             
                  wrapping missing root <files> in xml string, the format will be as following
                  <files>
                      <file>
                           <name>Invoice-INF43489.pdf</name>
                           <content>64Basestring</content>
                      </file>
                 </files>
                
                 */
                stringBaseFile = "<files>" + stringBaseFile;
                stringBaseFile = stringBaseFile + "</files>";

                try
                {
                    XElement xDoc = XElement.Parse(stringBaseFile); //parse the string to xml object
                    List<XElement> fileNode = xDoc.Elements("file").ToList(); // extrating child nod file
                    foreach (var item in fileNode)
                    {
                        name = ((System.Xml.Linq.XText)((System.Xml.Linq.XContainer)item.FirstNode).FirstNode).Value;
                        content = ((System.Xml.Linq.XText)((System.Xml.Linq.XContainer)item.LastNode).LastNode).Value;
                        Byte[] ContentInBytes = Convert.FromBase64String(content); // converting content to byte
                        filePath = tempFolderPathString + @"\" + name;
                        File.WriteAllBytes(filePath, ContentInBytes); //Creates a new file, writes the specified byte array to the file, and then closes the file. If the target file already exists, it is overwritten.
                        LogManager.Log(new Log { Message = string.Format("Temp File {0} have been created in {1} folder", name, tempFolderName), Level = Level.Info });

                    }
                }
                catch (Exception ex)
                {
                    LogManager.Log(new Log { Message = "Error message: " + ex, Level = Level.Error });
                }
            }
            return directory.FullName;
        }
    }
}
