using KheaiGameEngine.Core.KCollections.Extensions;

namespace KheaiGameEngine.Core.KCollections
{
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
