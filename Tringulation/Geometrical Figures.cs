using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections;

namespace Triangulation
{
    public static class MyExtension
    {
        public static int FindItem(this List<int> list, int item)
        {
            int res = -1;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == item)
                {
                    res = i;
                }
            }
            return res;
        }
    }

    public enum ElemType
    {
        Boundary,
        Inner
    }

    public class Node
    {
        double x;
        double y;
        ElemType ptype;

        public ElemType Ptype
        {
            get { return ptype; }
            set { ptype = value; }
        }


        public double X
        {
            get { return x; }
            set { x = value; }
        }
        public double Y
        {
            get { return y; }
            set { y = value; }
        }
        public Node()
        {
            x = 0;
            y = 0;
            ptype = ElemType.Boundary;
        }
        public Node(double x, double y)
        {
            this.x = x;
            this.y = y;
            ptype = ElemType.Boundary;
        }
        public Node(double x, double y, bool enable, ElemType pt)
        {
            this.x = x;
            this.y = y;
            ptype = pt;
        }

        public static double length(Node a, Node b)
        {
            return Math.Sqrt((b.x - a.x) * (b.x - a.x) + (b.y - a.y) * (b.y - a.y));
        }

        public static explicit operator Point(Node node)
        {
            return new Point((int)Math.Round(node.X), (int)Math.Round(node.Y));
        }
        public override string ToString()
        {
            return x + " ; " + y;
        }
        public static Node Parse(string nodeStr)
        {
            string[] mas = nodeStr.Split(';');
            return new Node(double.Parse(mas[0].Trim()), double.Parse(mas[1].Trim()));
        }

    }

    public class Segment
    {
        private Node a;
        private Node b;
        private int indexA;
        private int indexB;
        private ElemType segtype;
        private int edgeNumber;

        #region - Properties -
        public int EdgeNumber
        {
            get { return edgeNumber; }
            set { edgeNumber = value; }
        }

        public ElemType Segtype
        {
            get { return segtype; }
            set { segtype = value; }
        }

        public Node A
        {
            get { return a; }
            set { a = value; }
        }

        public Node B
        {
            get { return b; }
            set { b = value; }
        }

        public int IndexA
        {
            get { return indexA; }
            set { indexA = value; }
        }

        public int IndexB
        {
            get { return indexB; }
            set { indexB = value; }
        }
        #endregion

        public Segment()
        {
            a = new Node();
            b = new Node();
            segtype = ElemType.Boundary;
            indexA = 0;
            indexB = 0;
            edgeNumber = 0;

        }

        public Segment(Node a, Node b)
        {
            this.a = a;
            this.b = b;
            segtype = ElemType.Boundary;
            indexA = 0;
            indexB = 0;
            edgeNumber = 0;
        }

        public Segment(Node a, Node b, int edgeNumber)
        {
            this.a = a;
            this.b = b;
            segtype = ElemType.Boundary;
            indexA = 0;
            indexB = 0;
            this.edgeNumber = edgeNumber;
        }

        public Segment(Node a, Node b, ElemType et, int indexA, int indexB)
        {
            this.a = a;
            this.b = b;
            segtype = et;
            this.indexA = indexA;
            this.indexB = indexB;
            edgeNumber = 0;
        }

        public Segment(Node a, Node b, ElemType et, int indexA, int indexB, int edgeNumber)
        {
            this.a = a;
            this.b = b;
            segtype = et;
            this.indexA = indexA;
            this.indexB = indexB;
            this.edgeNumber = edgeNumber;
        }

        public double length()
        {
            return Node.length(a, b);
        }

        public double angle()
        {
            double res = Math.Acos((b.X - a.X) / this.length());
            if ((b.Y - a.Y) < 0)
            {
                res = -res;
            }
            return res;
        }
        public static double angle(Segment s1, Segment s2)
        {
            double alfa = s1.angle();
            double beta = s2.angle();
            double res = Math.PI + alfa - beta;
            if ((alfa >= 0) && (beta <= alfa - Math.PI))
                res =  - Math.PI + alfa - beta;
            if ((alfa <= 0) && (beta >= alfa + Math.PI))
                res = 3*Math.PI + alfa - beta;
            return res;
        }

        public List<Node> explode(int N)
        {
            List<Node> lp = new List<Node>();

            if (Math.Abs(a.X - this.b.X)  <= 0.00000000001)
            {
                double hy = (this.b.Y - a.Y) / N;
                double xcur = (a.X + this.b.X) / 2;
                for (int i = 0; i <= N; i++)
                {
                    lp.Add(new Node(xcur, a.Y + hy * i));
                }
            }
            else
            {
                double k = (a.Y - this.b.Y) / (a.X - this.b.X);
                double b = a.Y - k * a.X;
                double h = (this.b.X - a.X) / N;
                for (int i = 0; i <= N; i++)
                {
                    double xcur = (a.X + i * h);
                    lp.Add(new Node(xcur, k * xcur + b));
                }
            }
            return lp;
        }

        public bool Equal(Segment segment)
        {
            return ((indexA == segment.indexA) && (indexB == segment.indexB));
        }


    }

    public class Triangle
    {
        private int indexA;
        private int indexB;
        private int indexC;
        
        public int IndexA
        {
            get { return indexA; }
            set { indexA = value; }
        }

        public int IndexB
        {
            get { return indexB; }
            set { indexB = value; }
        }

        public int IndexC
        {
            get { return indexC; }
            set { indexC = value; }
        }

        public Triangle()
        {
            indexA = 0;
            indexB = 0;
            indexC = 0;
        }

        public Triangle(int indexA, int indexB, int indexC)
        {
            this.indexA = indexA;
            this.indexB = indexB;
            this.indexC = indexC;
        }

        public static double Square(Node a, Node b, Node c)
        {
            return Math.Abs((b.X * c.Y + a.X * b.Y + a.Y * c.X - a.Y * b.X - a.X * c.Y - c.X * b.Y) / 2);
        }

    }

    public class Polygon
    {
        private List<Node> vertexs;
        private int count;

        public int Count
        {
            get { return count; }
            set { count = value; }
        }

        public Polygon()
        {
            vertexs = new List<Node>();
            count = 0;
        }

        public Polygon(Node[] points)
        {
            vertexs = new List<Node>();
            count = points.Length;
            for (int i = 0; i < points.Length; i++)
            {
                vertexs.Add(points[i]);
            }
        }

        public RectangleF GetRectangle()
        {
            double maxX = 0;
            double maxY = 0; 
            double minX = 0;
            double minY = 0;
            for (int i = 0; i < Count; i++)
            {
                if (maxX < vertexs[i].X)
                    maxX = vertexs[i].X;
                if (maxY < vertexs[i].Y)
                    maxY = vertexs[i].Y;
                if (minX > vertexs[i].X)
                    minX = vertexs[i].X;
                if (minY > vertexs[i].Y)
                    minY = vertexs[i].Y;
            }
            RectangleF rect = new RectangleF((float)maxX, (float)maxY, (float)(maxX - minX), (float)(maxY - minY));
            return rect;
        }

        public void AddVertex(Node node)
        {
            vertexs.Add(node);
            count++;
        }

        public Node[] ToArray()
        {
            return vertexs.ToArray();
        }

        public Node this[int index]
        {
            get { return vertexs[index]; }
        }
    }

    public class Mesh
    {
        public List<Node> nodes;
        public List<Triangle> triangles;
        public List<Segment> boundary;

        public Mesh()
        {
            nodes = new List<Node>();
            triangles = new List<Triangle>();
            boundary = new List<Segment>();
        }

        public Mesh(List<Node> n, List<Triangle> t, List<Segment> s)
        {
            nodes = n;
            triangles = t;
            boundary = s;
        }

        private List<int> Connections(int index)
        {
            List<int> neighbour = new List<int>();
            foreach (Triangle triangle in triangles)
            {
                if (triangle.IndexA == index)
                {
                    if (neighbour.FindItem(triangle.IndexB) == -1)
                        neighbour.Add(triangle.IndexB);
                    if (neighbour.FindItem(triangle.IndexC) == -1)
                        neighbour.Add(triangle.IndexC);
                }
                if (triangle.IndexB == index)
                {
                    if (neighbour.FindItem(triangle.IndexA) == -1)
                        neighbour.Add(triangle.IndexA);
                    if (neighbour.FindItem(triangle.IndexC) == -1)
                        neighbour.Add(triangle.IndexC);
                }
                if (triangle.IndexC == index)
                {
                    if (neighbour.FindItem(triangle.IndexA) == -1)
                        neighbour.Add(triangle.IndexA);
                    if (neighbour.FindItem(triangle.IndexB) == -1)
                        neighbour.Add(triangle.IndexB);
                }
            }

            return neighbour;
        }

        

        /// <summary>
        /// Cuthill-McKee algoritm for renumbering nodes
        /// </summary>
        public void RenumberNodes()
        {
            int n = nodes.Count;
            List<int>[] listNodes = new List<int>[nodes.Count];
            for (int i = 0; i < n; i++)
            {
                listNodes[i] = Connections(i); 
            }
            // Sort all connection of i node
            myCompareClass mcc = new myCompareClass(listNodes);
            for (int i = 0; i < n; i++)
            {
                listNodes[i].Sort(mcc);
            }

            // Find node with minimum cinnections
            int minNode = 0;
            int minConnection = listNodes[minNode].Count;
            for (int i = 1; i < n; i++)
            {
                int curConnection = listNodes[i].Count;
                if (minConnection >= curConnection)
                {
                    minNode = i;
                    minConnection = curConnection;
                }
            }

            // Creating renumbered table
            List<int> renumberTable = new List<int>();
            renumberTable.Add(minNode);
            int cur = 0;
            while (renumberTable.Count < n)
            {
                // listNodes[renumberNodes[cur]] - list connection of curent node in renumberTable
                for (int i = 0; i < listNodes[renumberTable[cur]].Count; i++)
                {
                    int curNode = listNodes[renumberTable[cur]][i];
                    if (renumberTable.FindItem(curNode) == -1)
                        renumberTable.Add(curNode);
                }
                cur++; // next renumber node
            }

            // Nodes renumbering
            List<Node> renumberedNodes = new List<Node>();
            for (int i = 0; i < renumberTable.Count; i++)
            {
                renumberedNodes.Add(nodes[renumberTable[i]]);
            }
            nodes = renumberedNodes;

            // Triangles renumbering
            for (int i = 0; i < triangles.Count; i++)
            {
                triangles[i].IndexA = renumberTable.FindItem(triangles[i].IndexA);
                triangles[i].IndexB = renumberTable.FindItem(triangles[i].IndexB);
                triangles[i].IndexC = renumberTable.FindItem(triangles[i].IndexC);
            }

            // Boundary renumbering
            for (int i = 0; i < boundary.Count; i++)
            {
                boundary[i].IndexA = renumberTable.FindItem(boundary[i].IndexA);
                boundary[i].IndexB = renumberTable.FindItem(boundary[i].IndexB);
            }
        }

        private class myCompareClass : IComparer<int>
        {
            int[] arrayConnections;

            public myCompareClass(List<int>[] listNodes)
            {
                int n = listNodes.Length;
                arrayConnections = new int[n];
                for(int i = 0; i < n; i++)
                {
                    arrayConnections[i] = listNodes[i].Count;
                }
            }

            

            #region IComparer<int> Members

            public int Compare(int x, int y)
            {
                int xCount = arrayConnections[x];
                int yCount = arrayConnections[y];

                int res = -1;
                if (xCount == yCount)
                {
                    res = x.CompareTo(y);
                }
                res = xCount.CompareTo(yCount);
                return res;
            }

            #endregion
        }


    }

    public enum BoundaryCondition
    {
        First,
        Second,
        Third
    }

    public class BoundaryCharacteristic
    {
        private double delta;
        private double beta;
        private double uC;
        private double u0;
        private BoundaryCondition condition;

        public BoundaryCondition Condition
        {
            get { return condition; }
            set { condition = value; }
        }

        public double Delta
        {
            get { return delta; }
            set { delta = value; }
        }

        public double Beta
        {
            get { return beta; }
            set { beta = value; }
        }

        public double UC
        {
            get { return uC; }
            set { uC = value; }
        }

        public double U0
        {
            get { return u0; }
            set { u0 = value; }
        }

        public BoundaryCharacteristic()
        {
            delta = 1;
            beta = 1;
            uC = 0;
            u0 = 0;
            condition = BoundaryCondition.First;
        }

        public BoundaryCharacteristic(double delta, double beta, double uC, double u0)
        {
            this.delta = delta;
            this.beta = beta;
            this.uC = uC;
            this.u0 = u0;
            condition = BoundaryCondition.First;
        }
        public override string ToString()
        {
            return delta.ToString() + " " + beta.ToString() + " " + uC.ToString() + " " + u0.ToString() + " " + condition.ToString();
        }

        public static BoundaryCharacteristic Parse(string str)
        {
            string[] items = str.Split();
            BoundaryCharacteristic res = new BoundaryCharacteristic(double.Parse(items[0]), double.Parse(items[1]), double.Parse(items[2]), double.Parse(items[3]));
            res.Condition = (BoundaryCondition)Enum.Parse(typeof(BoundaryCondition), items[4]);
            return res;
        }
    }

    public class Boundary
    {
        public BoundaryCharacteristic[] characteristic;
        int N; // Amount of Polygon edge;

        public Boundary(BoundaryCharacteristic[] characteristic)
        {
            this.characteristic = characteristic;
            this.N = characteristic.Length;
        }

        public Boundary(int N)
        {
            this.characteristic = new BoundaryCharacteristic[N];
            this.N = N;
            for (int i = 0; i < N; i++)
            {
                characteristic[i] = new BoundaryCharacteristic();
            }
        }

        public BoundaryCharacteristic GetCharacteristic(int index)
        {
            return characteristic[index];
        }

    }

    public class Domain
    {
        public Mesh mesh;
        public Boundary boundary;

        public Domain()
        {
            this.mesh = new Mesh();
            this.boundary = new Boundary(0);

        }

        public Domain(Mesh mesh, Boundary boundary)
        {
            this.mesh = mesh;
            this.boundary = boundary;

        }
    }
}
