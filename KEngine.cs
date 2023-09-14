using SFML.Graphics;
using System.ComponentModel;

namespace KheaiGameEngine
{
    public abstract class KEngineComponent : IKComponent<KEngine>
    {
        public string ID => GetType().Name;
        public KEngine _engine { get; protected set; }

        #region Game logic
        public abstract void Attatch(KEngine engine);
        public abstract void Init();
        public abstract void Start();
        public abstract void End();
        public abstract void FixedUpdate();
        public abstract void FrameUpdate(double deltaTIme);
        #endregion
    }

    public class KEngine : KAppComponent, IKComponentContainer<string, KEngineComponent>
    {
        public uint UpdatesPerSecond = 30;
        public uint FramesPerSecond = 60;
        public float GameSpeed = 1;

        protected uint _tickRate = 0;
        protected uint _frameRate = 0;
        protected bool _isRunning = true;
        protected bool _isPaused = false;
        protected Dictionary<string, KEngineComponent> _engineComponents = new();

        public double CurrentTime => DateTime.UtcNow.Ticks;
        public KWindow Window { get; private set; }

        //Threading 
        protected Thread _engineThread;

        public KEngine()
        {
            _engineThread = new(Run);
        }

        #region Game logic
        public override void Start()
        {
            KDebug.Log("Starting Engine");
            KDebug.Log("Retriving Window");
            Window = (KWindow) Application.GetComponent<KWindow>();

            if (Window == null)
            {
                KDebug.Log("Window doesnt exist, failed to start engine");
                return;
            }
            _engineThread.Start();
        }

        public override void End()
        {
            _isRunning = false;
            _engineThread.Join();
        }

        public void FixedUpdate()
        {
            foreach (KEngineComponent component in _engineComponents.Values)
            {
                component.FixedUpdate();
            }
        }

        public void FrameUpdate(double deltaTime)
        {
            foreach (KEngineComponent component in _engineComponents.Values)
            {
                component.FrameUpdate(deltaTime);
            }
        }
        #endregion

        #region Gameloop
        protected void Run()
        {
            uint ticks = 0, frames = 0;
            double updateInterval = 1000d / UpdatesPerSecond * GameSpeed;
            double frameInterval = 1000d / FramesPerSecond;
            double startTime, lastTime, newTime, deltaTime = 0;
            double updateUnprocessedTime = 0, frameUnprocessedTime = 0;

            lastTime = startTime = DateTime.UtcNow.Ticks;

            while (_isRunning) //Core game-loop
            {
                newTime = DateTime.UtcNow.Ticks;
                frameUnprocessedTime += (newTime - lastTime) / TimeSpan.TicksPerMillisecond;
                updateUnprocessedTime += (newTime - lastTime) / TimeSpan.TicksPerMillisecond;
                deltaTime = (newTime - lastTime) / (updateInterval * GameSpeed);
                lastTime = newTime;

                //Keeps track of time between updates and catches up on updates in case of lag.
                while (updateUnprocessedTime >= updateInterval)
                {
                    updateUnprocessedTime -= updateInterval;
                    ticks++;

                    if (_isPaused || GameSpeed <= 0)
                    {
                        updateUnprocessedTime = 0;
                        break;
                    }
                    FixedUpdate();
                }

                //Limits frame update to desired FPS
                if (frameUnprocessedTime >= frameInterval)
                {
                    frameUnprocessedTime = 0;
                    frames++;

                    FrameUpdate(deltaTime);
                    //Window.Draw();
                }

                //The last few lines in this scope are to keep track of debug info.
                if ((DateTime.UtcNow.Ticks - startTime) / TimeSpan.TicksPerSecond >= 1)
                {
                    _tickRate = ticks;
                    _frameRate = frames;
                    ticks = frames = 0;
                    startTime = DateTime.UtcNow.Ticks;
                }
            }
        }
        #endregion

        #region Component management
        public void AddComponent(KEngineComponent component)
        {
            component.Attatch(this);
            component.Init();
            _engineComponents.Add(component.ID, component);
        }

        public void AddComponents(KEngineComponent[] components)
        {
            foreach (KEngineComponent component in components) 
            {
                AddComponent(component);
            }
        }

        public void RemoveComponent(string id)
        {
            _engineComponents.Remove(id);
        }

        public void RemoveComponent<Component>()
        {
            _engineComponents.Remove(typeof(Component).Name);
        }

        public bool HasComponent(string id)
        {
            return _engineComponents.ContainsKey(id);
        }

        public bool HasComponent<Component>()
        {
            return _engineComponents.ContainsKey(typeof(Component).Name);
        }

        public KEngineComponent GetComponent(string id)
        {
            return _engineComponents[id];
        }

        public KEngineComponent GetComponent<Component>() where Component : KEngineComponent
        {
            return _engineComponents[typeof(Component).Name];
        }
        #endregion

        #region Ignored
        public override void Init() { }
        #endregion
    }
}