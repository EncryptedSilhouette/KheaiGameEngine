using KheaiGameEngine.Core;

namespace KheaiGameEngine.GameObjects
{
    #region GameObject
    public class KGameObject : IKComponentContainer<KObjectComponent>
    {
        public string ID { get; protected set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public KTransform Transform { get; set; }
        public KGameObject Parent { get; set; }


        protected SortedSet<KObjectComponent> objectComponents = new(new KComponentSorter<KObjectComponent>());

        public KGameObject(string id) : this(id, id) { }

        public KGameObject(string id, string name)
        {
            ID = id;
            Name = name;
            Enabled = true;
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

        public KObjectComponent AddComponent(KObjectComponent component)
        {
            component.Owner = this;
            component.Init();
            objectComponents.Add(component);
            return component;
        }

        public KObjectComponent[] AddComponents(KObjectComponent[] components)
        {
            foreach (var component in components) AddComponent(component);
            return components;
        }

        public void RemoveComponent(string id)
        {
            foreach (var component in objectComponents)
            {
                if (component.ID.Equals(id))
                {
                    component.End();
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
                if (component is Component) return (Component) component;
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

        public void Update(uint currentFrame)
        {
            foreach (var component in objectComponents)
            {
                if (component.Enabled) 
                {
                    component.Update(currentFrame);
                }
            }
        }

        public void FrameUpdate(uint currentFrame)
        {
            foreach (var component in objectComponents)
            {
                if (component.Enabled)
                {
                    component.FrameUpdate(currentFrame);
                }
            }
        }
    }
    #endregion
}
