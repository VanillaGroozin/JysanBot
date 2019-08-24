using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JysanBot
{
    public static class EnvironmentVariables
    {
        public const string BotToken =
            "938339701:AAEwiQPszzLIIuYek_2-HSFnM-82lFKfN8w";
        public const string DatabaseUrl =
            "JysanBot.db";
        public static string LastPrintedMessage = string.Empty;
        public static string LastInputMessage = string.Empty;
        public static bool ShowLastPrintedMessage = false;
        public static string MessagePath = string.Empty;
    }
}
