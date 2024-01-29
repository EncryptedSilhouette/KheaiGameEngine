using KheaiGameEngine.DevDebug;
using SFML.Graphics;
using SFML.Window;

namespace KheaiGameEngine.Core
{
    public interface IKEngineManaged
    {
        public void Start();
        public void Update(ulong currentTick);
        public void FrameUpdate(ulong currentFrame);
    }

    public abstract class KEngineComponent : IKComponent, IKEngineManaged
    {
        public byte Order { get; set; }
        public string ID { get; set; }
        public KEngine Engine { get; set; }

        public KEngineComponent()
        {
            Order = 1;
            ID = GetType().Name;
        }

        public abstract void Init();
        public abstract void Start();
        public abstract void End();
        public abstract void Update(ulong currentTick);
        public abstract void FrameUpdate(ulong currentFrame);
    }

    //TODO make private & sealed
    public class KEngine : IKComponentContainer<KEngineComponent>
    {
        public const byte UpdateRateTarget = 60;

        protected KComponentSorter<KEngineComponent> componentSorter;
        protected SortedSet<KEngineComponent> engineComponents = new();

        public uint UpdateRate { get; protected set; } = 0;
        public uint MaxUpdatesPerSecond { get; protected set; } = 0;
        public uint MinUpdatesPerSecond { get; protected set; } = uint.MaxValue;
        public ulong CurrentTick { get; protected set; } = 0;
        public ulong CurrentFrame { get; protected set; } = 0;
        public float GameSpeed { get; protected set; } = 1;
        public bool IsRunning { get; protected set; } = true;
        public bool IsPaused { get; protected set; } = false;
        public RenderWindow Window { get; protected set; }
        public IKApplication Application { get; protected set; }
        public KEngineComponent this[string id] => GetComponent(id);

        //Test & Debug
        public DebugRenderer renderer;

        public KEngine(IKApplication app)
        {
            Application = app;
            renderer = new(this);
            componentSorter = new KComponentSorter<KEngineComponent>();
            Window = new(VideoMode.DesktopMode, app.AppName);
            Window.Closed += (ignoreA, ignoreB) => End();
        }

        #region Game logic
        public void Init() 
        {
            foreach (KEngineComponent component in engineComponents)
            {
                component.Start();
            }
        }

        public void End() => IsRunning = false;

        public void Update()
        {
            foreach (KEngineComponent component in engineComponents)
            {
                component.Update(CurrentTick);
            }
        }

        public void FrameUpdate()
        {
            foreach (KEngineComponent component in engineComponents)
            {
                component.FrameUpdate(CurrentFrame);
            }
        }
        #endregion

        #region Gameloop
        public void Start()
        {
            uint ticks = 0;
            uint frames = 0;
            double startTime;
            double lastTime;
            double newTime;
            double updateUnprocessedTime = 0;
            double updateInterval = 1000f / (UpdateRateTarget * GameSpeed);
            //KRenderManager renderer;

            Init();
            //renderer = GetComponent<KRenderManager>();
            lastTime = startTime = DateTime.UtcNow.Ticks;

            while (IsRunning) //Core game-loop
            {
                newTime = DateTime.UtcNow.Ticks;
                updateUnprocessedTime += (newTime - lastTime) / TimeSpan.TicksPerMillisecond;
                lastTime = newTime;

                //Keeps track of time between updates and catches up on updates in case of lag.
                if (updateUnprocessedTime >= updateInterval)
                {
                    updateUnprocessedTime -= updateInterval;

                    if (!IsPaused && GameSpeed > 0)
                    {
                        Update();

                        CurrentTick++;
                        ticks++;
                    }
                    FrameUpdate();

                    Window.Clear(Color.Black);
                    renderer.Draw(Window);
                    Window.Display();

                    CurrentFrame++;
                    frames++;
                }
                Window.DispatchEvents();

                //The last few lines are to keep track of debug info.
                if ((DateTime.UtcNow.Ticks - startTime) / TimeSpan.TicksPerSecond >= 1)
                {
                    UpdateRate = ticks;

                    if (ticks >= MaxUpdatesPerSecond) MaxUpdatesPerSecond = ticks;
                    if (ticks < MinUpdatesPerSecond) MinUpdatesPerSecond = ticks;

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
    }
}