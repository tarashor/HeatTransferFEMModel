using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Triangulation;
using System.IO;
using FEMLibrary;

namespace FEMprog
{
    public partial class FrmMain : Form
    {
        ProgramMode mode;
        Domain domain;
        Model model;
        Vector u;
        GeneratingMesh generatingMesh;
        Polygon polygon;
        Graphics g;
        Bitmap btm;
        Config config;
        Pallette pallette;
        FrmBoundary frm;
        
        public FrmMain()
        {
            InitializeComponent();
            InitializeLocalVariable();
        }

        private void InitializeLocalVariable()
        {
            DrawMode();
            GraphicFunctions.a = 0;
            GraphicFunctions.b = 10;
            GraphicFunctions.c = 0;
            GraphicFunctions.d = 10;
            GraphicFunctions.w = pictureBox1.Width;
            GraphicFunctions.h = pictureBox1.Height;

            domain = new Domain();
            ClearWindow();
            polygon = new Polygon();
            config = new Config();
            u = null;
            pallette = null;
            textBox1.Text = "";

            mode = ProgramMode.DrawMode;
            Matrix k = new Matrix(2, 2);
            k[0, 0] = 4;
            k[0, 1] = 0;
            k[1, 0] = 0;
            k[1, 1] = 3;
            model = new Model(k, 0, 1);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnAddPoint_Click(object sender, EventArgs e)
        {
            if (mode == ProgramMode.DrawMode)
            {
                FrmAddPoint frm = new FrmAddPoint(polygon);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        polygon = frm.ListNodes;
                        DrawVertexs();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Write correct coordinates" + ex.Message);
                    }
                }
            }
        }

        

        private void btnMesh_Click(object sender, EventArgs e)
        {
            toolStripProgressBar1.Value = 0;
            generatingMesh = new GeneratingMesh(polygon, config.Quality);
            toolStripProgressBar1.Value = 30;
            generatingMesh.eps = 0.000000000000001;
            domain.mesh = generatingMesh.GenerateGrid();
            toolStripProgressBar1.Value = 85;
            domain.mesh.RenumberNodes();
            toolStripProgressBar1.Value = 100;

            textBox1.Text = "Triangles:   " + domain.mesh.triangles.Count + "\r\n";
            textBox1.Text += "Points:   " + domain.mesh.nodes.Count + "\r\n";
            toolStripProgressBar1.Value = 0;
            
            MeshMode();
        }

