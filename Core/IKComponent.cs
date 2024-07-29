namespace KheaiGameEngine
{
    ///<summary>Interface for creating components using the KComponent structure.</summary>
    public interface IKComponent
    {
        ///<summary>The order the component will be updated.</summary>
        public short Order { get; init; }

        ///<summary>The ID for the component.</summary>
        public string ID { get; init; }

        ///<summary>Executes initilization tasks for the component. Should be called in the "Start" method</summary>
        public void Init();

        ///<summary>Executes starting tasks for the component.</summary>
        public void Start();

        ///<summary>Executes tasks for the end of execution.</summary>
        public void End();
    }

    ///<summary>Interface for a component container that implements the IKComponent interface.</summary>
    public interface IKComponentContainer<Component> where Component : IKComponent
    {
        ///<summary>Add a component to a collection.</summary>
        ///<param name="component">Component to be added to the component container.</param>
        public Component AddComponent(Component component);

        ///<summary>Add an array of components to a collection.</summary>
        ///<param name="components">Components to be added to the component container.</param>
        public Component[] AddComponents(Component[] components);

        ///<summary>Remove a component  from a collection. Returns true if component is successfully removed.</summary>
        ///<param name="id">Component's id.</param>
        public bool RemoveComponent(string id);

        ///<summary>Remove a component of a specified type or subtype from a collection. Returns true if component is successfully removed.</summary>
        public bool RemoveComponent<TComponent>();

        ///<summary>Remove components of a specified type or subtype from a collection. Returns count of components removed.</summary>
        public uint RemoveComponents<TComponent>();

        ///<summary>Check if a component with a specifed id exists in a collection.</summary>
        ///<param name="id">Component's id.</param>
        public bool HasComponent(string id);

        ///<summary>Check if a component with a specified type or subtype exists in a collection.</summary>
        public bool HasComponent<TComponent>();

        ///<summary>Check if multiple components of a specified type or subtype exists in a collection. 
        ///Returns the count or 0 if there's none.</summary>
        public uint HasComponents<TComponent>();

        ///<summary>Retrieve a component with a specifed id from a collection.</summary>
        ///<param name="id">Component's id.</param>
        public Component GetComponent(string id);

        ///<summary>Retrieve a component of a specifed type or subtype from a collection.</summary>
        public TComponent GetComponent<TComponent>() where TComponent : Component;

        ///<summary>Retrieves an array of components of a specified type or subtype from a collection.</summary>
        public Component[] GetComponents<TComponent>() where TComponent : Component;

        ///<summary>Get an array of all components from a collection.</summary>
        public Component[] GetAllComponents();
    }

    ///<summary>A class for sorting components that implement the IKComponent interface in a SortedSet.</summary>
    public class KComponentSorter<KComponent> : IComparer<KComponent> where KComponent : IKComponent
    {
        ///<summary>A comparer for sorting KComponents. Components are sorted by their Order. 
        ///Components with the same ID are considered the same.</summary>
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
