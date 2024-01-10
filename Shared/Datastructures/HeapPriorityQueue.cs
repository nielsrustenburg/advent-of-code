namespace Shared.Datastructures
{
    public class HeapPriorityQueue<T> : IPriorityQueue<T> where T : IComparable<T>
    {
        MinHeap<T> heap;

        public HeapPriorityQueue()
        {
            heap = new MinHeap<T>();
        }

        public T Dequeue()
        {
            return heap.Delete();
        }

        public void Enqueue(T element)
        {
            heap.Insert(element);
        }

        public bool IsEmpty()
        {
            return !heap.Any();
        }
    }
}
