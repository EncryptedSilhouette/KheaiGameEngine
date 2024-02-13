using KheaiGameEngine.Core;
using System.Text.Json.Serialization;

namespace KheaiGameEngine.GameObjects
{
    #region ObjectComponent
    [JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor, TypeDiscriminatorPropertyName = "TestComponent")]
    public abstract class KObjectComponent : IKComponent, IKEngineManaged
    {
        [JsonInclude][JsonPropertyOrder(1)]
        public ushort Order { get; set; }

        [JsonInclude][JsonPropertyOrder(0)]
        public string ID { get; set; }

        [JsonInclude][JsonPropertyOrder(2)]
        public KGameObject Owner { get; set; }
        
        public KObjectComponent()
        {
            ID = GetType().Name;
        }

        public abstract void Init();
        public abstract void Start();
        public abstract void End();
        public abstract void Update(uint currentTick);
        public abstract void FrameUpdate(uint currentFrame);
    }
    #endregion

    #region ObjectData
    public class KObjectData
    {
        public string ID { get; set; }
        public List<KObjectComponent> Components { get; set; }

        public KGameObject CreateObject()
        {
            return CreateObject(ID);
        }

        public KGameObject CreateObject(string name)
        {
            KGameObject gameObject = new(ID, name);

            foreach (var component in Components)
            {

            }

            return gameObject;
        }
    }
    #endregion

    #region GameObject
    public class KGameObject : IKComponentContainer<KObjectComponent>, IKEngineManaged
    {
        public string ID { get; protected set; }
        public string Name { get; set; }
        public KGameObject Parent { get; set; }
        public KSceneHandler Handler { get; set; }

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

        public void Update(uint currentTick)
        {
            foreach (var component in objectComponents)
            {
                component.Update(currentTick);
            }
        }

        public void FrameUpdate(uint currentFrame)
        {
            foreach (var component in objectComponents)
            {
                component.FrameUpdate(currentFrame);
            }
        }
    }
    #endregion
}
