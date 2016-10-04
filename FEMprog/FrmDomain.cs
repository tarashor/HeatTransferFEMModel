using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FEMLibrary;

namespace FEMprog
{
    public partial class FrmDomain : Form
    {
        public FrmDomain()
        {
            InitializeComponent();
        }

        public Model Parameters
        {
            get 
            {
                Matrix k = new Matrix(2, 2);
                string[] arrayK = textBox1.Text.Split();
                k[0, 0] = double.Parse(arrayK[0]);
                k[0, 1] = double.Parse(arrayK[1]);
                k[1, 0] = double.Parse(arrayK[2]);
                k[1, 1] = double.Parse(arrayK[3]);
                return new Model(k, double.Parse(textBox2.Text), double.Parse(textBox3.Text));
            }

            set
            {
                textBox1.Text = value.K[0, 0] + " " + value.K[0, 1] + " " + value.K[1, 0] + " " + value.K[1, 1];
                textBox2.Text = value.D.ToString();
                textBox3.Text = value.F.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
