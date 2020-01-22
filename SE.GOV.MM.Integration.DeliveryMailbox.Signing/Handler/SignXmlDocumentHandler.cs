using SE.GOV.MM.Integration.Log;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace SE.GOV.MM.Integration.DeliveryMailbox.Signing.Handler
{
    public class SignXmlDocumentHandler
    {
        /// <summary>
        /// Signs an SignSignedDeliveryXmlDocument with an xml signature. 
        /// </summary>
        public XmlDocument SignSignedDeliveryXmlDocument(XmlDocument document, X509Certificate2 certificate, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.Signing.Handler.SignXmlDocumentHandler: incoming SignSignedDeliveryXmlDocument with RequestId: {0}", requestId));

            document.Normalize();

            var signedXml = GetXmlSignature(document, certificate, requestId);

            // Append the computed signature. The signature must be placed as the sibling of the Issuer element.
            XmlNodeList nodes = document.DocumentElement.GetElementsByTagName("Delivery");
            nodes[0].ParentNode.InsertAfter(document.ImportNode(signedXml.GetXml(), true), nodes[0]);

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.Signing.Handler.SignXmlDocumentHandler: leaving SignSignedDeliveryXmlDocument with RequestId: {0}", requestId));
            return document;
        }

        /// <summary>
        /// Signs an SignSealedDeliveryXmlDocument with an xml signature. SignedDelivery
        /// </summary>
        public XmlDocument SignSealedDeliveryXmlDocument(XmlDocument document, X509Certificate2 certificate, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.Signing.Handler.SignXmlDocumentHandler: incoming SignSealedDeliveryXmlDocument with RequestId: {0}", requestId));

            document.Normalize();

            var signedXml = GetXmlSignature(document, certificate, requestId);

            // Append the computed signature. The signature must be placed as the sibling of the Issuer element.
            XmlNodeList nodes = document.DocumentElement.GetElementsByTagName("Seal");
            nodes[0].ParentNode.InsertAfter(document.ImportNode(signedXml.GetXml(), true), nodes[0]);

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.Signing.Handler.SignXmlDocumentHandler: leaving SignSealedDeliveryXmlDocument with RequestId: {0}", requestId));
            return document;
        }

        private SignedXml GetXmlSignature(XmlDocument document, X509Certificate2 certificate, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.Signing.Handler.SignXmlDocumentHandler: incoming GetXmlSignature with RequestId: {0}", requestId));

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

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.Signing.Handler.SignXmlDocumentHandler: leaving GetXmlSignature with RequestId: {0}", requestId));
            return signedXml;
        }
    }
}
