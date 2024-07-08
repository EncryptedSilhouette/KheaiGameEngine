using SFML.Graphics;
using SFML.Window;

using KheaiGameEngine.Debug;

namespace KheaiGameEngine
{
    #region KEngineComponent

    public abstract class KEngineComponent : IKComponent
    {
        ///<summary>The order the component will be updated.</summary>
        public ushort Order { get; set; } = 0;
        ///<summary>The ID for the component.</summary>
        public string ID { get; set; }
        ///<summary>The reference for the engine.</summary>
        public KEngine Engine { get; set; }

        ///Sets the component's ID to the type name.</summary>
        public KEngineComponent() => ID = GetType().Name;

        //Methods implemented from IKComponent
        public abstract void Init();
        public abstract void Start();
        public abstract void End();

        ///<summary>Executes code every update.</summary>
        ///<param name="currentUpdate">Keeps track of the current frame.</param>
        public abstract void Update(uint currentUpdate);

        ///<summary>Executes pre-draw code every update. This method is called after the update method.</summary>
        ///<param name="currentUpdate">Keeps track of the current frame.</param>
        public abstract void FrameUpdate(uint currentUpdate);
    }

    #endregion

    #region KEngine

    public sealed class KEngine : IKComponentContainer<KEngineComponent>
    {
        #region Class data

        private KComponentSorter<KEngineComponent> _componentSorter;
        private SortedSet<KEngineComponent> _engineComponents;

        ///<summary>The target number of updates in a second.</summary>
        public byte UpdateRateTarget { get; private set; } = 30;
        ///<summary>The current number of frames.</summary>
        public uint CurrentUpdate { get; private set; } = 0;
        ///<summary>Gets the update interval in milliseconds.</summary>
        public double UpdateInterval => 1000d / UpdateRateTarget;
        ///<summary>Whether or not the engine is running.</summary>
        public bool IsRunning { get; private set; } = true;
        ///<summary>The reference to the application.</summary>
        public IKApplication Application { get; private set; }
        ///<summary>The reference to the render window.</summary>
        public RenderWindow Window { get; private set; }

        ///<summary>Indexer to retrive an engine component given an the component id.</summary>
        public KEngineComponent this[string id] => GetComponent(id);
        ///<summary>Indexer to retrive an engine component given the component's type.</summary>
        public KEngineComponent this[Type type] => GetComponent(type.Name);

        #endregion

        #region Constructors

        ///<summary>Creates the window and sets the refrence to the application.</summary>
        ///<param name="app">Refrence to the KApplication.</param>
        public KEngine(IKApplication app)
        {
            Application = app;
            Window = new(VideoMode.DesktopMode, app.AppName);
            Window.Closed += (ignoreA, ignoreB) => End();

            _componentSorter = new KComponentSorter<KEngineComponent>();
            _engineComponents = new(_componentSorter);
        }

        ///<summary>Creates the window and sets the refrence to the application and the framerate target.</summary>
        ///<param name="app">Refrence to the KApplication.</param>  
        ///<param name="updateRateTarget">The target framerate.</param>
        public KEngine(IKApplication app, byte updateRateTarget) : this(app) => 
            UpdateRateTarget = updateRateTarget;

        #endregion

        #region Logic

        ///<summary>Executes any initilization code for the component. Should be called in the "Start" method</summary>
        public void Init()
        {
            if (!HasComponent<KDebugger>())
                AddComponent(new KDebugger());
            if (!HasComponent<KRenderer>())


            foreach (KEngineComponent component in _engineComponents) component.Start();
        }

        #region Gameloop

        ///<summary>Executes starting code for the component.</summary>
        public void Start()
        {
            double lastTime, newTime;
            double unprocessedTime = 0;

            Init();
            
            lastTime = DateTime.UtcNow.Ticks;

            while (IsRunning) //Core game-loop
            {
                newTime = DateTime.UtcNow.Ticks;
                unprocessedTime += (newTime - lastTime) / TimeSpan.TicksPerMillisecond;
                lastTime = newTime;

                //Keeps track of time between updates and catches up on updates in case of lag.
                if (unprocessedTime > UpdateInterval)
                {
                    while (unprocessedTime > UpdateInterval) 
                    {
                        unprocessedTime -= UpdateInterval;
                        CurrentUpdate++;

                        Update();
                    }
                    FrameUpdate();

                    Window.Clear(Color.Black); 
                    KRenderer.RenderFrame(Window);
                    Window.Display();
                }
                Window.DispatchEvents();
            }

            foreach (KEngineComponent component in _engineComponents) component.End();
        }
        #endregion

        ///<summary>Executes code for the end of execution.</summary>
        public void End() => IsRunning = false;

        ///<summary>Executes code every update.</summary>
        public void Update()
        {
            foreach (KEngineComponent component in _engineComponents) component.Update(CurrentUpdate);
        }

        ///<summary>Executes pre-draw code every update.
        public void FrameUpdate()
        {
            foreach (KEngineComponent component in _engineComponents) component.FrameUpdate(CurrentUpdate);
        }

        #endregion

        //Component management implemented from IKComponentContainer
        #region Component management

        public KEngineComponent AddComponent(KEngineComponent component)
        {
            component.Engine = this;
            component.Init();
            _engineComponents.Add(component);
            return component;
        }

        public KEngineComponent[] AddComponents(KEngineComponent[] components)
        {
            foreach (var component in components) AddComponent(component);
            return components;
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

    #endregion
}