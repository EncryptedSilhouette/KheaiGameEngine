using KheaiGameEngine.Extensions;

namespace KheaiGameEngine.Core
{
    ///<summary>A simple game engine that loops over a collection of IKEngineObjects. Calling each one's Update and FrameUpdate method, untill stopped.</summary>
    public sealed class KEngine
    {
        private bool _isRunning = false;
        private uint _updateTarget = 30;
        private IKRenderer? _renderer;
        private KSortedQueuedList<IKEngineObject> _engineObjects = new(new KEngineObjectComparer<IKEngineObject>());

        public Action<KEngine>? OnStart;

        ///<summary>Represents the running state of the Engine.</summary>
        public bool IsRunning => _isRunning;

        ///<summary>The time interval in milliseconds between updates.</summary>
        public double UpdateInterval { get; private set; } = 0;

        ///<summary>The target number of updates in a second.</summary>
        public uint UpdateRateTarget
        {
            get => _updateTarget;
            private set => UpdateInterval = 1000d / (_updateTarget = value);
        }

        public KEngine(uint updateTarget = 30, IKRenderer? renderer = null) 
        {
            (UpdateRateTarget, _renderer) = (updateTarget, renderer);
            _engineObjects.OnRemoved += value => value.End();
        }

        public KEngine(IEnumerable<IKEngineObject> engineObjects, uint updateTarget = 30, IKRenderer? renderer = null) : 
            this(updateTarget, renderer) =>
            _engineObjects.AddAll(engineObjects); //Adds the whole enumerable collection instead of queueing them.

        ///<summary>Executes starting tasks and starts the game-loop. This method is blocking.</summary>
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

            _isRunning = true;

            _engineObjects.UpdateContents();
            //Starts the existing objects that were added before the engine was started.
            _engineObjects.ForEach(value => value.Start());
            //Any objects added past this point will be started when they are added to the engine.
            _engineObjects.OnInsertion += value => value.Start();

            _renderer ??= _engineObjects.Find(value => value is IKRenderer) as IKRenderer;

            if (_renderer is null)
            {
                KDebugger.ErrorLog("Engine err: Failed to start, renderer is null.");
                return 1;
            }

            //Executes any additional starting tasks
            OnStart?.Invoke(this);

            lastTime = DateTime.UtcNow.Ticks; //Required for loop timing.

            while (_isRunning) //Game loop
            {
                newTime = DateTime.UtcNow.Ticks;    
                unprocessedTime += (newTime - lastTime) / TimeSpan.TicksPerMillisecond;
                lastTime = newTime;

                //Checks if the unprocessedTime is greater than the UpdateInterval
                if (unprocessedTime < UpdateInterval)
                {
                    //TODO Test this; maybe this will be good practice?
                    Thread.Sleep((int) (UpdateInterval - unprocessedTime));
                    continue;
                }

                //Loops to compensate for any lag, skipping pre-draw and render tasks. 
                do
                {
                    //Update.
                    _engineObjects.UpdateContents();
                    _engineObjects.ForEach(kEngineObject => kEngineObject.Update(currentUpdate));

                    currentUpdate++;
                    unprocessedTime -= UpdateInterval;
                }
                while (unprocessedTime > UpdateInterval && IsRunning);

                //Frame update.
                _engineObjects.ForEach(kEngineObject => kEngineObject.FrameUpdate(currentUpdate));

                //Render frame.
                _renderer.RenderFrame(currentUpdate);
            }
            _engineObjects.ForEach(value => value.End());

            return 0;
        }

        //Only exists to add simplicity.
        //I think it would be odd to call Start to start the engine but change a boolean to stop it.
        //A start method with an accompanying stop method makes more sense i think.
        ///<summary>Finishes the current interation's updates and stops the engine.</summary>
        public void Stop() => _isRunning = false;

        ///<summary>TODO.</summary>
        public void Attach(IKEngineObject engineObject)
        {
            engineObject.Init(this);
            _engineObjects.QueueAdd(engineObject);
        }

        ///<summary>TODO.</summary>
        public void Detach(IKEngineObject engineObject) => _engineObjects.QueueRemove(engineObject);

        ///<summary>TODO.</summary>
        public void AttachAll(IEnumerable<IKEngineObject> engineObject) => engineObject.ForEach(Attach);

        ///<summary>TODO.</summary>
        public void DetachAll(IEnumerable<IKEngineObject> engineObject) => engineObject.ForEach(Detach);
    }
}