namespace KheaiGameEngine
{
    ///<summary>Interface for creating components using the KComponent structure.</summary>
    public interface IKComponent
    {
        ///<summary>The order the component will be updated.</summary>
        public ushort Order { get; set; }

        ///<summary>/The ID for the component.</summary>
        public string ID { get; set; }

        ///<summary>Executes any initilization code for the component. Should be called in the "Start" method</summary>
        public void Init();

        ///<summary>Executes starting code for the component.</summary>
        public void Start();

        ///<summary>Executes code for the end of execution.</summary>
        public void End();
    }

    ///<summary>Interface for a component container that implements the IKComponent interface.</summary>
    public interface IKComponentContainer<KComponent> where KComponent : IKComponent
    {
        ///<summary>Add a component to a collection.</summary>
        public void AddComponent(KComponent component);

        ///<summary>Add an array of components to a collection.</summary>
        public void AddComponents(KComponent[] components);

        ///<summary>Remove an array of components to a collection.</summary>
        public void RemoveComponent(string id);

        ///<summary>Remove a component of a specified type from a collection.</summary>
        public void RemoveComponent<Component>();

        ///<summary>Check if a component id exists in a collection.</summary>
        public bool HasComponent(string id);

        ///<summary>Check if a component with a specified type exists in a collection.</summary>
        public bool HasComponent<Component>();

        ///<summary>Retrieve a component from a collection.</summary>
        public KComponent GetComponent(string id);

        ///<summary>Remove an array of components to a collection.</summary>
        public Component GetComponent<Component>() where Component : KComponent;
    }

    ///<summary>A class for sorting components that implement the IKComponent interface in a SortedSet.</summary>
    public class KComponentSorter<KComponent> : IComparer<KComponent> where KComponent : IKComponent
    {
        ///<summary>
        ///A comparer for sorting KComponents. 
        ///Components are sorted by their Order. Components with the same ID are considered the same.
        ///</summary>
        public int Compare(KComponent a, KComponent b)
        {
            //Checks if the components have the same ID.
            //This prevents multiple components with the same ID.
            if (a.ID == b.ID) return 0;

            //Sorts the components by order
            //Only this one check matters since we dont want to check if the order is equal as it will remove it from the set.
            if (a.Order < b.Order) return -1;
            return 1;
        }
    }
}
