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
                Log.Info("Starting transformer execution...");
                var logDir = Path.Combine(Environment.CurrentDirectory, "app.log");
                Log.Info($"\r\nNote that the full log with additional detail can be found at [{logDir}].");

                // get user input...
                string inputFilePath;
                string outputFilePath = null;
                if (args.Length == 0)
                {
                    System.Console.WriteLine($"\r\nPlease enter the full path to the INPUT file (Ex: '{InputFilePathExample}')...");
                    inputFilePath = System.Console.ReadLine();

                    System.Console.WriteLine(
                        $"\r\nPlease enter the full path to the OUTPUT directory or press enter and the file will be written to [{DefaultDestPath}].");
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
                    System.Console.WriteLine($"\r\nThe INPUT file path [{inputFilePath}] is invalid, please enter a valid file path or enter 'x' to exit...");
                    inputFilePath = System.Console.ReadLine();
                    if (inputFilePath?.ToLower() == "x")
                    {
                        Environment.Exit(0);
                    }
                }

                if (string.IsNullOrWhiteSpace(outputFilePath))
                {
                    System.Console.WriteLine($"\r\nThe default output path will be used: [{DefaultDestPath}].");
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
                        Log.Debug(e);
                        Log.Warn($"\r\nUnable to create directory for path [{outputFilePath}].  The default path will be used: [{DefaultDestPath}].");
                        outputFilePath = DefaultDestPath;
                    }
                }

                // transformation...
                Log.Info($"\r\nBeginning transformation of file [{inputFilePath}]...");
                var csTransform = new CsvTransform.CS.CsTransform(inputFilePath);
                var transformer = new CsvTransformer.CsvTransformer(csTransform);
                var outputPath = transformer.Execute(outputFilePath);

                Log.Info("\r\nTransformation complete!");
                Log.Info($"\r\nThe output file has been created at [{outputPath}]");
                System.Console.WriteLine("Press any key to exit...");
                System.Console.ReadKey();
            }
            catch (Exception e)
            {
                Log.Error("\r\nThere was an error during the transformation.  See the following exception...");
                Log.Error(e);
                System.Console.ReadKey();
            }
        }
    }
}
