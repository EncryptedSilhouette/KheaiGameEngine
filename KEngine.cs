using System.ComponentModel;

namespace KheaiGameEngine
{
    public abstract class KEngineComponent : KComponent<KEngine>
    {
        #region Game logic
        public abstract void FixedUpdate();
        public abstract void FrameUpdate(double deltaTIme);
        #endregion
    }


    public class KEngine : KComponent<KApplication>, IKComponentContainer<string, KEngineComponent>
    {
        protected uint tickRate = 0;
        protected uint maxUpdatesPerSecond = 0;
        protected uint minUpdatesPerSecond;
        protected uint frameRate = 0;
        protected uint maxFramesPerSecond = 0;
        protected uint minFramesPerSecond;
        protected bool isRunning = true;
        protected bool isPaused = false;
        protected List<KEngineComponent> engineComponents = new();

        //Threading 
        protected Thread engineThread;

        public double CurrentTime => DateTime.UtcNow.Ticks;
        public uint GameSpeed { get; set; } = 1;
        public uint UpdatesPerSecond { get; set; } = 30;
        public uint FramesPerSecond { get; set; } = 60;
        public KWindow Window { get; protected set; }

        public KEngine()
        {
            minUpdatesPerSecond = UpdatesPerSecond;
            minFramesPerSecond = FramesPerSecond;
        }

        #region Game logic
        public override void Init()
        {
            engineThread = Owner.CreateThread("engine_thread", Run);
            KDebug.AddLog("engine");
        }

        public override void Start()
        {
            KDebug.Log("engine", "Engine: Retriving Window");
            Window = Owner.GetComponent<KWindow>();
            if (Window == null)
            {
                KDebug.Log(KDebug.ERROR, "Window doesnt exist, failed to start engine");
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

        public void FrameUpdate(double deltaTime)
        {
            foreach (KEngineComponent component in engineComponents)
            {
                component.FrameUpdate(deltaTime);
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
                    ticks++;

                    if (isPaused || GameSpeed <= 0)
                    {
                        updateUnprocessedTime = 0;
                        break;
                    }
                    FixedUpdate();
                }

                //Limits frame update to desired FPS
                if (frameUnprocessedTime >= frameInterval)
                {
                    frameUnprocessedTime -= frameInterval;
                    frames++;

                    FrameUpdate(deltaTime);
                    Window.Draw();
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
            component.Attatch(this);
            component.Init();
            engineComponents.Sort(SortByID);
            engineComponents.Add(component);
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
            foreach(var component in engineComponents)
            {
                if (component.ID.Equals(id))
                {
                    engineComponents.Remove(component);
                    return;
                }
            }
            KDebug.Log($"Failed to remove component {id}.");
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
            KDebug.Log($"Failed to remove component {typeof(Component).Name}.");
        }

        public bool HasComponent(string id)
        {
            foreach (var component in engineComponents)
            {
                if (component.ID.Equals(id)) return true;
            }
            return false;
        }

        public bool HasComponent<Component>()
        {
            foreach (var component in engineComponents)
            {
                if (component is Component) return true;
            }
            return false;
        }

        public Component GetComponent<Component>() where Component : KEngineComponent
        {
            foreach (var component in engineComponents)
            {
                if (component is Component) return (Component) component;
            }
            return null;
        }

        public int SortByID(KEngineComponent a, KEngineComponent b)
        {
            if (a.ID.Equals(b.ID)) return 0;
            if (a.Order > b.Order) return 1;
            return -1;
        }
        #endregion
    }
}