using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Triangulation;
using System.IO;

namespace FEMLibrary
{

    public class PDESolver
    {
        public static Vector Solve(Matrix K, double d, double f, Domain domain)
        {
            Matrix totalMatrix = new Matrix(domain.mesh.nodes.Count, domain.mesh.nodes.Count);
            Vector totalVector = new Vector(domain.mesh.nodes.Count);
            // Surface integral
            foreach (Triangle triangle in domain.mesh.triangles)
            {
                List<int> indexs = new List<int>();
                indexs.Add(triangle.IndexA);
                indexs.Add(triangle.IndexB);
                indexs.Add(triangle.IndexC);
                double square = Triangle.Square(domain.mesh.nodes[indexs[0]],
                    domain.mesh.nodes[indexs[1]],
                    domain.mesh.nodes[indexs[2]]);

                double[] b = new double[3];
                double[] c = new double[3];
                
                c[0] = domain.mesh.nodes[indexs[1]].X - domain.mesh.nodes[indexs[2]].X;
                b[0] = domain.mesh.nodes[indexs[2]].Y - domain.mesh.nodes[indexs[1]].Y;

                c[1] = domain.mesh.nodes[indexs[2]].X - domain.mesh.nodes[indexs[0]].X;
                b[1] = domain.mesh.nodes[indexs[0]].Y - domain.mesh.nodes[indexs[2]].Y;

                c[2] = domain.mesh.nodes[indexs[0]].X - domain.mesh.nodes[indexs[1]].X;
                b[2] = domain.mesh.nodes[indexs[1]].Y - domain.mesh.nodes[indexs[0]].Y;


                for (int i = 0; i < indexs.Count; i++)
                {
                    for (int j = 0; j < indexs.Count; j++)
                    {
                        if (i == j)
                            totalMatrix[indexs[i], indexs[j]] += (K[0, 0] * b[j] * b[i] + K[1, 1] * c[j] * c[i]) / (4 * square) + d * square / 6;
                        else
                            totalMatrix[indexs[i], indexs[j]] += (K[0, 0] * b[j] * b[i] + K[1, 1] * c[j] * c[i]) / (4 * square) + d * square / 12;

                    }
                    totalVector[indexs[i]] += f * square / 3;
                }
            }



            //Line integral

            List<int> firstConditionNodes = new List<int>();
            List<double> firstConditionValues = new List<double>();
            foreach (Segment segment in domain.mesh.boundary)
            {
                BoundaryCharacteristic curCharacteristic = domain.boundary.GetCharacteristic(segment.EdgeNumber);
                if (curCharacteristic.Condition == BoundaryCondition.Third)
                {
                    List<int> indexs = new List<int>();
                    indexs.Add(segment.IndexA);
                    indexs.Add(segment.IndexB);

                    double length = Node.length(domain.mesh.nodes[indexs[0]], domain.mesh.nodes[indexs[1]]);//segment.length();
                    double coef = curCharacteristic.Delta / curCharacteristic.Beta;

                    for (int i = 0; i < indexs.Count; i++)
                    {
                        for (int j = 0; j < indexs.Count; j++)
                        {
                            if (i == j)
                                totalMatrix[indexs[i], indexs[j]] += coef * length / 3;
                            else
                                totalMatrix[indexs[i], indexs[j]] += coef * length / 6;

                        }
                        totalVector[indexs[i]] += coef * curCharacteristic.UC * length / 2;
                    }
                }
                else
                {
                    if (curCharacteristic.Condition == BoundaryCondition.First)
                    {
                        if (firstConditionNodes.FindItem(segment.IndexA) == -1)
                        {
                            firstConditionNodes.Add(segment.IndexA);
                            firstConditionValues.Add(curCharacteristic.U0);
                        }
                        else
                        {
                            int k = firstConditionNodes.FindItem(segment.IndexA);
                            firstConditionValues[k] = (firstConditionValues[k] + curCharacteristic.U0) / 2;
                        }
                        if (firstConditionNodes.FindItem(segment.IndexB) == -1)
                        {
                            firstConditionNodes.Add(segment.IndexB);
                            firstConditionValues.Add(curCharacteristic.U0);
                        }
                        else
                        {

                            int k = firstConditionNodes.FindItem(segment.IndexB);
                            firstConditionValues[k] = (firstConditionValues[k] + curCharacteristic.U0) / 2;
                        }
                    }
                }
            }


            //First boundary condition
            for (int i = 0; i < firstConditionNodes.Count; i++)
            {
                for (int j = 0; j < totalVector.Length; j++)
                {
                    totalVector[j] -= firstConditionValues[i] * totalMatrix[j, firstConditionNodes[i]];
                }
            }

            totalMatrix = Matrix.DecreaseMatrix(totalMatrix, firstConditionNodes);
            totalVector = Vector.DecreaseVector(totalVector, firstConditionNodes);

            Vector notBoundaryElements = totalMatrix.LUalgorithm(totalVector);

            Vector res = new Vector(domain.mesh.nodes.Count);
            int curNotBoundaryElements = 0;
            for (int i = 0; i < res.Length; i++)
            {
                if (firstConditionNodes.FindItem(i) == -1)
                {
                    res[i] = notBoundaryElements[curNotBoundaryElements];
                    curNotBoundaryElements++;
                }
                else
                {
                    res[i] = firstConditionValues[firstConditionNodes.FindItem(i)];
                }

            }


            return res;
        }

