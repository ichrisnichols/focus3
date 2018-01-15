using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;

namespace Focus3.Transformer.Console
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            //var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            var logRepository = LogManager.CreateRepository(
                Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
            XmlConfigurator.Configure(logRepository, new FileInfo("log.config"));

            // TODO: add unit tests for CsvTransformer

            var inputFilePath = "";
            var csTransform = new CsvTransform.CS.Transform(inputFilePath);
            var transformer = new CsvTransformer.CsvTransformer(csTransform);
            transformer.Execute();

            System.Console.WriteLine("Hello World!");
            Log.Debug("Hello World Success!");
            System.Console.ReadKey();
        }
    }
}
