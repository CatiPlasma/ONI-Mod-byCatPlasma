using System;
using System.Reflection;
using System.Threading;

namespace CatUtilLib
{
    public static class LogUtil
    {
        public static void Info(string msg)
        {
            Console.WriteLine($"{CurrentTIme()} [INFO] [{ModName}]: {msg}");
        }

        public static void Debug(string msg)
        {
            if (CatUtils.isDebug) Console.WriteLine($"{CurrentTIme()} [DEBUG] [{ModName}]: {msg}");
        }

        public static void Warn(string msg)
        {
            Console.Write($"{CurrentTIme()} [WARN] [{ModName}]: {msg}");
        }

        private static string CurrentTIme()
        {
            return
                $"[{TimeZoneInfo.ConvertTimeToUtc(System.DateTime.Now):HH:mm:ss.fff}] [{Thread.CurrentThread.ManagedThreadId}]";
        }
        
        private static readonly string ModName = Assembly.GetExecutingAssembly().GetName().Name[..^8];
    }
}