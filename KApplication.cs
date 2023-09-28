using System.Collections;
using System.Text.Json;

namespace KheaiGameEngine
{
    public delegate void KEventManager();

    public class KComponentSorter
    {

    }

    public interface IKComponentContainer<Key,Value>
    {
        #region Component management
        public void AddComponent(Value component);
        public void AddComponents(Value[] components);
        public void RemoveComponent(Key id);
        public void RemoveComponent<Component>();
        public bool HasComponent(Key id);
        public bool HasComponent<Component>();
        public Value GetComponent(Key id);
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

        //Component Management
        private SortedSet<KComponent<KApplication>> _appComponents = new();

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
            foreach (KComponent<KApplication> component in _appComponents)
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

            foreach (KComponent<KApplication> component in _appComponents)
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
            _appComponents.Add(component);
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
            foreach (var component in _appComponents)
            {
                if (component.ID.Equals(id))
                {
                    _appComponents.Remove(component);
                    return;
                }
            }
            KDebug.Log($"Failed to remove component {id}.");
        }

        public void RemoveComponent<Component>()
        {
            foreach (var component in _appComponents)
            {
                if (component is Component)
                {
                    _appComponents.Remove(component);
                    return;
                }
            }
            KDebug.Log($"Failed to remove component {typeof(Component).Name}.");
        }

        public bool HasComponent(string id)
        {
            foreach (var component in _appComponents)
            {
                if (component.ID.Equals(id)) return true;
            }
            return false;
        }

        public bool HasComponent<Component>()
        {
            foreach (var component in _appComponents)
            {
                if (component is Component) return true;
            }
            return false;
        }

        public KComponent<KApplication> GetComponent(string id)
        {
            foreach (var component in _appComponents)
            {
                if (component.ID.Equals(id)) return component;
            }
            return null;
        }

        public Component GetComponent<Component>() where Component : KComponent<KApplication>
        {
            foreach (var component in _appComponents)
            {
                if (component is Component) return (Component)component;
            }
            return null;
        }

        public int SortByID(KComponent<KApplication> a, KComponent<KApplication> b)
        {
            if (a.ID.Equals(b.ID)) return 0;
            if (a.Order > b.Order) return 1;
            return -1;
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
            return null;
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
