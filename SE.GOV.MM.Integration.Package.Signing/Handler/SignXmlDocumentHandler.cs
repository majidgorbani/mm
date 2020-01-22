using SE.GOV.MM.Integration.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SE.GOV.MM.Integration.Package.Signing.Handler
{
    public class SignXmlDocumentHandler
    {
        /// <summary>
        /// Signs an XmlDocument with an xml signature.
        /// </summary>
        public XmlDocument SignXmlDocument(XmlDocument document, X509Certificate2 certificate, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Signing.Handler.SignXmlDocumentHandler: incoming SignXmlDocument with RequestId: {0}", requestId));

            document.Normalize();

            SignedXml signedXml = new SignedXml(document);           
            signedXml.SigningKey = certificate.PrivateKey;

            // Retrieve the value of the "ID" attribute on the root assertion element.
            Reference reference = new Reference("");

            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());

            signedXml.AddReference(reference);

            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;

            // Include the public key of the certificate in the assertion.
            signedXml.KeyInfo = new KeyInfo();

            var keyInfoData = new KeyInfoX509Data();
            keyInfoData.AddSubjectName(certificate.SubjectName.Name);
            keyInfoData.AddCertificate(certificate);
            signedXml.KeyInfo.AddClause(keyInfoData);

            signedXml.ComputeSignature();

            // Append the computed signature. The signature must be placed as the sibling of the Issuer element.
            XmlNodeList nodes = document.DocumentElement.GetElementsByTagName("Delivery");
            nodes[0].ParentNode.InsertAfter(document.ImportNode(signedXml.GetXml(), true), nodes[0]);

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Signing.Handler.SignXmlDocumentHandler: leaving SignXmlDocument with RequestId: {0}", requestId));
            return document;
        }
    }
}
