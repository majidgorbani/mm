using SE.GOV.MM.Integration.Log;
using System;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace SE.GOV.MM.Integration.DeliveryMailbox.Signing.Helper
{
    public class SigningCertificateHelper
    {              
        /// <summary>
        /// Get the certificate used to sign xml document.
        /// Signing certificate is valid to sign, but since no root is found it isnt validating...
        /// </summary>
        public X509Certificate2 GetXMLSigningCertificate(string signingCertificateSubjectName, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.BusinessLayer.Helper.SigningCertificateHelper: incoming call GetXMLSigningCertificate with RequestId: {0}", requestId));

            var certificate = new X509Certificate2();
            var certificates = new X509Certificate2Collection();
            var store = new X509Store();

            try
            {
                store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadOnly);
                //Since there is no CA för this certificate, onlyValid is set to false
                certificates = store.Certificates.Find(X509FindType.FindBySubjectName, signingCertificateSubjectName, false);

            }
            catch (CryptographicException ce)
            {
                LogManager.Log(new Log.Log() { EventId = EventId.SigningCertificateNotAvailable, Exception = ce, Level = Level.Error, Message = ce.Message });
            }
            catch (SecurityException se)
            {
                LogManager.Log(new Log.Log() { EventId = EventId.SigningCertificateNotAvailable, Exception = se, Level = Level.Error, Message = se.Message });
            }
            catch (ArgumentException ae)
            {
                LogManager.Log(new Log.Log() { EventId = EventId.SigningCertificateNotAvailable, Exception = ae, Level = Level.Error, Message = ae.Message });
            }

            if (certificates.Count == 0)
            {
                var errorMessage = string.Format("SE.GOV.MM.Integration.DeliveryMailbox.BusinessLayer.Helper.SigningCertificateHelper: No certificate found, sign xml. RequestId: {0}", requestId);
                LogManager.Log(new Log.Log() { EventId = EventId.SigningCertificateNotAvailable, Level = Level.Error, Message = errorMessage });
                store.Close();
                throw new Exception(errorMessage);
            }
            else
            {
                certificate = certificates[0];
            }

            store.Close();

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.BusinessLayer.Helper.SigningCertificateHelper: leaving GetXMLSigningCertificate with RequestId: {0}", requestId));
            return certificate;
        }
    }
}
