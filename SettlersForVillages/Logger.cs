using System;
using System.IO;
using TaleWorlds.Core;

namespace SettlersForVillages
{
    public static class Logger
    {
        private static string LOGGING_PATH = "./../../Modules/SettlersForVillages/ErrorLogs/";
        private static string FILEPATH = "./../../Modules/SettlersForVillages/ErrorLogs/Errors.log";

        static Logger()
        {
            if (!Directory.Exists(LOGGING_PATH)) Directory.CreateDirectory(LOGGING_PATH);
            if (!File.Exists(FILEPATH)) File.Create(FILEPATH);
        }

        public static void log(string log)
        {
            using (StreamWriter streamWriter = new StreamWriter(FILEPATH, true))
                streamWriter.WriteLine(log);
        }

        public static void logError(Exception exception)
        {
            log("Message " + exception.Message);
            log("Error at " + exception.Source.ToString() + " in function " + exception.Message);
            log("With stacktrace :\n" + exception.StackTrace);
            log("----------------------------------------------------");
        }

        public static void DisplayInfoMsg(string message)
        {
            InformationManager.DisplayMessage(new InformationMessage(message));
        }
    }
}