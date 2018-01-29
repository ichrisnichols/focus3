using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Focus3.CsvTransformer
{
    public class CsvTransformer
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string DefaultDestFileName = "{company}_{date}.csv";

        private readonly CsvTransformationBase _transform;

        public CsvTransformer(CsvTransformationBase transform)
        {
            _transform = transform;
        }

        public void Execute(string destPath)
        {
            try
            {
                CreateDestPath(destPath);

                var headerColumnMappings = _transform.LoadHeaderColumnMappings();

                var models = _transform.LoadModels();

                var fileName = DefaultDestFileName
                    .Replace("{company}", _transform.CompanyName)
                    .Replace("{date}", DateTime.Now.ToShortDateString());

                var filePath = Path.Combine(destPath, fileName);

                GenerateCsvFile(filePath, headerColumnMappings, models);
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
        }

        protected void GenerateCsvFile(string outputFilePath, Dictionary<string, string> headerColumnMappings, IEnumerable<IDictionary<string, object>> models)
        {
            using (var streamWriter = File.CreateText(outputFilePath))
            {
                var headers = headerColumnMappings.Keys.ToList();
                streamWriter.WriteLine(string.Join(',', headers));

                foreach (var model in models)
                {
                    foreach (var headerColumn in headers)
                    {
                        if (!headerColumnMappings.ContainsKey(headerColumn))
                        {
                            throw new InvalidDataException($"Cannot find the property key for header column [{headerColumn}].  Please check the column header to property mapping file (ex: headerToPropertyMapping.json)");
                        }
                        var propertyKey = headerColumnMappings[headerColumn];

                        if (!model.ContainsKey(propertyKey))
                        {
                            Log.Warn($"Unable to find the value for property [{propertyKey}].");
                            streamWriter.Write(',');
                            continue;
                        }
                        streamWriter.Write(model[propertyKey].ToString() + ',');
                    }
                    streamWriter.WriteLine();
                }
            }
        }

        private void CreateDestPath(string destPath)
        {
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }
        }
    }
}
