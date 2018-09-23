using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SharpGL;
using metaball;

namespace metaballGL
{
    /// <summary>
    /// The main form class.
    /// </summary>
    public partial class SharpGLForm : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SharpGLForm"/> class.
        /// </summary>

        private Metaball TestBall;
        private Metaball TestBall2;
        private Metaball TestBall3;
        private Metaball TestBall4;
        private Metaball TestBall5;

        private MetaballEngine Engine;
        
        public SharpGLForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RenderEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, RenderEventArgs e)
        {
            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            //  Clear the color and depth buffer.
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            //  Load the identity matrix.
            gl.LoadIdentity();
            gl.PointSize(2.0f);
            Engine.CalculateFrame();
            gl.Begin(OpenGL.GL_POINTS);           
            for (int i = 0; i < Engine.World.Length / Engine.World.GetLength(0); i++)
                for (int j = 0; j < Engine.World.GetLength(0); j++)
                    if ((Engine.World[i, j].R + Engine.World[i, j].G + Engine.World[i, j].B) / 3 > Engine.Threshold)
                    {
                        gl.Color(Engine.World[i, j].R, Engine.World[i, j].G, Engine.World[i, j].B);
                        gl.Vertex(j, i);
                    }
            gl.End();
            
        }



        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, EventArgs e)
        {
            //  TODO: Initialise OpenGL here.

            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            //  Set the clear color.
            gl.ClearColor(0, 0, 0, 0);

            System.Windows.Media.Color clr =System.Windows.Media.Color.FromRgb(255,0,0);
            System.Windows.Media.Color clr2 = System.Windows.Media.Color.FromRgb(0, 255, 0);
            System.Windows.Media.Color clr3 = System.Windows.Media.Color.FromRgb(0, 0, 255);
            System.Windows.Media.Color clr4 = System.Windows.Media.Color.FromRgb(255, 255, 0);
            System.Windows.Media.Color clr5 = System.Windows.Media.Color.FromRgb(0, 255, 255);
            Engine = new MetaballEngine(300, 30);
            TestBall = new Metaball(151, 200,0,clr,6,95,Engine);
            TestBall2 = new Metaball(121, 50, 0, clr2, 3, 270, Engine);
            TestBall3 = new Metaball(151, 100, 0, clr3, 6, 135, Engine);
            TestBall4 = new Metaball(191, 100, 0, clr4, 9, 45, Engine);
            TestBall5 = new Metaball(191, 100, 0, clr5, 9, 75, Engine);


            Engine.AddMetaball(TestBall);
            Engine.AddMetaball(TestBall2);
            Engine.AddMetaball(TestBall3);
            Engine.AddMetaball(TestBall4);
            Engine.AddMetaball(TestBall5);
            
            //Engine.CalculateFrame();
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, EventArgs e)
        {
            //  TODO: Set the projection matrix here.

            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            //  Set the projection matrix.
            gl.MatrixMode(OpenGL.GL_PROJECTION);

            //  Load the identity.
            gl.LoadIdentity();

            gl.Ortho2D(0, 300, 0, 300);

            //  Set the modelview matrix.
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        private void SharpGLForm_Load(object sender, EventArgs e)
        {

        }

       
    }
}
