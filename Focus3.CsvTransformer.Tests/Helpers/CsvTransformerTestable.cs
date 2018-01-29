using System;
using System.Collections.Generic;
using System.Text;

namespace Focus3.CsvTransformer.Tests.Helpers
{
    public class CsvTransformerTestable : CsvTransformer
    {
        public CsvTransformerTestable(CsvTransformationBase transform) : base(transform)
        {
        }

        public void GenerateCsvTestable(string filePath, Dictionary<string, string> headerColumnMappings,
            IEnumerable<IDictionary<string, object>> models)
        {
            GenerateCsvFile(filePath, headerColumnMappings, models);
        }
    }
}
