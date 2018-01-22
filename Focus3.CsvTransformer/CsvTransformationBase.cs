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

        public abstract IEnumerable<string> LoadHeaderColumns();

        public abstract IEnumerable<IDictionary<string, object>> LoadModels();
    }
}
