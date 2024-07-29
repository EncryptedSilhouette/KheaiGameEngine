using System.Text;

namespace KheaiGameEngine.Debug
{ 
    ///<summary>/Debug component for the KEngine. Contains static methods for managing debug logs.</summary>
    public class KDebugger : IKEngineComponent
    {
        protected record class KTextLog(ushort maxLength, StringBuilder StringBuilder);

        #region Static 

        ///<summary>Constant id for the debug log.</summary>
        public static readonly ushort DEBUG_LOG = 0;

        ///<summary>Constant id for the error log.</summary>
        public static readonly ushort ERROR_LOG = 1;

        ///<summary>Whether or not the program will dump debug information to a .txt file when the end method is called.</summary>
        public static bool DumpToFile = true;

        ///<summary>The file directory for debug text files.</summary>
        public static string FileDirectory = "Debug";

        ///<summary>Dictionary for containing text logs.</summary>
        protected static List<KTextLog> logs = new(4)
        {
            //Debug log (index 0), Error log (index 1)
            new KTextLog(512, new StringBuilder(512)), 
            new KTextLog(512, new StringBuilder(512))
        };

        ///<summary>Get the current date and time as a string.</summary>
        public static string CurrentDateTime => DateTime.UtcNow.ToString();

        ///<summary>Get the current date as a formatted string ("MM-dd-yy").</summary>
        public static string CurrentDate => DateTime.UtcNow.ToString("MM-dd-yy");

        ///<summary>Get the current time as a formatted string ("HH:mm:ss").</summary>
        public static string CurrentTime => DateTime.UtcNow.ToString("HH:mm:ss");

        ///<summary>Submit a message to a specified log. Returns the log id.</summary>
        ///<param name="maxLength">The max character count for a log.</param>
        public static int AddLog(ushort maxLength) 
        {            
            logs.Add(new KTextLog(maxLength, new StringBuilder(maxLength)));
            //returns the index of the most recently added log (this one)
            return logs.Count - 1;
        }

        ///<summary>Clear a specified log.</summary>
        ///<param name="logID">The id of the log to clear.</param>
        public static void ClearLog(ushort logID) => logs[logID].StringBuilder.Clear();

        ///<summary>Retrieves a specified log as an enumerable collection.</summary>
        ///<param name="logID">The id of the log to retrieve.</param>
        public static string GetLog(ushort logID)
        {
            if (logs.Capacity < logID)
            {
                ErrorLog($"Log {logID} doesn't exist.");
                return null;
            }
            return logs[logID].ToString();
        }

        ///<summary>Submit a message to a specified log.</summary>
        ///<param name="logID">The id of the target log.</param>
        ///<param name="message">The message to submit to the log.</param>
        public static void Log(ushort logID, string message)
        {
            if (logs.Capacity < logID)
            {
                ErrorLog($"Log {logID} doesn't exist.\n\t{message}");
                return;
            }

            ushort overflowBuffer = 512;
            KTextLog log;

            log = logs[logID];
            log.StringBuilder.AppendLine(message);

            if (log.maxLength == 0) return;

            //Checks if the length of the string builder is greater than the max length plus half the overflow buffer. 
            //Culls any text beyond the max amount of characters in FIFO order.
            if (log.StringBuilder.Length > log.maxLength + overflowBuffer / 2)
            {
                //Cull text and resize capacity
                log.StringBuilder.Remove(0, log.StringBuilder.Length - log.maxLength);
                log.StringBuilder.EnsureCapacity(log.maxLength + overflowBuffer);
            }
        }

        ///<summary>Submit a message to the debug log.</summary>
        ///<param name="message">The message to submit to the log.</param>
        public static void DebugLog(string message) => Log(DEBUG_LOG, message);

        ///<summary>Submit a message to the error log.</summary>
        ///<param name="message">The message to submit to the log.</param>
        public static void ErrorLog(string message) => Log(ERROR_LOG, message);

        #endregion

