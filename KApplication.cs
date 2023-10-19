using System.ComponentModel;

namespace KheaiGameEngine
{
    public delegate void KEventManager();

    public interface IKAppComponent : IKComponent
    {
        #region Game logic
        public KApplication App { get; set; }
        public abstract void Update();
        #endregion
    }

    public class KApplication : IKComponentContainer<IKAppComponent>
    {

        public bool IsRunning;
        public string AppName;
        protected SortedSet<IKAppComponent> appComponents;

        public int UpdateRate { get; set; } = 60;

        public KApplication(string appName)
        {
            AppName = appName;
            appComponents = new(new KComponentSorter<IKAppComponent>());
        }

        #region Logic
        public void Start()
        {
            IsRunning = true;

            KDebug.Log($"Starting App: {AppName}");
            foreach (IKComponent component in appComponents)
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

            foreach (IKComponent component in appComponents)
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

        public void AddComponent(IKAppComponent component)
        {
            component.App = this;
            component.Init();
            appComponents.Add(component);
        }

        public void AddComponents(IKAppComponent[] components)
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
                    return;
                }
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

        public Component GetComponent<Component>() where Component : class, IKAppComponent
        {
            foreach (IKComponent component in appComponents)
            {
                if (component is Component) return (Component) component;
            }
            return null;
        }

        public IKAppComponent GetComponent(string id)
        {
            foreach (var component in appComponents)
            {
                if (component.ID.Equals(id)) return component;
            }
            return null;
        }
    }
}
