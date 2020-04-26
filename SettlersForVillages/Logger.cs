using System;
using System.IO;
using TaleWorlds.Core;

namespace SettlersForVillages
{
    public static class Logger
    {
        private static string LOGGING_PATH = "./../../Modules/SettlersForVillages/Logs/";
        private static string FILE_ERROR_PATH = "./../../Modules/SettlersForVillages/Logs/Errors.log";
        private static string FILE_PATH = "./../../Modules/SettlersForVillages/Logs/Debug.log";

        static Logger()
        {
            if (!Directory.Exists(LOGGING_PATH)) Directory.CreateDirectory(LOGGING_PATH);
            if (!File.Exists(FILE_ERROR_PATH)) File.Create(FILE_ERROR_PATH);
            if (!File.Exists(FILE_PATH)) File.Create(FILE_PATH);
        }

        public static void logDebug(string log)
        {
            if (!Main.Settings.DebugMode) return;
            using (StreamWriter streamWriter = new StreamWriter(FILE_PATH, true))
                streamWriter.WriteLine(log);

            DisplayInfoMsg("DEBUG | " + log);
        }

        public static void logError(string log)
        {
            using (StreamWriter streamWriter = new StreamWriter(FILE_ERROR_PATH, true))
                streamWriter.WriteLine(log);
        }

        public static void logError(Exception exception)
        {
            logError("Message " + exception.Message);
            logError("Error at " + exception.Source.ToString() + " in function " + exception.Message);
            logError("With stacktrace :\n" + exception.StackTrace);
            logError("----------------------------------------------------");

            if (!Main.Settings.DebugMode) return;
            DisplayInfoMsg(exception.Message);
            DisplayInfoMsg(exception.Source);
            DisplayInfoMsg(exception.StackTrace);
        }

        public static void DisplayInfoMsg(string message)
        {
            InformationManager.DisplayMessage(new InformationMessage(message));
        }
    }
}