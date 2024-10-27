#nullable disable

using KheaiGameEngine.Extensions;
using System.Collections;
using System.Collections.Specialized;

namespace KheaiGameEngine.Core
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

    public class KSortedList<Type> : ICollection<Type>, IKSearchableCollection<Type>
    {
        private List<Type> _contents;
        private IComparer<Type> _comparer;

        public bool IsReadOnly => false;

        ///<summary>The number of elements in this collection.</summary>
        public int Count => _contents.Count;

        ///<summary>The comparer used for sorting.</summary>
        public IComparer<Type> Comparer 
        {
            get => _comparer;
            set
            {
                _comparer = value;
                _contents.Sort(_comparer);
            }
        }

        public Type this[int index] => _contents[index];

        ///<summary>Create a new instance of KSortedList.</summary>                //Sorry i got lazy. Normally i'd never use "this".    
        public KSortedList(IComparer<Type> comparer, int capacity = 0) => (_contents, _comparer) = (new(capacity), comparer);

        ///<summary>Adds a value to the collection. Does a binary search before insertion to maintain a sorted collection.</summary>
        public void Add(Type value) => _contents.BinaryInsert(value, _comparer.Compare);

        ///<summary>Adds an IEnumerable collection of values to the collection. Does a binary search before insertion to maintain a sorted collection.</summary>
        public void AddAll(IEnumerable<Type> values) => _contents.AddRange(values);

        ///<summary>Removes the specified value from the collection.</summary>
        public bool Remove(Type value) => _contents.Remove(value);

        ///<summary>Removes an IEnumerable collection of values from the collection.</summary>
        public void RemoveAll(IEnumerable<Type> values) => values.ForEach((value) => _contents.Remove(value));

        ///<summary>Removes all elements from the collection.</summary>
        public void Clear() => _contents.Clear();

        ///<summary>Copies the elements from this collection to an array.</summary>
        public void CopyTo(Type[] array, int arrayIndex) => _contents.CopyTo(array, arrayIndex);

        public Type Find(Predicate<Type> match) => _contents.Find(match);

        public IEnumerable<Type> FindALL(Predicate<Type> match) => _contents.FindAll(match);

        public bool Contains(Type item) => _contents.Contains(item);

        public IEnumerator<Type> GetEnumerator() => _contents.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _contents.GetEnumerator();
    }

    ///<summary>Applies the enqueued changes to the collection.</summary>
    public class KSortedQueuedList<Type> : KSortedList<Type>, IKQueuedOnlyCollection<Type> where Type : IKEngineObject
    {
        private int _removeCount;
        private PriorityQueue<Type, byte> _queue = new();

        public event Action<Type> OnRemoved, OnInsertion;

        public KSortedQueuedList(IComparer<Type> comparer, int capacity = 0) : base(comparer, capacity) => Comparer = comparer;

        public IEnumerable<Type> UpdateContents() 
        {
            while (_queue.Count > 0)
            {
                Type item = _queue.Dequeue();

                if (_removeCount > 0)
                {
                    _removeCount--;
                    OnRemoved?.Invoke(item);
                    Remove(item);
                    continue;
                }
                Add(item);
                OnInsertion?.Invoke(item);
            }

            //Return contents.
            return this;
        }

        public void QueueAdd(Type value) => _queue.Enqueue(value, 1);

        public void QueueAddAll(IEnumerable<Type> values) => values.ForEach(QueueAdd);

        public void QueueRemove(Type value)
        {
            _queue.Enqueue(value, 0);
            _removeCount++;
        }

        public void QueueRemoveAll(IEnumerable<Type> values) => values.ForEach(QueueRemove);
    }
}

#nullable enable