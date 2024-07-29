using SFML.Graphics;
using SFML.Window;

using KheaiGameEngine.Debug;

namespace KheaiGameEngine
{
    public interface IKEngineComponent : IKComponent
    {
        ///<summary>The reference for the engine.</summary>
        public KEngine Engine { get; set; }

        ///<summary>Executes code every update.</summary>
        ///<param name = "currentUpdate">Keeps track of the current frame.</param>
        public abstract void Update(uint currentUpdate);

        ///<summary>Executes pre-draw code every update. This method is called after the update method.</summary>
        ///<param name = "currentUpdate">Keeps track of the current frame.</param>
        public abstract void FrameUpdate(uint currentUpdate);
    }

    public sealed class KEngine : IKComponentContainer<IKEngineComponent>
    {
        #region Static 

        private static IKRenderer s_activeRenderer; //The active renderer for drawing.

        ///<summary>Event handler for when the active instance is changed</summary>
        public static event Action OnInstanceChanged;

        ///<summary>Property for the static refrence to the active instance. Fires an event when the instance is changed.</summary>
        public static IKRenderer ActiveRenderer
        {
            get => s_activeRenderer;
            set
            {
                s_activeRenderer = value;
                OnInstanceChanged?.Invoke();
            }
        }
        #endregion

        private SortedSet<IKEngineComponent> _engineComponents; //The collection for the engine's components.

        ///<summary>The target number of updates in a second.</summary>
        public byte UpdateRateTarget { get; private set; }
        ///<summary>The current number of frames.</summary>
        public double UpdateInterval => 1000d / UpdateRateTarget;
        ///<summary>Whether or not the engine is running.</summary>
        public bool IsRunning { get; private set; } = true;
        ///<summary>The reference to the application.</summary>
        public IKApplication Application { get; private set; }
        ///<summary>The reference to the render window.</summary>
        public RenderWindow Window { get; private set; }

        ///<summary>Indexer to retrive an engine component given an the component id.</summary>
        public IKEngineComponent this[string id] => GetComponent(id);
        ///<summary>Indexer to retrive an engine component given the component's type.</summary>
        public IKEngineComponent this[Type type] => GetComponent(type.Name);

        ///<summary>Creates the window, sets the update rate, and sets a refrence to the application.</summary>
        ///<param name = "app">Refrence to the application.</param>
        ///<param name = "updateRateTarget">The target framerate.</param>
        public KEngine(IKApplication app, IKRenderer renderer, byte updateRateTarget = 30)
        {
            Application = app;
            UpdateRateTarget = updateRateTarget;
            Window = new(VideoMode.DesktopMode, app.AppName);
            Window.Closed += (ignoreA, ignoreB) => End();

            _engineComponents = new(new KComponentSorter<IKEngineComponent>());
        }

        ///<summary>Executes initilization tasks for the engine. Should be called in the "Start" method</summary>
        public void Init()
        {
            if (!HasComponent<KDebugger>()) AddComponent(new KDebugger());
            if (ActiveRenderer == null) KDebugger.ErrorLog("There is no active renderer.");

            foreach (IKEngineComponent component in _engineComponents) component.Start();
        }

        ///<summary>Executes starting tasks, and starts the game loop.</summary>
        public void Start()
        {
            //Define time variables.
            uint currentUpdate = 0;
            double lastTime, newTime;
            double unprocessedTime = 0;

            Init(); //Run initilization tasks.
            lastTime = DateTime.UtcNow.Ticks; //Required for loop timing.

            while (IsRunning) //Core game-loop
            {
                newTime = DateTime.UtcNow.Ticks;
                unprocessedTime += (newTime - lastTime) / TimeSpan.TicksPerMillisecond;
                lastTime = newTime;

                //Keeps track of time between updates, if the interval is great enough, allows another update.
                if (unprocessedTime > UpdateInterval)
                {
                    //Loops to compensate for any lag, skipping pre-draw and render tasks. 
                    while (unprocessedTime > UpdateInterval && IsRunning) 
                    {
                        unprocessedTime -= UpdateInterval;
                        currentUpdate++;

                        Update(currentUpdate);
                    }
                    FrameUpdate(currentUpdate);

                    Window.Clear(Color.Black);
                    ActiveRenderer.Render(Window);
                    Window.Display();
                }
                Window.DispatchEvents();
            }
            foreach (IKEngineComponent component in _engineComponents) component.End();
        }

        ///<summary>Stops the game loop; executing tasks for the end of execution.</summary>
        public void End() => IsRunning = false;

        ///<summary>Executes code every update.</summary>
        ///<param name = "currentUpdate">Keeps track of the current frame.</param>
        public void Update(uint currentUpdate)
        {
            foreach (IKEngineComponent component in _engineComponents) component.Update(currentUpdate);
        }

        ///<summary>Executes pre-draw code every update. This method is called after the update method.</summary>
        ///<param name = "currentUpdate">Keeps track of the current frame.</param>
        public void FrameUpdate(uint currentUpdate)
        {
            foreach (IKEngineComponent component in _engineComponents) component.FrameUpdate(currentUpdate);
        }

        public IKEngineComponent AddComponent(IKEngineComponent component)
        {
            component.Engine = this;
            component.Init();
            _engineComponents.Add(component);
            return component;
        }

        public IKEngineComponent[] AddComponents(IKEngineComponent[] components)
        {
            foreach (var component in components) AddComponent(component);
            return components;
        }

        public bool RemoveComponent(string id)
        {
            foreach(var component in _engineComponents)
            {
                if (component.ID.Equals(id))
                {
                    _engineComponents.Remove(component);
                    return true;
                }
            }
            return false;
        }

        public bool RemoveComponent<TComponent>()
        {
            foreach (var component in _engineComponents)
            {
                if (component is TComponent)
                {
                    _engineComponents.Remove(component);
                    return true;
                }
            }
            return false;
        }

        public uint RemoveComponents<TComponent>()
        {
            uint count = 0;
            foreach (var component in _engineComponents) 
            {
                if (component is TComponent)
                {
                    _engineComponents.Remove(component);
                    count++;
                }
            }
            return count;
        }

        public bool HasComponent<TComponent>()
        {
            foreach (var component in _engineComponents)
                if (component is TComponent) return true;
            return false;
        }

        public bool HasComponent(string id)
        {
            foreach (var component in _engineComponents)
                if (component.ID.Equals(id)) return true;
            return false;
        }

        public uint HasComponents<TComponent>()
        {
            uint count = 0;

            foreach (var component in _engineComponents)
                if (component is TComponent) count++;
            return count;
        }

        public TComponent GetComponent<TComponent>() where TComponent : IKEngineComponent
        {
            foreach (IKComponent component in _engineComponents)
                if (component is TComponent) return (TComponent) component;
            return default;
        }

        public IKEngineComponent GetComponent(string id)
        {
            foreach (var component in _engineComponents)
                if (component.ID.Equals(id)) return component;
            return default;
        }

        public IKEngineComponent[] GetAllComponents() => _engineComponents.ToArray();

        public IKEngineComponent[] GetComponents<TComponent>() where TComponent : IKEngineComponent =>
            _engineComponents.Where((comp) => comp is TComponent).ToArray();
    }
}