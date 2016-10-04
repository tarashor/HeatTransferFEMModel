using FEMLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Triangulation;
using System.Collections.Generic;

namespace PDESolverUnitTest
{
    
    
    /// <summary>
    ///This is a test class for PDESolverTest and is intended
    ///to contain all PDESolverTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PDESolverTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Solve
        ///</summary>
        [TestMethod()]
        public void SolveTest()
        {
            Matrix k = new Matrix(2, 2);
            k[0, 0] = 4;
            k[0, 1] = 0;
            k[1, 0] = 0;
            k[1, 1] = 3;
            double d = 0;
            double f = 1;

            List<Node> nodes = new List<Node>();
            nodes.Add(new Node(0,0));
            nodes.Add(new Node(1,0));
            nodes.Add(new Node(1,1));
            nodes.Add(new Node(2,1));
            nodes.Add(new Node(0,2));
            nodes.Add(new Node(0,1));

            List<Segment> segments = new List<Segment>();
            segments.Add(new Segment(nodes[0], nodes[1], ElemType.Boundary, 0,1,0));
            segments.Add(new Segment(nodes[1], nodes[2], ElemType.Boundary, 1,2,1));
            segments.Add(new Segment(nodes[2], nodes[3], ElemType.Boundary, 2,3,1));
            segments.Add(new Segment(nodes[3], nodes[4], ElemType.Boundary, 3,4,2));
            segments.Add(new Segment(nodes[4], nodes[5], ElemType.Boundary, 4,5,3));
            segments.Add(new Segment(nodes[5], nodes[0], ElemType.Boundary, 5,0,3));

            List<Triangle> triangles = new List<Triangle>();
            triangles.Add(new Triangle(0,1,2));
            triangles.Add(new Triangle(0,2,5));
            triangles.Add(new Triangle(5,2,3));
            triangles.Add(new Triangle(5,3,4));

            Mesh mesh = new Mesh(nodes, triangles, segments);



            Boundary boundary = new Boundary(4);
            boundary.characteristic[0].Condition = BoundaryCondition.Third;
            boundary.characteristic[1].Condition = BoundaryCondition.Third;
            boundary.characteristic[2].Condition = BoundaryCondition.Third;
            boundary.characteristic[3].Condition = BoundaryCondition.First;

            for (int i = 0; i < 4; i++)
            {
                boundary.characteristic[i].Beta = 1;
                boundary.characteristic[i].Delta = 1;
                boundary.characteristic[i].UC = 0;
                boundary.characteristic[i].U0 = 100;
            }




            Domain domain = new Domain(mesh, boundary); // TODO: Initialize to an appropriate value
            Vector expected = null; // TODO: Initialize to an appropriate value
            Vector actual;
            actual = PDESolver.Solve(k, d, f, domain);
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
