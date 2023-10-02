using System.Collections;
using System.Text.Json;

namespace KheaiGameEngine
{
    public delegate void KEventManager();

    public abstract class KAppComponent : KComponent
    {
        KApplication Owner { get; set; }
    }

    public class KApplication : IKComponentManager
    {
        #region Static
        //Events
        public static event KEventManager OnStart;
        public static event KEventManager OnEventDispatch;
        public static event KEventManager OnEnd;
        #endregion

        public int EventPollRate { get; set; } = 60;
        public bool IsRunning { get; private set; }
        public string AppName { get; private set; }
        public string PrefsFilePath { get; set; } = "prefs";
        public Hashtable Prefrences { get; set; } = new();

        //Component Management
        protected SortedSet<KComponent> appComponents;
        protected KComponentSorter<KComponent> componentSorter;

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
            foreach (KComponent component in appComponents)
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

            foreach (KComponent component in appComponents)
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

        #region Component management
        public void AddComponent(KComponent component)
        {
            KDebug.Log($"Initializing component: {component.ID}");

            component.Owner = this;
            component.Init();

            appComponents.Add(component);
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
            foreach (var component in appComponents)
            {
                if (component.ID.Equals(id))
                {
                    appComponents.Remove(component);
                    return;
                }
            }
            KDebug.Log($"Failed to remove component {id}.");
        }

        public void RemoveComponent<Component>()
        {
            foreach (var component in appComponents)
            {
                if (component is Component)
                {
                    appComponents.Remove(component);
                    return;
                }
            }
            KDebug.Log($"Failed to remove component {typeof(Component).Name}.");
        }

        public bool HasComponent(string id)
        {
            foreach (var component in appComponents)
            {
                if (component.ID.Equals(id)) return true;
            }
            return false;
        }

        public bool HasComponent<Component>()
        {
            foreach (var component in appComponents)
            {
                if (component is Component) return true;
            }
            return false;
        }

        public KComponent GetComponent(string id)
        {
            foreach (var component in appComponents)
            {
                if (component.ID.Equals(id)) return component;
            }
            return null;
        }

        public Component GetComponent<Component>() where Component : KComponent
        {
            foreach (var component in appComponents)
            {
                if (component is Component) return (Component)component;
            }
            return null;
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
    }
}
