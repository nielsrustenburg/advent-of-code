namespace Shared.Datastructures
{
    public interface IPriorityQueue<T> where T : IComparable<T>
    {
        bool IsEmpty();

        void Enqueue(T element);

        T Dequeue();
    }
}
