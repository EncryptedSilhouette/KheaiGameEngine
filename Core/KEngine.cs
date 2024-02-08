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
        public ushort Order { get; set; }
        public string ID { get; set; }
        public KEngine Engine { get; set; }

        public KEngineComponent()
        {
            Order = 5;
            ID = GetType().Name;
        }

        public abstract void Init();
        public abstract void Start();
        public abstract void End();
        public abstract void Update(ulong currentTick);
        public abstract void FrameUpdate(ulong currentFrame);
    }

    public sealed class KEngine : IKComponentContainer<KEngineComponent>
    {
        public const byte UpdateRateTarget = 60;

        private KComponentSorter<KEngineComponent> _componentSorter;
        private SortedSet<KEngineComponent> _engineComponents = new();

        public byte TicksInASecond => UpdateRateTarget;
        public byte updateInterval { get; } = UpdateRateTarget / 1000;
        public ulong CurrentTick { get; private set; } = 0;
        public ulong CurrentFrame { get; private set; } = 0;
        public bool IsRunning { get; private set; } = true;
        public bool IsPaused { get; private set; } = false;
        public RenderWindow Window { get; private set; }
        public IKApplication Application { get; private set; }

        public KEngineComponent this[string id] => GetComponent(id);
        public KEngineComponent this[Type id] => GetComponent(id.ToString());

        public KEngine(IKApplication app)
        {
            Application = app;
            Window = new(VideoMode.DesktopMode, app.AppName);
            Window.Closed += (ignoreA, ignoreB) => End();

            _componentSorter = new KComponentSorter<KEngineComponent>();
        }

        #region Game logic
        public void Init()
        {
            if (!HasComponent<KDebugger>())
            {
                AddComponent(new KDebugger());
            }
            foreach (KEngineComponent component in _engineComponents)
            {
                component.Start();
            }
        }

        #region Gameloop
        public void Start()
        {
            long lastTime;
            long newTime;
            double updateUnprocessedTime = 0;
            KRenderManager renderer;

            Init();
            renderer = GetComponent<KRenderManager>();

            if (renderer == null)
            {
                KDebugger.ErrorLog("Renderer is null");
            }

            lastTime = DateTime.UtcNow.Ticks;

            while (IsRunning) //Core game-loop
            {
                newTime = DateTime.UtcNow.Ticks;
                updateUnprocessedTime += (newTime - lastTime) / TimeSpan.TicksPerMillisecond;
                lastTime = newTime;

                //Keeps track of time between updates and catches up on updates in case of lag.
                if (updateUnprocessedTime >= updateInterval)
                {
                    updateUnprocessedTime -= updateInterval;

                    if (!IsPaused)
                    {
                        Update();
                        CurrentTick++;
                    }
                    FrameUpdate();

                    Window.Clear(Color.Black);
                    renderer.Draw(Window);
                    Window.Display();

                    CurrentFrame++;
                }
                Window.DispatchEvents();
            }
        }
        #endregion

        public void End() => IsRunning = false;

        public void Update()
        {
            foreach (KEngineComponent component in _engineComponents)
            {
                component.Update(CurrentTick);
            }
        }

        public void FrameUpdate()
        {
            foreach (KEngineComponent component in _engineComponents)
            {
                component.FrameUpdate(CurrentFrame);
            }
        }
        #endregion

        #region Component management
        public void AddComponent(KEngineComponent component)
        {
            component.Engine = this;
            component.Init();
            _engineComponents.Add(component);
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
            foreach(var component in _engineComponents)
            {
                if (component.ID.Equals(id))
                {
                    _engineComponents.Remove(component);
                    return;
                }
            }
        }

        public void RemoveComponent<Component>()
        {
            foreach (var component in _engineComponents)
            {
                if (component is Component)
                {
                    _engineComponents.Remove(component);
                    return;
                }
            }
        }

        public bool HasComponent<Component>()
        {
            foreach (var component in _engineComponents)
            {
                if (component is Component) return true;
            }
            return false;
        }

        public bool HasComponent(string id)
        {
            foreach (var component in _engineComponents)
            {
                if (component.ID.Equals(id)) return true;
            }
            return false;
        }

        public Component GetComponent<Component>() where Component : KEngineComponent
        {
            foreach (IKComponent component in _engineComponents)
            {
                if (component is Component) return (Component) component;
            }
            return null;
        }

        public KEngineComponent GetComponent(string id)
        {
            foreach (var component in _engineComponents)
            {
                if (component.ID.Equals(id)) return component;
            }
            return null;
        }
        #endregion
    }
}