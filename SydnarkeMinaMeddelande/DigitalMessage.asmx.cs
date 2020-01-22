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
        public string SendMessage(string recepientId, string sender, string subject, string message, string filesinXml, string supInfoEmailAddress, string supInfoPhoneNumber, string supInfoText, string supInfoUrI, string senderOrgNumber, string senderOrgName, string certificationBySubjectName)
        {
            string stats;
            LogManager.Log(new Log { Message = string.Format(Environment.NewLine), Level = Level.Trace });
            LogManager.Log(new Log { Message = string.Format("######################### Starting Send Message Proccess ###################################")  , Level = Level.Trace });
            stats = SendMessageToMinaMeddelande(recepientId, sender, subject, message, filesinXml, supInfoEmailAddress, supInfoPhoneNumber, supInfoText, supInfoUrI, senderOrgNumber, senderOrgName, certificationBySubjectName);
            return stats;
        }

        [WebMethod]
        public string TestMessage(string recepientId, string sender, string subject, string message, string filesinXml)
        {

            LogManager.Log(new Log { Message = string.Format(Environment.NewLine), Level = Level.Trace });
            LogManager.Log(new Log { Message = string.Format("######################### Starting Send Message Proccess ###################################"), Level = Level.Trace });

            return "Test Ok!";
           // sendMessageToMinaMeddelande(recepientId, sender, subject, message, filesinXml);
        }


        /// <summary>
        /// skickar meddelande till mina meddelande for avsluta anställning
        /// </summary>
        /// <param name="sender">avsändare </param>
        /// <param name="recepientId">oersonnummer</param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="stringBaseFile"></param>
        private string  SendMessageToMinaMeddelande(string recepientId , string sender, string subject , string message , string stringBaseFile, 
                                                    string supInfoEmailAddress, string supInfoPhoneNumber, string supInfoText, 
                                                    string supInfoUrI, string senderOrgNumber, string senderOrgName, string certificationBySubjectName)
        {
            List<string> filenNameList = new List<string>();
            string tempDirectoryPath = string.Empty;
            string result = string.Empty;
            HelpMethod f = new HelpMethod();
            //creating log string
            string logMessage = string.Format(  "Send Message: "    + Environment.NewLine +
                                                "recepientId: {0} " + Environment.NewLine +
                                                "sender: {1} "      + Environment.NewLine +
                                                "subject: {2} ", recepientId, sender, subject );


            LogManager.Log(new Log { Message = logMessage, Level = Level.Info }); //write to log
            tempDirectoryPath =downloadFilesFromBase64(rootPath, stringBaseFile); // returns list of file path 

            if (recepientId == string.Empty)
            {

                recepientId = "197902152383";   // User 1
               //  recepientId = "198503092390";   // User 2

               // recepientId = "167696295455";   // User 3 företag

                // recepientId = "200703032383";   // User 4

                certificationBySubjectName = "Test_Server";
                sender = "Sydnärke kommun";
                subject = "Test Message";
                message = string.Format(@"<a hrefTill Avsändarmyndigheten</a>" +
                    "<p>Är en paragraf som kommer att ge visst utrymme före och efter ett stycke</p>" +
                    "<br/> ger ett radbyte<br/>" +
                    "<b>this bold text</b>");


             

                supInfoEmailAddress = "peter.stromberg@it.sydnarke.com";
                supInfoPhoneNumber = "-058548103";
                supInfoText = "Support Info Text";
                supInfoUrI = "www.lekeberg.com";
                senderOrgNumber = "162120002981";
                //senderOrgNumber = "162120002982";
                senderOrgName = "Lekebergs kommunnn";

            //    //System.ServiceModel.FaultException`1: 'ERROR CODE: [6009 DISPATCHER_NOT_TRUSTED_SIGNER] - 
            //    ERROR CODE DESCRIPTION: [The dispatcher is not authorized signer.] - MESSAGE: 
            //    [Dispatcher [162120002981] is not authorized as signer for the sender 
            }
            
            try
            {
                var mailHelper = new MailHelper();
                // Creating Mailitem to object specified in Message service

                string reference = "Lekeberg Kommun integration";//ConfigHelper.ConfigurationEntity.Reference;
                
                Mail mailItem = mailHelper.CreateMailItem(sender, recepientId, subject, message, reference, supInfoEmailAddress, supInfoPhoneNumber, supInfoText, supInfoUrI, senderOrgNumber, senderOrgName, certificationBySubjectName);
                
                getAttachmentToMailItem(ref mailItem, tempDirectoryPath); //Attaching file to mailitem object
                var messageSender = new MessageSender(sender); // set sender for exp "Lekebergs Kommun"
                var sentResult = messageSender.SendMailToPackage(mailItem); //send message as a package

                result = sentResult.DeliveryStatus;
                LogManager.Log(new Log { Message = string.Format("MinaMeddelanden.Sydnarke: Message has been sent with the following status: {0} ", result), Level = Level.Info });
               

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
                    LogManager.Log(new Log { Message = string.Format("folder {0} have been deleted.", tempDirectoryPath), Level = Level.Info });

                }
               
            }
            return result;
        }

        private void getAttachmentToMailItem(ref Mail mailItem, string folderPath)
        {
            folderPath = @"C:\Dev\Projects\MinMeddelanden\SydnarkeMinaMeddelande\UploadsFolder";
            string[] files = Directory.GetFiles(folderPath);
            mailItem.Attachments = new List<BO.Attachment>(); //BO BusinessObject project
  
            AttachmentHelper attachmentHelper = new AttachmentHelper();
            if (files.Length >= 0) // controll for filenNameList is not null
            {
                foreach (var filepath in files)
                {
                    //if (counter == 0)
                    //{
                        FileInfo fileAttachment = new FileInfo(filepath);
                        BO.Attachment attac = attachmentHelper.GetAttachment(fileAttachment); // create Attachment object
                        mailItem.Attachments.Add(attac);
                        if (attachmentHelper.ValidateAttachment(mailItem.Attachments)) // check if the attachment is valid
                        {
                            LogManager.Log(new Log { Message = string.Format("Attached file: {0} succesfully", fileAttachment.Name), Level = Level.Info });
                        }
                       // counter++;

                   // }
                   

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
