using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Triangulation
{
    public class GeneratingMesh
    {
        private List<Node> listpoint;
        private List<Segment> front;
        private List<Triangle> listtriang;
        private List<Segment> boundary;
        private double lengthSegment;
        public double eps; // the smallest distance to nearest node

        public GeneratingMesh()
        {
            listpoint = new List<Node>();
            front = new List<Segment>();
            listtriang = new List<Triangle>();
            boundary = new List<Segment>();

        }
        public GeneratingMesh(Polygon polygon, int quality)
        {
            listpoint = new List<Node>();
            front = new List<Segment>();
            listtriang = new List<Triangle>();
            boundary = new List<Segment>();

            Segment curSegment;
            int curQuality;
            double lengthMinSegment = FindMinSegmentLenth(polygon.ToArray()) / quality;
            int beginBoundary = 1;
            for (int i = 1; i < polygon.Count; i++)
            {
                curSegment = new Segment(polygon[i - 1], polygon[i]);
                curQuality = (int)(curSegment.length() / lengthMinSegment);
                List<Node> lp = (curSegment).explode(curQuality);
                

                for (int j = 1; j < lp.Count; j++)
                {
                    listpoint.Add(lp[j]);
                }

                for (int j = beginBoundary; j < listpoint.Count; j++)
                {
                    boundary.Add(new Segment(listpoint[j - 1], listpoint[j],ElemType.Boundary, j-1, j, i - 1));
                }

                beginBoundary = listpoint.Count;
            }

            curSegment = new Segment(polygon[polygon.Count - 1], polygon[0]);
            curQuality = (int)(curSegment.length() / lengthMinSegment );
            List<Node> lp1 = (curSegment).explode(curQuality);
            for (int j = 1; j < lp1.Count; j++)
            {
                listpoint.Add(lp1[j]);
            }

            for (int j = beginBoundary; j < listpoint.Count; j++)
            {
                boundary.Add(new Segment(listpoint[j - 1], listpoint[j], ElemType.Boundary, j - 1, j, polygon.Count - 1));
            }

            boundary.Add(new Segment(listpoint[listpoint.Count - 1], listpoint[0], ElemType.Boundary, listpoint.Count - 1, 0, 0));
            
            for (int i = 1; i < listpoint.Count; i++)
            {
                front.Add(new Segment(listpoint[i - 1], listpoint[i], ElemType.Boundary, i - 1, i));
            }
            front.Add(new Segment(listpoint[listpoint.Count - 1], listpoint[0], ElemType.Boundary, listpoint.Count - 1, 0));
            lengthSegment = lengthMinSegment;
        }

        private double FindMinSegmentLenth(Node[] nodes)
        {
            double minLengthSegment = Node.length(nodes[nodes.Length - 1], nodes[0]);
            double curLengthSegment;
            for (int i = 1; i < nodes.Length; i++)
            {
                curLengthSegment = Node.length(nodes[i - 1], nodes[i]);
                if (curLengthSegment < minLengthSegment)
                    minLengthSegment = curLengthSegment;
            }
            return minLengthSegment;
        }

        /// <summary>
        /// Create new node
        /// </summary>
        /// <param name="s">current segment</param>
        /// <param name="angle">angle</param>
        /// <param name="r">distance</param>
        /// <returns></returns>
        private Node CreateOneNode(Segment s, double angle, double r)
        {
            double omega = s.angle() + angle;
            return new Node(s.A.X + Math.Cos(omega) * r, s.A.Y + Math.Sin(omega) * r, false, ElemType.Inner);

        }

        /// <summary>
        /// Find smallest angle in front
        /// </summary>
        /// <param name="s1">previous segment</param>
        /// <param name="s2">next segment</param>
        /// <param name="k">index of s1 in front</param>
        /// <returns>angle</returns>
        private double FindSmallAngle(out Segment s1, out Segment s2, out int k)
        {
            double minang = Segment.angle(front[front.Count - 1], front[0]);
            k = front.Count - 1;
            s1 = front[front.Count - 1];
            s2 = front[0];
            for (int i = 1; i < front.Count; i++)
            {
                double angle = Segment.angle(front[i - 1], front[i]);
                if (minang > angle)
                {
                    minang = angle;
                    s1 = front[i - 1];
                    s2 = front[i];
                    k = i - 1;
                }
            }
            return minang;
        }


        /// <summary>
        /// Algorithm has been described in article 
        /// "A C# algorithm for creating triangular meshes of highly-Irregular 2D domains using the advancing front technique." 
        /// by Hassan S. Naji
        /// Triangle has CCW numbered nodes
        /// </summary>
        public Mesh GenerateGrid()
        {
            if (front.Count == 3)
            {
                listtriang.Add(new Triangle(front[0].IndexA, front[1].IndexA, front[2].IndexA));
                return new Mesh(listpoint, listtriang, boundary);
            }
            else
            {
                while (front.Count > 4)
                {
                    int N = front.Count;
                    Segment s1 = new Segment();
                    Segment s2 = new Segment();
                    int k;
                    double angle = FindSmallAngle(out s1, out s2, out k);
                    if (angle < Math.PI / 2)
                    {
                        AngleLess90(N, s1, s2, k);
                    }
                    else
                    {
                        if (angle >= Math.PI / 2 && angle <= 5 * Math.PI / 6)
                        {
                            AngleLess150(N, s1, s2, k, angle);

                        }
                        else
                        {
                            AngleMore150(N, s2, k);
                        }
                    }
                }
                if ((Segment.angle(front[0], front[1]) + Segment.angle(front[2], front[3])) < (Segment.angle(front[1], front[2]) + Segment.angle(front[3], front[0])))
                {
                    listtriang.Add(new Triangle(front[0].IndexA, front[1].IndexB, front[2].IndexB));
                    listtriang.Add(new Triangle(front[0].IndexA, front[0].IndexB, front[1].IndexB));
                }
                else
                {
                    listtriang.Add(new Triangle(front[0].IndexB, front[2].IndexB, front[3].IndexB));
                    listtriang.Add(new Triangle(front[0].IndexB, front[1].IndexB, front[2].IndexB));
                }
                RefineGrid();

            }
            return new Mesh(listpoint, listtriang, boundary);
        }

        private void AngleMore150(int N, Segment s2, int k)
        {
            Node p = CreateOneNode(s2, Math.PI / 3, s2.length());
            p.Ptype = ElemType.Inner;
            
            if (!CheckDistance(p))
            {
                listpoint.Add(p);
                Segment s_1 = new Segment(s2.A, p, ElemType.Inner, s2.IndexA, listpoint.Count-1);
                Segment s_2 = new Segment(p, s2.B, ElemType.Inner, listpoint.Count - 1, s2.IndexB);
                listtriang.Add(new Triangle( s2.IndexA, s2.IndexB, listpoint.Count - 1));
                
                List<Segment> l1 = new List<Segment>();
                List<Segment> l2 = new List<Segment>();
                if (k == N - 1)
                {
                    for (int i = 1; i < N; i++)
                    {
                        l1.Add(front[i]);
                    }
                    front = new List<Segment>();
                    front.Add(s_1);
                    front.Add(s_2);
                    for (int i = 0; i < l1.Count; i++)
                    {
                        front.Add(l1[i]);
                    }

                }
                else
                {
                    for (int i = 0; i <= k; i++)
                    {
                        l1.Add(front[i]);
                    }
                    for (int i = k + 2; i < N; i++)
                    {
                        l2.Add(front[i]);
                    }

                    front = new List<Segment>();

                    for (int i = 0; i <= k; i++)
                    {
                        front.Add(l1[i]);
                    }
                    front.Add(s_1);
                    front.Add(s_2);
                    for (int i = 0; i < N - k - 2; i++)
                    {
                        front.Add(l2[i]);
                    }
                }
                
            }
        }

        private void AngleLess150(int N, Segment s1, Segment s2, int k, double angle)
        {
            Node p = CreateOneNode(s2, angle / 2, (s1.length() + s2.length()) / 2);
            p.Ptype = ElemType.Inner;
            if (!CheckDistance(p))
            {
                listpoint.Add(p);
                Segment s_1 = new Segment(s1.A, p, ElemType.Inner, s1.IndexA, listpoint.Count - 1);
                Segment s_2 = new Segment(p, s2.B, ElemType.Inner, listpoint.Count - 1, s2.IndexB);
                listtriang.Add(new Triangle(s1.IndexA, s1.IndexB, listpoint.Count - 1));
                listtriang.Add(new Triangle(listpoint.Count - 1, s2.IndexA, s2.IndexB));

                List<Segment> l1 = new List<Segment>();
                List<Segment> l2 = new List<Segment>();
                if (k == N - 1)
                {
                    for (int i = 1; i < N - 1; i++)
                    {
                        l1.Add(front[i]);
                    }
                    front = new List<Segment>();
                    for (int i = 0; i < l1.Count; i++)
                    {
                        front.Add(l1[i]);
                    }
                    front.Add(s_1);
                    front.Add(s_2);
                }
                else
                {

                    for (int i = 0; i < k; i++)
                    {
                        l1.Add(front[i]);
                    }
                    for (int i = k + 2; i < N; i++)
                    {
                        l2.Add(front[i]);
                    }

                    front = new List<Segment>();

                    for (int i = 0; i < k; i++)
                    {
                        front.Add(l1[i]);
                    }
                    front.Add(s_1);
                    front.Add(s_2);
                    for (int i = 0; i < N - k - 2; i++)
                    {
                        front.Add(l2[i]);
                    }
                }
                
            }
        }

        private void AngleLess90(int N, Segment s1, Segment s2, int k)
        {
            Segment s = new Segment(s1.A, s2.B, ElemType.Inner, s1.IndexA, s2.IndexB);
            List<Segment> l1 = new List<Segment>();
            List<Segment> l2 = new List<Segment>();
            if (k == N - 1)
            {
                for (int i = 1; i < N - 1; i++)
                {
                    l1.Add(front[i]);
                }
                front = new List<Segment>();
                for (int i = 0; i < l1.Count; i++)
                {
                    front.Add(l1[i]);
                }
                front.Add(s);
            }
            else
            {
                for (int i = 0; i < k; i++)
                {
                    l1.Add(front[i]);
                }
                for (int i = k + 2; i < N; i++)
                {
                    l2.Add(front[i]);
                }

                front = new List<Segment>();

                for (int i = 0; i < k; i++)
                {
                    front.Add(l1[i]);
                }
                front.Add(s);
                for (int i = 0; i < N - k - 2; i++)
                {
                    front.Add(l2[i]);
                }
            }
            listtriang.Add(new Triangle( s1.IndexA, s1.IndexB, s2.IndexB));
        }

        private bool CheckDistance(Node p)
        {
            bool near = false;
            foreach (Node point in listpoint)
            {
                if (Node.length(point, p) < eps)
                {
                    near = true;
                    break;
                }
            }
            return near;
        }

        public void RefineGrid()
        {
            for (int i = 0; i < listpoint.Count; i++)
            {
                if (listpoint[i].Ptype == ElemType.Inner)
                    ReplacePoint(i);
            }
        }

        private void ReplacePoint(int index)
        {
            int[] neighbourPoints = FindNeighbourPoints(index);
            int n = neighbourPoints.Length;
            double newX = 0;
            double newY = 0;
            for (int i = 0; i < n; i++)
            {
                newX += listpoint[neighbourPoints[i]].X;
                newY += listpoint[neighbourPoints[i]].Y;
            }
            listpoint[index] = new Node(newX / n, newY / n, true, ElemType.Inner);
        }

        private int[] FindNeighbourPoints(int index)
        {
            List<int> neighbourPoints = new List<int>();
            foreach (Triangle triang in listtriang)
            {
                if (triang.IndexA == index)
                {
                    if (!FindIndex(neighbourPoints, triang.IndexA))
                        neighbourPoints.Add(triang.IndexB);
                    if (!FindIndex(neighbourPoints, triang.IndexA))
                        neighbourPoints.Add(triang.IndexC);
                }
                if (triang.IndexB == index)
                {
                    if (!FindIndex(neighbourPoints, triang.IndexA))
                        neighbourPoints.Add(triang.IndexA);
                    if (!FindIndex(neighbourPoints, triang.IndexA))
                        neighbourPoints.Add(triang.IndexC);
                }
                if (triang.IndexC == index)
                {
                    if (!FindIndex(neighbourPoints, triang.IndexA))
                        neighbourPoints.Add(triang.IndexB);
                    if (!FindIndex(neighbourPoints, triang.IndexA))
                        neighbourPoints.Add(triang.IndexA);
                }
            }


            return neighbourPoints.ToArray();

        }

        private static bool FindIndex(List<int> list, int index)
        {
            bool res = false;
            foreach (int i in list)
            {
                if (i == index)
                {
                    res = true;
                }
            }
            return res;
        }

    }
}
