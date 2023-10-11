using System.Collections;
using System.Text.Json;

namespace KheaiGameEngine
{
    public delegate void KEventManager();

    public abstract class KAppComponent : IKComponent
    {
        public int Order { get; set; }
        public string ID { get; init; }
        public KApplication Owner { get; protected set; }        

        public KAppComponent()
        {
            ID = GetType().Name;
        }

        public abstract void Init<Container>(Container owner);
        public abstract void Start();
        public abstract void End();
    }

    public class KApplication : IKComponentContainer<KAppComponent>
    {
        public int EventPollRate { get; set; } = 60;
        public bool IsRunning { get; private set; }
        public string AppName { get; private set; }
        public string PrefsFilePath { get; set; } = "prefs";
        public Hashtable Prefrences { get; set; } = new();

        //Component Management
        protected SortedSet<KAppComponent> appComponents;
        protected KComponentSorter<KAppComponent> componentSorter;

        //Events
        public static event KEventManager OnStart;
        public static event KEventManager OnEventDispatch;
        public static event KEventManager OnEnd;

        public KApplication(string appName)
        {
            AppName = appName;
            componentSorter = new();
            appComponents = new(componentSorter);
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
            foreach (IKComponent component in appComponents)
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

            foreach (IKComponent component in appComponents)
            {
                component.End();
            }
            KThreadManager.JoinAllThreads();
        }

        public void End()
        {
            IsRunning = false;
            OnEnd?.Invoke();
        }
        #endregion

        public void AddComponent(KAppComponent component)
        {
            KDebug.Log("");
            component.Init(component);
            appComponents.Add(component);
        }

        public void AddComponents(KAppComponent[] components)
        {
            throw new NotImplementedException();
        }

        public void RemoveComponent<Comp>()
        {
            throw new NotImplementedException();
        }

        public void RemoveComponent(string id)
        {
            throw new NotImplementedException();
        }

        public void HasComponent<Comp>()
        {
            throw new NotImplementedException();
        }

        public void HasComponent(string id)
        {
            throw new NotImplementedException();
        }

        public void GetComponent<Comp>()
        {
            throw new NotImplementedException();
        }

        public void GetComponent(string id)
        {
            throw new NotImplementedException();
        }

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
    }
}
