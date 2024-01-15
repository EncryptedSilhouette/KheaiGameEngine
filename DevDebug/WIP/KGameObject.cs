using KheaiGameEngine.EngineComponents;

namespace KheaiGameEngine
{
    #region ObjectComponent
    public abstract class KObjectComponent : IKComponent, IKEngineManaged
    {
        public int Order { get; set; }
        public string ID { get; init; }
        public KGameObject Owner { get; set; }

        public KObjectComponent()
        {
            ID = GetType().Name;
        }

        public abstract void Init();
        public abstract void Start();
        public abstract void End();
        public abstract void FixedUpdate();
        public abstract void FrameUpdate(double deltaTIme);
    }
    #endregion

    #region GameObject
    public class KGameObject : IKComponentContainer<KObjectComponent>, IKContainerManaged, IKEngineManaged
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public KSceneHandler SceneManager { get; set; }

        protected SortedSet<KObjectComponent> objectComponents = new(new KComponentSorter<KObjectComponent>());

        public KGameObject(string id) : this(id, id) { }

        public KGameObject(string id, string name)
        {
            ID = id;
            Name = name;
        }

        public void Init()
        {
        }

        public void Start()
        {
            foreach (KObjectComponent component in objectComponents)
            {
                component.Start();
            }
        }

        public void End()
        {
            foreach (KObjectComponent component in objectComponents)
            {
                component.End();
            }
        }

        public void AddComponent(KObjectComponent component)
        {
            component.Owner = this;
            component.Init();
            objectComponents.Add(component);
        }

        public void AddComponents(KObjectComponent[] components)
        {
            foreach (var component in components)
            {
                AddComponent(component);
            }
        }

        public void RemoveComponent(string id)
        {
            foreach (var component in objectComponents)
            {
                if (component.ID.Equals(id))
                {
                    objectComponents.Remove(component);
                    return;
                }
            }
        }

        public void RemoveComponent<Component>()
        {
            foreach (var component in objectComponents)
            {
                if (component is Component)
                {
                    objectComponents.Remove(component);
                    return;
                }
            }
        }

        public bool HasComponent<Component>()
        {
            foreach (var component in objectComponents)
            {
                if (component is Component) return true;
            }
            return false;
        }

        public bool HasComponent(string id)
        {
            foreach (var component in objectComponents)
            {
                if (component.ID.Equals(id)) return true;
            }
            return false;
        }

        public Component GetComponent<Component>() where Component : KObjectComponent
        {
            foreach (IKComponent component in objectComponents)
            {
                if (component is Component) return (Component)component;
            }
            return null;
        }

        public KObjectComponent GetComponent(string id)
        {
            foreach (var component in objectComponents)
            {
                if (component.ID.Equals(id)) return component;
            }
            return null;
        }

        public void FixedUpdate()
        {
            //throw new NotImplementedException();
        }

        public void FrameUpdate(double deltaTIme)
        {
            //throw new NotImplementedException();
        }
    }
    #endregion
}
