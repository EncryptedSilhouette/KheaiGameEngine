using System.Collections;
using System.Text.Json;

namespace KheaiGameEngine
{
    public delegate void KThreadManager();
    public delegate void KAppEventManager();

    public interface IKComponentContainer<Key,Value>
    {
        public void AddComponent(Value component);
        public void AddComponents(Value[] components);
        public void RemoveComponent(Key id);
        public bool HasComponent(Key id);
        public bool HasComponent<Component>();
        public Value GetComponent(Key id);
        public Value GetComponent<Component>();
    }

    public abstract class KComponent
    {
        public string ID => GetType().Name;
        public KApplication Application { get; protected set; }

        public abstract void Init(KApplication app);
        public abstract void Start();
        public abstract void End();
    }

    public class KApplication : IKComponentContainer<string, KComponent>
    {
        private Dictionary<string, KComponent> _appComponents = new();

        public int EventPollRate { get; set; }
        public bool IsRunning { get; private set; }
        public string PrefsFilePath { get; set; }
        public Hashtable Prefrences { get; set; }

        //Events
        public event KAppEventManager OnStart;
        public event KAppEventManager OnEventDispatch;
        public event KAppEventManager OnEnd;

        //Threading
        private List<Thread> _threads = new();

        public KApplication()
        {
            EventPollRate = 60;
            PrefsFilePath = "prefs";
            Prefrences = new();
        }

        public void Start(string prefsFilePath)
        {
            LoadPrefsFromJson(LoadPrefsFromFile(prefsFilePath));
            Start();
        }

        public void Start()
        {   
            foreach (KComponent component in _appComponents.Values)
            {
                component.Start();
            }
            OnStart();

            while (IsRunning)
            {
                lock (OnEventDispatch)
                {
                    OnEventDispatch();
                }
                Thread.Sleep(1 / EventPollRate);
            }

            foreach (KComponent component in _appComponents.Values)
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
            OnEnd();
            KDebug.DumpLog();
        }

        public void AddComponent(KComponent component)
        {
            component.Init(this);
            _appComponents.Add(component.ID, component);
        }

        public void AddComponents(KComponent[] components)
        {
            foreach(KComponent component in components)
            {
                AddComponent(component);
            }
        }

        public void RemoveComponent(string id)
        {
            _appComponents[id].End();
            _appComponents.Remove(id);
        }

        public bool HasComponent(string id)
        {
            return _appComponents.ContainsKey(id);
        }

        public bool HasComponent<Component>()
        {
            return _appComponents.ContainsKey(typeof(Component).Name);
        }

        public KComponent GetComponent(string id)
        {
            return _appComponents[id];
        }

        public KComponent GetComponent<Component>()
        {
            return _appComponents[typeof(Component).Name];
        }

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

        //Threading
        public void RegisterThread(Thread thread)
        {
            lock (thread)
            {
                _threads.Add(thread);
            }
        }
    }
}
