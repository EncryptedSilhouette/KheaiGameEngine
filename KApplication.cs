using System.ComponentModel;

namespace KheaiGameEngine
{
    public delegate void KEventManager();

    public abstract class KAppComponent : KComponent
    {
        public KApplication App { get; set; }

        public abstract void Update();
    }

    public class KApplication : IKComponentContainer<KAppComponent>
    {
        #region Static
        public static bool IsRunning;
        public static string AppName;
        #endregion

        public int UpdateRate { get; set; } = 60;

        //Component Management
        protected SortedSet<KAppComponent> appComponents;

        public KApplication(string appName)
        {
            AppName = appName;
            appComponents = new(new KComponentSorter<KAppComponent>());
        }

        #region Logic
        public void Start()
        {
            IsRunning = true;

            KDebug.Log($"Starting App: {AppName}");
            foreach (KComponent component in appComponents)
            {
                KDebug.Log($"Starting Component: {component.ID}");
                component.Start();
            }

            while (IsRunning)
            {
                foreach (var component in appComponents)
                {
                    component.Update();
                }
                Thread.Sleep(1 / UpdateRate);
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
        }
        #endregion

        public void AddComponent(KAppComponent component)
        {
            component.App = this;
            component.Init();

            if (appComponents.Add(component))
            {
                KDebug.Log($"{AppName}: Added component- {component.ID}.");
            }
            else KDebug.Log($"{AppName}: Failed to add component- {typeof(Component).Name}.");
        }

        public void AddComponents(KAppComponent[] components)
        {
            foreach (var component in components)
            {
                AddComponent(component);
            }
        }

        public void RemoveComponent<Component>()
        {
            foreach (var component in appComponents)
            {
                if (component is Component)
                {
                    appComponents.Remove(component);
                    KDebug.Log($"{AppName}: Removed component- {typeof(Component).Name}.");
                    return;
                }
            }
            KDebug.Log($"{AppName}: Failed to remove component- {typeof(Component).Name}.");
        }

        public void RemoveComponent(string id)
        {
            foreach (var component in appComponents)
            {
                if (component.ID.Equals(id))
                {
                    appComponents.Remove(component);
                    KDebug.Log($"{AppName}: Removed component- {id}.");
                    return;
                }
            }
            KDebug.Log($"{AppName}: Failed to remove component- {id}.");
        }

        public bool HasComponent<Component>()
        {
            foreach (var component in appComponents)
            {
                if (component is Component) return true;
            }
            return false;
        }

        public bool HasComponent(string id)
        {
            foreach (var component in appComponents)
            {
                if (component.ID.Equals(id)) return true;
            }
            return false;
        }

        public Component GetComponent<Component>() where Component : KAppComponent
        {
            foreach (KComponent component in appComponents)
            {
                if (component is Component) return (Component) component;
            }
            return null;
        }

        public KAppComponent GetComponent(string id)
        {
            foreach (var component in appComponents)
            {
                if (component.ID.Equals(id)) return component;
            }
            return null;
        }
    }
}
