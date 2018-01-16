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

        public void GenerateCsvTestable(string filePath, IEnumerable<string> headerColumns,
            IEnumerable<Dictionary<string, string>> models)
        {
            GenerateCsv(filePath, headerColumns, models);
        }
    }
}
