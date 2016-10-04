using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Triangulation;
using FEMLibrary;

namespace FEMprog
{
    class Saver
    {
        public static void SaveInputData(StreamWriter sw, Polygon polygon, Model model, Boundary boundary)
        {
            sw.WriteLine(polygon.Count.ToString());
            for (int i = 0; i < polygon.Count; i++)
            {
                sw.WriteLine(polygon[i].ToString());
            }
            string k = model.K[0, 0] + " " + model.K[0, 1] + " " + model.K[1, 0] + " " + model.K[1, 1];
            sw.WriteLine(k);
            sw.WriteLine(model.D.ToString());
            sw.WriteLine(model.F.ToString());
            for (int i = 0; i < boundary.characteristic.Length; i++)
            {
                sw.WriteLine(boundary.characteristic[i].ToString());
            }

        }
        /*
        
        private void WriteMesh(StreamWriter sw, Mesh m)
        {
            WriteNodes(sw, m.nodes.ToArray());
            sw.WriteLine();
            WriteTriangles(sw, m.triangles.ToArray());
        }

        private void WriteTriangles(StreamWriter sw, Triangle[] triangles)
        {
            int n = triangles.Length;
            sw.WriteLine(n);
            for (int i = 0; i < n; i++)
            {
                sw.WriteLine(triangles[i].IndexA + " " + triangles[i].IndexB + " " + triangles[i].IndexC);
            }
        }

        private void WriteNodes(StreamWriter sw, Node[] nodes)
        {
            int n = nodes.Length;
            sw.WriteLine(n);
            for (int i = 0; i < n; i++)
            {
                sw.WriteLine(nodes[i]);
            }
        }
         */
    }
}
