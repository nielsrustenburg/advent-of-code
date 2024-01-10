namespace Shared.Datastructures
{
    public class SortedNaivePriorityQueue<T> : IPriorityQueue<T> where T : IComparable<T>
    {
        List<T> queue;

        public SortedNaivePriorityQueue()
        {
            queue = new List<T>();
        }

        public T Dequeue()
        {
            var element = queue.First();
            queue.RemoveAt(0);
            return element;
        }

        public void Enqueue(T element)
        {
            var lastLower = queue.FindLastIndex(other => other.CompareTo(element) == -1);
            queue.Insert(lastLower + 1, element);
        }

        public bool IsEmpty()
        {
            return !queue.Any();
        }
    }
}
