using SFML.Graphics;
using SFML.Window;

namespace KheaiGameEngine.Core
{
    public interface IKEngineManaged
    {
        public void Start();
        public void Update(uint currentTick);
        public void FrameUpdate(uint currentFrame);
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
        public abstract void Update(uint currentTick);
        public abstract void FrameUpdate(uint currentFrame);
    }

    public sealed class KEngine : IKComponentContainer<KEngineComponent>
    {
        public const byte UpdateRateTarget = 60;

        private KComponentSorter<KEngineComponent> _componentSorter;
        private SortedSet<KEngineComponent> _engineComponents;

        public byte TicksInASecond => UpdateRateTarget;
        public byte updateInterval { get; } = UpdateRateTarget / 1000;
        public uint CurrentTick { get; private set; } = 0;
        public uint CurrentFrame { get; private set; } = 0;
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
            _engineComponents = new(_componentSorter);
        }

        #region Game logic
        public void Init()
        {
            AddComponent(new KDrawHandler());
            foreach (KEngineComponent component in _engineComponents)
            {
                component.Start();
            }
        }

        #region Gameloop
        public void Start()
        {
            long lastTime, newTime;
            double updateUnprocessedTime = 0;
            KDebugger debugger;
            KDrawHandler drawHandler;

            Init();

            debugger = GetComponent<KDebugger>();
            if (debugger == null)
            {
                AddComponent(new KDebugger());
            }

            drawHandler = GetComponent<KDrawHandler>();
            if (drawHandler == null)
            {
                KDebugger.ErrorLog("Draw handler is null");
                return;
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
                    drawHandler.Draw();
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