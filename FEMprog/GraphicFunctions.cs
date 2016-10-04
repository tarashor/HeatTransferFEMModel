using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Triangulation;
using System.Drawing.Drawing2D;

namespace FEMprog
{
    
    class GraphicFunctions
    {
        public static double w;
        public static double h;
        public static double a;
        public static double b;
        public static double c;
        public static double d;

        public readonly static double INDENT = 0.1;
        public readonly static int THICKNESS = 3;

        public void SetAllParameters(RectangleF rect, int width, int height)
        {
            w = width;
            h = height;
            int pixelsPerUnitX = (int)Math.Round(((double)w) / rect.Width);
            int pixelsPerUnitY = (int)Math.Round(((double)h) / rect.Height);
            int pixelsPerUnit = 0;
            if (pixelsPerUnitX < pixelsPerUnitY)
            {
                //pixelsPerUnit = pixelsPerUnitX;
            }
            else
            {
                //pixelsPerUnit = pixelsPerUnitY;
            }
            //a = rect.X;
            
        }

        public static Point ConvertToScreen(Node x)
        {
            return (Point)new Node((x.X - a) * w / (b - a),  (x.Y - d) * h / (c - d));
        }

        public static Node ConvertToOwn(Point x)
        {
            return new Node((b - a) * x.X / w + a, (c - d) * x.Y / h+ d);
        }

        public static bool IsVisible(Node node)
        {
            bool res = false;
            if ((a < node.X) && (node.X < b) && (c < node.Y) && (node.Y < d))
            {
                res = true;
            }
            return res;
        }

        public static void BecomeVisible(Node node)
        {
            if (a >= node.X) 
            {
                a = node.X - INDENT;
            }
            if (node.X >= b) 
            {
                b = node.X + INDENT;
            }
            if (c >= node.Y)
            {
                c = node.Y - INDENT;
            }
            if (node.Y >= d)
            {
                d = node.Y + INDENT;
            }
        }

        public static void DrawNode(Node node, Graphics g, bool isNumbered, int index)
        {
            BecomeVisible(node);
            Point point = ConvertToScreen(node);
            Rectangle rect = new Rectangle(point.X - THICKNESS, point.Y - THICKNESS, 2 * THICKNESS, 2 * THICKNESS);
            g.FillEllipse(Brushes.Red, rect);
            if (isNumbered)
            {
                g.DrawString(index.ToString(), new Font("Arial", 8), Brushes.Blue, new PointF(point.X + 2 * THICKNESS, point.Y - 3 * THICKNESS)); 
            }
        }

        public static void DrawNodes(Node[] nodes, Graphics g, bool isNumbered)
        {
            for (int i = 0; i < nodes.Length;i++)
            {
                DrawNode(nodes[i], g, isNumbered, i);
            }
        }

        public static void DrawPolygon(Node[] nodes, Graphics g)
        {
            Pen pen = new Pen(Color.Green, THICKNESS);
            for(int i = 1; i < nodes.Length; i++)
            {
                g.DrawLine(pen, ConvertToScreen(nodes[i - 1]), ConvertToScreen(nodes[i]));
            }
            g.DrawLine(pen, ConvertToScreen(nodes[nodes.Length - 1]), ConvertToScreen(nodes[0]));
        }

        public static void DrawPolygonEdge(Polygon polygon, int edge, Graphics g)
        {
            Pen pen = new Pen(Color.Aqua, THICKNESS);
            if (edge < polygon.Count)
            {
                if (edge == polygon.Count - 1)
                    g.DrawLine(pen, ConvertToScreen(polygon[polygon.Count - 1]), ConvertToScreen(polygon[0]));
                else
                    g.DrawLine(pen, ConvertToScreen(polygon[edge]), ConvertToScreen(polygon[edge + 1]));
            }
            
            
        }

        public static void DrawTriangle(Node A, Node B, Node C, Graphics g)
        {
            Pen pen = new Pen(Color.Gray);
            g.DrawLine(pen, ConvertToScreen(A), ConvertToScreen(B));
            g.DrawLine(pen, ConvertToScreen(B), ConvertToScreen(C));
            g.DrawLine(pen, ConvertToScreen(C), ConvertToScreen(A));
        }



        public static void DrawGrid(Mesh mesh, Graphics g)
        {
            for (int i = 0; i < mesh.triangles.Count; i++)
            {
                DrawTriangle(mesh.nodes[mesh.triangles[i].IndexA], mesh.nodes[mesh.triangles[i].IndexB], mesh.nodes[mesh.triangles[i].IndexC], g);
            }
        }

