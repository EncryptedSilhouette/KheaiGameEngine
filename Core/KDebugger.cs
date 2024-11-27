using System.Text;

namespace KheaiGameEngine.Core
{
    ///<summary>Debugger for the KEngine. Contains static methods for managing logs.</summary>
    public sealed class KDebugger : IKEngineObject
    {
        ///<summary>Constant ID for the debug log.</summary>
        public const string DEBUG_LOG = "debug";
        ///<summary>Constant ID for the error log.</summary>
        public const string ERROR_LOG = "error";

        #region Static 

        ///<summary>Dictionary for containing logs as individual StringBuilders.</summary>
        private static Dictionary<string, StringBuilder> logs = new(4) { { DEBUG_LOG, new(512) }, { ERROR_LOG, new(512) } };

        ///<summary>The current date and time as a string.</summary>
        public static string CurrentDateTime => DateTime.UtcNow.ToString();
        ///<summary>The current date as a formatted string ("MM-dd-yy").</summary>
        public static string CurrentDate => DateTime.UtcNow.ToString("MM-dd-yy");
        ///<summary>The current time as a formatted string ("HH:mm:ss").</summary>
        public static string CurrentTime => DateTime.UtcNow.ToString("HH:mm:ss");

        ///<summary>Submit a message to a specified log.</summary>
        ///<param name = "logID">The ID of the target log.</param>
        public static void CreateLog(string logID) => logs.Add(logID, new StringBuilder(512));

        ///<summary>Clear a specified log.</summary>
        ///<param name = "logID">The ID of the target log.</param>
        public static void ClearLog(string logID) => logs[logID].Clear();

        ///<summary>Retrieves a specified log as a string.</summary>
        ///<param name = "logID">The ID of the target log.</param>
        public static string GetLog(string logID)
        {
            if (!logs.ContainsKey(logID))
            {
                ErrorLog($"Log {logID} doesn't exist.");
                return string.Empty;
            }
            else return logs[logID].ToString();
        }

        ///<summary>Submit a message to a specified log.</summary>
        ///<param name = "logID">The ID of the target log.</param>
        ///<param name = "message">The message to submit to the log.</param>
        public static void Log(string logID, string message)
        {
#if DEBUG
            Console.WriteLine(message);
#elif RELEASE
            if (logs.ContainsKey(logID)) logs[logID].AppendLine(message);
            else ErrorLog($"Log {logID} doesn't exist.");
#endif
        }

        ///<summary>Submit a message to the debug log.</summary>
        ///<param name = "message">The message to submit to the log.</param>
        public static void DebugLog(string message) => Log(DEBUG_LOG, message);

        ///<summary>Submit a message to the error log.</summary>
        ///<param name = "message">The message to submit to the log.</param>
        public static void ErrorLog(string message) => Log(ERROR_LOG, message);

        #endregion

        //Variables to keep track of preformance.
        private byte updateRateCounter = 0, frameRateCounter = 0;
        private uint updates = 0, frames = 0;
        private long timerStart = 0;

        ///<summary>The file directory for log text files.</summary>
        public static string FileDirectory = "Debug";

        ///<summary>Fires when the start method is called.</summary>
        public event Action<KDebugger>? OnDebugStart;
        ///<summary>Fires when the end method is called</summary>
        public event Action<KDebugger>? OnDebugEnd;
        ///<summary>Fires every update.</summary>
        public event Action<KDebugger>? OnDebugUpdate;
        ///<summary>Fires every frame update.</summary>
        public event Action<KDebugger>? OnDebugFrameUpdate;

        //Implemented from IKEngineObject
        public bool Enabled => true;
        public int Order { get; init; }
        public string ID { get; init; }

        ///<summary>The current elapsed time in ticks.</summary>
        public double SessionTime => DateTime.UtcNow.Ticks - StartTime;
        ///<summary>The average update rate.</summary>
        public double AverageUpdateRate => UpdateRate / SessionTime / TimeSpan.TicksPerSecond;
        ///<summary>The average frame rate.</summary>
        public double AverageFrameRate => FrameRate / SessionTime / TimeSpan.TicksPerSecond;
        ///<summary>The current update rate.</summary>
        public byte UpdateRate { get; private set; } = 0;
        ///<summary>The current frame rate.</summary>
        public byte FrameRate { get; private set; } = 0;
        ///<summary>The start time in ticks.</summary>
        public long StartTime { get; private set; } = 0;

        public KDebugger(bool dumpToFileOnEnd = true)
        {
            Order = -1;
            ID = GetType().Name;
            OnDebugStart += (i) => StartTime = timerStart = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;

            //if (dumpToFileOnEnd) OnDebugEnd += (i) => DumpLogsToFile();
        }

        public void Init<TParent>(TParent parent) { /*ignore, may be used in the future.*/ } 

        public void Start() => OnDebugStart?.Invoke(this);

        public void End() => OnDebugEnd?.Invoke(this);

        public void Update(ulong currentUpdate)
        {
            updates++;
            updateRateCounter++;
            OnDebugUpdate?.Invoke(this);
        }

        public void FrameUpdate(ulong currentUpdate)
        {
            //Track frames
            frames++;
            frameRateCounter++;

            //Get update/frame rate and reset timer
            if ((DateTime.UtcNow.Ticks - timerStart) / TimeSpan.TicksPerSecond >= 1)
            {
                UpdateRate = updateRateCounter;
                FrameRate = frameRateCounter;
                updateRateCounter = frameRateCounter = 0;
                timerStart = DateTime.UtcNow.Ticks;
            }
            OnDebugFrameUpdate?.Invoke(this);
        }

        ///<summary>TODO.</summary>
        /*public void DumpLogsToFile()
        {
            string path = $"{FileDirectory}\\Log_{CurrentDateTime}";
            StreamWriter writer;

            Directory.CreateDirectory(FileDirectory);

            writer = new(File.Create($"{path}"));
            writer.WriteLine($"Session time: {SessionTime / TimeSpan.TicksPerHour} hours");
            writer.WriteLine($"Average update rate: {AverageUpdateRate}");
            writer.WriteLine($"Average frame rate: {AverageFrameRate}");
            writer.WriteLine($"Last update: {updates}\n");

            foreach (var logKV in logs)
            {
                writer.WriteLine(logKV.Key);
                writer.WriteLine($"{logKV.Value.ToString()}\n");
            }
            writer.Close();
        }*/
    }
}
