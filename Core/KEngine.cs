using KheaiGameEngine.Core.KCollections;
using KheaiGameEngine.Core.KCollections.Extensions;

namespace KheaiGameEngine.Core
{
    ///<summary>A simple game engine that loops over a collection of IKEngineObjects. Calls each IKEngineObjects's Update and FrameUpdate methods until stopped.</summary>
    public sealed class KEngine 
    {
        private bool _isRunning = false;
        private uint _updateTarget = 30;
        private KSortedQueuedList<IKEngineObject> _engineObjects = new(new KEngineObjectComparer<IKEngineObject>());

        ///<summary>Represents the running state of this Engine</summary>
        public bool IsRunning => _isRunning;
        ///<summary>The renderer for this engine.</summary>
        public IKRenderer? Renderer { get; private set; }
        ///<summary>The time interval in milliseconds between updates.</summary>
        public double UpdateInterval { get; private set; } = 0.0d;
        ///<summary>A searchable collection of the IKEngineObjects attached to this engine.</summary>
        public IKSearchableCollection<IKEngineObject> EngineObjects => _engineObjects;

        ///<summary>The target number of updates per second.</summary>
        public uint UpdateRateTarget
        {
            get => _updateTarget;
            private set => UpdateInterval = 1000.0d / (_updateTarget = value);
        }

        public KEngine(IKRenderer? renderer = null, uint updateTarget = 30) => (Renderer, UpdateRateTarget) = (renderer, updateTarget);

        public KEngine(IEnumerable<IKEngineObject> kEngineObjects, IKRenderer? renderer = null, uint updateTarget = 30) :
            this(renderer, updateTarget) => _engineObjects.AddAll(kEngineObjects);

        ///<summary>Executes starting tasks and starts the game-loop.</summary>
        public int Start()
        {
            //Prevents engine from starting if it's already running.
            if (_isRunning)
            {
                KDebugger.ErrorLog("Engine err: Can not start, this engine instance is already running.");
                return 1;
            }

            //Timing variables.
            uint currentUpdate = 0;
            double lastTime, newTime, unprocessedTime = 0;

            //Any IKEngineObjects added to the engine won't be started until the engine is running.
            _engineObjects.ForEach(value => value.Start());
            //Any IKEngineObjects added after this point will have their start method called on insersion.
            _engineObjects.OnInsertion += item => item.Start();
            _engineObjects.OnRemoved += item => item.End();
            _engineObjects.UpdateContents();

            Renderer = _engineObjects.Find<IKRenderer>();

            if (Renderer is null)
            {
                KDebugger.ErrorLog("Engine err: Failed to start, renderer is null.");
                return 1;
            }

            _isRunning = true;
            lastTime = DateTime.UtcNow.Ticks; //Required for loop timing.

            while (_isRunning) //Game loop
            {
                newTime = DateTime.UtcNow.Ticks;    
                unprocessedTime += (newTime - lastTime) / TimeSpan.TicksPerMillisecond;
                lastTime = newTime;

                //Checks if the unprocessedTime is greater than the UpdateInterval
                if (unprocessedTime < UpdateInterval)
                {
                    Thread.Sleep((int) (UpdateInterval - unprocessedTime));
                    continue;
                }

                //Loops to compensate for any lag, skipping pre-draw and render tasks. 
                do
                {
                    _engineObjects.UpdateContents();
                    _engineObjects.ForEach(kEngineObject => kEngineObject.Update(currentUpdate));

                    currentUpdate++;
                    unprocessedTime -= UpdateInterval;
                }
                while (unprocessedTime > UpdateInterval && IsRunning);

                _engineObjects.ForEach(kEngineObject => kEngineObject.FrameUpdate(currentUpdate));
                Renderer?.RenderFrame(currentUpdate);
            }
            _engineObjects.ForEach(value => value.End());

            return 0;
        }

        //Only exists to add simplicity.
        //I think it would be odd to call Start to start the engine but change a boolean to stop it.
        //A start method with an accompanying stop method makes more sense i think.
        ///<summary>Finishes the current interation's updates and stops the engine.</summary>
        public void Stop() => _isRunning = false;

        ///<summary>Attaches an IKEngineObject to the engine on the next update. Will call the IKEngineObject's start method if engine is already running. 
        ///If the engine is not running, the IKEngineObject's start method will be called by the engine's start method.</summary>
        public void Attach(IKEngineObject kEngineObject) => _engineObjects.QueueAdd(kEngineObject);

        ///<summary>Detaches an IKEngineObject from the engine on the next update. Will call the IKEngineObject's end method.</summary>
        public void Detach(IKEngineObject kEngineObject) => _engineObjects.QueueRemove(kEngineObject);

        ///<summary>Attaches an IEnumerable collection of IKEngineObjects to the engine on the next update. Will call each IKEngineObject's start method if engine is already running. 
        ///If the engine is not running, each IKEngineObject's start method will be called by the engine's start method.</summary>
        public void AttachAll(IEnumerable<IKEngineObject> kEngineObjects) => kEngineObjects.ForEach(Attach);

        ///<summary>Detaches an IEnumerable collection of IKEngineObjects from the engine on the next update. Will call each IKEngineObject's end method.</summary>
        public void DetachAll(IEnumerable<IKEngineObject> kEngineObjects) => kEngineObjects.ForEach(Detach);
    }
}