        private void btnClosePolygon_Click(object sender, EventArgs e)
        {
            DomainMode();
            domain.boundary = new Boundary(polygon.Count);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (mode == ProgramMode.DrawMode)
            {
                Point point = new Point(e.X, e.Y);
                Node node = GraphicFunctions.ConvertToOwn(point);
                polygon.AddVertex(node);
                GraphicFunctions.DrawNode(node, g, false, 0);
                pictureBox1.Image = btm;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Point point = new Point(e.X, e.Y);
            Node node = GraphicFunctions.ConvertToOwn(point);
            toolStripStatusLabel1.Text = "[ " + node.ToString() + " ]";
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InitializeLocalVariable();
        }

        private void ClearWindow()
        {
            btm = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(btm);
            pictureBox1.Image = btm;
            
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mode == ProgramMode.DomainMode)
            {
                SaveFileDialog savedlg = new SaveFileDialog();
                savedlg.Filter = "Input files(*.iff)|*.iff";
                savedlg.InitialDirectory = "files";
                if (savedlg.ShowDialog() == DialogResult.OK)
                {
                    string fname = savedlg.FileName;
                    using (StreamWriter sw = new StreamWriter(fname))
                    {
                        Saver.SaveInputData(sw, polygon, model, domain.boundary);
                    }

                }
            }
        }


        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog opendlg = new OpenFileDialog();
            opendlg.Filter = "Input files(*.iff)|*.iff";
            opendlg.InitialDirectory = "files";
            if (opendlg.ShowDialog() == DialogResult.OK)
            {
                string fname = opendlg.FileName;
                try
                {
                    using (StreamReader sr = new StreamReader(fname))
                    {
                        Opener.OpenInputData(sr, out polygon, out model, out domain.boundary);
                        DomainMode();
                    }
                }
                catch(Exception ex)
                {
                    domain.mesh = new Mesh();
                    MessageBox.Show(ex.Message);
 
                }
            }
        }

        

        private void meshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmMeshOption frmMeshOption = new FrmMeshOption(config);
            if (frmMeshOption.ShowDialog() == DialogResult.OK)
            {
                config = frmMeshOption.MeshConfig;
            }

        }

        private void btnSolve_Click(object sender, EventArgs e)
        {
            u = PDESolver.Solve(model.K, model.D, model.F, domain);
            textBox1.Text += "Min : " + u.Min() + "\r\n";
            textBox1.Text += "Max : " + u.Max() + "\r\n";
            ResultMode();
        }

        private void DrawMode()
        {
            mode = ProgramMode.DrawMode;
            saveResultAsImageToolStripMenuItem.Enabled = false;
            saveResultToolStripMenuItem.Enabled = false;
            btnAddPoint.Enabled = true;
            btnMesh.Enabled = false;
            btnSolve.Enabled = false;
            if (polygon != null)
                DrawVertexs();
        }

        private void MeshMode()
        {
            mode = ProgramMode.MeshMode;
            saveResultAsImageToolStripMenuItem.Enabled = false;
            saveResultToolStripMenuItem.Enabled = false;
            btnAddPoint.Enabled = false;
            btnMesh.Enabled = true;
            btnSolve.Enabled = true;

            DrawMesh();

        }
        private void ResultMode()
        {
            mode = ProgramMode.MeshMode;
            saveResultAsImageToolStripMenuItem.Enabled = true;
            saveResultToolStripMenuItem.Enabled = true;
            btnAddPoint.Enabled = false;
            btnMesh.Enabled = true;
            btnSolve.Enabled = true;

            DrawResult();
        }

        private void DomainMode()
        {
            mode = ProgramMode.DomainMode;
            saveResultAsImageToolStripMenuItem.Enabled = false;
            saveResultToolStripMenuItem.Enabled = false;
            btnAddPoint.Enabled = false;
            btnMesh.Enabled = true;
            btnSolve.Enabled = false;

            DrawDomain();
        }

        private void DrawVertexs()
        {
            ClearWindow();
            GraphicFunctions.DrawNodes(polygon.ToArray(), g, false);
            pictureBox1.Image = btm;
        }

        private void DrawResult()
        {
            ClearWindow();
            pallette = new Pallette(u.Min(), u.Max());
            foreach (Triangle tr in domain.mesh.triangles)
            {
                GraphicFunctions.FillTriangle(g, domain.mesh.nodes[tr.IndexA], domain.mesh.nodes[tr.IndexB], domain.mesh.nodes[tr.IndexC],
                     u[tr.IndexA], u[tr.IndexB], u[tr.IndexC], pallette);
            }
            pictureBox1.Image = btm;
        }

        private void DrawDomain()
        {
            ClearWindow();
            GraphicFunctions.DrawNodes(polygon.ToArray(), g, false);
            GraphicFunctions.DrawPolygon(polygon.ToArray(), g);
            pictureBox1.Image = btm;
        }

        private void DrawMesh()
        {
            ClearWindow();
            GraphicFunctions.DrawGrid(domain.mesh, g);
            if (config.HasAdditionalPoint)
            {
                GraphicFunctions.DrawNodes(domain.mesh.nodes.ToArray(), g, config.IsNumberedNode);
            }
            pictureBox1.Image = btm;
        }
        

        private void domainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DomainMode();
            FrmDomain frm = new FrmDomain();
            frm.Parameters = model;
            if (frm.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    model = frm.Parameters;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Write correct info" + ex.Message);
                }
            }
        }

        private void boundaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DomainMode();
            frm = new FrmBoundary(domain.boundary);
            frm_OnListBox1Changed();
            frm.OnListBox1Changed += new MyDelegateFunc(frm_OnListBox1Changed);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    domain.boundary = frm.boundary;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Write correct info" + ex.Message);
                }
            }

            DrawDomain();
        }

        void frm_OnListBox1Changed()
        {
            DrawDomain();
            GraphicFunctions.DrawPolygonEdge(polygon, frm.curSelected, g);
            pictureBox1.Image = btm;
        }

        private void btnDrawMode_Click(object sender, EventArgs e)
        {
            DrawMode();
        }

        private void saveResultAsImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog savedlg = new SaveFileDialog();
            savedlg.Filter = "Bitmap (*.bmp)|*.bmp";
            savedlg.InitialDirectory = "files";
            if (savedlg.ShowDialog() == DialogResult.OK)
            {
                string fname = savedlg.FileName;
                btm.Save(fname);

            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrnAbout frmAbout = new FrnAbout();
            frmAbout.Show();
        }


        private void matrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmMatrixViewer frmMatrixViewer = new FrmMatrixViewer(domain, model);
            frmMatrixViewer.Show();

        }



        
    }
}
