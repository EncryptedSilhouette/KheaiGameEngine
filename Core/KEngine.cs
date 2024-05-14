using SFML.Graphics;
using SFML.Window;

using KheaiGameEngine.Components;
using KheaiGameEngine.Debug;

namespace KheaiGameEngine.Core
{
    public interface IKEngine 
    {
        ///<summary>Starting point for the engine.</summary>
        void Start();
    }

    public interface IKEngineManaged
    {
        ///<summary>Executes any starting tasks.</summary>
        void Start();

        ///<summary>Executes tasks every tick.</summary>
        ///<param name="currentFrame">Keeps track of the current frame.</param>
        void Update(uint currentFrame);

        ///<summary>Executes tasks every frame.</summary>
        ///<param name="currentFrame">Keeps track of the current frame.</param>
        void FrameUpdate(uint currentFrame);
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
        public abstract void Update(uint currentFrame);
        public abstract void FrameUpdate(uint currentFrame);
    }

    public sealed class KEngine : IKComponentContainer<KEngineComponent>, IKEngine
    {
        private byte _frameRateTarget = 30;
        private KComponentSorter<KEngineComponent> _componentSorter;
        private SortedSet<KEngineComponent> _engineComponents;

        public byte FrameRateTarget => _frameRateTarget;
        public double FrameInterval => 1000d / _frameRateTarget;
        public uint CurrentFrame { get; private set; } = 0;
        public bool IsRunning { get; private set; } = true;
        public bool IsPaused { get; private set; } = false;
        public RenderWindow Window { get; private set; }
        public KDrawHandler DrawHandler { get; private set; }
        public IKApplication Application { get; private set; } 

        public KEngineComponent this[string id] => GetComponent(id);
        public KEngineComponent this[Type id] => GetComponent(id.Name);

        #region Constructors

        public KEngine(IKApplication app)
        {
            Application = app;
            Window = new(VideoMode.DesktopMode, app.AppName);
            Window.Closed += (ignoreA, ignoreB) => End();

            _componentSorter = new KComponentSorter<KEngineComponent>();
            _engineComponents = new(_componentSorter);
        }

        public KEngine(IKApplication app, KDrawHandler renderer) : this(app) => AddComponent(renderer);
        public KEngine(IKApplication app, KDrawHandler renderer, byte frameRateTarget) : this(app, renderer) => 
            _frameRateTarget = frameRateTarget;

        #endregion

        #region Game logic

        public void Init()
        {
            if (!HasComponent<KDebugger>())
                AddComponent(new KDebugger());

            foreach (KEngineComponent component in _engineComponents) component.Start();
        }

        #region Gameloop

        public void Start()
        {
            double lastTime, newTime;
            double updateUnprocessedTime = 0;

            Init();
            
            lastTime = DateTime.UtcNow.Ticks;

            while (IsRunning) //Core game-loop
            {
                newTime = DateTime.UtcNow.Ticks;
                updateUnprocessedTime += (newTime - lastTime) / TimeSpan.TicksPerMillisecond;
                lastTime = newTime;

                //Keeps track of time between updates and catches up on updates in case of lag.
                if (updateUnprocessedTime >= FrameInterval)
                {
                    updateUnprocessedTime -= FrameInterval;
                    CurrentFrame++;

                    Update();
                    FrameUpdate();
                    Draw();
                }
                Window.DispatchEvents();
            }

            foreach (KEngineComponent component in _engineComponents) component.End();
        }
        #endregion

        public void End() => IsRunning = false;

        public void Update()
        {
            foreach (KEngineComponent component in _engineComponents) component.Update(CurrentFrame);
        }

        public void FrameUpdate()
        {
            foreach (KEngineComponent component in _engineComponents) component.FrameUpdate(CurrentFrame);
        }

        public void Draw() => DrawHandler.Draw(Window);
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
            foreach (var component in components) AddComponent(component);
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
                if (component is Component) return true;
            return false;
        }

        public bool HasComponent(string id)
        {
            foreach (var component in _engineComponents)
                if (component.ID.Equals(id)) return true;
            return false;
        }

        public Component GetComponent<Component>() where Component : KEngineComponent
        {
            foreach (IKComponent component in _engineComponents)
                if (component is Component) return (Component) component;
            return null;
        }

        public KEngineComponent GetComponent(string id)
        {
            foreach (var component in _engineComponents)
                if (component.ID.Equals(id)) return component;
            return null;
        }

        #endregion
    }
}