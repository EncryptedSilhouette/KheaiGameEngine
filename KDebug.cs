using System.Text;

namespace KheaiGameEngine
{
    public class KDebugLog
    {
        public int MaxLogLength = 0;

        protected Queue<string> _lines = new();

        public string ID {  get; private init; }

        public KDebugLog(string id) 
        {
            ID = id;    
        }

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
            { GENERAL, new KDebugLog(GENERAL) },
            { ERROR, new KDebugLog(ERROR) }
        };

        public static void AddLog(string logID)
        {
            _logs.Add(logID, new KDebugLog(logID));
        }

        public static string GetLog(string id)
        {
            return _logs[id].GetLog();
        }

        public static void RemoveLog(string id)
        {
            if (id == GENERAL || id == ERROR) return;
            _logs.Remove(id);
        }

        public static void Log(string message)
        {
            Log(GENERAL, message);
        }

        public static void Log(string id, string message)
        {
            _logs[id.ToLower()].Log(message);
        }

        public static void DumpLog()
        {
            string DebugPath = $"Logs/Debug0.txt";
            Directory.CreateDirectory("Logs");
            for (int i = 1; File.Exists(DebugPath); i++)
            {
                DebugPath = $"Logs/Debug{i}.txt";
            };

            StreamWriter writer = new StreamWriter(File.Create(DebugPath));
            foreach (KDebugLog log in _logs.Values)
            {
                writer.WriteLine($"{log.ID.ToUpper()}-");
                writer.WriteLine($"{log.GetLog()}");
            }
            writer.Close();
        }
    }
}
