using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Summary description for Class1
/// </summary>
/// 



/*screen orientation
 *0------------->x
 *|
 *|
 *|
 *|
 *|
 *V
 *Y
 *
 * player direction
 *      -1
 *       |
 *       |
 * -1 <--O-->+1
 *       |
 *       |
 *       1
 *       
 * angle direction
 *       |
 *      +|-
 *    +  | ->
 *0----------->
 * 
 */

namespace Raycasting
{
    class Camera
    {
	    private int posX;
        private int posY;
        private double fov;
        private int dirX;
        private int dirY;
        private double angle;

        public int PosX
        { 
            get {return posX;}
            set {posX=value;}
        }
	    public int PosY
        {
            get{return posY;}
            set{posY=value;}
        }
	    public double Fov 
        {
            get {return fov;}
        }
        public int DirX
        {
            get { return dirX;}
            set { dirX = value;}
        }
	    public int DirY
        {
            get {return dirY;}
            set {dirY=value;}
        }

        public double Angle { get { return angle; } set { angle = value; } }
	
	
	    public Camera (int cameraX, int cameraY, int fov, double angle)
        {
		    posX=cameraX;
		    posY=cameraY;
            fov = Math.Min(fov, 90); //lock fov to max 90 degree;
            this.fov = fov;//keep it in degrees, easier to loop;
		    angle = angle % 360;
            this.angle = angle;
            
	    }

    }


    class Viewport
    {
	    private int width;
        private int height;

        public int Width { get { return width; } }
        public int Height { get { return height; } }
	
	    public Viewport(int w, int h)
        {
		    width=w;
		    height=h;
	    }
    }

    class Map
    {
        private int brickWidth;
        private int brickHeight;

        private int[,] chart;

        public int[,] Chart { get { return chart; } }
        public int Width { get { return brickWidth;} }
        public int Height { get { return brickHeight; } }

        public Map(int brickWidth, int brickHeight, int[,] chart)
        {
            this.brickHeight = brickHeight;
            this.brickWidth = brickWidth;
            this.chart = chart;
        }

    }
}
