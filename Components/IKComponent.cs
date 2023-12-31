﻿namespace KheaiGameEngine
{
    public interface IKContainerManaged
    {
        public void Init();
        public void Start();
        public void End();
    }

    public interface IKComponent : IKContainerManaged
    {
        public int Order { get; set; }
        public string ID { get; init; }
    }

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
