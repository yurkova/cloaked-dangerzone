using System;
using System.Collections.Generic;
using System.Linq;

namespace InformationRetrieval
{
    public class BinaryHeap<T>
        where T : IComparable<T>
    {
        // for tests
        public const string InitializationFromNullArrayError = "Initial array can not be null.";

        private List<T> list;

        public BinaryHeap()
        {
            list = new List<T>();
        }

        public int HeapSize
        {
            get { return list.Count(); }
        }

        public void Add(T value)
        {
            list.Add(value);
            int i = HeapSize - 1;
            int parent = (i - 1)/2;

            while (i > 0 && (list[parent].CompareTo(list[i]) < 0))
            {
                T temp = list[i];
                list[i] = list[parent];
                list[parent] = temp;

                i = parent;
                parent = (i - 1)/2;
            }
        }

        public void Regularize(int i)
        {
            for (;;)
            {
                int leftChild = 2*i + 1;
                int rightChild = 2*i + 2;
                int largestChild = i;

                if (leftChild < HeapSize && (list[leftChild].CompareTo(list[largestChild]) > 0))
                {
                    largestChild = leftChild;
                }
                if (rightChild < HeapSize && (list[rightChild].CompareTo(list[largestChild]) > 0))
                {
                    largestChild = rightChild;
                }
                if (i == largestChild)
                {
                    break;
                }

                T temp = list[i];
                list[i] = list[largestChild];
                list[largestChild] = temp;

                i = largestChild;
            }
        }

        public void BuildHeap(IEnumerable<T> array)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array", "Initial array can not be null.");
            }
            list = array.ToList();
            for (int i = HeapSize/2; i >= 0; i--)
            {
                Regularize(i);
            }
        }

        public T GetMax()
        {
            T result = list[0];
            list[0] = list[HeapSize - 1];
            list.RemoveAt(HeapSize - 1);
            Regularize(0);
            return result;
        }

        public T Max()
        {
            return list[0];
        }
    }
}