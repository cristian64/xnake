using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace AStar
{
    /**
     * Implementación del algoritmo A* con Java.
     * Dada una matriz de nodos (Node) e indicando cuál es el nodo de salida
     * y el nodo de llegada, calcula el mejor camino posible que hay que seguir
     * evitando los obstáculos (es decir, nodos no transitables).
     */
    public class AStar
    {
        /**
         * Matriz con una serie de filas y una serie de columnas que representa el tablero sobre
         * el que se buscará el camino. Cada celda de la matriz es un nodo que puede
         * ser transitable o no y que tiene un coste particular (por ejemplo, sería
         * más difícil caminar por el barro que por tierra seca).
         */
        private Node[][] nodes;

        /**
         * Nodo de partida que indica una posición (x,y) en la matriz de celdas.
         * No importa si es un nodo intransitable y tampoco se considera el coste.
         */
        private Node startNode;

        /**
         * Nodo de llegada que indica una posición (x,y) en la matriz de celdas.
         * Debe ser un nodo transitable.
         */
        private Node endNode;

        /**
         * Constructor por defecto.
         * @param matriz Matriz con una serie de filas y una serie de columnas que representa el tablero sobre
         * el que se buscará el camino.
         * @param nodoInicial Nodo de partida que indica una posición (x,y) en la matriz de celdas.
         * @param nodoFinal Nodo de llegada que indica una posición (x,y) en la matriz de celdas.
         */
        public AStar(Node[][] nodes, Node startNode, Node endNode)
        {
            this.nodes = nodes;
            this.startNode = startNode;
            this.endNode = endNode;
        }

        /**
         * Ejecuta el algoritmo de A*, calculando el camino, si existe, desde el
         * punto de inicio hasta el punto final.
         * @return Devuelve una lista (no vacía) de nodos si existe el camino. Si no existe, devuelve null.
         */
        public ArrayList FindPath()
        {
            SortedSet<Node> openList = new SortedSet<Node>(new NodeComparer());
            ArrayList closedList = new ArrayList();
            Node currentNode = null;
            bool pathFound = false;

            int columns = nodes.Length;
            int rows = 0;
            if (columns > 0)
                rows = nodes[0].Length;

            for (int i = 0; i < columns; i++)
                for (int j = 0; j < rows; j++)
                {
                    nodes[i][j].EndNode = endNode;
                    nodes[i][j].Column = i;
                    nodes[i][j].Row = j;
                }

            // Añadimos el cuadro inicial a la lista abierta.
            openList.Add(startNode);

            int iterations = 0;
            // Buscamos el camino mientras queden nodos candidatos y no lo hayamos encontrado.
            while (openList.Count != 0 && !pathFound)
            {
                iterations++;
                // Extraemos el nodo de menor F desde la lista abierta hacia la lista cerrada.
                currentNode = openList.Min;
                openList.Remove(currentNode);
                closedList.Add(currentNode);

                // Extraemos los nodos adyacentes al nodo actual.
                ArrayList nearNodes = new ArrayList();
                if (0 <= currentNode.Column + 1 && currentNode.Column + 1 < columns && 0 <= currentNode.Row && currentNode.Row < rows)
                {
                    if (nodes[currentNode.Column + 1][currentNode.Row].Transitable)
                    {
                        nearNodes.Add(nodes[currentNode.Column + 1][currentNode.Row]);
                    }
                }
                if (0 <= currentNode.Column - 1 && currentNode.Column - 1 < columns && 0 <= currentNode.Row && currentNode.Row < rows)
                {
                    if (nodes[currentNode.Column - 1][currentNode.Row].Transitable)
                    {
                        nearNodes.Add(nodes[currentNode.Column - 1][currentNode.Row]);
                    }
                }
                if (0 <= currentNode.Column && currentNode.Column < columns && 0 <= currentNode.Row - 1 && currentNode.Row - 1 < rows)
                {
                    if (nodes[currentNode.Column][currentNode.Row - 1].Transitable)
                    {
                        nearNodes.Add(nodes[currentNode.Column][currentNode.Row - 1]);
                    }
                }
                if (0 <= currentNode.Column && currentNode.Column < columns && 0 <= currentNode.Row + 1 && currentNode.Row + 1 < rows)
                {
                    if (nodes[currentNode.Column][currentNode.Row + 1].Transitable)
                    {
                        nearNodes.Add(nodes[currentNode.Column][currentNode.Row + 1]);
                    }
                }

                // Para cada nodo encontrado, comprobamos si hemos llegado al punto de destino.
                while (nearNodes.Count != 0 && !pathFound)
                {
                    Node nearNode = (Node)nearNodes[0];
                    nearNodes.RemoveAt(0);
                    if (!closedList.Contains(nearNode))
                    {
                        if (!openList.Contains(nearNode))
                        {
                            nearNode.ParentNode = currentNode;
                            openList.Add(nearNode);

                            if (endNode == nearNode)
                            {
                                pathFound = true;
                            }
                        }
                        else
                        {
                            if (currentNode.G + 10 < nearNode.G)
                            {
                                openList.Remove(nearNode);
                                nearNode.ParentNode = currentNode;
                                openList.Add(nearNode);
                            }
                        }
                    }
                }
            }

            Console.WriteLine("Iterations:" + iterations);
            foreach (Node i in closedList)
                Console.Write(i + ",");
            Console.WriteLine();

            // Si hemos llegado al nodo final, volvemos hacia atrás desde ese nodo extrayendo el camino hasta el nodo inicial.
            if (pathFound)
            {
                ArrayList path = new ArrayList();
                Node auxNode = endNode;
                while (auxNode != null)
                {
                    path.Insert(0, auxNode);
                    auxNode = auxNode.ParentNode;
                }
                return path;
            }
            else
            {
                Console.WriteLine("Path not found!");
                return null;
            }
        }
    }

}
