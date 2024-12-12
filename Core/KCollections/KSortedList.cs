#nullable disable

using KheaiGameEngine.Core.KCollections.Extensions;
using System.Collections;

namespace KheaiGameEngine.Core.KCollections
{
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
}

#nullable enable