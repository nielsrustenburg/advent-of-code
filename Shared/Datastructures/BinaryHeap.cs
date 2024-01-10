using System.Collections;

namespace Shared.Datastructures
{
    abstract class BinaryHeap<T> : IEnumerable<T>
        where T : IComparable<T>
    {
        protected List<T> heap;

        public BinaryHeap()
        {
            heap = new List<T>();
        }

        public void Insert(T item)
        {
            heap.Add(item);
            SiftUp(heap.Count - 1);
        }

        public T Delete()
        {
            var oldRoot = heap[0];
            if (heap.Count > 1)
            {
                var tempRoot = heap[heap.Count - 1];
                heap.RemoveAt(heap.Count - 1);
                heap[0] = tempRoot;
                SiftDown(0);
            }
            else
            {
                heap.RemoveAt(0);
            }
            return oldRoot;
        }

        public T Peek()
        {
            return heap[0];
        }

        private void SiftUp(int childIndex)
        {
            if (childIndex > 0)
            {
                int parentIndex = (childIndex - 1) / 2;
                var child = heap[childIndex];
                var parent = heap[parentIndex];
                if (!CompareKeys(parent, child))
                {
                    heap[childIndex] = parent;
                    heap[parentIndex] = child;
                    SiftUp(parentIndex);
                }
            }
        }

        private void SiftDown(int index)
        {
            int leftIndex = 2 * index + 1;
            if (leftIndex <= heap.Count - 1)
            {
                int rightIndex = leftIndex + 1;
                bool minChildIsLeft = true;
                if (rightIndex <= heap.Count - 1)
                {
                    minChildIsLeft = CompareKeys(heap[leftIndex], heap[rightIndex]);
                }

                int minChildIndex = minChildIsLeft ? leftIndex : rightIndex;
                var minChild = heap[minChildIndex];
                var parent = heap[index];
                if (!CompareKeys(parent, minChild))
                {
                    heap[index] = minChild;
                    heap[minChildIndex] = parent;
                    SiftDown(minChildIndex);
                    ;
                }
            }
        }

        protected abstract bool CompareKeys(T higher, T lower); //Should return true if 'higher' should sooner become root than 'lower', or if keys are equal

        public IEnumerator<T> GetEnumerator()
        {
            return heap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    class MinHeap<T> : BinaryHeap<T> where T : IComparable<T>
    {
        public MinHeap() : base() { }

        public MinHeap(MinHeap<T> copyMe)
        {
            heap = new List<T>(copyMe.heap);
        }

        protected override bool CompareKeys(T higher, T lower)
        {
            return higher.CompareTo(lower) == -1;
        }
    }

    class MaxHeap<T> : BinaryHeap<T> where T : IComparable<T>
    {
        public MaxHeap() : base() { }

        public MaxHeap(MaxHeap<T> copyMe)
        {
            heap = new List<T>(copyMe.heap);
        }

        protected override bool CompareKeys(T higher, T lower)
        {
            return higher.CompareTo(lower) == 1;
        }
    }
}
