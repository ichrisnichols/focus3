using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;
using Focus3.CsvTransform.CS.Extensions;
using Focus3.CsvTransform.CS.Models;
using Focus3.CsvTransformer;
using log4net;
using Newtonsoft.Json;

namespace Focus3.CsvTransform.CS
{
    public class CsTransform : CsvTransformationBase
    {
        /*
         * Test counter for counting the number of employees with an empty Enrollments element
         */
        private int _emptyEnrollmentCount = 0;

        public int EmptyEnrollmentEmployeeCount
        {
            get { return _emptyEnrollmentCount; }
        }

        private const string CompanyNameElement = "Company Name";

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string _inputXmlFilePath;
        private readonly Dictionary<string, string> _mapping;

        public CsTransform(string inputXmlFilePath) : base("CyberScout")
        {
            if (!File.Exists(inputXmlFilePath))
            {
                throw new ArgumentException($"Unable to find input XML file at the following path: [{_inputXmlFilePath}].");
            }

            _inputXmlFilePath = inputXmlFilePath;

            const string mappingFileName = "headerToPropertyMapping.json";
            Log.Info($"Loading column header to entity property mapping file [{mappingFileName}].");
            _mapping = LoadMappingFile(mappingFileName);
        }

        public override Dictionary<string, string> LoadHeaderColumnMappings()
        {
            return _mapping;
        }

        public override IEnumerable<IDictionary<string, object>> ExtractEnrolleeDictionaries()
        {
            var xDocument = LoadXDocument(_inputXmlFilePath);

            var enrollees = BuildEnrolleeModels(xDocument);

            var models = enrollees.Select(enrollee => enrollee.AsDictionary()).ToList();

            return models;
        }

