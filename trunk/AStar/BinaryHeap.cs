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

        public void Remove()
        {
            if (innerList.Count > 0)
            {
                T item = innerList[innerList.Count - 1];
                innerList[0] = item;
                innerList.RemoveAt(innerList.Count - 1);

                int itemIndex = 0, oldIndex;
                int leftChildIndex;
                int rightChildIndex;

                do
                {
                    oldIndex = itemIndex;
                    leftChildIndex = itemIndex * 2 + 1;
                    rightChildIndex = itemIndex * 2 + 2;

                    if (leftChildIndex < innerList.Count && comparer.Compare(innerList[itemIndex], innerList[leftChildIndex]) > 0)
                        itemIndex = leftChildIndex;
                    if (rightChildIndex < innerList.Count && comparer.Compare(innerList[itemIndex], innerList[rightChildIndex]) > 0)
                        itemIndex = rightChildIndex;

                    if (oldIndex == itemIndex)
                        break;
                    innerList[oldIndex] = innerList[itemIndex];
                    innerList[itemIndex] = item;
                } while (true);
            }
        }

        public int Find(T item, int index = 0)
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

        public void Remove(int index)
        {
            if (innerList.Count > index)
            {
                T item = innerList[innerList.Count - 1];
                innerList[index] = item;
                innerList.RemoveAt(innerList.Count - 1);

                int itemIndex = index, oldIndex;
                int leftChildIndex;
                int rightChildIndex;

                do
                {
                    oldIndex = itemIndex;
                    leftChildIndex = itemIndex * 2 + 1;
                    rightChildIndex = itemIndex * 2 + 2;

                    if (leftChildIndex < innerList.Count && comparer.Compare(innerList[itemIndex], innerList[leftChildIndex]) > 0)
                        itemIndex = leftChildIndex;
                    if (rightChildIndex < innerList.Count && comparer.Compare(innerList[itemIndex], innerList[rightChildIndex]) > 0)
                        itemIndex = rightChildIndex;

                    if (oldIndex == itemIndex)
                        break;
                    innerList[oldIndex] = innerList[itemIndex];
                    innerList[itemIndex] = item;
                } while (true);
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

        }
    }
}
