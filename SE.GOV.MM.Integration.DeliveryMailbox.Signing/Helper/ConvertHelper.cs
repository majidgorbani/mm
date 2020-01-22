using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SE.GOV.MM.Integration.DeliveryMailbox.Signing.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class ConvertHelper
    {
        
        /// <summary>
        /// Gets the MD5 checksum for AttachmentBody.
        /// </summary>
        public string GetMD5ChecksumForAttachmentBody(byte[] body)
        {
            string checksum = string.Empty;

            using (var md5 = MD5.Create())
            {
                var byteArray = md5.ComputeHash(body);
                checksum = BitConverter.ToString(byteArray).Replace("-", string.Empty);
            }
            return checksum;
        }

        /// <summary>
        /// Encodes a string to UTF8 and converts it to byte[].
        /// </summary>
        public byte[] EncodeToBase64FromStringToByteArray(string textToEncode)
        {          
            byte[] toEncodeAsBytes = UTF8Encoding.UTF8.GetBytes(textToEncode);
            return toEncodeAsBytes;
        }
    }
}
