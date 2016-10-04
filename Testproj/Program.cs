using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Triangulation;
 
namespace Testproj
{
    class Program
    {
        static void Main(string[] args)
        {
            Polygon lp = new Polygon();
            lp.AddVertex(new Node(0,0));
            lp.AddVertex(new Node(1,0));
            lp.AddVertex(new Node(1,1));
            lp.AddVertex(new Node(0,1));
            GeneratingMesh gener = new GeneratingMesh(lp, 2);
            gener.eps = 0.001;
            Mesh m = gener.GenerateGrid();
           
            /*
            Segment s1 = new Segment(new Point(0, 0), new Point(1, 0));
            Segment s2 = new Segment(new Point(1, 0), new Point(1, 1));
            
            Console.WriteLine(Segment.angle(s1,s2));*/
        }
    }
}
