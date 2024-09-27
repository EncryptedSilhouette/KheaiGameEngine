#nullable disable

using KheaiGameEngine.Extensions;
using System.Collections;

namespace KheaiGameEngine.Core
{
    public class KObjectComparer<T> : IComparer<T> where T : IKObject
    {
        public int Compare(T a, T b) => a.Order > b.Order ? 1 : a.Order < b.Order ? -1 : 0;
    }

    public class KSortedList<Type> : ICollection<Type>
    {
        private List<Type> _contents;

        protected IComparer<Type> comparer;

        public bool IsReadOnly => false;

        ///<summary>The number of elements in this collection.</summary>
        public int Count => _contents.Count;

        ///<summary>The comparer used for sorting.</summary>
        public IComparer<Type> Comparer 
        {
            get => comparer;
            set
            {
                comparer = value;
                _contents.Sort(comparer);
            }
        }

        ///<summary>Gets the element from the specified index.</summary>
        public Type this[int index] => _contents[index];

        ///<summary>Create a new instance of KSortedList.</summary>
        public KSortedList(int capacity = 0) => _contents = new(capacity);

        ///<summary>Adds a value to the collection. Does a binary search before insertion to maintain a sorted collection.</summary>
        public void Add(Type value) => _contents.BinaryInsert(value, comparer.Compare);

        ///<summary>Adds an enumerable collection of values to the collection. Does a binary search before insertion to maintain a sorted collection.</summary>
        public void AddAll(IEnumerable<Type> values) => _contents.AddRange(values);

        ///<summary>Removes the specified value from the collection.</summary>
        public bool Remove(Type value) => _contents.Remove(value);

        ///<summary>Removes an enumerable collection of values from the collection.</summary>
        public void RemoveAll(IEnumerable<Type> values) => values.ForEach((value) => _contents.Remove(value));

        ///<summary>Removes all elements from the collection.</summary>
        public void Clear() => _contents.Clear();

        ///<summary>Retrieves an element that matches the conditions defined by the specified predicate.</summary>
        public Type Find(Predicate<Type> match) => _contents.Find(match);

        ///<summary>Retrieves all elements that match the conditions defined by the specified predicate.</summary>
        public IEnumerable<Type> FindALL(Predicate<Type> match) => _contents.FindAll(match);

        ///<summary>Determines whether an element exists in the collection.</summary>
        public bool Contains(Type item) => _contents.Contains(item);

        ///<summary>Copies the elements from this collection to an array.</summary>
        public void CopyTo(Type[] array, int arrayIndex) => _contents.CopyTo(array, arrayIndex);

        public IEnumerator<Type> GetEnumerator() => _contents.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class KSortedQueuedList<Type> : KSortedList<Type> where Type : IKObject
    {
        private int removeCount;
        private PriorityQueue<Type, byte> Queue = new();

        ///<summary>Applies the enqueued changes to the collection.</summary>
        public void UpdateContents() 
        {
            while (Queue.Count > 0)
            {
                Type item = Queue.Dequeue();

                if (removeCount > 0)
                {
                    removeCount--;
                    Remove(item);
                    continue;
                }
                Add(item);
            }
        }

        ///<summary>Enqueues a value to be added to the collection.</summary>
        public void QueueAdd(Type value) => Queue.Enqueue(value, 1);

        ///<summary>Enqueues an enumerable collection of values to be added to the collection.</summary>
        public void QueueAdd(IEnumerable<Type> values) => values.ForEach(QueueAdd);

        ///<summary>Enqueues a value to be removed from the collection.</summary>
        public void QueueRemove(Type value)
        {
            Queue.Enqueue(value, 0);
            removeCount++;
        }

        ///<summary>Enqueues an enumerable collection of values to be removed from the collection.</summary>
        public void QueueRemove(IEnumerable<Type> values) => values.ForEach(QueueRemove);
    }
}
