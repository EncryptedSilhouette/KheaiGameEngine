using SFML.Graphics;
using System.Collections;

namespace KheaiGameEngine
{
    public interface KEngineComponent
    {
        public string ID => GetType().Name;
        public KEngine _engine { get; protected set; } 

        public void Init(KEngine engine);
        public void Start();
        public void End();
        public void FixedUpdate();
        public void FrameUpdate();
    }

    public class KEngine : IKComponent, IKComponentContainer<string, KEngineComponent>
    {
        public uint UpdatesPerSecond = 30;
        public uint FramesPerSecond = 60;
        public float GameSpeed = 1;

        protected bool _isRunning = true;
        protected bool _isPaused = false;
        protected uint _tickRate = 0;
        protected uint _frameRate = 0;
        protected RenderWindow _window;
        protected Dictionary<string, KEngineComponent> _engineComponents = new();

        public double CurrentTime => DateTime.UtcNow.Ticks;
        public KApplication Application { get; private set; }
        public RenderWindow Window { get; set; }

        //Threading 
        protected Thread _engineThread;

        public KEngine()
        {
            _engineThread = new(Run);
        }

        public void Init(KApplication app)
        {
            Application = app;
        }

        public void Start()
        {
            _engineThread.Start();
        }

        public void End()
        {
            _engineThread.Join();
        }

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

                    //FrameUpdate(deltaTime);
                    //Draw(Renderer);
                }
                //Window?.DispatchEvents();

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

        public void FixedUpdate()
        {

        }

        public void FrameUpdate()
        {

        }

        public void AddComponent(KEngineComponent component)
        {
            _engineComponents.Add(component.ID, component);
            component.Init(_application, this);
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
            throw new NotImplementedException();
        }

        public bool HasComponent(string id)
        {
            throw new NotImplementedException();
        }

        public KEngineComponent GetComponent(string id)
        {
            throw new NotImplementedException();
        }
    }
}