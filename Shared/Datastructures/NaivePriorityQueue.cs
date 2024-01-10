namespace Shared.Datastructures
{
    public class NaivePriorityQueue<T> : IPriorityQueue<T> where T : IComparable<T>
    {
        List<T> unsortedQueue;

        public NaivePriorityQueue()
        {
            unsortedQueue = new List<T>();
        }

        public T Dequeue()
        {
            if (!IsEmpty())
            {
                var (minElement, index) = unsortedQueue.Select((ele, i) => (ele, i)).Aggregate((a, b) => a.ele.CompareTo(b.ele) == 1 ? b : a);
                unsortedQueue.RemoveAt(index);
                return minElement;
            }
            return default;
        }

        public void Enqueue(T element)
        {
            unsortedQueue.Add(element);
        }

        public bool IsEmpty()
        {
            return !unsortedQueue.Any();
        }
    }
}
