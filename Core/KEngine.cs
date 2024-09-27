using KheaiGameEngine.Extensions;
using SFML.Graphics;
using SFML.Window;

namespace KheaiGameEngine.Core
{
    ///<summary>A simple game engine that loops over a collection of IKObjects. Calling each one's Update and FrameUpdate method, untill stopped.</summary>
    public class KEngine 
    {
        private bool _isRunning = false;
        private uint _updateRateTarget;
        private double _updateInterval;
        private IKRenderer _renderer;
        private RenderWindow _renderWindow;

        ///<summary>Fires when the start method is called.</summary>
        public event Action<KEngine> OnStart;

        ///<summary>Fires when the stop method is called.</summary>
        public event Action<KEngine> OnEnd;

        ///<summary>Represents the running state of the Engine</summary>
        public bool IsRunning => _isRunning; 

        ///<summary>The time interval in milliseconds between updates.</summary>
        public double UpdateInterval => _updateInterval;

        ///<summary>The reference to the render window.</summary>
        public RenderWindow Window => _renderWindow;

        ///<summary>Refrence to the renderer for this engine.</summary>
        public IKRenderer Renderer => _renderer;

        ///<summary>A sorted collection of IKObjects that can have its changes queued instead of immediate.</summary>
        public KSortedQueuedList<IKObject> KObjects { get; init; } = new();

        ///<summary>The target number of updates in a second.</summary>
        public uint UpdateRateTarget
        {
            get => _updateRateTarget;
            private set
            {
                _updateRateTarget = value;
                _updateInterval = 1000d / value;
            }
        }

        ///<summary>Creates a new KEngine instance. Requires a renderer and a target update rate for the game-loop.</summary> 
        ///<param name = "updateRateTarget">The target framerate.</param>
        ///<param name = "renderer">The renderer for the application.</param>
        public KEngine(IKRenderer renderer, byte updateRateTarget = 30)
        {
            (_renderer, UpdateRateTarget, _renderWindow) = (renderer, updateRateTarget, new(VideoMode.DesktopMode, ""));
            OnStart += engine => KObjects.ForEach(kObject => kObject.Start());
            OnEnd += engine => KObjects.ForEach(kObject => kObject.End());
        }

        ///<summary>Executes starting tasks and starts the game-loop.</summary>
        public void Start()
        {
            //Prevents engine from starting if it's already running.
            if (_isRunning) return;

            //Timing variables.
            uint currentUpdate = 0;
            double lastTime, newTime, unprocessedTime = 0;

            //set the engine's state to running, then executes starting tasks.
            _isRunning = true;
            OnStart?.Invoke(this);
            lastTime = DateTime.UtcNow.Ticks; //Required for loop timing.

            while (IsRunning) //Core game-loop
            {
                newTime = DateTime.UtcNow.Ticks;
                unprocessedTime += (newTime - lastTime) / TimeSpan.TicksPerMillisecond;
                lastTime = newTime;

                //Checks if the unprocessedTime is greater than the UpdateInterval
                if (unprocessedTime < UpdateInterval)
                {
                    //TODO Test this; maybe this will be good practice?
                    //Thread.Sleep((int) (UpdateInterval - unprocessedTime));
                    continue;
                }

                //Loops to compensate for any lag, skipping pre-draw and render tasks. 
                do
                {
                    currentUpdate++;
                    unprocessedTime -= UpdateInterval;
                    KObjects.ForEach(kObject => kObject.Update(currentUpdate));
                }
                while (unprocessedTime > UpdateInterval && _isRunning);

                //Frame update & render frame
                KObjects.ForEach(kObject => kObject.FrameUpdate(currentUpdate));
                Renderer.Render(Window);
            }
            OnEnd?.Invoke(this);
        }

        //Only exists to add simplicity.
        //I think it would be odd to call Start to start the engine but change a boolean to stop it.
        //A start method with an accompanying stop method makes more sense i think.
        ///<summary>Finishes the current interation's updates and stops the engine.</summary>
        public void Stop() => _isRunning = false;
    }
}