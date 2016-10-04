using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Triangulation;

namespace FEMprog
{
    public delegate void MyDelegateFunc();

    public partial class FrmBoundary : Form
    {
        BoundaryCharacteristic[] listBoundary;
        public int curSelected;

        public event MyDelegateFunc OnListBox1Changed;

        public FrmBoundary(Boundary bound)
        {
            InitializeComponent();

            listBoundary = bound.characteristic;
            
            comboBox1.Items.Add(Enum.GetName(typeof(BoundaryCondition), 0));
            comboBox1.Items.Add(Enum.GetName(typeof(BoundaryCondition), 1));
            comboBox1.Items.Add(Enum.GetName(typeof(BoundaryCondition), 2));
            for (int i = 0; i < listBoundary.Length; i++)
                listBox1.Items.Add(i);
            curSelected = 0;

            Open(curSelected);
            listBox1.SelectedIndex = curSelected;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public Boundary boundary
        {
            get 
            {
                Save(curSelected);
                return new Boundary(listBoundary);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Save(curSelected);
            curSelected = listBox1.SelectedIndex;
            Open(curSelected);
            if (OnListBox1Changed != null)
                OnListBox1Changed();
            
        }

        private void Save(int i)
        {
            BoundaryCharacteristic bc = new BoundaryCharacteristic();
            bc.Delta = double.Parse(textBox1.Text);
            bc.Beta = double.Parse(textBox2.Text);
            bc.UC = double.Parse(textBox3.Text);
            bc.U0 = double.Parse(textBox4.Text);
            bc.Condition = (BoundaryCondition)comboBox1.SelectedIndex;
            listBoundary[i] = bc;
        }

        private void Open(int i)
        {
            textBox1.Text = listBoundary[i].Delta.ToString();
            textBox2.Text = listBoundary[i].Beta.ToString();
            textBox3.Text = listBoundary[i].UC.ToString();
            textBox4.Text = listBoundary[i].U0.ToString();
            comboBox1.SelectedItem = listBoundary[i].Condition.ToString();
            
        }

    }
}
