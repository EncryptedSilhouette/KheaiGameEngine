namespace KheaiGameEngine
{
    public interface IKComponentManager 
    {
        public void AddComponent(KComponent component);
        public void AddComponents(KComponent[] components);
        public void RemoveComponent<Component>();
        public void RemoveComponent(string id);
        public bool HasComponent<Component>();
        public bool HasComponent(string id);
        public Component GetComponent<Component>() where Component : KComponent;
        public KComponent GetComponent(string id);
    }

    public abstract class KComponent
    {
        public int Order { get; set; }
        public string ID { get; protected init; }

        public KComponent() 
        {
            ID = GetType().Name;    
        }
           
        public abstract void Init(IKComponentManager owner);
        public abstract void Start();
        public abstract void End();
    }

    public class KComponentSorter<Component> : IComparer<Component> where Component : KComponent
    {
        public int Compare(Component x, Component y)
        {
            if (x.ID == y.ID) return 0;
            if (x.Order > y.Order) return 1;
            return -1;
        }
    }
}
