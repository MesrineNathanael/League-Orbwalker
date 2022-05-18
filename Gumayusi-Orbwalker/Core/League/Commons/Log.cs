using System;
using System.Diagnostics;

namespace Gumayusi_Orbwalker.Core.League.Commons
{
    public class Log
    {
        public static void Write(string message)
        {
            Trace.WriteLine(message);
        }

        public static void WriteInfo(string message)
        {
            Trace.WriteLine(GetTime() + " [INFO] " + message);
        }

        public static void WriteError(string message)
        {
            Trace.WriteLine(GetTime() + " [ERROR] " + message);
        }

        public static void WriteWarning(string message)
        {
            Trace.WriteLine(GetTime() + " [WARNING] " + message);
        }

        public static void WriteDebug(string message)
        {
#if DEBUG
            Trace.WriteLine(GetTime() + " [DEBUG] " + message);
#endif
        }

        public static void WriteException(Exception ex)
        {
            Trace.WriteLine(GetTime() + " [EXCEPTION] " + ex.Message);
        }

        public static void CarriageReturn()
        {
            Trace.WriteLine("");
        }

        private static string GetTime()
        {
            return DateTime.Now.ToString("HH:mm:ss:fff");
        }
    }
}
