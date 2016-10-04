using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Triangulation;

namespace FEMLibrary
{
    #region - Vector -
    public class Vector
    {
        private double[] vector;
        private int n; // kilkist elementiv

        public Vector(int length)
        {
            vector = new double[length];
            n = length;
        }
        public Vector(Vector v)
        {
            this.n = v.n;
            vector = new double[this.n];
            for (int i = 0; i < this.n; i++)
            {
                vector[i] = v[i];
            }
        }

        #region - Properties -
        public int Length
        {
            get { return n; }
        }

        public double this[int i]
        {
            get
            {
                if ((0 <= i) && (i <= n))
                    return vector[i];
                else throw new IndexOutOfRangeException();
            }
            set
            {
                if ((0 <= i) && (i <= n))
                    vector[i] = value;
                else throw new IndexOutOfRangeException();
            }
        }

        #endregion


        public override string ToString()
        {
            string str_vector = "{";
            for (int i = 0; i < n; i++)
            {
                str_vector += vector[i].ToString("00.000") + "   ";
            }
            str_vector += "}";
            return str_vector;
        }

        public double[] ToArray()
        {
            return vector;
        }

        #region - Reload operation -
        public static Vector operator +(Vector v1, Vector v2)
        {
            if (v1.Length == v2.Length)
            {
                Vector res = new Vector(v1.Length);
                for (int i = 0; i < v1.Length; i++)
                {
                    res[i] = v1[i] + v2[i];
                }
                return res;
            }
            else
            {
                throw new Exception("Sum of vector cannot be count!!!");
            }
        }

        public static Vector operator -(Vector v1, Vector v2)
        {
            if (v1.Length == v2.Length)
            {
                Vector res = new Vector(v1.Length);
                for (int i = 0; i < v1.Length; i++)
                {
                    res[i] = v1[i] - v2[i];
                }
                return res;
            }
            else
            {
                throw new Exception("Sum of vector cannot be count!!!");
            }
        }

        public static Vector operator *(double c, Vector v)
        {
            Vector res = new Vector(v.Length);
            for (int i = 0; i < v.Length; i++)
            {
                res[i] = c * v[i];
            }
            return res;
        }
        #endregion

        public static double Norm(Vector v)
        {
            double sum = 0;
            for (int i = 0; i < v.Length; i++)
            {
                sum += v[i] * v[i];
            }
            return Math.Sqrt(sum);
        }

        public double Max()
        {
            double max = vector[0];
            for (int i = 0; i < n; i++)
            {
                if (max < vector[i])
                    max = vector[i];
            }
            return max;

        }
        public double Min()
        {
            double min = vector[0];
            for (int i = 0; i < n; i++)
            {
                if (min > vector[i])
                    min = vector[i];
            }
            return min;
        }

        public static Vector DecreaseVector(Vector vector, List<int> indexDelete)
        {
            Vector res = new Vector(vector.n - indexDelete.Count);
            int iRes = 0;
            for (int i = 0; i < vector.n; i++)
            {
                if (indexDelete.FindItem(i) == -1)
                {
                    res[iRes] = vector[i];
                    iRes++;
                }
            }
            return res;

        }
    }
    #endregion
}
