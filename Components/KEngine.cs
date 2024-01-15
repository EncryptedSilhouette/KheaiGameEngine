using SFML.Graphics;
using SFML.Window;

namespace KheaiGameEngine
{
    public interface IKEngineManaged
    {
        public abstract void FixedUpdate();
        public abstract void FrameUpdate(double deltaTIme, RenderTarget target);
    }

    public abstract class KEngineComponent : IKComponent, IKEngineManaged
    {
        public int Order { get; set; }
        public string ID { get; set; }
        public KEngine Engine { get; set; }

        public KEngineComponent()
        {
            ID = GetType().Name;
        }

        public abstract void Init();
        public abstract void Start();
        public abstract void End();
        public abstract void FixedUpdate();
        public abstract void FrameUpdate(double deltaTIme, RenderTarget target);
    }

    public class KEngine : KAppComponent, IKComponentContainer<KEngineComponent>, IKEngineManaged
    {
        protected uint tickRate = 0;
        protected uint maxUpdatesPerSecond = 0;
        protected uint minUpdatesPerSecond;
        protected uint frameRate = 0;
        protected uint maxFramesPerSecond = 0;
        protected uint minFramesPerSecond;
        protected bool isRunning = true;
        protected bool isPaused = false;
        protected RenderWindow window;
        protected KComponentSorter<KEngineComponent> componentSorter = new KComponentSorter<KEngineComponent>();
        protected SortedSet<KEngineComponent> engineComponents = new();

        //Threading 
        protected Thread engineThread;

        public double CurrentTime => DateTime.UtcNow.Ticks;
        public uint UpdatesPerSecond { get; private set; } = 30;
        public uint FramesPerSecond { get; private set; } = 60;
        public double GameSpeed { get; private set; } = 1;

        public KEngine()
        {
            ID = GetType().Name;
            Order = 0;
            minUpdatesPerSecond = UpdatesPerSecond;
            minFramesPerSecond = FramesPerSecond;
            window = new(VideoMode.DesktopMode, App.AppName);
            engineThread = KThreadManager.CreateThread("engine_thread", Run);
        }

        #region Game logic
        public override void Init()
        {
            KDebug.AddLog("engine");
        }

        public override void Start()
        {
            KDebug.Log("engine", "Engine: Retriving Window");

            if (window == null)
            {
                KDebug.Log(KDebug.ERROR, "Window doesnt exist, failed to start engine");
                App.End();  
                return;
            }
            engineThread.Start();
        }

        public override void End()
        {
            KDebug.Log("engine", $"Tickrate: {UpdatesPerSecond}, " +
                                 $"Max: {maxUpdatesPerSecond}, " +
                                 $"Min: {minUpdatesPerSecond}");

            KDebug.Log("engine", $"Framerate: {FramesPerSecond}, " +
                                 $"Max: {maxFramesPerSecond}, " +
                                 $"Min: {minFramesPerSecond}");

            isRunning = false;
            engineThread.Join();
        }

        public void FixedUpdate()
        {
            foreach (KEngineComponent component in engineComponents)
            {
                component.FixedUpdate();
            }
        }

        public void FrameUpdate(double deltaTime, RenderTarget target)
        {
            foreach (KEngineComponent component in engineComponents)
            {
                component.FrameUpdate(deltaTime, target);
            }
        }
        #endregion

        #region Gameloop
        protected void Run()
        {
            uint ticks = 0;
            uint frames = 0;
            double startTime;
            double lastTime;
            double newTime;
            double deltaTime = 0;
            double updateUnprocessedTime = 0;
            double frameUnprocessedTime = 0;
            double updateInterval = 1000d / UpdatesPerSecond * GameSpeed;
            double frameInterval = 1000d / FramesPerSecond;

            lastTime = startTime = DateTime.UtcNow.Ticks;

            while (isRunning) //Core game-loop
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

                    if (!isPaused && GameSpeed > 0)
                    {
                        FixedUpdate();
                        ticks++;
                    }
                }

                //Limits frame update to desired FPS
                if (frameUnprocessedTime >= frameInterval)
                {
                    frameUnprocessedTime -= frameInterval;

                    FrameUpdate(deltaTime, window);
                    frames++;
                }

                //The last few lines are to keep track of debug info.
                if ((DateTime.UtcNow.Ticks - startTime) / TimeSpan.TicksPerSecond >= 1)
                {
                    tickRate = ticks;
                    frameRate = frames;

                    if (ticks >= maxUpdatesPerSecond) maxUpdatesPerSecond = ticks;
                    if (ticks < minUpdatesPerSecond) minUpdatesPerSecond = ticks;

                    if (frames >= maxFramesPerSecond) maxFramesPerSecond = frames;
                    if (frames < minFramesPerSecond) minFramesPerSecond = frames;

                    ticks = frames = 0;
                    startTime = DateTime.UtcNow.Ticks;
                }
            }
        }
        #endregion

        #region Component management
        public void AddComponent(KEngineComponent component)
        {
            component.Engine = this;
            component.Init();
            engineComponents.Add(component);
        }

        public void AddComponents(KEngineComponent[] components)
        {
            foreach (var component in components) 
            {
                AddComponent(component);
            }
        }

        public void RemoveComponent(string id)
        {
            foreach(var component in engineComponents)
            {
                if (component.ID.Equals(id))
                {
                    engineComponents.Remove(component);
                    return;
                }
            }
        }

        public void RemoveComponent<Component>()
        {
            foreach (var component in engineComponents)
            {
                if (component is Component)
                {
                    engineComponents.Remove(component);
                    return;
                }
            }
        }

        public bool HasComponent<Component>()
        {
            foreach (var component in engineComponents)
            {
                if (component is Component) return true;
            }
            return false;
        }

        public bool HasComponent(string id)
        {
            foreach (var component in engineComponents)
            {
                if (component.ID.Equals(id)) return true;
            }
            return false;
        }

        public Component GetComponent<Component>() where Component : KEngineComponent
        {
            foreach (IKComponent component in engineComponents)
            {
                if (component is Component) return (Component) component;
            }
            return null;
        }

        public KEngineComponent GetComponent(string id)
        {
            foreach (var component in engineComponents)
            {
                if (component.ID.Equals(id)) return component;
            }
            return null;
        }
        #endregion

        #region Ignored
        public override void Update() { }
        #endregion
    }
}