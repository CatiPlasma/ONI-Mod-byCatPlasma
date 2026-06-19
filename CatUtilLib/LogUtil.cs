using System;
using System.Reflection;
using System.Threading;

namespace CatUtilLib
{
    public static class LogUtil
    {
        public static void Info(string msg)
        {
            Console.WriteLine($"{CurrentTIme()} [INFO] [{Assembly.GetCallingAssembly().GetName().Name}]: {msg}");
        }

        public static void Debug(string msg)
        {
            Console.WriteLine($"{CurrentTIme()} [DEBUG] [{Assembly.GetCallingAssembly().GetName().Name}]: {msg}");
        }

        private static string CurrentTIme()
        {
            return
                $"[{TimeZoneInfo.ConvertTimeToUtc(System.DateTime.Now).ToString("HH:mm:ss.fff")}] [{Thread.CurrentThread.ManagedThreadId}]";
        }
    }
}