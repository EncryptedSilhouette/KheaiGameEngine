using KheaiGameEngine.Extensions;

namespace KheaiGameEngine.Core
{
    ///<summary>A simple game engine that loops over a collection of IKEngineObjects. Calling each one's Update and FrameUpdate method, untill stopped.</summary>
    public class KEngine 
    {
        private uint _updateTarget;
        private IKQueuedOnlyCollection<IKEngineObject> _engineObjects = new KSortedQueuedList<IKEngineObject>(new KEngineObjectComparer<IKEngineObject>());

        ///<summary>The IKEngineObjects attached with this engine.</summary>
        public IEnumerable<IKEngineObject> EngineObjects => _engineObjects;
        ///<summary>Refrence to the renderer for this engine.</summary>
        public IKRenderer Renderer { get; init; }
        ///<summary>Represents the running state of the Engine</summary>
        public bool IsRunning { get; private set; } = false;
        ///<summary>The time interval in milliseconds between updates.</summary>
        public double UpdateInterval { get; private set; } = 0;

        ///<summary>The target number of updates in a second.</summary>
        public uint UpdateRateTarget
        {
            get => _updateTarget;
            private set => UpdateInterval = 1000d / value;
        }

        ///<summary>Creates a new KEngine instance. Requires a renderer and a target update rate for the game-loop.</summary> 
        ///<param name = "updateTarget">The target framerate.</param>
        ///<param name = "renderer">The renderer for the application.</param>
        public KEngine(IKRenderer renderer, uint updateTarget = 30) => (Renderer, UpdateRateTarget) = (renderer, updateTarget);

        public KEngine(IKRenderer renderer, IEnumerable<IKEngineObject> kEngineObjects, uint updateTarget = 30) :
            this(renderer, updateTarget) => _engineObjects.QueueAddAll(kEngineObjects);

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
            _engineObjects.ForEach(value => value.Start());
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
                    _engineObjects.UpdateContents();
                    _engineObjects.ForEach(kEngineObject => kEngineObject.Update(currentUpdate));
                }
                while (unprocessedTime > UpdateInterval && IsRunning);

                //Frame update & render frame
                _engineObjects.ForEach(kEngineObject => kEngineObject.FrameUpdate(currentUpdate));
                Renderer.RenderFrame();
            }
            _engineObjects.ForEach(value => value.End());
        }

        //Only exists to add simplicity.
        //I think it would be odd to call Start to start the engine but change a boolean to stop it.
        //A start method with an accompanying stop method makes more sense i think.
        ///<summary>Finishes the current interation's updates and stops the engine.</summary>
        public void Stop() => IsRunning = false;
    }
}