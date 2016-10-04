using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Triangulation;
using FEMLibrary;

namespace FEMprog
{
    class Opener
    {
        public static void OpenInputData(StreamReader sr, out Polygon polygon, out Model model, out Boundary boundary)
        {
            int countNodes = int.Parse(sr.ReadLine());
            polygon = new Polygon();
            for (int i = 0; i < countNodes; i++)
            {
                polygon.AddVertex(Node.Parse(sr.ReadLine()));
            }
            model = new Model();
            string k = sr.ReadLine();
            string[] kItems = k.Split();
            model.K[0, 0] = double.Parse(kItems[0]);
            model.K[0, 1] = double.Parse(kItems[1]);
            model.K[1, 0] = double.Parse(kItems[2]);
            model.K[1, 1] = double.Parse(kItems[3]);
            model.D = double.Parse(sr.ReadLine());
            model.F = double.Parse(sr.ReadLine());
            List<BoundaryCharacteristic> listBoundaryCharacteristic = new List<BoundaryCharacteristic>();
            for (int i = 0; i < countNodes; i++)
            {
                listBoundaryCharacteristic.Add(BoundaryCharacteristic.Parse(sr.ReadLine()));
            }
            boundary = new Boundary(listBoundaryCharacteristic.ToArray());

        }
        /*
         private Mesh ReadMesh(StreamReader sw)
        {
            Mesh mesh = new Mesh();
            mesh.nodes = ReadNodes(sw);
            sw.ReadLine();
            mesh.triangles = ReadTriangles(sw, mesh.nodes);
            return mesh;
        }

        private List<Triangle> ReadTriangles(StreamReader sw, List<Node> listNode)
        {
            int n = int.Parse(sw.ReadLine());
            List<Triangle> listTriangle = new List<Triangle>();
            for (int i = 0; i < n; i++)
            {
                string[] mas = sw.ReadLine().Split();
                int iA = int.Parse(mas[0]);
                int iB = int.Parse(mas[1]);
                int iC = int.Parse(mas[2]);
                Triangle triangle = new Triangle(iA, iB, iC);
                listTriangle.Add(triangle);
            }
            return listTriangle;
        }

        private List<Node> ReadNodes(StreamReader sw)
        {
            int n = int.Parse(sw.ReadLine());
            List<Node> listNode = new List<Node>();
            for (int i = 0; i < n; i++)
            {
                listNode.Add(Node.Parse(sw.ReadLine()));
            }
            return listNode;
        } 
         
         */
    }
}
