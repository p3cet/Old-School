using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SharpGL;
using Raycasting;

namespace SharpGLWinformsApplication1
{
    /// <summary>
    /// The main form class.
    /// </summary>
    public partial class SharpGLForm : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SharpGLForm"/> class.
        /// </summary>

        private Scene Level1;
        
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

            List<Tuple<int, int>> slices;
                int counter = 0;
                slices = Level1.calculateFrame();
                foreach (Tuple<int, int> i in slices)
                {
                    switch (i.Item1)
                    {
                        case 1: gl.Color(1.0, 0.0, 0.0);
                            break;
                        case 2: gl.Color(0.0, 1.0, 0.0);
                            break;
                        case 3: gl.Color(0.0, 0.0, 1.0);
                            break;
                        default: gl.Color(1.0, 0.0, 1.0);
                            break;
                    }
                    
                    gl.Begin(OpenGL.GL_LINES);
                    gl.Vertex(counter, 160 - i.Item2 / 2);
                    gl.Vertex(counter, 160 + i.Item2 / 2);
                    gl.End();
                    counter++;
                }
                
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

            int[,] mapLevel1 =  {{1,1,1,2,1,3,2,2},
                                {1,1,1,0,0,0,0,2},
                                {1,1,0,0,0,0,0,3},
                                {2,0,0,0,0,0,0,2},
                                {2,0,0,0,0,0,0,2},
                                {2,0,0,0,2,2,0,2},
                                {2,0,0,0,0,0,0,3},
                                {2,3,3,3,3,3,3,3}};

            Level1 = new Scene(160, 240, 60, 72, 320, 200, 64, 64, mapLevel1);

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

            gl.Ortho2D(0,320,0,400);
           
            //  Set the modelview matrix.
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        private void Movement(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D) Level1.incAngle();
            else if (e.KeyCode == Keys.A) Level1.decAngle();
            else if (e.KeyCode == Keys.W) Level1.moveForward();
            label1.Text = Level1.getAngle().ToString(); 
        }
    }
}