        protected Dictionary<string, string> LoadMappingFile(string filename)
        {
            string filePath = filename;
            if (!File.Exists(filePath))
            {
                var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                filePath = Path.Combine(assemblyDir, filename);
                if (!File.Exists(filePath))
                {
                    throw new IOException($"Cannot find mapping file [{filename}].");
                }
            }

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

        // currently not used...
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

        protected IEnumerable<Enrollee> BuildEnrolleeModels(XDocument document)
        {
            var companyElements = document.Root?.Element("Companies")?.Elements("Company").ToList();

            if (companyElements == null || !companyElements.Any())
            {
                throw new InvalidDataException($"The input file at [{_inputXmlFilePath}] does not contain any 'Company' elements.");
            }

            var enrolleesAll = new List<Enrollee>();

            foreach (var companyElement in companyElements)
            {
                var company = MapCompany(companyElement);
                Log.Info($"Building model for company [{company.Name}]...");

                var employeeElements = companyElement.Element("Employees")?.Elements("Employee").ToList();

                if (employeeElements == null || !employeeElements.Any())
                {
                    Log.Warn($"The input file at [{_inputXmlFilePath}] does not contain any 'Employee' elements for company [{company.Name}].");
                    continue;
                }

                var enrolleesCompany = new List<Enrollee>();
                foreach (var employeeElement in employeeElements)
                {
                    var employee = MapEmployee(employeeElement, company);

                    if (employee == null)
                    {
                        continue;
                    }
                        
                    enrolleesCompany.Add(employee);

                    var dependentElements =
                        (employeeElement.Element("Dependents")?.Elements("Dependent") ?? new List<XElement>()).ToList();

                    if (!dependentElements.Any())
                    {
                        Log.Debug($"No dependents found for employee with ID [{employee.EmployeeId}] and name [{employee.CommonPersonalData?.FirstName} {employee.CommonPersonalData?.LastName}].");
                        continue;
                    }

                    foreach (var dependentElement in dependentElements)
                    {
                        // only map dependents that have matching enrollment nodes based on sequence number...
                        var sequenceNum = dependentElement.Element("SequenceNumber")?.Value ?? "-1";
                        if (string.IsNullOrWhiteSpace(sequenceNum))
                        {
                            throw new InvalidDataException($"Invalid or missing sequence number for dependent of employee with ID [{employee.EmployeeId}] and name [{employee.CommonPersonalData?.FirstName} {employee.CommonPersonalData?.LastName}].");
                        }

                        var enrolled = employeeElement.Element("Enrollments")?.Elements("Enrollment")
                            .FirstOrDefault()?.Element("DependentEnrollees")?.Elements("Enrollee")
                            .Any(de => de.Element("SequenceNumber")?.Value == sequenceNum) ?? false;

                        if (enrolled)
                        {
                            enrolleesCompany.Add(MapDependent(dependentElement, company, employee));
                        }
                        else
                        {
                            Log.Debug($"The dependent with SequenceNumber [{sequenceNum}] for employee with ID [{employee.EmployeeId}] does not have a matching DependentEnrollees/Enrollee element, so will not be added to the output file.");
                        }
                    }
                }
                enrolleesAll.AddRange(enrolleesCompany);
                Log.Info($"[{enrolleesCompany.Count}] enrollees have been added for company [{company.Name}], consisting of [{enrolleesCompany.Count(e => e is Employee)}] employees and [{enrolleesCompany.Count(e => e is Dependent)}] dependents.");
            }
            Log.Info($"[{enrolleesAll.Count}] total enrollees have been added, consisting of [{enrolleesAll.Count(e => e is Employee)}] employees and [{enrolleesAll.Count(e => e is Dependent)}] dependents.");
            return enrolleesAll;
        }

        protected Company MapCompany(XElement companyElement)
        {
            return new Company
            {
                Id = companyElement?.Element("Identifier")?.Value,
                Name = companyElement?.Element("Name")?.Value
            };
        }

        protected Employee MapEmployee(XElement employeeElement, Company company)
        {
            var employeeId = employeeElement.Element("ExternalEmployeeId")?.Value;
            var enrollmentElement = employeeElement.Element("Enrollments")?.Elements("Enrollment").FirstOrDefault();

            if (enrollmentElement == null)
            {
                _emptyEnrollmentCount++;
                Log.Warn($"No enrollments found for Employee ID: {employeeId} from Company ID: {company.Id}.");
                return null;
            }

            return new Employee
            {
                Company = company,
                Address = MapAddress(employeeElement),
                CommonPersonalData = MapCommonData(employeeElement),
                EmployeeId = employeeId,
                EmployeeSsn = employeeElement.Element("SSN")?.Value,
                HireDate = ParseDate(employeeElement.Element("HireDate")),
                Status = employeeElement.Element("EmploymentStatus")?.Value,
                TermDate = ParseDate(employeeElement.Element("TerminationDate")),
                Enrollment = MapEnrollment(enrollmentElement)
            };
        }

        protected Dependent MapDependent(XElement dependentElement, Company company, Employee employee)
        {
            if (dependentElement == null || !dependentElement.HasElements)
            {
                throw new ArgumentException($"Cannot map dependent for empty or null XElement for EmployeeID: {employee.EmployeeId}.");
            }

            return new Dependent
            {
                Ssn = dependentElement.Element("SSN")?.Value,
                Relationship = dependentElement.Element("Relationship")?.Value,
                SequenceNum = dependentElement.Element("SequenceNumber")?.Value,
                Company = company,
                EmployeeSsn = employee.EmployeeSsn,
                EmployeeId = employee.EmployeeId,
                CommonPersonalData = MapCommonData(dependentElement),
                Address = MapAddress(dependentElement)
            };
        }

        protected Enrollment MapEnrollment(XElement enrollmentElement)
        {
            if (enrollmentElement == null || !enrollmentElement.HasElements)
            {
                throw new ArgumentException("Cannot map enrollment for empty or null XElement");
            }

            return new Enrollment
            {
                Carrier = CompanyName,
                CarrierPlanCode = enrollmentElement.Element("CarrierPlanCode")?.Value,
                PlanName = enrollmentElement.Element("Plan")?.Value,
                EnrollmentType = enrollmentElement.Element("EnrollmentType")?.Value,
                CoverageLevel = enrollmentElement.Element("CoverageLevel")?.Value,
                EnrolledOn = ParseDate(enrollmentElement.Element("EnrolledOn")),
                StartDate = ParseDate(enrollmentElement.Element("StartDate")),
                EndDate = ParseDate(enrollmentElement.Element("EndDate")),
                EmployeeCost = ParseDecimal(enrollmentElement.Element("EmployeeCost")),
                EmployerCost = ParseDecimal(enrollmentElement.Element("EmployerCost"))
            };
        }

        protected CommonPersonalData MapCommonData(XElement element)
        {
            if (element == null || !element.HasElements)
            {
                throw new ArgumentException("Cannot map Common Personal Data for empty or null XElement");
            }

            return new CommonPersonalData
            {
                LastName = element.Element("LastName")?.Value,
                FirstName = element.Element("FirstName")?.Value,
                MiddleName = element.Element("MiddleName")?.Value,
                Suffix = element.Element("Suffix")?.Value,
                BirthDate = ParseDate(element.Element("DOB")),
                Gender = element.Element("Gender")?.Value,
                Phone = element.Element("Phone")?.Value,
                Email = element.Element("Email")?.Value
            };
        }

        protected Address MapAddress(XElement element)
        {
            if (element == null || !element.HasElements)
            {
                throw new ArgumentException("Cannot map address for empty or null XElement");
            }

            return new Address
            {
                Address1 = element.Element("Address1")?.Value,
                Address2 = element.Element("Address2")?.Value,
                City = element.Element("City")?.Value,
                State = element.Element("State")?.Value,
                Zip = element.Element("ZIP")?.Value
            };
        }

        protected string ParseDate(XElement dateElement)
        {
            var strDateValue = dateElement?.Value;

            if (!string.IsNullOrWhiteSpace(strDateValue) &&
                DateTime.TryParse(strDateValue, out var dateTime))
            {
                return dateTime.ToString("MM/dd/yyyy");
            }

            return null;
        }

        protected string ParseDecimal(XElement decimalElement)
        {
            var strDecValue = decimalElement?.Value;

            if (!string.IsNullOrWhiteSpace(strDecValue) &&
                decimal.TryParse(strDecValue, out var decimalValue))
            {
                return $"{decimalValue:C2}";
            }

            return null;
        }

        protected string GetElementValue(XElement companyElement, string xpath, string propertyName)
        {
            return companyElement.XPathEvaluate($"string({xpath})") as string;
        }
    }
}
