using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using SE.GOV.MM.Integration.Log;
using SE.GOV.MM.Integration.Package.DataTransferObjects.BusinessObjects;
using BO = SE.GOV.MM.Integration.Package.DataTransferObjects.BusinessObjects;

namespace MinaMeddelanden.Sydnarke
{
  
    public class HelpMethod
    {
     
        public string rootPath { get; set; }
     


        public HelpMethod() {

            rootPath = @"C:\Dev\done\MinMeddelandenProj2\TestConsole";

        }
     
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
            string tempFolderPathString = Path.Combine(ApplicationRootPath, tempFolderName); //get the folder path

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


        public void getAttachmentToMailItem(ref Mail mailItem, string folderPath)
        {
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
                    //if (attachmentHelper.ValidateAttachment(mailItem.Attachments)) // check if the attachment is valid
                    //{
                    //    LogManager.Log(new Log { Message = string.Format("Attached file: {0} succesfully", fileAttachment.Name), Level = Level.Info });
                    //}

                }

            }
        }
    }
}