        public static void FillTriangle(Graphics g, Node pa, Node pb, Node pc, double ua, double ub, double uc, Pallette pallette)
        {
            double[] v = new double[] { ua, ub, uc };
            Node[] p = new Node[] { pa, pb, pc };
            int[] indexator = new int[v.Length];
            indexator[0] = IMin(v);
            indexator[2] = IMax(v);
            indexator[1] = 3 - indexator[0] - indexator[2];

            Node a = p[indexator[0]];
            Node b = p[indexator[2]];
            Node c = p[indexator[1]];
            double vA = v[indexator[0]];
            double vB = v[indexator[2]];
            double vC = v[indexator[1]];
            double vD = v[indexator[1]];

            Node d = AdditionNode(v, p);

            FillHalfTriangle(g, ConvertToScreen(a), ConvertToScreen(d), ConvertToScreen(c), vA, vC, pallette);
            FillHalfTriangle(g, ConvertToScreen(b), ConvertToScreen(d), ConvertToScreen(c), vB, vC, pallette);
        }

        private static void FillHalfTriangle(Graphics grfx, Point a, Point b, Point c, double va, double v, Pallette pallette)
        {
            if ((a.Y == b.Y) && (b.Y == c.Y))
                return;
            if ((a.X == b.X) && (b.X == c.X))
                return;

            PathGradientBrush pthGrBrush = new PathGradientBrush(new Point[] { a, b, c });


            ColorBlend colorBlend = new ColorBlend();
            if (va < v)
            {
                List<Color> l = pallette.GetColorPallette(va, v);
                l.Reverse();
                colorBlend.Colors = l.ToArray();
            }
            else
            {
                List<Color> l = pallette.GetColorPallette(v, va);
                colorBlend.Colors = l.ToArray();
            }

            colorBlend.Positions = pallette.GetRelativaPossition();
            pthGrBrush.InterpolationColors = colorBlend;
            pthGrBrush.CenterPoint = a;

            grfx.FillRectangle(pthGrBrush, new Rectangle(0,0,(int)w,(int)h));
        }

        private static int IMax(double[] ar)
        {
            int imax = 0;
            double max = ar[imax];
            for (int i = 0; i < ar.Length; i++)
            {
                if (max <= ar[i])
                {
                    imax = i;
                    max = ar[i];
                }
            }
            return imax;
        }

        private static int IMin(double[] ar)
        {
            int imin = 0;
            double min = ar[imin];
            for (int i = 0; i < ar.Length; i++)
            {
                if (min > ar[i])
                {
                    imin = i;
                    min = ar[i];
                }
            }
            return imin;
        }


        private static  Node AdditionNode(double[] v, Node[] p)
        {
            int[] indexator = new int[v.Length];
            indexator[0] = IMin(v);
            indexator[2] = IMax(v);
            indexator[1] = 3 - indexator[0] - indexator[2];

            Node d = new Node();

            Node a = p[indexator[0]];
            Node b = p[indexator[2]];
            Node c = p[indexator[1]];
            double vA = v[indexator[0]];
            double vB = v[indexator[2]];
            double vC = v[indexator[1]];
            double square2 = b.X * c.Y + a.X * b.Y + a.Y * c.X - a.Y * b.X - a.X * c.Y - c.X * b.Y;
            
            double aA = (b.X * c.Y - b.Y * c.X);
            double bA = (b.Y - c.Y);
            double cA = (c.X - b.X);

            double aB = (c.X * a.Y - c.Y * a.X);
            double bB = (c.Y - a.Y);
            double cB = (a.X - c.X);


            if (Math.Abs(vA - vB) <= 0.000000001)
            {
                d.X = (a.X + b.X) / 2;
                d.Y = (a.Y + b.Y) / 2;
            }
            else
            {
                if (Math.Abs(a.X - b.X) <= 0.000000001)
                {
                    d.X = (a.X + b.X) / 2;
                    double kLine = (vA - vB) / (a.Y - b.Y);
                    double bLine = vA - kLine * a.Y;
                    d.Y = (vC - bLine) / kLine;
                }
                else
                {
                    double kLine = (a.Y - b.Y) / (a.X - b.X);
                    double bLine = a.Y - kLine * a.X;

                    d.X = (square2 * vC - vA * (aA + cA * bLine) - vB * (aB + bLine * cB)) / (vA * (bA + cA * kLine) + vB * (bB + cB * kLine));
                    d.Y = kLine * d.X + bLine;

                }
            }
            return d;
        }

    }
}
