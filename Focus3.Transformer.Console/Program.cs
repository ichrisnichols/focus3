using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;

namespace Focus3.Transformer.Console
{
    class Program
    {
        private const string DefaultDestPath = @"C:\Focus3\CsvTransformations\Output";
        private const string InputFilePathExample = @"C:\Focus3\CsvTransformations\Input\EnrollmentData.xml";

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            //var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            var logRepository = LogManager.CreateRepository(
                Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
            XmlConfigurator.Configure(logRepository, new FileInfo("log.config"));

            try
            {
                // TODO: add unit tests for CsvTransformer
                // TODO: alter data structure to be one record per employee

                // get user input...
                string inputFilePath;
                string outputFilePath = null;
                if (args.Length == 0)
                {
                    System.Console.WriteLine($"Please enter the full path to the input file (Ex: '{InputFilePathExample}')...");
                    inputFilePath = System.Console.ReadLine();

                    System.Console.WriteLine(
                        $"Please enter the full path to the output directory or leave blank and the file will be written to [{DefaultDestPath}].");
                    outputFilePath = System.Console.ReadLine();
                }
                else
                {
                    inputFilePath = args[0];

                    if (!string.IsNullOrWhiteSpace(args[1]))
                    {
                        outputFilePath = args[1];
                    }
                }

                // validate input
                while (string.IsNullOrWhiteSpace(inputFilePath) || !File.Exists(inputFilePath))
                {
                    System.Console.WriteLine($"The input file path [{inputFilePath}] is invalid, please enter a valid path to the input file or type 'x' to exit...");
                    inputFilePath = System.Console.ReadLine();
                    if (inputFilePath?.ToLower() == "x")
                    {
                        Environment.Exit(0);
                    }
                }

                if (string.IsNullOrWhiteSpace(outputFilePath))
                {
                    System.Console.WriteLine($"No valid parameter found for output directory path.  The default path [{DefaultDestPath}] will be used.");
                    outputFilePath = DefaultDestPath;
                }
                else if (!Directory.Exists(outputFilePath))
                {
                    try
                    {
                        Directory.CreateDirectory(outputFilePath);
                    }
                    catch (Exception e)
                    {
                        Log.Warn(e);
                        Log.Warn($"Unable to create directory for path [{outputFilePath}].  The default path [{DefaultDestPath}] will be used.");
                        outputFilePath = DefaultDestPath;
                    }
                }

                // transformation...
                Log.Debug($"Beginning transformation of file [{inputFilePath}]...");
                var csTransform = new CsvTransform.CS.CsTransform(inputFilePath);
                var transformer = new CsvTransformer.CsvTransformer(csTransform);
                transformer.Execute(outputFilePath);

                Log.Debug("Transformation complete!");
                System.Console.WriteLine("Press any key to exit...");
                System.Console.ReadKey();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
