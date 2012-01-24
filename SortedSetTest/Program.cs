using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AStar;

namespace SortedSetTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();
            SortedSet<Node> nodes = new SortedSet<Node>(new NodeComparer());
            for (int i = 0; i < 100; i++)
            {
                Node node = new Node(i, i + 100);
                node.F = random.Next() % 100;
                nodes.Add(node);
                Console.Write(node.F + ", ");
            }
            Console.WriteLine();
            while (nodes.Count > 0)
            {
                Console.Write(nodes.Max.F + ", ");
                nodes.Remove(nodes.Max);
            }
            Console.WriteLine();

            Console.ReadLine();
        }
    }
}
