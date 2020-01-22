
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace DigitalEmployment.Lekeberg
{
    public static class ResourceHelper
    {
        private static Dictionary<int, Resource> _resources = null;

        /// <summary>
        /// Dictionary containing all texts shown in the application.
        /// </summary>
        public static Dictionary<int, 
            Resource> Resources 
        { 
            get
            {
                if (_resources == null)
                {
                    _resources = new Dictionary<int, Resource>();
                    ListOfResources();
                }
                return _resources;
            } 
        }

        /// <summary>
        /// Converts a xml document with att resources to a List of Resource objects.
        /// </summary>
        private static void ListOfResources()
        {
            var xDocument = LoadEmbeddedResources();
            var resourceList = (from resourceValue in xDocument.Element("Resources").Element("Resource").Elements("Message")
                                    select new Resource
                                    {
                                        Id = int.Parse(resourceValue.Element("Id").Value),
                                        Text = resourceValue.Element("Text").Value                                                    
                                    }
                                );

            foreach (var resource in resourceList)
            {
                _resources.Add(resource.Id, resource);
            }
        }

        /// <summary>
        /// Creates a XDocument from embedded Resource.xml.
        /// </summary>
        private static XDocument LoadEmbeddedResources()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "SE.GOV.MM.Integration.Outlook.BusinessLayer.Resource.xml";
            string result = string.Empty;

            //*** Read embedded files from this assembly ***
            //Assembly _assembly;
            //_assembly = Assembly.GetExecutingAssembly();
            //string[] names = _assembly.GetManifestResourceNames();

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }

            byte[] byteArray = Encoding.UTF8.GetBytes(result);
            MemoryStream mStream = new MemoryStream(byteArray);
            XDocument xDocument = XDocument.Load(mStream);

            return xDocument;
        }
    }
}
