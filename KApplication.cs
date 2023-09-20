using System.Collections;
using System.Text.Json;

namespace KheaiGameEngine
{
    public delegate void KEventManager();

    public interface IKComponentContainer<Key,Value>
    {
        #region Component managemanet
        public void AddComponent(Value component);
        public void AddComponents(Value[] components);
        public void RemoveComponent(Key id);
        public void RemoveComponent<Component>();
        public bool HasComponent(Key id);
        public bool HasComponent<Component>();
        public Component GetComponent<Component>() where Component : Value;
        #endregion
    }

    public abstract class KComponent<Container>
    {
        public int Order { get; set; } = 0;
        public string ID { get; private init; }
        public Container Owner { get; protected set; }

        public KComponent()
        {
            ID = GetType().Name;   
        }

        public KComponent(int order) : this()
        {
            Order = order;
        }

        #region Logic
        public void Attatch(Container container)
        {
            Owner = container;
        }

        public abstract void Init();
        public abstract void Start();
        public abstract void End();
        #endregion
    }

    public class KApplication : IKComponentContainer<string, KComponent<KApplication>>
    {
        public int EventPollRate { get; set; } = 60;
        public bool IsRunning { get; private set; }
        public string AppName { get; private set; }
        public string PrefsFilePath { get; set; } = "prefs";
        public Hashtable Prefrences { get; set; } = new();

        private Dictionary<string, KComponent<KApplication>> _appComponents = new();

        //Threading
        private List<Thread> _threads = new();

        //Events
        public event KEventManager OnStart;
        public event KEventManager OnEventDispatch;
        public event KEventManager OnEnd;

        public KApplication(string appName)
        {
            AppName = appName;
        }

        #region Logic
        public void Start(string prefsFilePath)
        {
            LoadPrefsFromJson(LoadPrefsFromFile(prefsFilePath));
            Start();
        }

        public void Start()
        {
            IsRunning = true;

            KDebug.Log($"Starting App: {AppName}");
            foreach (KComponent<KApplication> component in _appComponents.Values)
            {
                KDebug.Log($"Starting Component: {component.ID}");
                component.Start();
            }
            OnStart?.Invoke();

            while (IsRunning)
            {
                lock (this)
                {
                    OnEventDispatch?.Invoke();
                }
                Thread.Sleep(1 / EventPollRate);
            }

            foreach (KComponent<KApplication> component in _appComponents.Values)
            {
                component.End();
            }
            lock (_threads)
            {
                foreach (Thread t in _threads)
                {
                    t.Join();
                }
            }
        }

        public void End()
        {
            IsRunning = false;
            OnEnd?.Invoke();
        }
        #endregion

        #region Component management
        public void AddComponent(KComponent<KApplication> component)
        {
            KDebug.Log($"Initializing component: {component.ID}");
            component.Attatch(this);
            component.Init();
            _appComponents.Add(component.ID, component);
        }

        public void AddComponents(KComponent<KApplication>[] components)
        {
            foreach(KComponent<KApplication> component in components)
            {
                AddComponent(component);
            }
        }

        public void RemoveComponent(string id)
        {
            KDebug.Log($"Removing component: {id}");
            _appComponents[id].End();

            if (!_appComponents.Remove(id))
            {
                KDebug.Log("Failed to remove component.");
            }
        }

        public void RemoveComponent<Component>()
        {
            _appComponents.Remove(typeof(Component).Name);
        }

        public bool HasComponent(string id)
        {
            return _appComponents.ContainsKey(id);
        }

        public bool HasComponent<Component>()
        {
            return _appComponents.ContainsKey(typeof(Component).Name);
        }

        public Component GetComponent<Component>() where Component : KComponent<KApplication>
        {
            return (Component) _appComponents[typeof(Component).Name];
        }
        #endregion

        #region Serialization
        public string LoadPrefsFromFile(string filePath)
        {
            KDebug.Log($"Loading prefrences from {filePath}.");
            if (File.Exists(filePath))
            {
                PrefsFilePath = filePath;
                return File.ReadAllText(filePath);
            }
            else return null;
        }

        public void LoadPrefsFromJson(string jsonString)
        {
            if (jsonString == null) return;
            Prefrences = JsonSerializer.Deserialize<Hashtable>(jsonString);
        }

        public void ClearPrefs()
        {
            File.Create(PrefsFilePath);
        }
        #endregion

        #region Threading
        public Thread CreateThread(string name, ThreadStart start)
        {
            Thread thread = new Thread(start);
            thread.Name = name;

            RegisterThread(thread);
            return thread;
        }

        public void RegisterThread(Thread thread)
        {
            KDebug.Log($"Registering thread: {thread.Name}");
            lock (thread)
            {
                _threads.Add(thread);
            }
        }
        #endregion
    }
}
