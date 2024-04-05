namespace KheaiGameEngine
{
    ///<summary>
    ///Interface for components using the KComponent structure.
    ///</summary>
    public interface IKComponent
    {
        ///<summary>
        ///The order the component will be updated.
        ///</summary>
        public ushort Order { get; set; }

        ///<summary>
        ///The ID for the component. 
        ///</summary>
        public string ID { get; set; }

        public void Init();
        public void Start();
        public void End();
    }

    ///<summary>
    ///Interface for containers using components that implement the IKComponent interface.
    ///</summary>
    public interface IKComponentContainer<Component> where Component : IKComponent
    {
        public void AddComponent(Component component);
        public void AddComponents(Component[] components);
        public void RemoveComponent<Comp>();
        public void RemoveComponent(string id);
        public bool HasComponent<Comp>();
        public bool HasComponent(string id);
        public Comp GetComponent<Comp>() where Comp : Component;
        public Component GetComponent(string id);
    }

    ///<summary>
    ///Class for sorting components that implement the IKComponent interface for a SortedSet
    ///</summary>
    public class KComponentSorter<Component> : IComparer<Component> where Component : IKComponent
    {
        public int Compare(Component a, Component b)
        {
            //Checks if the components have the same ID.
            //This prevents multiple components with the same ID.
            if (a.ID == b.ID) return 0;

            //Sorts the components by order
            //Only this one check matters since we dont want to check if the order is equal as it will remove it from the set.
            if (a.Order > b.Order) return 1;
            return -1;
        }
    }
}
