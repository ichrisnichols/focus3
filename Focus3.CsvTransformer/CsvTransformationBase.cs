using System.Collections.Generic;

namespace Focus3.CsvTransformer
{
    public abstract class CsvTransformationBase
    {
        public string CompanyName;

        protected CsvTransformationBase(string companyName)
        {
            CompanyName = companyName;
        }

        public abstract Dictionary<string, string> LoadHeaderColumnMappings();

        public abstract IEnumerable<IDictionary<string, object>> ExtractEnrolleeDictionaries();
    }
}
