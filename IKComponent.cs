namespace KheaiGameEngine
{
    public interface IKComponent
    {
        public int Order { get; set; }
        public string ID { get; init; }
        public IKComponentContainer<IKComponent> owner { get; set; }

        public void Init();
        public void Start();
        public void End();
    }

    public interface IKComponentContainer<Component> where Component : IKComponent
    {
        public void AddComponent(Component component);
        public void AddComponents(Component[] components);
        public void RemoveComponent<Comp>();
        public void RemoveComponent(string id);
        public void HasComponent<Comp>();
        public void HasComponent(string id);
        public void GetComponent<Comp>();
        public void GetComponent(string id);
    }

    public class KComponentSorter<Component> : IComparer<Component> where Component : IKComponent
    {
        public int Compare(Component x, Component y)
        {
            if (x.ID == y.ID) return 0;
            if (x.Order > y.Order) return 1;
            return -1;
        }
    }
}
