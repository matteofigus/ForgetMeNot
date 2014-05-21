using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PriorityQueue
{
    /// <summary>
    /// Priority queue implemented with a heap.
    /// The heap is always maintained in order; the maximum element is always on top.
    /// Therefore, the Max() operation is completed in constant time.
    /// Inserts are done in log N time; the item is added to the top of the queue.
    /// Heap order is then maintained by sinking the latest element down the heap.
    /// Construction of the heap is proportional to the number of elements to add.
    /// Array will double / halve in size as needed; this is fine as we amortize this
    /// for inserts / deletions.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PriorityQueue<T>
    {
        private readonly IComparer<T> _comparer;
        private readonly Func<T, T, bool> _comparerFunc; 
        private int _count = 0;
        private T[] _heap;

        public PriorityQueue(int size, IComparer<T> comparer)
        {
            _comparer = comparer;
            _heap = new T[size];
        }

        public PriorityQueue(IComparer<T> comparer) :
            this(1, comparer)
        {}

        public PriorityQueue(int size, Func<T, T, bool> comparer)
        {
            _comparerFunc = comparer;
            _heap = new T[size];
        }

        public PriorityQueue(Func<T,T,bool> comparer) :
            this(1, comparer)
        {}

        /// <summary>
        /// Construct a priority queue from a collection of items
        /// </summary>
        /// <param name="items"></param>
        /// <param name="comparer"></param>
        public PriorityQueue(IEnumerable<T> items, IComparer<T> comparer)
        {
            
        }

        public bool IsEmpty
        {
            get { return _count == 0; }
        }

        public int Size
        {
            get { return _count; }
        }

        /// <summary>
        /// Returns the largest item in the queue, but does not delete that item
        /// </summary>
        /// <returns></returns>
        public T Max()
        {
            if(IsEmpty)
                throw new InvalidOperationException("The priority queue is empty.");

            return _heap[1];
        }

        public T RemoveMax()
        {
            if (IsEmpty)
                throw new InvalidOperationException("The priority queue is empty.");

            var max = _heap[1];
            //re-order the heap...

            return max;
        }

        public void Insert(T item)
        {
            
        }

        private void Sink()
        {
            
        }

        private void swim()
        {

        }
    }
}
