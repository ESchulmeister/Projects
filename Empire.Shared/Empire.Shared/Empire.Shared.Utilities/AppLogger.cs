using log4net;

namespace Empire.Shared.Utilities
{
    public static class AppLogger
    {
        public const string HeaderEntry = "---------------------------------------------------------------------------";

        public static void info(string information)
        {
            try
            {
                // Configure logging and write to log
                log4net.Config.XmlConfigurator.Configure();
                ILog logger = LogManager.GetLogger("FieldViewLogger");
                if (logger.IsDebugEnabled)
                {
                    WriteToFile(logger, information);
                }
            }
            finally { }
        }

        private static void WriteToFile(ILog logger, string logString)
        {
            logger.Debug(logString.XssSanitize());
        }
    }
}
