using KheaiGameEngine.Extensions;

namespace KheaiGameEngine.Core
{
    ///<summary>A simple game engine that loops over a collection of IKEngineObjects. Calling each one's Update and FrameUpdate method, untill stopped.</summary>
    public class KEngine 
    {
        private bool _isRunning = false;
        private uint _updateTarget = 30;
        private KSortedQueuedList<IKEngineObject> _engineObjects = new(new KEngineObjectComparer<IKEngineObject>());

        ///<summary>Represents the running state of the Engine</summary>
        public bool IsRunning => _isRunning;
        ///<summary>The IKEngineObjects attached with this engine.</summary>
        public IKSearchableCollection<IKEngineObject> EngineObjects => _engineObjects;
        ///<summary>The time interval in milliseconds between updates.</summary>
        public double UpdateInterval { get; private set; } = 0;

        ///<summary>The target number of updates in a second.</summary>
        public uint UpdateRateTarget
        {
            get => _updateTarget;
            private set => UpdateInterval = 1000d / (_updateTarget = value);
        }

        public KEngine(uint updateTarget = 30)
        {
            UpdateRateTarget = updateTarget;
            _engineObjects.OnInsertion += item => item.Start();
            _engineObjects.OnRemoved += item => item.End();
        }

        public KEngine(IEnumerable<IKEngineObject> kEngineObjects, uint updateTarget = 30) :
            this(updateTarget) => _engineObjects.AddAll(kEngineObjects);

        ///<summary>Executes starting tasks and starts the game-loop.</summary>
        public int Start()
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

            _engineObjects.UpdateContents();

            //It is now i realize how far from god we have gotten.
            if (_engineObjects.Find(value => value is IKRenderer) is not IKRenderer renderer)
            {
                KDebugger.ErrorLog("Fatal engine err: There is no renderer attatched.");
                return 1;
            }

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
                renderer.RenderFrame();
            }
            _engineObjects.ForEach(value => value.End());

            return 0;
        }

        //Only exists to add simplicity.
        //I think it would be odd to call Start to start the engine but change a boolean to stop it.
        //A start method with an accompanying stop method makes more sense i think.
        ///<summary>Finishes the current interation's updates and stops the engine.</summary>
        public void Stop() => _isRunning = false;

        public void Attach(IKEngineObject kEngineObject) => _engineObjects.QueueAdd(kEngineObject);

        public void Detach(IKEngineObject kEngineObject) => _engineObjects.QueueRemove(kEngineObject);

        public void AttachAll(IEnumerable<IKEngineObject> kEngineObjects) => kEngineObjects.ForEach(Attach);

        public void DetachAll(IEnumerable<IKEngineObject> kEngineObjects) => kEngineObjects.ForEach(Detach);
    }
}