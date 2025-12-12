using System;
using System.Collections.Generic;

namespace Dsu.Framework
{
    public class Heap<T> where T : IComparable<T>
    {
        private readonly List<T> elements = new List<T>();
        private readonly IComparer<T> comparer;

        public Heap(IComparer<T> customComparer = null)
        {
            this.comparer = customComparer ?? Comparer<T>.Default;
        }

        public int Count { get { return elements.Count; } }

        public void Enqueue(T item)
        {
            elements.Add(item);
            HeapifyUp(elements.Count - 1);
        }

        public T Dequeue()
        {
            if (elements.Count == 0)
                throw new InvalidOperationException("Heap is empty");

            T result = elements[0];
            elements[0] = elements[elements.Count - 1];
            elements.RemoveAt(elements.Count - 1);
            HeapifyDown(0);
            return result;
        }

        public T Peek()
        {
            if (elements.Count == 0)
                throw new InvalidOperationException("Heap is empty");
            return elements[0];
        }

        public bool Contains(T item)
        {
            return elements.Contains(item);
        }

        public void Clear()
        {
            elements.Clear();
        }

        public T[] ToArray()
        {
            return elements.ToArray();
        }

        private void HeapifyUp(int index)
        {
            while (index > 0) {
                int parent = (index - 1) / 2;
                if (comparer.Compare(elements[index], elements[parent]) >= 0)
                    break;

                T temp = elements[index];
                elements[index] = elements[parent];
                elements[parent] = temp;

                index = parent;
            }
        }

        private void HeapifyDown(int index)
        {
            int lastIndex = elements.Count - 1;
            while (true) {
                int smallest = index;
                int left = 2 * index + 1;
                int right = 2 * index + 2;

                if (left <= lastIndex && comparer.Compare(elements[left], elements[smallest]) < 0)
                    smallest = left;
                if (right <= lastIndex && comparer.Compare(elements[right], elements[smallest]) < 0)
                    smallest = right;
                if (smallest == index)
                    break;

                T temp = elements[index];
                elements[index] = elements[smallest];
                elements[smallest] = temp;

                index = smallest;
            }
        }
    }
}

/*  // MinHeap example
    var minHeap = new Dsu.Framework.Heap<int>(); // Default comparer -> MinHeap
    minHeap.Enqueue(3);
    minHeap.Enqueue(1);
    minHeap.Enqueue(5);

    Console.WriteLine(minHeap.Dequeue()); // output: 1

    // MaxHeap example
    var maxHeap = new Dsu.Framework.Heap<int>(Comparer<int>.Create((a, b) => b.CompareTo(a)));
    maxHeap.Enqueue(3);
    maxHeap.Enqueue(1);
    maxHeap.Enqueue(5);

    Console.WriteLine(maxHeap.Dequeue()); // output: 5

*/
