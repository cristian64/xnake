using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AStar
{

    public class NodeComparer : IComparer<Node>
    {
        public int Compare(Node node1, Node node2)
        {
            return node1.F - node2.F;
        }
    }

    /**
     * Esta clase representa un nodo del mapa de celdas del algoritmo del A*.
     * Indica cuál es su posición (x,y) en ese mapa, así como su valor F, G, H.
     */
    public class Node : IComparable<Node>
    {
	    /**
	     * Componente x (columna) de la posición del nodo en el mapa.
	     */
        public int Column;

	    /**
	     * Componente y (fila) de la posición del nodo en el mapa.
	     */
        public int Row;

	    /**
	     * Indica si el nodo puede ser visitado o accedido.
	     */
        public bool Transitable;

	    /**
	     * Coste extra del nodo. Por ejemplo, sería más costoso caminar sobre el
	     * barro que sobre tierra firme.
	     */
        private int cost;

	    /**
	     * Valor total del nodo.
	     * F = G + H + coste
	     */
        public int F;

	    /**
	     * Valor desde el nodo hasta el nodo inicial.
	     * Las diagonales suman 14 y las ortogonales 10.
	     */
        public int G;

	    /**
	     * Valor desde el nodo hasta el nodo final.
	     * Se considera el peor caso, en el no se pueden hacer diagonales, por lo
	     * tanto, H + 10*(diferencia de filas + diferencia de columnas entre el nodo
	     * y el nodo final).
	     */
        public int H;

	    /**
	     * Referencia al nodo padre. Necesario para calcular G.
	     */
        private Node parentNode;

	    /**
	     * Referencia al nodo final. Necesario para calcular H.
	     */
        private Node endNode;

	    /**
	     * Constructor por defecto.
	     * Inicializa a unos valores neutros del nodo.
	     */
        public Node(int column, int row)
        {
            this.Column = column;
            this.Row = row;

            Transitable = true;
            Cost = 0;

            F = 0;
            G = 0;
            H = 0;

            parentNode = null;
            endNode = null;
        }

	    /**
	     * Recalcula el valor de F. Normalmente, cuando G, H o coste han cambiado.
	     */
	    private void recalculateF()
	    {
		    F = G + H + cost;
	    }

	    /**
	     * Recalcula el valor de G. Normalmente, cuando el padre se ha modificado.
	     */
	    private void recalculateG()
	    {
            G = parentNode.G + 10;
		    recalculateF();
	    }

        public void Reset()
        {
            F = G = H = 0;
            parentNode = null;
            endNode = null;
        }

	    /**
	     * Recalcula el valor de H. Normalmente, cuando el nodo final ha cambiado.
	     */
	    private void recalculateH()
	    {
		    H = (Math.Abs(Column - endNode.Column) + Math.Abs(Row - endNode.Row)) * 10;
		    recalculateF();
	    }

	    /**
	     * Establece un nuevo nodo padre. Recalcula G (y F) forzadamente.
	     * @param nodoPadre Referencia al nodo padre que se va a asignar.
	     */
        public Node ParentNode
        {
            get { return parentNode; }
            set
            {
                parentNode = value;
                recalculateG();
            }
        }

	    /**
	     * Establece un nuevo nodo final. Recalcula H (y F) forzadamente.
	     * @param nodoFinal Referencia al nodo final que se va a asignar.
	     */
        public Node EndNode
        {
            get { return endNode; }
            set
            {
                endNode = value;
                recalculateH();
            }
        }

	    /**
	     * Establece un nuevo valor para el coste. Recalcula F forzadamente.
	     * @param coste Nuevo valor para el coste del nodo.
	     * @return Devuelve verdadero si ha podido modificar el coste. Sólo si es mayor o igual que 0.
	     */
        public int Cost
        {
            get { return cost; }
            set
            {
                cost = value;
                recalculateF();
            }
        }

        public int CompareTo(Node other)
        {
            return F - other.F;
        }

	    /**
	     * Genera una cadena de caracteres con la posición del nodo (fila, columna).
	     * @return Devuelve una cadena con la posición (fila, columna).
	     */
	    override public String ToString()
	    {
		    return "(" + Row + ", " + Column + ")";
	    }
    }
}
