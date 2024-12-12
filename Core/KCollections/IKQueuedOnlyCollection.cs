namespace KheaiGameEngine.Core.KCollections
{
    public interface IKQueuedOnlyCollection<Type> : IEnumerable<Type>, IReadOnlyList<Type>, IKSearchableCollection<Type>
    {
        ///<summary>Enqueues a value to be added to the collection.</summary>
        public void QueueAdd(Type value);

        ///<summary>Enqueues an IEnumerable collection of values to be added to the collection.</summary>
        public void QueueAddAll(IEnumerable<Type> values);

        ///<summary>Enqueues a value to be removed from the collection.</summary>
        public void QueueRemove(Type value);

        ///<summary>Enqueues an IEnumerable collection of values to be removed from the collection.</summary>
        public void QueueRemoveAll(IEnumerable<Type> values);

        ///<summary>Applies the enqueued changes to the collection.</summary>
        public IEnumerable<Type> UpdateContents();
    }
}