        public static Matrix GetStiffnessMatrix(Matrix K, double d, double f, Domain domain, int k)
        {
            Matrix m = new Matrix(3, 3);
            Triangle triangle = domain.mesh.triangles[k];
            List<int> indexs = new List<int>();
            indexs.Add(triangle.IndexA);
            indexs.Add(triangle.IndexB);
            indexs.Add(triangle.IndexC);
            double square = Triangle.Square(domain.mesh.nodes[indexs[0]],
                domain.mesh.nodes[indexs[1]],
                domain.mesh.nodes[indexs[2]]);

            double[] b = new double[3];
            double[] c = new double[3];

            b[0] = domain.mesh.nodes[indexs[1]].X - domain.mesh.nodes[indexs[2]].X;
            c[0] = domain.mesh.nodes[indexs[2]].Y - domain.mesh.nodes[indexs[1]].Y;

            b[1] = domain.mesh.nodes[indexs[2]].X - domain.mesh.nodes[indexs[0]].X;
            c[1] = domain.mesh.nodes[indexs[0]].Y - domain.mesh.nodes[indexs[2]].Y;

            b[2] = domain.mesh.nodes[indexs[0]].X - domain.mesh.nodes[indexs[1]].X;
            c[2] = domain.mesh.nodes[indexs[1]].Y - domain.mesh.nodes[indexs[0]].Y;


            for (int i = 0; i < indexs.Count; i++)
            {
                for (int j = 0; j < indexs.Count; j++)
                {
                    m[i, j] += (K[0, 0] * b[j] * b[i] + K[1, 1] * c[j] * c[i]) / (4 * square);
                }
            }



            return m;
        }

        public static Matrix GetMassMatrix(Matrix K, double d, double f, Domain domain, int k)
        {
            Matrix m = new Matrix(3, 3);
            Triangle triangle = domain.mesh.triangles[k];
            List<int> indexs = new List<int>();
            indexs.Add(triangle.IndexA);
            indexs.Add(triangle.IndexB);
            indexs.Add(triangle.IndexC);
            double square = Triangle.Square(domain.mesh.nodes[indexs[0]],
                domain.mesh.nodes[indexs[1]],
                domain.mesh.nodes[indexs[2]]);


            for (int i = 0; i < indexs.Count; i++)
            {
                for (int j = 0; j < indexs.Count; j++)
                {
                    if (i == j)
                        m[i, j] = d * square / 6;
                    else
                        m[i, j] = d * square / 12;

                }
            }

            return m;
        }