        //Variables to keep track of preformance.
        protected byte updateRateCounter = 0;
        protected byte frameRateCounter = 0;
        protected uint updates = 0;
        protected uint frames = 0;
        protected long timerStart = 0;

        //Implemented from KEngineComponent.
        public string ID { get; init; }
        public short Order { get; init; }
        public KEngine Engine { get; set; }

        ///<summary>Fires when component is initalized.</summary>
        public event Action<KDebugger> OnDebugInit;
        ///<summary>Fires when component is started.</summary>
        public event Action<KDebugger> OnDebugStart;
        ///<summary>Fires when component's execution ends.</summary>
        public event Action<KDebugger> OnDebugEnd;
        ///<summary>Fires when the component is updated.</summary>
        public event Action<KDebugger> OnDebugUpdate;
        ///<summary>Fires when the component is updated for drawing.</summary>
        public event Action<KDebugger> OnDebugFrameUpdate;

        //Time related properties for debugging.
        ///<summary>The current time in ticks.</summary>
        public double SessionTime => DateTime.UtcNow.Ticks - StartTime;
        ///<summary>The average update rate.</summary>
        public double AverageUpdateRate => UpdateRate / SessionTime / TimeSpan.TicksPerSecond;
        ///<summary>The average frame rate.</summary>
        public double AverageFrameRate => FrameRate / SessionTime / TimeSpan.TicksPerSecond;
        ///<summary>The current update rate.</summary>
        public byte UpdateRate { get; private set; } = 0;
        ///<summary>The current frame rate.</summary>
        public byte FrameRate { get; private set; } = 0;
        ///<summary>The start time of this component.</summary>
        public long StartTime { get; private set; } = 0;
        ///<summary>The end time of this component.</summary>
        public long EndTime { get; private set; } = 0;

        public KDebugger()
        {
            ID = GetType().Name;
            Order = -1;   
            StringBuilder sb = new StringBuilder();

            //Set the start time and start the timer to track updates/frames every second.
            OnDebugStart += (ignored) => StartTime = timerStart = DateTime.UtcNow.Ticks;
            //Set the end time.
            OnDebugEnd += (ignored) => EndTime = DateTime.UtcNow.Ticks;
            //Dump debug logs to file if true.
            if (DumpToFile) OnDebugEnd += (ignored) => DumpLogsToFile();
        }

        public virtual void Init() => OnDebugInit?.Invoke(this);

        public virtual void Start() => OnDebugStart?.Invoke(this);

        public virtual void End() => OnDebugEnd?.Invoke(this);

        public virtual void Update(uint currentUpdate) 
        {
            //Track updates.
            updates++;
            updateRateCounter++;
            OnDebugUpdate.Invoke(this);
        }

        public virtual void FrameUpdate(uint currentUpdate)
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

        ///<summary>Dump debug info to a .txt file. Override for custom implementation.</summary>
        public virtual void DumpLogsToFile()
        {
            int logNum = 0;
            string path = $"{FileDirectory}\\Log_{CurrentDate}";
            StreamWriter writer;

            Directory.CreateDirectory(FileDirectory);

            while (File.Exists($"{path}({logNum})")) logNum++;

            writer = new(File.Create($"{path}({logNum})"));
            writer.WriteLine($"Start time: {new DateTime(StartTime).ToString("MM-dd-yy H:mm:ss")}");
            writer.WriteLine($"End time: {new DateTime(EndTime).ToString("MM-dd-yy H:mm:ss")}");
            writer.WriteLine($"Average update rate: {AverageUpdateRate}");
            writer.WriteLine($"Average frame rate: {AverageFrameRate}");
            writer.WriteLine($"Last frame: {updates}\n");

            for (int i = 0; i < logs.Count; i++)
            {
                if (i == 0) writer.WriteLine("Debug Log");
                if (i == 1) writer.WriteLine("Error Log");
                writer.Write($"{logs[i].StringBuilder.ToString()}\n");
            }      
            writer.Close();
        }
    }
}
