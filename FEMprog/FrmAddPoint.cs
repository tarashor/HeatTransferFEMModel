using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using Triangulation;

namespace FEMprog
{
    public partial class FrmAddPoint : Form
    {
        public FrmAddPoint(Polygon polygon)
        {
            InitializeComponent();
            
            dataGridView1.ColumnCount = 2;
            dataGridView1.Columns[0].Name = "X";
            dataGridView1.Columns[1].Name = "Y";
            dataGridView1.Rows.Clear();
            for (int i = 0; i < polygon.Count; i++)
            {
                List<string> row = new List<string>();
                row.Add(polygon[i].X.ToString());
                row.Add(polygon[i].Y.ToString());

                dataGridView1.Rows.Add(row.ToArray());
            }
        }

        public Polygon ListNodes 
        {
            get
            {
                List<Node> ln = new List<Node>();
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    ln.Add(new Node(double.Parse(dataGridView1[0, i].Value.ToString()), double.Parse(dataGridView1[1, i].Value.ToString())));
                }
                return new Polygon(ln.ToArray());
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            List<string> row = new List<string>();
            row.Add(textBox1.Text.ToString());
            row.Add(textBox2.Text.ToString());

            dataGridView1.Rows.Add(row.ToArray());
        }
    }
}
