using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Empire.Shared.Utilities;
using Postmark.Business;

namespace PostmarkUtils
{



    class Program
    {

        private static string LogFilePath
        {
            get
            {
                return ConfigReader.Mandatory.Read("LogFilePath");
            }
        }
        

        private static void Log(StreamWriter oStreamWriter,  string sMsg = "" )
        {
            string sLog = null;

            if (String.IsNullOrWhiteSpace(sMsg))
            {
                sLog = $"-  {DateTime.Now.ToDateFormatted()}";
            }
            else
            {

                sLog = sMsg;
            }
            Console.WriteLine(sLog);
            oStreamWriter.WriteLine(sLog);
        }

      
        static async Task Main(string[] args)
        {
            FileStream oFileStream = null;
            StreamWriter oStreamWriter = null;

            string sErrorMessage = "Cannot delete file";
            try
            {
                ProcessLogs();

                oFileStream = new FileStream($"{Program.LogFilePath}Log_{DateTime.Now.ToString("yyyyMMdd")}.log", FileMode.Append, FileAccess.Write);
                oStreamWriter = new StreamWriter(oFileStream);

                sErrorMessage = "Cannot open log file";
                Program.Log(oStreamWriter);

                var oServerFactory = new ServerFactory();

                var lstServers = await oServerFactory.List();

                lstServers.ToList().ForEach(async oServer => await Program.RecordBounces(oServer, oStreamWriter));


                Program.Log(oStreamWriter);

            }
            catch(IOException oIOException)
            {
                Console.WriteLine($"{sErrorMessage} - {oIOException.Message}");

                string sLog = $"Error : \n {oIOException.Message}";
                Console.WriteLine(sLog);

                if (oStreamWriter != null)
                {
                    Program.Log(oStreamWriter, sLog);
                    Program.Log(oStreamWriter);

                }

                //  oIOException.Process(sMethod: null, bStackTrace: false, bLog: false);

            }
            catch (Exception oEx)
            {

                string sLog = $"Error : \n {oEx.Message} @ \n {oEx.StackTrace}";
                Console.WriteLine(sLog);

                if (oStreamWriter != null)
                {
                    Program.Log(oStreamWriter, sLog);
                    Program.Log(oStreamWriter);
                }

              //  oEx.Process(sMethod: null, bStackTrace: false, bLog: false);

            }
            finally
            {
                if (oStreamWriter != null)
                {
                    Console.SetOut(oStreamWriter);
                    oStreamWriter.Close();
                    oStreamWriter.Dispose();
                }

                if (oFileStream != null)
                {
                    oFileStream.Close();
                    oFileStream.Dispose();
                }
            }
        }

        private static async Task RecordBounces(Server oServer, StreamWriter oWriter)
        {
            var oServerFactory = new ServerFactory();

            await oServerFactory.RecordBounces(oServer, oWriter);

            await oServerFactory.UpdateLastRun(oServer, oWriter);
        }

        private static void ProcessLogs()
        {
            string[] aFiles = Directory.GetFiles(Program.LogFilePath);
            aFiles.ToList().ForEach(sFile => Program.DeleteFile(sFile));

        }



        private static void DeleteFile(string sFile)
        {
            var oFileInfo = new FileInfo(sFile);
            DateTime dtUpdated = oFileInfo.LastWriteTime;

            int iLogFileDeleteSpan = ConfigReader.Default.Read("LogFileDeleteSpan", 7);

            if(dtUpdated.AddDays(iLogFileDeleteSpan) < DateTime.Now)
            {
                oFileInfo.Delete();
            }
        }

    }
}
