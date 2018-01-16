using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Focus3.CsvTransform.CS;

namespace Focus3.CsvTransformer.Tests.Helpers
{
    public class CsTransformTestable : CsTransform
    {
        public CsTransformTestable(string inputXmlFilePath) : base(inputXmlFilePath)
        {
        }

        public Dictionary<string,string> LoadMappingFileTestable(string filePath)
        {
            return LoadMappingFile(filePath);
        }

        public string GetElementValueTestable(XElement companyElement, string xpath, string propertyName)
        {
            return GetElementValue(companyElement, xpath, propertyName);
        }

        public XDocument LoadXDocumentTestable(string xmlFilePath)
        {
            return LoadXDocument(xmlFilePath);
        }

        public IEnumerable<Dictionary<string, string>> BuildModelsFromXmlTestable(XDocument document)
        {
            return BuildModelsFromXml(document);
        }
    }
}
