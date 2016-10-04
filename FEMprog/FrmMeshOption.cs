using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FEMprog
{
    public partial class FrmMeshOption : Form
    {
        public FrmMeshOption()
        {
            InitializeComponent();
        }

        public FrmMeshOption(Config cfg)
        {
            InitializeComponent();
            textBox1.Text = cfg.Quality.ToString();
            checkBox1.Checked = cfg.IsNumberedNode;
            checkBox2.Checked = cfg.HasAdditionalPoint;
        }

        public Config MeshConfig
        {
            get 
            {
                Config cfg = new Config(int.Parse(textBox1.Text), checkBox1.Checked, checkBox2.Checked);
                return cfg;
            }
            set 
            {
                textBox1.Text = value.Quality.ToString();
                checkBox1.Checked = value.IsNumberedNode;
                checkBox2.Checked = value.HasAdditionalPoint;
            }
        }

        

        

    }
}
