namespace KheaiGameEngine.Core.KCollections
{
    public interface IKSearchableCollection<Type> : IEnumerable<Type>, IReadOnlyList<Type>
    {
        ///<summary>Determines whether an element exists in the collection.</summary>
        public bool Contains(Type item);

        ///<summary>Retrieves an element that matches the conditions defined by the specified predicate.</summary>
        public Type Find(Predicate<Type> match);

        ///<summary>Retrieves all elements that match the conditions defined by the specified predicate.</summary>
        public IEnumerable<Type> FindALL(Predicate<Type> match);
    }
}
