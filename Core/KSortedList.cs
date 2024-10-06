#nullable disable

using KheaiGameEngine.Extensions;
using System.Collections;

namespace KheaiGameEngine.Core
{
    public interface IKQueuedOnlyCollection<Type> : IEnumerable<Type>
    {
        public IEnumerable<Type> Contents { get; }

        ///<summary>Enqueues a value to be added to the collection.</summary>
        public void QueueAdd(Type value);

        ///<summary>Enqueues an enumerable collection of values to be added to the collection.</summary>
        public void QueueAddAll(IEnumerable<Type> values);

        ///<summary>Enqueues a value to be removed from the collection.</summary>
        public void QueueRemove(Type value);

        ///<summary>Enqueues an enumerable collection of values to be removed from the collection.</summary>
        public void QueueRemoveAll(IEnumerable<Type> values);

        ///<summary>Applies the enqueued changes to the collection.</summary>
        public void UpdateContents();
    }

    public interface IKSearchableCollection<Type> : IEnumerable<Type>
    {
        public IEnumerable<Type> Contents { get; }

        public Type this[int index] { get; }

        ///<summary>Retrieves an element that matches the conditions defined by the specified predicate.</summary>
        public Type Find(Predicate<Type> match);

        ///<summary>Retrieves all elements that match the conditions defined by the specified predicate.</summary>
        public IEnumerable<Type> FindALL(Predicate<Type> match);

        ///<summary>Determines whether an element exists in the collection.</summary>
        public bool Contains(Type item);
    }

    public class KSortedList<Type> : ICollection<Type>, IKSearchableCollection<Type>
    {
        protected List<Type> contents;
        protected IComparer<Type> comparer;

        public bool IsReadOnly => false;
        ///<summary>The number of elements in this collection.</summary>
        public int Count => contents.Count;

        public IEnumerable<Type> Contents => contents;
        ///<summary>The comparer used for sorting.</summary>
        public IComparer<Type> Comparer 
        {
            get => comparer;
            set
            {
                comparer = value;
                contents.Sort(comparer);
            }
        }

        ///<summary>Gets the element from the specified index.</summary>
        public Type this[int index] => contents[index];

        ///<summary>Create a new instance of KSortedList.</summary>                //Sorry i got lazy. Normally i'd never use "this".    
        public KSortedList(IComparer<Type> comparer, int capacity = 0) => (contents, this.comparer) = (new(capacity), comparer);

        ///<summary>Adds a value to the collection. Does a binary search before insertion to maintain a sorted collection.</summary>
        public void Add(Type value) => contents.BinaryInsert(value, comparer.Compare);

        ///<summary>Adds an enumerable collection of values to the collection. Does a binary search before insertion to maintain a sorted collection.</summary>
        public void AddAll(IEnumerable<Type> values) => contents.AddRange(values);

        ///<summary>Removes the specified value from the collection.</summary>
        public bool Remove(Type value) => contents.Remove(value);

        ///<summary>Removes an enumerable collection of values from the collection.</summary>
        public void RemoveAll(IEnumerable<Type> values) => values.ForEach((value) => contents.Remove(value));

        ///<summary>Removes all elements from the collection.</summary>
        public void Clear() => contents.Clear();

        ///<summary>Retrieves an element that matches the conditions defined by the specified predicate.</summary>
        public Type Find(Predicate<Type> match) => contents.Find(match);

        ///<summary>Retrieves all elements that match the conditions defined by the specified predicate.</summary>
        public IEnumerable<Type> FindALL(Predicate<Type> match) => contents.FindAll(match);

        ///<summary>Determines whether an element exists in the collection.</summary>
        public bool Contains(Type item) => contents.Contains(item);

        ///<summary>Copies the elements from this collection to an array.</summary>
        public void CopyTo(Type[] array, int arrayIndex) => contents.CopyTo(array, arrayIndex);

        public IEnumerator<Type> GetEnumerator() => contents.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class KSortedQueuedList<Type> : KSortedList<Type>, IKQueuedOnlyCollection<Type> where Type : IKEngineObject
    {
        private int _removeCount;
        private PriorityQueue<Type, byte> _queue = new();

        public int QueuedChanges => _queue.Count;

        public KSortedQueuedList(IComparer<Type> comparer, int capacity = 0) : base(comparer, capacity) => Comparer = comparer;

        ///<summary>Applies the enqueued changes to the collection.</summary>
        public void UpdateContents() 
        {
            while (_queue.Count > 0)
            {
                Type item = _queue.Dequeue();

                if (_removeCount > 0)
                {
                    _removeCount--;
                    Remove(item);
                    continue;
                }
                Add(item);
            }
        }

        ///<summary>Enqueues a value to be added to the collection.</summary>
        public void QueueAdd(Type value) => _queue.Enqueue(value, 1);

        ///<summary>Enqueues an enumerable collection of values to be added to the collection.</summary>
        public void QueueAddAll(IEnumerable<Type> values) => values.ForEach(QueueAdd);

        ///<summary>Enqueues a value to be removed from the collection.</summary>
        public void QueueRemove(Type value)
        {
            _queue.Enqueue(value, 0);
            _removeCount++;
        }

        ///<summary>Enqueues an enumerable collection of values to be removed from the collection.</summary>
        public void QueueRemoveAll(IEnumerable<Type> values) => values.ForEach(QueueRemove);
    }
}
