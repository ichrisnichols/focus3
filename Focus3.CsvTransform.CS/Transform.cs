using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;
using Focus3.CsvTransformer;
using log4net;
using Newtonsoft.Json;

namespace Focus3.CsvTransform.CS
{
    public class Transform : CsvTransformationBase
    {
        private const string CompanyNameElement = "Company Name";

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string _inputXmlFilePath;
        private readonly Dictionary<string, string> _mapping;

        public Transform(string inputXmlFilePath) : base("CyberScout")
        {
            if (!File.Exists(inputXmlFilePath))
            {
                throw new ArgumentException($"Unable to find input XML file at the following path: [{_inputXmlFilePath}].");
            }

            _inputXmlFilePath = inputXmlFilePath;

            Log.Debug("Loading XML -> CSV mapping from mapping.json file.");

            _mapping = LoadMappingFile("mapping.json");
        }

        public override IEnumerable<string> LoadHeaderColumns()
        {
            return _mapping.Keys;
        }

        public override IEnumerable<Dictionary<string, string>> LoadModels()
        {
            var xDocument = LoadXDocument(_inputXmlFilePath);

            return BuildModelsFromXml(xDocument);
        }

        protected Dictionary<string, string> LoadMappingFile(string filePath)
        {
            var mappingStr = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(mappingStr);
        }

        protected XDocument LoadXDocument(string xmlFilePath)
        {
            var xmlStr = File.ReadAllText(_inputXmlFilePath);
            if (string.IsNullOrWhiteSpace(xmlStr))
            {
                throw new InvalidDataException($"The file at [{_inputXmlFilePath}] is empty.");
            }

            XDocument xDocument;
            try
            {
                xDocument = XDocument.Parse(xmlStr);
            }
            catch (Exception)
            {
                Log.Error($"The input file at [{_inputXmlFilePath}] is not valid XML.");
                throw;
            }

            return xDocument;
        }

        protected IEnumerable<Dictionary<string, string>> BuildModelsFromXml(XDocument document)
        {
            var companyElements = document.Root?.Element("Companies")?.Elements("Company").ToList();

            if (companyElements == null || !companyElements.Any())
            {
                throw new InvalidDataException($"The input file at [{_inputXmlFilePath}] does not contain any 'Company' elements.");
            }

            var modelList = new List<Dictionary<string, string>>();

            foreach (var companyElement in companyElements)
            {
                var modelDictionary = new Dictionary<string, string>();

                foreach (var map in _mapping)
                {
                    var value = GetElementValue(companyElement, map.Value, map.Key);

                    if (map.Key == CompanyNameElement)
                    {
                        Log.Debug($"Building model for company [{value}].");
                    }
                    
                    modelDictionary.Add(map.Key, value);
                }
                modelList.Add(modelDictionary);
            }

            return modelList;
        }

        protected string GetElementValue(XElement companyElement, string xpath, string propertyName)
        {
            return companyElement.XPathEvaluate($"string({xpath})") as string;
        }
    }
}
