using KheaiGameEngine.Extensions;

namespace KheaiGameEngine.Core
{
    ///<summary>A simple game engine that loops over a collection of IKEngineObjects. Calling each one's Update and FrameUpdate method, untill stopped.</summary>
    public class KEngine 
    {
        private bool _isRunning = false;
        private uint _updateTarget = 30;

        protected KSortedQueuedList<IKEngineObject> _engineObjects = new(new KEngineObjectComparer<IKEngineObject>());

        ///<summary>Represents the running state of the Engine.</summary>
        public bool IsRunning => _isRunning;
        ///<summary>The renderer for the engine.</summary>
        public IKRenderer Renderer { get; init; }
        ///<summary>The time interval in milliseconds between updates.</summary>
        public double UpdateInterval { get; private set; } = 0;

        ///<summary>The target number of updates in a second.</summary>
        public uint UpdateRateTarget
        {
            get => _updateTarget;
            private set => UpdateInterval = 1000d / (_updateTarget = value);
        }

        public KEngine(IKRenderer renderer, uint updateTarget = 30) => 
            (Renderer, UpdateRateTarget) = (renderer, updateTarget);

        public KEngine(IKRenderer renderer, IEnumerable<IKEngineObject> engineObjects, uint updateTarget = 30) : 
            this(renderer, updateTarget) =>
            _engineObjects.AddAll(engineObjects);

        ///<summary>TODO.</summary>
        protected virtual void Load() { }

        ///<summary>TODO.</summary>
        protected virtual void Update(ulong currentUpdate)
        {
            _engineObjects.UpdateContents();
            _engineObjects.ForEach(kEngineObject => kEngineObject.Update(currentUpdate));
        }

        ///<summary>TODO.</summary>
        protected virtual void FrameUpdate(ulong currentUpdate) => _engineObjects.ForEach(kEngineObject => kEngineObject.FrameUpdate(currentUpdate));

        ///<summary>Executes starting tasks and starts the game-loop.</summary>
        public virtual int Start()
        {
            //Prevents engine from starting if it's already running.
            if (IsRunning)
            {
                KDebugger.ErrorLog("Engine err: Cannot start, this engine instance is already running.");
                return 1;
            }

            //Timing variables.
            uint currentUpdate = 0;
            double lastTime, newTime, unprocessedTime = 0;

            _isRunning = true;
            _engineObjects.ForEach(value => value.Start());
            _engineObjects.OnInsertion += value =>
            {
                value.Init(this);
                value.Start();
            };
            _engineObjects.OnRemoved += value => value.End();
            _engineObjects.UpdateContents();

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
                    Update(currentUpdate);
                    currentUpdate++;
                    unprocessedTime -= UpdateInterval;
                }
                while (unprocessedTime > UpdateInterval && IsRunning);

                FrameUpdate(currentUpdate);
                Renderer.RenderFrame(currentUpdate);
            }
            _engineObjects.ForEach(value => value.End());

            return 0;
        }

        //Only exists to add simplicity.
        //I think it would be odd to call Start to start the engine but change a boolean to stop it.
        //A start method with an accompanying stop method makes more sense i think.
        ///<summary>Finishes the current interation's updates and stops the engine.</summary>
        public virtual void Stop() => _isRunning = false;

        ///<summary>TODO.</summary>
        public void Attach(IKEngineObject kEngineObject) => _engineObjects.QueueAdd(kEngineObject);

        ///<summary>TODO.</summary>
        public void Detach(IKEngineObject kEngineObject) => _engineObjects.QueueRemove(kEngineObject);

        ///<summary>TODO.</summary>
        public void AttachAll(IEnumerable<IKEngineObject> kEngineObjects) => kEngineObjects.ForEach(Attach);

        ///<summary>TODO.</summary>
        public void DetachAll(IEnumerable<IKEngineObject> kEngineObjects) => kEngineObjects.ForEach(Detach);
    }
}