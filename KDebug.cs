using System.Text;

namespace KheaiGameEngine
{
    public class KDebugLog
    {
        public int MaxLogLength = 0;

        private Queue<string> _lines = new();

        public void Log(string message)
        {
            _lines.Enqueue(message);

            if (MaxLogLength > 0 && MaxLogLength < _lines.Count)
            {
                _lines.Dequeue();
            }
        }

        public string GetLog()
        {
            StringBuilder stringBuilder = new();

            foreach (string line in _lines)
            {
                stringBuilder.AppendLine(line);
            }
            return stringBuilder.ToString();    
        }

        public void ClearLog()
        {
            _lines.Clear();
        }
    }

    public static class KDebug
    {
        //Default log ids.
        public static readonly string GENERAL = "general";
        public static readonly string ERROR = "error";

        private static Dictionary<string, KDebugLog> _logs = new Dictionary<string, KDebugLog>()
        {
            { "general", new KDebugLog() },
            { "error", new KDebugLog() }
        };

        public static void AddLog(string logID)
        {
            _logs.Add(logID, new KDebugLog());
        }

        public static void RemoveLog(string logID)
        {
            if (logID == GENERAL || logID == ERROR) return;
            _logs.Remove(logID);
        }

        public static void Log(string message)
        {
            Log(GENERAL, message);

        }

        public static void Log(string logID, string message)
        {
            _logs[logID].Log(message);
        }

        public static void DumpLog()
        {
            string DebugPath = "Logs//Debug0.txt";
            for (int i = 1; File.Exists(DebugPath); i++)
            {
                DebugPath = $"Logs//Debug{i}.txt";
            };
            StreamWriter writer = new StreamWriter(File.Create(DebugPath));

            foreach (KDebugLog log in _logs.Values)
            {
                writer.WriteLine(log.ToString());
            }
        }
    }
}
