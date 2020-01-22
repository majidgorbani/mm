using SE.GOV.MM.Integration.Log;
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SE.GOV.MM.Integration.Package.Signing.Helper
{
    public class SerializeHelper
    {
        /// <summary>
        /// Serialize a SignedDelivery object to a XmlDocument. using defaultnamespace, that is a property from config file, add this to xmlserializer.
        /// </summary>
        public XmlDocument SerializeToXmlDocumentV3(SignedDelivery1 signedDelivery, string defaultNameSpace, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.BusinessLayer.Helper.SerializeHelper: incoming SerializeToXmlDocumentV3 with RequestId: {0}", requestId));

            var xmlDocument = new XmlDocument();
            xmlDocument.PreserveWhitespace = false;

            var xmlSerializerNameSpace = new XmlSerializerNamespaces();
            xmlSerializerNameSpace.Add("", defaultNameSpace);

            using (var memoryStream = new MemoryStream())
            {
                var xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.OmitXmlDeclaration = true;
                xmlWriterSettings.Encoding = Encoding.UTF8;

                using (var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings))
                {
                    var xmlSerializer = new XmlSerializer(typeof(SignedDelivery1), defaultNameSpace);
                    xmlSerializer.Serialize(xmlWriter, signedDelivery, xmlSerializerNameSpace);
                }

                memoryStream.Position = 0;
                xmlDocument.Load(memoryStream);
            }

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.BusinessLayer.Helper.SerializeHelper: leaving SerializeToXmlDocumentV3 with RequestId: {0}", requestId));
            return xmlDocument;
        }

        /// <summary>
        /// Read the xmldocument and deserialize it to a SignedDelivery object. Important to use the default namespace.
        /// </summary>
        public SignedDelivery1 DeserializeXmlToSignedDeliveryV3(XmlDocument xmlDocument, string defaultNameSpace, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.BusinessLayer.Helper.SerializeHelper: incoming DeserializeXmlToSignedDeliveryV3 with RequestId: {0}", requestId));

            SignedDelivery1 signedDelivery;
            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlDocument.OuterXml)))
            {
                var xmlSerializer = new XmlSerializer(typeof(SignedDelivery1), defaultNameSpace);
                signedDelivery = (SignedDelivery1)xmlSerializer.Deserialize(memoryStream);

            };

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.BusinessLayer.Helper.SerializeHelper: leaving DeserializeXmlToSignedDeliveryV3 with RequestId: {0}", requestId));
            return signedDelivery;
        }

    }
}
