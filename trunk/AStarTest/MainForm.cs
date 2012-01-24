using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AStar;

namespace AStarTest
{
    public partial class MainForm : Form
    {
        AStar.Node[][] nodes;
        AStar.Node startNode;
        AStar.Node endNode;

        public MainForm()
        {
            InitializeComponent();
            nodes = new AStar.Node[20][];
            for (int i = 0; i < 20; i++)
            {
                nodes[i] = new AStar.Node[11];
                for (int j = 0; j < 11; j++)
                    nodes[i][j] = new AStar.Node(i, j);
            }
        }

        private void buttonFindPath_Click(object sender, EventArgs e)
        {
            buttonClear_Click(null, null);

            AStar.AStar astar = new AStar.AStar(nodes, startNode, endNode);
            ArrayList results = astar.FindPath();

            Console.WriteLine("Drawing :)");
            if (results != null)
            foreach (Node i in results)
            {
                if (i != startNode && i != endNode)
                {
                    int column = i.Column;
                    int row = i.Row;
                    foreach (Button j in Controls)
                    {
                        if (j.TabIndex == row * 20 + column)
                        {
                            j.BackColor = Color.Gray;
                            break;
                        }
                    }
                }
            }

            for (int j = 0; j < 11; j++)
            {
                for (int i = 0; i < 20; i++)
                {
                    if (!nodes[i][j].Transitable)
                        Console.Write("#");
                    else if (nodes[i][j] == startNode)
                        Console.Write("1");
                    else if (nodes[i][j] == endNode)
                        Console.Write("2");
                    else
                        Console.Write("-");
                }
                Console.WriteLine();
            }
        }

        private void button_MouseDown(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            int column = button.TabIndex % 20;
            int row = button.TabIndex / 20;
            Node node = nodes[column][row];

            if (e.Button == MouseButtons.Middle && node != startNode && node != endNode)
            {
                node.Transitable = !node.Transitable;
                if (!node.Transitable)
                    button.BackColor = Color.Black;
                else
                    button.BackColor = DefaultBackColor;
            }
            else if (e.Button == MouseButtons.Left && node != endNode)
            {
                if (startNode != null)
                {
                    int tabIndex = startNode.Row * 20 + startNode.Column;
                    foreach (Button j in Controls)
                    {
                        if (j.TabIndex == tabIndex)
                            j.BackColor = DefaultBackColor;
                    }
                }
                startNode = node;
                button.BackColor = Color.Blue;
                node.Transitable = true;
            }
            else if (e.Button == MouseButtons.Right && node != startNode)
            {
                if (endNode != null)
                {
                    int tabIndex = endNode.Row * 20 + endNode.Column;
                    foreach (Button j in Controls)
                    {
                        if (j.TabIndex == tabIndex)
                            j.BackColor = DefaultBackColor;
                    }
                }
                endNode = node;
                button.BackColor = Color.Green;
                node.Transitable = true;
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 20; i++)
                for (int j = 0; j < 11; j++)
                {
                    nodes[i][j].Reset();
                }
            foreach (Button j in Controls)
            {
                if (j.BackColor == Color.Gray)
                    j.BackColor = DefaultBackColor;
            }
        }
    }
}
