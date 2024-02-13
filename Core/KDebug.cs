using KheaiGameEngine.Core;
using SFML.Graphics;
using System.Text;

namespace KheaiGameEngine
{
    ///<summary>
    ///Debug component for the KEngine. Contains static methods for managing debug logs.
    ///</summary>
    public class KDebugger : KEngineComponent
    {
        ///<summary>
        ///Constant id for the "debug" log.
        ///</summary>
        public const string DEBUG = "debug";
        ///<summary>
        ///Constant id for the "error" log.
        ///</summary>
        public const string ERROR = "error";

        public static bool submitToFile
#if DEBUG
            = true;
#else
            = false;
#endif
        public static string FileDirectory = "Debug";

        private static StringBuilder s_stringBuilder = new();
        private static Dictionary<string, List<string>> s_logs = new()
        {
            { DEBUG, new() },
            { ERROR, new() }
        };

        public static string CurrentDateTime => DateTime.Now.ToString();
        public static string CurrentDate => DateTime.Now.ToString("MM-dd-yy");
        public static string CurrentTime => DateTime.Now.ToString("HH:mm:ss");

        private byte _updatesThisCycle = 0;
        private long _timerStart = 0;

        public double SessionTime => (DateTime.Now.Ticks - StartTime) / TimeSpan.TicksPerSecond;
        public double AverageUpdateRate => Engine.CurrentTick / SessionTime;
        public byte UpdateRate { get; private set; } = 0;
        public byte MaxUpdatesPerSecond { get; private set; } = 0;
        public byte MinUpdatesPerSecond { get; private set; } = byte.MaxValue;
        public long StartTime { get; private set; } = 0;
        public long EndTime { get; private set; } = 0;

        public KDebugger()
        {
            Order = 0;
        }

        public override void Init()
        {

        }

        public override void Start()
        {
            StartTime = DateTime.Now.Ticks;
        }

        public override void End()
        {
            EndTime = DateTime.Now.Ticks;

            if (submitToFile)
            {
                DumpToFile();
            }
        }

        public override void Update(uint currentTick)
        {

        }

        public override void FrameUpdate(uint currentFrame)
        {
            _updatesThisCycle++;

            //The last few lines are to keep track of debug info.
            if ((DateTime.UtcNow.Ticks - _timerStart) / TimeSpan.TicksPerSecond >= 1)
            {
                UpdateRate = _updatesThisCycle;

                if (_updatesThisCycle >= MaxUpdatesPerSecond) MaxUpdatesPerSecond = _updatesThisCycle;
                if (_updatesThisCycle < MinUpdatesPerSecond) MinUpdatesPerSecond = _updatesThisCycle;

                _updatesThisCycle = 0;
                _timerStart = DateTime.UtcNow.Ticks;
            }
        }

        public void Draw(RenderTarget target)
        {

        }

        public void DumpToFile()
        {
            int count = 0;
            string startTime = new DateTime(StartTime).ToString();
            string endTime = new DateTime(EndTime).ToString();
            string sessionTime = new DateTime(StartTime - EndTime).ToString("dd:hh:mm:ss");
            string DebugFilePath;
            StreamWriter writer;

            Directory.CreateDirectory(FileDirectory);
            do
            {
                DebugFilePath = $"{FileDirectory}\\Debug_{CurrentDate}_{count}.txt";
                count++;
            }
            while (File.Exists(DebugFilePath));

            writer = new StreamWriter(File.Create(DebugFilePath));
            writer.WriteLine(CurrentDateTime);
            writer.WriteLine($"Start Time: {startTime}");
            writer.WriteLine($"End Time: {endTime}");
            writer.WriteLine($"Session time: {sessionTime}");
            writer.WriteLine($"Last tick: {Engine.CurrentTick}");
            writer.WriteLine($"Last Frame: {Engine.CurrentFrame}");
            writer.WriteLine($"Max Update Rate: {MaxUpdatesPerSecond}");
            writer.WriteLine($"Min Update Rate: {MinUpdatesPerSecond}");
            writer.WriteLine($"Average Update Rate: {AverageUpdateRate}");

            foreach (var logID in s_logs.Keys)
            {
                writer.WriteLine($"\n{GetLog(logID)}");
            }

            writer.Close();
        }

        public static void Log(string logID, string message)
        {
            if (!s_logs.ContainsKey(logID))
            {
                ErrorLog($"Log {logID} doesn't exist.\n\t" + message);
                return;
            }
            s_logs[logID].Add(message);
        }

        public static void DebugLog(string message)
        {
            Log(DEBUG, message);
        }

        public static void ErrorLog(string message)
        {
            Log(ERROR, $"{CurrentTime}, Err: {message}");
        }

        public static void ClearLog(string logID)
        {
            s_logs[logID].Clear();
        }

        public static IReadOnlyList<string> GetLog(string logID)
        {
            if (!s_logs.ContainsKey(logID))
            {
                ErrorLog($"Log {logID} doesn't exist.");
                return null;
            }
            return s_logs[logID];
        }

        public static IReadOnlyList<string> GetLog(string logID, ushort maxLines)
        {
            if (!s_logs.ContainsKey(logID))
            {
                ErrorLog($"Log {logID} doesn't exist.");
                return null;
            }

            //Gets the start index to return the specified amount of lines.
            int startIndex = s_logs[logID].Count - maxLines;

            if (startIndex < 0)
            {
                return s_logs[logID];
            }

            if (startIndex > s_logs[logID].Count - 1)
            {
                return new List<string>();
            }

            return s_logs[logID].Skip(startIndex).ToList();
        }
    }
}
