using System.Text;

namespace KheaiGameEngine
{
    public class KDebugLog
    {
        int logOrder;
    }

    public static class KDebug
    {
        private static List<StringBuilder> logs = new List<StringBuilder>();

        static void Log()
        {

        }

        static void LogErr()
        {

        }

        static void DumpLog()
        {
            string DebugPath = "Logs//Debug0.txt";
            for (int i = 1; File.Exists(DebugPath); i++)
            {
                DebugPath = $"Logs//Debug{i}.txt";
            };
            FileStream stream = File.Create(DebugPath);
        }
    }
}
