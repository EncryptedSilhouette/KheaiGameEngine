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
        public Value GetComponent(Key id);
        public Value GetComponent<Component>() where Component : Value;
        #endregion
    }

    public interface IKComponent<Container>
    {
        public void Attatch(Container container);
        public void Init();
        public void Start();
        public void End();
    }

    public abstract class KAppComponent : IKComponent<KApplication>
    {
        public string ID => GetType().Name;
        public KApplication Application { get; protected set; }

        public void Attatch(KApplication app)
        {
            Application = app;
        }

        public abstract void Init();
        public abstract void Start();
        public abstract void End(); 
    }

    public class KApplication : IKComponentContainer<string, KAppComponent>
    {
        public int EventPollRate { get; set; }
        public bool IsRunning { get; private set; }
        public string AppName { get; private set; }
        public string PrefsFilePath { get; set; }
        public Hashtable Prefrences { get; set; }

        private Dictionary<string, KAppComponent> _appComponents = new();

        //Events
        public event KEventManager OnStart;
        public event KEventManager OnEventDispatch;
        public event KEventManager OnEnd;

        //Threading
        private List<Thread> _threads = new();

        public KApplication(string appName)
        {
            AppName = appName;
            EventPollRate = 60;
            PrefsFilePath = "prefs";
            Prefrences = new();
        }

        #region Logic
        public void Start(string prefsFilePath)
        {
            LoadPrefsFromJson(LoadPrefsFromFile(prefsFilePath));
            Start();
        }

        public void Start()
        {   
            foreach (KAppComponent component in _appComponents.Values)
            {
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

            foreach (KAppComponent component in _appComponents.Values)
            {
                component.End();
            }
            End();
        }

        private void End()
        {
            lock (_threads)
            {
                foreach (Thread t in _threads)
                {
                    t.Join();
                }
            }
            OnEnd?.Invoke();
        }
        #endregion

        #region Component management
        public void AddComponent(KAppComponent component)
        {
            component.Attatch(this);
            component.Init();
            _appComponents.Add(component.ID, component);
        }

        public void AddComponents(KAppComponent[] components)
        {
            foreach(KAppComponent component in components)
            {
                AddComponent(component);
            }
        }

        public void RemoveComponent(string id)
        {
            _appComponents[id].End();
            _appComponents.Remove(id);
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

        public KAppComponent GetComponent(string id)
        {
            return _appComponents[id];
        }

        public KAppComponent GetComponent<Component>() where Component : KAppComponent
        {
            return (Component) _appComponents[typeof(Component).Name];
        }
        #endregion

        #region Serialization
        public string LoadPrefsFromFile(string filePath)
        {
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

        //Wtf is even my plan here
        //Threading fuck me
        public void RegisterThread(Thread thread)
        {
            lock (thread)
            {
                _threads.Add(thread);
            }
        }
    }
}
