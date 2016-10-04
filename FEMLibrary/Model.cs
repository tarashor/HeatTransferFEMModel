using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FEMLibrary
{
    public class Model
    {
        Matrix k;
        double d;
        double f;

        public Matrix K
        {
            get { return k; }
            set { k = value; }
        }

        public double D
        {
            get { return d; }
            set { d = value; }
        }

        public double F
        {
            get { return f; }
            set { f = value; }
        }

        public Model()
        {
            k = new Matrix(2, 2);
            d = 0;
            f = 0;
        }
        
        public Model(Matrix k, double d, double f)
        {
            this.k = k;
            this.d = d;
            this.f = f;
        }

    }
}
