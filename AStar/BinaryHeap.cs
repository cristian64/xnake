using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AStar
{
    public class BinaryHeap<T>
    {
        //ArrayList innerList;
        List<T> innerList;
        IComparer<T> comparer;

        public BinaryHeap()
        {
            this.comparer = Comparer<T>.Default;
            innerList = new List<T>();
        }

        public BinaryHeap(IComparer<T> comparer)
        {
            this.comparer = comparer;
            innerList = new List<T>();
        }

        public BinaryHeap(IComparer<T> comparer, int capacity)
        {
            this.comparer = comparer;
            innerList = new List<T>(capacity);
        }

        public T Top()
        {
            if (innerList.Count > 0)
                return innerList[0];
            else
                return default(T);
        }

        public void Insert(T item)
        {
            int itemIndex = innerList.Count;
            innerList.Add(item);
            int parentIndex = (itemIndex - 1) / 2;
            while (itemIndex != 0 && comparer.Compare(item, innerList[parentIndex]) <= 0)
            {
                innerList[itemIndex] = innerList[parentIndex];
                innerList[parentIndex] = item;
                itemIndex = parentIndex;
                parentIndex = (itemIndex - 1) / 2;
            }
        }

        public T Remove()
        {
            return Remove(0);
        }

        public int Find(T item)
        {
            return Find(item, 0);
        }

        private int Find(T item, int index)
        {
            if (index < innerList.Count)
            {
                if (innerList[index].Equals(item))
                {
                    return index;
                }
                else if (comparer.Compare(item, innerList[index]) > 0)
                {
                    return Math.Max(Find(item, index * 2 + 1), Find(item, index * 2 + 2));
                }
            }
            return -1;
        }

        public bool Contains(T item)
        {
            return Find(item, 0) != -1;
        }

        public T Remove(int index)
        {
            if (innerList.Count > index)
            {
                T result = innerList[index];
                T item = innerList[innerList.Count - 1];
                innerList[index] = item;
                innerList.RemoveAt(innerList.Count - 1);

                int oldIndex;
                int leftChildIndex;
                int rightChildIndex;

                do
                {
                    oldIndex = index;
                    leftChildIndex = index * 2 + 1;
                    rightChildIndex = index * 2 + 2;

                    if (leftChildIndex < innerList.Count && comparer.Compare(innerList[index], innerList[leftChildIndex]) > 0)
                        index = leftChildIndex;
                    if (rightChildIndex < innerList.Count && comparer.Compare(innerList[index], innerList[rightChildIndex]) > 0)
                        index = rightChildIndex;

                    if (oldIndex == index)
                        break;
                    innerList[oldIndex] = innerList[index];
                    innerList[index] = item;
                } while (true);

                return result;
            }
            else
            {
                return default(T);
            }
        }

        public int Count
        {
            get { return innerList.Count; }
        }

        public void Clear()
        {
            innerList.Clear();
        }

        public T this[int index]
        {
            get { return innerList[index]; }
            set
            {
                innerList[index] = value;
                Update(index);
            }
        }

        public void Update(int index)
        {
            int itemIndex = index;
            T item = innerList[index];
            int parentIndex = (itemIndex - 1) / 2;
            while (itemIndex != 0 && comparer.Compare(item, innerList[parentIndex]) < 0)
            {
                innerList[itemIndex] = innerList[parentIndex];
                innerList[parentIndex] = item;
                itemIndex = parentIndex;
                parentIndex = (itemIndex - 1) / 2;
            }

            if (itemIndex != index)
                return;

            int oldIndex;
            int leftChildIndex;
            int rightChildIndex;

            do
            {
                oldIndex = index;
                leftChildIndex = index * 2 + 1;
                rightChildIndex = index * 2 + 2;

                if (leftChildIndex < innerList.Count && comparer.Compare(innerList[index], innerList[leftChildIndex]) > 0)
                    index = leftChildIndex;
                if (rightChildIndex < innerList.Count && comparer.Compare(innerList[index], innerList[rightChildIndex]) > 0)
                    index = rightChildIndex;

                if (oldIndex == index)
                    break;
                innerList[oldIndex] = innerList[index];
                innerList[index] = item;
            } while (true);
        }
    }
}