        public static Matrix GetBoundaryMatrix(Matrix K, double d, double f, Domain domain, int k)
        {
            Matrix m = new Matrix(3, 3);
            Triangle triangle = domain.mesh.triangles[k];
            List<int> indexs = new List<int>();
            indexs.Add(triangle.IndexA);
            indexs.Add(triangle.IndexB);
            indexs.Add(triangle.IndexC);

            Segment s1 = new Segment(domain.mesh.nodes[indexs[0]],domain.mesh.nodes[indexs[1]],ElemType.Boundary, indexs[0],indexs[1]);
            Segment s2 = new Segment(domain.mesh.nodes[indexs[1]],domain.mesh.nodes[indexs[2]],ElemType.Boundary, indexs[1],indexs[2]);
            Segment s3 = new Segment(domain.mesh.nodes[indexs[2]],domain.mesh.nodes[indexs[0]],ElemType.Boundary, indexs[2],indexs[0]);

            
            for (int q = 0;q < domain.mesh.boundary.Count;q++)
            {
                Segment segment = domain.mesh.boundary[q];
                BoundaryCharacteristic curCharacteristic = domain.boundary.GetCharacteristic(segment.EdgeNumber);
                if (curCharacteristic.Condition == BoundaryCondition.Third)
                {
                    double length = segment.length();
                    double coef = curCharacteristic.Delta / curCharacteristic.Beta;
                    
                    if (segment.Equal(s1))
                    {
                        List<int> ind = new List<int>();
                        ind.Add(0);
                        ind.Add(1);
                        for (int i = 0; i < ind.Count; i++)
                        {
                            for (int j = 0; j < ind.Count; j++)
                            {
                                if (i == j)
                                    m[ind[i], ind[j]] += coef * length / 3;
                                else
                                    m[ind[i], ind[j]] += coef * length / 6;

                            }
                        }
                    }

                    if (segment.Equal(s2))
                    {
                        List<int> ind = new List<int>();
                        ind.Add(1);
                        ind.Add(2);
                        for (int i = 0; i < ind.Count; i++)
                        {
                            for (int j = 0; j < ind.Count; j++)
                            {
                                if (i == j)
                                    m[ind[i], ind[j]] += coef * length / 3;
                                else
                                    m[ind[i], ind[j]] += coef * length / 6;

                            }
                        }
                    }

                    if (segment.Equal(s3))
                    {
                        List<int> ind = new List<int>();
                        ind.Add(2);
                        ind.Add(0);
                        for (int i = 0; i < ind.Count; i++)
                        {
                            for (int j = 0; j < ind.Count; j++)
                            {
                                if (i == j)
                                    m[ind[i], ind[j]] += coef * length / 3;
                                else
                                    m[ind[i], ind[j]] += coef * length / 6;

                            }
                        }
                    }
                }

            }
            

            return m;
        }

        public static Vector GetBoundaryVector(Matrix K, double d, double f, Domain domain, int k)
        {
            Vector v = new Vector(3);
            Triangle triangle = domain.mesh.triangles[k];
            List<int> indexs = new List<int>();
            indexs.Add(triangle.IndexA);
            indexs.Add(triangle.IndexB);
            indexs.Add(triangle.IndexC);

            Segment s1 = new Segment(domain.mesh.nodes[indexs[0]], domain.mesh.nodes[indexs[1]], ElemType.Boundary, indexs[0], indexs[1]);
            Segment s2 = new Segment(domain.mesh.nodes[indexs[1]], domain.mesh.nodes[indexs[2]], ElemType.Boundary, indexs[1], indexs[2]);
            Segment s3 = new Segment(domain.mesh.nodes[indexs[2]], domain.mesh.nodes[indexs[0]], ElemType.Boundary, indexs[2], indexs[0]);


            for (int q = 0; q < domain.mesh.boundary.Count; q++)
            {
                Segment segment = domain.mesh.boundary[q];
                BoundaryCharacteristic curCharacteristic = domain.boundary.GetCharacteristic(segment.EdgeNumber);
                if (curCharacteristic.Condition == BoundaryCondition.Third)
                {
                    double length = segment.length();
                    double coef = curCharacteristic.Delta / curCharacteristic.Beta;

                    if (segment.Equal(s1))
                    {
                        List<int> ind = new List<int>();
                        ind.Add(0);
                        ind.Add(1);
                        for (int i = 0; i < ind.Count; i++)
                        {
                            v[ind[i]] += coef * curCharacteristic.UC * length / 2;
                        }
                    }

                    if (segment.Equal(s2))
                    {
                        List<int> ind = new List<int>();
                        ind.Add(1);
                        ind.Add(2);
                        for (int i = 0; i < ind.Count; i++)
                        {
                            v[ind[i]] += coef * curCharacteristic.UC * length / 2;
                        }
                    }

                    if (segment.Equal(s3))
                    {
                        List<int> ind = new List<int>();
                        ind.Add(2);
                        ind.Add(0);
                        for (int i = 0; i < ind.Count; i++)
                        {
                            v[ind[i]] += coef * curCharacteristic.UC * length / 2;
                        }
                    }
                }

            }


            return v;
        }

        public static Vector GetVector(Matrix K, double d, double f, Domain domain, int k)
        {
            Vector v = new Vector(3);
            Triangle triangle = domain.mesh.triangles[k];
            List<int> indexs = new List<int>();
            indexs.Add(triangle.IndexA);
            indexs.Add(triangle.IndexB);
            indexs.Add(triangle.IndexC);
            double square = Triangle.Square(domain.mesh.nodes[indexs[0]],
                domain.mesh.nodes[indexs[1]],
                domain.mesh.nodes[indexs[2]]);



            for (int i = 0; i < indexs.Count; i++)
            {
                v[i] += f * square / 3;
            }
            return v;
        }

    }
}