using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Triangulation;

namespace FEMLibrary
{
    #region - Matrix -
    public class Matrix
    {
        private double[,] matrix;
        private int n; // kilkist rjadky
        private int m; // kilkist stovpciv

        /// <summary>
        /// Matrix constructor
        /// </summary>
        /// <param name="n">rows count</param>
        /// <param name="m">column count</param>
        public Matrix(int n, int m)
        {
            matrix = new double[n, m];
            this.n = n;
            this.m = m;

        }
        public Matrix(Matrix m)
        {
            this.n = m.n;
            this.m = m.m;
            matrix = new double[this.n, this.m];
            for (int i = 0; i < this.n; i++)
            {
                for (int j = 0; j < this.m; j++)
                {
                    matrix[i, j] = m[i, j];
                }
            }
        }

        #region - Properties -
        public bool IsQuadratic
        {
            get { return (n == m); }
        }

        public int CountRows
        {
            get { return n; }
        }

        public int CountColumns
        {
            get { return m; }
        }

        public double this[int i, int j]
        {
            get
            {
                if (((0 <= i) && (i <= n)) && ((0 <= j) && (j <= m)))
                    return matrix[i, j];
                else throw new IndexOutOfRangeException();
            }
            set
            {
                if (((0 <= i) && (i <= n)) && ((0 <= j) && (j <= m)))
                    matrix[i, j] = value;
                else throw new IndexOutOfRangeException();
            }
        }

        #endregion

        #region - Algorithms -

        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            if (m1.CountColumns == m2.CountRows)
            {
                Matrix res = new Matrix(m1.CountRows, m2.CountColumns);
                int N = m1.CountColumns;
                for (int i = 0; i < m1.CountRows; i++)
                {
                    for (int j = 0; j < m2.CountColumns; j++)
                    {
                        double sum = 0;
                        for (int k = 0; k < N; k++)
                        {
                            sum += m1[i, k] * m2[k, j];
                        }
                        res[i, j] = sum;
                    }
                }
                return res;
            }
            else
            {
                throw new CountException();
            }
        }

        public static Matrix operator *(double d, Matrix m)
        {
            Matrix res = new Matrix(m);
            for (int i = 0; i < m.CountRows; i++)
            {
                for (int j = 0; j < m.CountColumns; j++)
                {
                    res[i, j] *= d;
                }
            }
            return res;
        }

        public static Matrix operator *(Matrix m, double d)
        {
            Matrix res = new Matrix(m);
            for (int i = 0; i < m.CountRows; i++)
            {
                for (int j = 0; j < m.CountColumns; j++)
                {
                    res[i, j] *= d;
                }
            }
            return res;
        }

        public static Matrix operator +(Matrix m1, Matrix m2)
        {
            if ((m1.CountColumns == m2.CountColumns) && (m1.CountRows == m2.CountRows))
            {
                Matrix res = new Matrix(m1.CountRows, m1.CountColumns);
                for (int i = 0; i < m1.CountRows; i++)
                {
                    for (int j = 0; j < m2.CountColumns; j++)
                    {
                        res[i, j] = m1[i, j] + m2[i, j];
                    }
                }
                return res;
            }
            else
            {
                throw new CountException();
            }
        }

        public static Matrix Transpose(Matrix mat)
        {
            Matrix res = new Matrix(mat.CountColumns, mat.CountRows);
            for (int i = 0; i < mat.CountRows; i++)
            {
                for (int j = 0; j < mat.CountColumns; j++)
                {
                    res[j, i] = mat[i, j];
                }
            }
            return res;
        }


        #region - Solving system equation -

        public double Determinant()
        {
            if (IsQuadratic)
            {
                int N = this.n;
                Matrix L = new Matrix(N, N);
                Matrix U = new Matrix(N, N);
                this.GetLUMatrixs(L, U);

                double res = 1;
                for (int i = 0; i < N; i++)
                {
                    res *= U[i, i];
                }
                return res;
            }
            else throw new CountException("Matrix is not Quadratic!!!");
        }

        public void GetLUMatrixs(Matrix L, Matrix U)
        {
            if (IsQuadratic && L.IsQuadratic && U.IsQuadratic)
            {
                int N = this.m;
                for (int i = 0; i < N; i++)
                {
                    for (int j = i; j < N; j++)
                    {
                        double sum1 = 0;
                        for (int k = 0; k < i; k++)
                        {
                            sum1 += L[i, k] * U[k, j];
                        }
                        U[i, j] = this[i, j] - sum1;

                    }
                    for (int j = i + 1; j < N; j++)
                    {
                        double sum2 = 0;
                        for (int k = 0; k < i; k++)
                        {
                            sum2 += L[i, k] * U[k, j];
                        }
                        L[j, i] = (this[j, i] - sum2) / U[i, i];

                    }
                }
                for (int i = 0; i < N; i++)
                {
                    L[i, i] = 1;
                }
            }
            else throw new CountException("Matrix is not Quadratic!!!");
        }

