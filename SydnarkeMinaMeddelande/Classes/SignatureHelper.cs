using SE.GOV.MM.Integration.Outlook.BusinessLayer.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SE.GOV.MM.Integration.Outlook.Helper
{
    public class SignatureHelper
    {
        /// <summary>
        /// Get Outlook signatures saved on the client.
        /// </summary>
        /// <returns></returns>
        public List<Signature> ReadSignature()
        {
            try
            {
                var signatureDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + ConfigHelper.ConfigurationEntity.RelativePathToSignature;
                Signature signature;
                var listOfSignature = new List<Signature>();

                DirectoryInfo diInfo = new DirectoryInfo(signatureDir);

                if (diInfo.Exists)
                {
                    FileInfo[] signatures = diInfo.GetFiles("*.txt");

                    if (signatures.Length > 0)
                    {
                        foreach (FileInfo info in signatures)
                        {
                            StreamReader sr = new StreamReader(info.FullName, Encoding.UTF8);

                            //Create signature object
                            signature = new Signature();
                            signature.Name = info.Name;
                            signature.Text = sr.ReadToEnd();
                            signature.Text = signature.Text.Trim();

                            //Remove file extension
                            signature.Name = signature.Name.Replace(".txt", string.Empty);

                            //Add to list
                            listOfSignature.Add(signature);
                        }
                    }
                }
                return listOfSignature;
            }
            catch (Exception)
            { }

            return new List<Signature>();
        }
    }

    public class Signature
    {
        public string Name { get; set; }
        public string Text { get; set; }
    }
}
