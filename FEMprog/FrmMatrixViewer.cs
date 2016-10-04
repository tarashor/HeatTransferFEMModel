using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FEMLibrary;
using Triangulation;

namespace FEMprog
{
    public partial class FrmMatrixViewer : Form
    {
        public FrmMatrixViewer()
        {
            InitializeComponent();
        }

        Domain domain;
        Model model;

        public FrmMatrixViewer(Domain d, Model m)
        {
            InitializeComponent();
            domain = d;
            model = m;
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Matrix m = new Matrix(3, 3);
            Vector v = new Vector(3);
            int elementNumber = 0;
            try
            {
                elementNumber = int.Parse(textBox1.Text);
                if (checkBox1.Checked)
                {
                    m += PDESolver.GetStiffnessMatrix(model.K, model.D, model.F, domain, elementNumber);
                    v += PDESolver.GetVector(model.K, model.D, model.F, domain, elementNumber);
                }
                if (checkBox2.Checked)
                    m += PDESolver.GetMassMatrix(model.K, model.D, model.F, domain, elementNumber);
                if (checkBox3.Checked)
                {
                    m += PDESolver.GetBoundaryMatrix(model.K, model.D, model.F, domain, elementNumber);
                    v += PDESolver.GetBoundaryVector(model.K, model.D, model.F, domain, elementNumber);
                }

                PrintMatrix(m);
                PrintVector(v);
            }
            catch
            {
                MessageBox.Show("Write correct element number!!!!");
            }

            
            
        }

        private void PrintMatrix(Matrix m)
        {
            dataGridView1.RowCount = 0;
            dataGridView1.ColumnCount = m.CountColumns;

            for (int i = 0; i < m.CountRows; i++)
            {
                List<string> row = new List<string>();
                for (int j = 0; j < m.CountColumns; j++)
                    row.Add(m[i, j].ToString("0.0000000000"));

                dataGridView1.Rows.Add(row.ToArray());
            }

            

        }

        private void PrintVector(Vector v)
        {
            dataGridView2.RowCount = 0;
            dataGridView2.ColumnCount = v.Length;

            List<string> row = new List<string>();
            for (int j = 0; j < v.Length; j++)
                row.Add(v[j].ToString("0.0000000000"));

            dataGridView2.Rows.Add(row.ToArray());
        }

    }
}
