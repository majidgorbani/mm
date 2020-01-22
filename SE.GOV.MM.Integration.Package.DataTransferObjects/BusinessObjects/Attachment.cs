using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SE.GOV.MM.Integration.Package.DataTransferObjects.BusinessObjects
{

    [DataContract(Namespace = "https://SE.GOV.MM.Integration.Package/2015/04")]
    public class Attachment
    {
        //Ursprungligt namn på fil.
        [DataMember]
        public string Filename { get; set; }
        //Enligt IANA MIME media types [7]. Tillåtna typer är application/pdf.
        [DataMember]
        public string ContentType { get; set; }
        //Base64 kodat binärt rådata för en bilaga.
        [DataMember]
        public byte[] Body { get; set; }

        //Size of attachment
        [DataMember]
        public int Size { get; set; }
    }
}
