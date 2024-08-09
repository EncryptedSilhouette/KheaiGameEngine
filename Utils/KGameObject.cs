using KheaiGameEngine;

namespace KheaiUtils
{
    public class KGameObject : IKComponentContainer<KObjectComponent>
    {
        private bool _enabled = true;
        private List<KGameObject> _children = new();
        
        ///<summary>Container for objectComponents.</summary>
        protected SortedSet<KObjectComponent> objectComponents = new(new KComponentSorter<KObjectComponent>());

        ///<summary>The entity id for this gameobject.</summary>
        public string ID;
        ///<summary>The entity name for this gameobject.</summary>
        public string Name;
        ///<summary>The parent object for this gameobject.</summary>
        public KGameObject Parent;

        ///<summary>Fires when the gameobject is initialized.</summary>
        public event Action<KGameObject> OnInit;
        ///<summary>Fires when the gameobject is enabled.</summary>
        public event Action<KGameObject> OnEnable;
        ///<summary>Fires when the gameobject is disabled.</summary>
        public event Action<KGameObject> OnDisable;

        ///<summary>Gets the child object at the specifed index.</summary>
        public KGameObject this[int index] => _children[index];
        ///<summary>Gets the child object with the specifed name.</summary>
        public KGameObject this[string name] => _children.Find(child => child.Name == name);

        ///<summary>Activates or deactivates a gameobject.</summary>
        public bool Enabled 
        {
            get => _enabled;
            set
            {
                //prevents code from being fired if it is being set to the same state
                if (_enabled == value) return;
                
                if (value == true) OnEnable.Invoke(this);
                else OnDisable.Invoke(this);
                
                _enabled = value;
            }
        }

        public KGameObject(string id, string name = null)
        {
            ID = id;
            Name = name ?? id;
        }

        public void Init()
        {
            OnInit.Invoke(this);
        }

        public void Start()
        {
            foreach (KObjectComponent component in objectComponents) component.Start();
        }

        public void End()
        {
            foreach (KObjectComponent component in objectComponents) component.End();
        }

        public void Update(uint currentFrame)
        {
            foreach (var component in objectComponents)
                if (component.Enabled) component.Update(currentFrame);
        }

        public void FrameUpdate(uint currentFrame)
        {
            foreach (var component in objectComponents)
                if (component.Enabled) component.FrameUpdate(currentFrame);
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

        public bool RemoveComponent(string id)
        {
            foreach (var component in objectComponents)
            {
                if (component.ID.Equals(id))
                {
                    component.End();
                    objectComponents.Remove(component);
                    return true;
                }
            }
            return false;
        }

        public bool RemoveComponent<Component>()
        {
            foreach (var component in objectComponents)
            {
                if (component is Component)
                {
                    objectComponents.Remove(component);
                    return true;
                }
            }
            return false;
        }

        public uint RemoveComponents<TComponent>()
        {
            uint count = 0;
            foreach (var component in objectComponents)
            {
                if (component is TComponent)
                {
                    objectComponents.Remove(component);
                    count++;
                }
            }
            return count;
        }

        public bool HasComponent<Component>()
        {
            foreach (var component in objectComponents)
                if (component is Component) return true;
            return false;
        }

        public bool HasComponent(string id)
        {
            foreach (var component in objectComponents)
                if (component.ID.Equals(id)) return true;
            return false;
        }

        public uint HasComponents<TComponent>()
        {
            uint count = 0;

            foreach (var component in objectComponents)
                if (component is TComponent) count++;
            return count;
        }

        public Component GetComponent<Component>() where Component : KObjectComponent
        {
            foreach (IKComponent component in objectComponents)
                if (component is Component) return (Component) component;
            return default;
        }

        public KObjectComponent GetComponent(string id)
        {
            foreach (var component in objectComponents)
                if (component.ID.Equals(id)) return component;
            return default;
        }

        public KObjectComponent[] GetAllComponents() => objectComponents.ToArray();

        public KObjectComponent[] GetComponents<ComponentT>() where ComponentT : KObjectComponent =>
            objectComponents.Where((comp) => comp is ComponentT).ToArray();
    }
}