using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Focus3.CsvTransformer
{
    public class CsvTransformer
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string DefaultDestPath = @"C:\Focus3\CsvTransformations";
        private const string DefaultDestFileName = "{company}_{date}.csv";

        private readonly CsvTransformationBase _transform;

        public CsvTransformer(CsvTransformationBase transform)
        {
            _transform = transform;
        }

        public void Execute(string sourcePath = ".", string destPath = DefaultDestPath)
        {
            try
            {
                CreateDestPath(destPath);

                var headerColumns = _transform.LoadHeaderColumns();

                var models = _transform.LoadModels();

                var fileName = DefaultDestFileName
                    .Replace("{company}", _transform.CompanyName)
                    .Replace("{date}", DateTime.Now.ToShortDateString());

                var filePath = Path.Combine(destPath, fileName);

                GenerateCsv(filePath, headerColumns, models);
            }
            catch (Exception e)
            {

                throw;
            }
        }

        private void GenerateCsv(string filePath, IEnumerable<string> headerColumns, IEnumerable<Dictionary<string, string>> models)
        {
            using (var streamWriter = File.CreateText(filePath))
            {
                var headers = headerColumns.ToList();
                streamWriter.WriteLine(string.Join(',', headers));

                foreach (var model in models)
                {
                    foreach (var headerColumn in headers)
                    {
                        streamWriter.Write(model[headerColumn] + ',');
                    }
                }
            }
        }

        private void CreateDestPath(string destPath)
        {
            if (Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }
        }
    }
}
