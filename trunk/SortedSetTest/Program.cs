using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using AStar;
using Wintellect.PowerCollections;

namespace SortedSetTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();
            ArrayList list = new ArrayList();
            for (int i = 0; i < 10000; i++)
            {
                Node node = new Node(0, 0);
                node.F = random.Next();
                list.Add(node);
            }

            DateTime time1 = DateTime.Now;

            /*OrderedBag<Node> nodes = new OrderedBag<Node>(new NodeComparer());
            for (int i = 0; i < list.Count; i++)
            {
                nodes.Add((Node)list[i]);
            }*/

            DateTime time2 = DateTime.Now;

            PriorityQueueB<Node> nodes2 = new PriorityQueueB<Node>(new NodeComparer(), list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                nodes2.Push((Node)list[i]);
            }

            DateTime time3 = DateTime.Now;

            BinaryHeap<Node> nodes3 = new BinaryHeap<Node>(new NodeComparer(), list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                nodes3.Insert((Node)list[i]);
            }

            DateTime time4 = DateTime.Now;

            //Console.WriteLine("OrderedBag:     " + (time2 - time1).Milliseconds +  " " + nodes.Min());
            Console.WriteLine("PriorityQueueB: " + (time3 - time2).Milliseconds + " " + nodes2.Peek());
            Console.WriteLine("BinaryHeap:     " + (time4 - time3).Milliseconds + " " + nodes3.Top());

            Console.WriteLine("Comparing...");
            nodes2 = new PriorityQueueB<Node>(new NodeComparer(), list.Count);
            nodes3 = new BinaryHeap<Node>(new NodeComparer(), list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                nodes2.Push((Node)list[i]);
                nodes3.Insert((Node)list[i]);

                if (nodes2.Peek() != nodes3.Top())
                {
                    Console.WriteLine("not the same " + i);
                    break;
                }
            }

            Console.WriteLine("Removing...");
            while (nodes2.Count > 0)
            {
                if (nodes2.Peek() != nodes3.Top())
                {
                    Console.WriteLine("not the same " + (list.Count - nodes2.Count));
                    break;
                }
                nodes2.Pop();
                nodes3.Remove();
            }

            nodes2 = new PriorityQueueB<Node>(new NodeComparer(), list.Count);
            nodes3 = new BinaryHeap<Node>(new NodeComparer(), list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                nodes2.Push((Node)list[i]);
                nodes3.Insert((Node)list[i]);
            }

            int randomIndex = random.Next() % list.Count;
            Node itemToFind = nodes3[randomIndex];
            if (randomIndex == nodes3.Find(itemToFind))
                Console.WriteLine("encontrado!");
            else
                Console.WriteLine("no encontrado!");

            itemToFind = new Node(0, 0);
            itemToFind.F = random.Next();
            if (-1 != nodes3.Find(itemToFind))
                Console.WriteLine("encontrado!");
            else
                Console.WriteLine("no encontrado!");

            DateTime time5 = DateTime.Now;

            while (nodes2.Count > 0)
                nodes2.Pop();

            DateTime time6 = DateTime.Now;

            while (nodes3.Count > 0)
                nodes3.Remove();

            DateTime time7 = DateTime.Now;

            Console.WriteLine("PriorityQueueB: " + (time6 - time5).Milliseconds);
            Console.WriteLine("BinaryHeap:     " + (time7 - time6).Milliseconds);

            Console.WriteLine("Press [Enter] to exit");
            Console.ReadLine();
        }
    }
}
