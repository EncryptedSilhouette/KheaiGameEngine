using KheaiGameEngine.Extensions;

namespace KheaiGameEngine.Core
{
    ///<summary>A simple game engine that loops over a collection of IKEngineObjects. Calling each one's Update and FrameUpdate method, untill stopped.</summary>
    public class KEngine 
    {
        private uint _updateTarget;

        //The following 2 events are self explanatory but serve an additional purpose.
        //They allow IKEngineObjects to be added and removed at the start and end of the engine's runtime.
        //This is useful for a number of cases, one being a situation where the engine needs to restart.
        ///<summary>Fires when the start method is called.</summary>
        public event Action<KEngine>? OnStart;
        ///<summary>Fires when the stop method is called.</summary>
        public event Action<KEngine>? OnEnd;

        ///<summary>Refrence to the renderer for this engine.</summary>
        public IKRenderer Renderer { get; init; }
        ///<summary>A sorted collection of IKEngineObjects that can have its changes queued instead of immediate.</summary>
        public KSortedQueuedList<IKEngineObject> EngineObjects { get; init; } = new(new KEngineObjectComparer<IKEngineObject>());

        ///<summary>Represents the running state of the Engine</summary>
        public bool IsRunning { get; private set; } = false;
        ///<summary>The time interval in milliseconds between updates.</summary>
        public double UpdateInterval { get; private set; } = 0;

        ///<summary>The target number of updates in a second.</summary>
        public uint UpdateRateTarget
        {
            get => _updateTarget;
            private set
            {
                _updateTarget = value;
                UpdateInterval = 1000d / value;
            }
        }

        ///<summary>Creates a new KEngine instance. Requires a renderer and a target update rate for the game-loop.</summary> 
        ///<param name = "updateTarget">The target framerate.</param>
        ///<param name = "renderer">The renderer for the application.</param>
        public KEngine(IKRenderer renderer, uint updateTarget = 30)
        {
            (Renderer, UpdateRateTarget) = (renderer, updateTarget);
            OnStart += engine => IsRunning = true;
            OnStart += engine => EngineObjects.ForEach(kEngineObject => kEngineObject.Start());
            OnEnd += engine => EngineObjects.ForEach(kEngineObject => kEngineObject.End());
        }

        public KEngine(IKRenderer renderer, IEnumerable<IKEngineObject> kEngineObjects, uint updateTarget = 30) : 
            this(renderer, updateTarget) => EngineObjects.AddAll(kEngineObjects);

        ///<summary>Executes starting tasks and starts the game-loop.</summary>
        public void Start()
        {
            //Prevents engine from starting if it's already running.
            if (IsRunning) return;

            //Timing variables.
            uint currentUpdate = 0;
            double lastTime, newTime, unprocessedTime = 0;

            //set the engine's state to running, then executes starting tasks.
            //Updates the contents of IKEngineObjects, in the case that any starting tasks have made changes.
            OnStart?.Invoke(this);
            EngineObjects.UpdateContents();

            lastTime = DateTime.UtcNow.Ticks; //Required for loop timing.

            while (IsRunning) //Game loop
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
                    EngineObjects.ForEach(kEngineObject => kEngineObject.Update(currentUpdate));
                }
                while (unprocessedTime > UpdateInterval && IsRunning);

                //Frame update & render frame
                EngineObjects.ForEach(kEngineObject => kEngineObject.FrameUpdate(currentUpdate));
                Renderer.RenderFrame();
            }

            //Executes ending tasks.
            //Updates the contents of kEngineObjects, in the case that any ending tasks have made changes.
            OnEnd?.Invoke(this);
            EngineObjects.UpdateContents();
        }

        //Only exists to add simplicity.
        //I think it would be odd to call Start to start the engine but change a boolean to stop it.
        //A start method with an accompanying stop method makes more sense i think.
        ///<summary>Finishes the current interation's updates and stops the engine.</summary>
        public void Stop() => IsRunning = false;
    }
}