        public Vector LUalgorithm(Vector b)
        {
            if (this.IsQuadratic)
            {
                int N = this.n;
                Matrix U = new Matrix(N, N);
                Matrix L = new Matrix(N, N);

                GetLUMatrixs(L, U);

                double[] y = new double[N];
                for (int i = 0; i < N; i++)
                {
                    double sum1 = 0;
                    for (int k = 0; k < i; k++)
                    {
                        sum1 += L[i, k] * y[k];
                    }
                    y[i] = b[i] - sum1;
                }

                Vector x = new Vector(N);
                for (int i = N - 1; i >= 0; i--)
                {
                    double sum2 = 0;
                    for (int k = N - 1; k > i; k--)
                    {
                        sum2 += U[i, k] * x[k];
                    }
                    x[i] = (y[i] - sum2) / U[i, i];
                }
                return x;
            }
            else throw new CountException("Matrix is not Quadratic!!!");
        }

        #endregion

        #endregion

        public override string ToString()
        {
            string str_matrix = "";
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    str_matrix += matrix[i, j].ToString("00.000") + "   ";
                }
                str_matrix += "\r\n";
            }
            return str_matrix;
        }

        /// <summary>
        /// Delete row - k and column - l
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public static Matrix DecreaseMatrix(Matrix matrix, List<int> indexDelete)
        {
            Matrix res = new Matrix(matrix.n - indexDelete.Count, matrix.m - indexDelete.Count);
            int iRes = 0;
            int jRes = 0;
            for (int i = 0; i < matrix.n; i++)
            {
                if (indexDelete.FindItem(i) == -1)
                {
                    for (int j = 0; j < matrix.m; j++)
                    {
                        if (indexDelete.FindItem(j) == -1)
                        {
                            res[iRes, jRes] = matrix[i, j];
                            jRes++;
                        }
                    }
                    jRes = 0;
                    iRes++;
                }
            }
            return res;

        }

        #region - Holetckyy Method -
        public Matrix Holetckyy()
        {
            Matrix U = new Matrix(n, m);
            if (m == n)
            {
                for (int i = 0; i < n; i++)
                {
                    double sum = 0;
                    for (int k = 0; k < i; k++)
                        sum += U[k, i] * U[k, i];

                    double d = matrix[i, i] - sum;

                    if (d < 0.0000001)          //мало би бути по модулю, але не нах. тоді залежних рядків;
                        d = 0;
                    U[i, i] = Math.Sqrt(d);

                    if (U[i, i] != 0)
                    {
                        for (int j = i + 1; j < m; j++)
                        {
                            sum = 0;
                            for (int k = 0; k < i; k++)
                                sum += U[k, i] * U[k, j];
                            U[i, j] = (matrix[i, j] - sum) / U[i, i];
                        }
                    }

                }

                return U;
            }
            else
                throw new Exception(" usage of non quadrad matrix: ");

            //return U;
        }

        

        public Vector LinSolveLtriang(Vector b)
        {
            Vector x = new Vector(n);

            if (b.Length == n)
            {
                for (int i = 0; i < n; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < i; j++)
                        sum += matrix[i, j] * x[j];

                    x[i] = (b[i] - sum) / matrix[i, i];
                }

                return x;
            }
            else
            { 
                throw new Exception(" wrong input parameters: "); 
            }
        }

        public Vector LinSolveRtriang(Vector b)
        {
            Vector x = new Vector(n);
            if (b.Length == n)
            {
                for (int i = n - 1; i >= 0; i--)
                {
                    double sum = 0;
                    for (int j = i + 1; j < m; j++)
                        sum += matrix[i, j] * x[j];

                    x[i] = (b[i] - sum) / matrix[i, i];
                }

                return x;
            }
            else
            { 
                throw new Exception(" wrong input parameters: "); 
            }
        }

        public Vector SolveHol(Vector b)
        {
            Matrix triang = Holetckyy();
            Vector res = new Vector(n);

            if (n == b.Length)
            {
                //res = new Matrix(triang.LinSolveRtriang(Matrix.Transpose(triang).LinSolveLtriang(b)));
            }

            return res;
        }
        #endregion

    }
    #endregion
}
