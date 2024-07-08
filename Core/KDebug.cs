using KheaiGameEngine.Core;
using SFML.Graphics;

namespace KheaiGameEngine.Debug
{
    ///<summary>/Debug component for the KEngine. Contains static methods for managing debug logs.</summary>
    public class KDebugger : KEngineComponent
    {
        #region Static 

        #region LogIDs

        ///<summary>Constant id for the "debug" log.</summary>
        public static readonly string DEBUG = "debug";

        ///<summary>Constant id for the "error" log.</summary>
        public static readonly string ERROR = "error";

        #endregion

        ///<summary>Whether or not to dump debug information to a .txt file when the end method is called.</summary>
        public static bool SubmitToFile = true;

        ///<summary>The file directory for debug text files.</summary>
        public static string FileDirectory = "Debug";

        ///<summary>Dictionary for containing text logs.</summary>
        protected static Dictionary<string, List<string>> logs = new()
        {
            { DEBUG, new() },
            { ERROR, new() }
        };

        ///<summary>Get the current date and time as a string.</summary>
        public static string CurrentDateTime => DateTime.UtcNow.ToString();

        ///<summary>Get the current date as a formatted string ("MM-dd-yy").</summary>
        public static string CurrentDate => DateTime.UtcNow.ToString("MM-dd-yy");

        ///<summary>Get the current time as a formatted string ("HH:mm:ss").</summary>
        public static string CurrentTime => DateTime.UtcNow.ToString("HH:mm:ss");

        #endregion

        #region Class data

        //Variables to keep track of updates per second
        private byte updatesThisCycle = 0;
        private long timerStart = 0;

        //Time related properties for debugging.
        ///<summary>Get the current time in ticks.</summary>
        public double SessionTime => DateTime.UtcNow.Ticks - StartTime;
        ///<summary>Gets the average updates per second.</summary>
        public double AverageUpdateRate => Engine.CurrentUpdate / (SessionTime / TimeSpan.TicksPerSecond);
        ///<summary>Get the current update rate.</summary>
        public byte UpdateRate { get; private set; } = 0;
        ///<summary>Gets the maximum updates in a cycle.</summary>
        public byte MaxUpdatesPerSecond { get; private set; } = 0;
        ///<summary>Gets the minimum updates in a cycle.</summary>
        public byte MinUpdatesPerSecond { get; private set; } = byte.MaxValue;
        ///<summary>Get the start time of this component.</summary>
        public long StartTime { get; private set; } = 0;
        ///<summary>Get the end time of this component.</summary>
        public long EndTime { get; private set; } = 0;

        #endregion

        #region Constructors 

        public KDebugger() => Order = 0;

        public KDebugger(bool submitToFile) : this() => SubmitToFile = submitToFile;

        public KDebugger(string filePath) : this(true) => FileDirectory = filePath;

        #endregion

        #region Logging

        ///<summary>Submit a message to a specified log.</summary>
        public static void Log(string logID, string message)
        {
            if (!logs.ContainsKey(logID))
            {
                ErrorLog($"Log {logID} doesn't exist.\n\t{message}");
                return;
            }
            logs[logID].Add(message);
        }

        ///<summary>Submit a message to the debug log.</summary>
        public static void DebugLog(string message) => Log(DEBUG, message);

        ///<summary>Submit a message to the error log.</summary>
        public static void ErrorLog(string message) => Log(ERROR, message);

        ///<summary>Clear a specified log.</summary>
        public static void ClearLog(string logID) => logs[logID].Clear();

        ///<summary>Get a specified log as an enumerable collection.</summary>
        public static IEnumerable<string> GetLog(string logID)
        {
            if (!logs.ContainsKey(logID))
            {
                ErrorLog($"Log {logID} doesn't exist.");
                return null;
            }
            return logs[logID];
        }

        ///<summary>Get a specified log as an enumerable collection.</summary>
        public static IEnumerable<string> GetLog(string logID, ushort maxLines) => 
            GetLog(logID)?.Skip(logs[logID].Count - maxLines);

        #endregion

        #region Logic 

        public override void Init() { }

        public override void Start() => StartTime = timerStart = DateTime.UtcNow.Ticks;

        public override void End()
        {
            EndTime = DateTime.UtcNow.Ticks;

            if (SubmitToFile) DumpToFile();
        }

        public override void Update(uint currentUpdate) { }

        public override void FrameUpdate(uint currentUpdate)
        {
            updatesThisCycle++;

            //The last few lines are to keep track of debug info.
            if ((DateTime.UtcNow.Ticks - timerStart) / TimeSpan.TicksPerSecond >= 1)
            {
                UpdateRate = updatesThisCycle;

                if (updatesThisCycle >= MaxUpdatesPerSecond) MaxUpdatesPerSecond = updatesThisCycle;
                if (updatesThisCycle < MinUpdatesPerSecond) MinUpdatesPerSecond = updatesThisCycle;

                updatesThisCycle = 0;
                timerStart = DateTime.UtcNow.Ticks;
            }
        }

        ///<summary>Draw a frame.</summary>
        public void Draw(RenderTarget target) { }

        #endregion

        ///<summary>Dump debug info to a .txt file. Override for custom implementation.</summary>
        public virtual void DumpToFile()
        {
            int logNum = 0;
            string path = $"{FileDirectory}\\Log_{CurrentDate}";
            StreamWriter writer;

            Directory.CreateDirectory(FileDirectory);

            while (File.Exists($"{path}({logNum})")) logNum++;

            writer = new(File.Create($"{path}({logNum})"));
            writer.WriteLine($"Start time: {new DateTime(StartTime).ToString("MM-dd-yy H:mm:ss")}");
            writer.WriteLine($"End time: {new DateTime(EndTime).ToString("MM-dd-yy H:mm:ss")}");
            writer.WriteLine($"AverageFrameRate: {AverageUpdateRate}");
            writer.WriteLine($"MaxFramesPerSecond: {MaxUpdatesPerSecond}");
            writer.WriteLine($"MinFramesPerSecond: {MinUpdatesPerSecond}");
            writer.WriteLine($"Last Frame: {Engine.CurrentUpdate}");

            foreach (var log in logs)
            {
                writer.WriteLine($"\n{log.Key}:");

                foreach (var line in log.Value) writer.WriteLine(line);
            }            

            writer.Close();
        }
    }
}
