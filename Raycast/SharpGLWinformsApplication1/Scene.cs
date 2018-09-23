using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Raycasting
{
    class Scene
    {
        private Camera cameraPlayer;
        private Viewport vPort;
        private Map level;
        private int distFromVp;
        private int numOfRays;
        private double angleBetweenRay;      //angle between each ray

        private double degToRad(double degree)
        {
            return degree * Math.PI / 180;
        }

        public Scene(int cameraX, int cameraY, int fov, int angle, int vportWidth, int vportHeight, int brickWidth, int brickHeight,int[,] chart)
        {
            cameraPlayer = new Camera(cameraX, cameraY, fov, angle);
            vPort = new Viewport(vportWidth, vportHeight);

            //calculate distance to viewport
            distFromVp = (int)((vPort.Width / 2) / Math.Tan(degToRad(cameraPlayer.Fov / 2)));
            numOfRays = vportWidth;  //number of rays is the same as width of viewport
            angleBetweenRay = (double)fov / (numOfRays-1);
            level = new Map(brickWidth, brickHeight, chart);

        }

        public double getAngle()
        {
            return cameraPlayer.Angle;
        }

        public void moveForward()
        {
            int step = 10;
            double angle = cameraPlayer.Angle;
            if (angle >= 0 && angle < 90)
            {
                cameraPlayer.PosX += (int)(-Math.Cos(degToRad(angle)) * step);
                cameraPlayer.PosY += (int)(-Math.Sin(degToRad(angle)) * step);
            }
            else if (angle>=90 && angle<180)
            {
                cameraPlayer.PosX += (int)(Math.Cos(degToRad(180-angle)) * step);
                cameraPlayer.PosY += (int)(-Math.Sin(degToRad(180-angle)) * step);
            }
            else if (angle>=180 && angle<270)
            {
                cameraPlayer.PosX += (int)(Math.Cos(degToRad(angle-180)) * step);
                cameraPlayer.PosY += (int)(Math.Sin(degToRad(angle-180)) * step);
            }
            else if (angle>=270 && angle<360)
            {
                cameraPlayer.PosX += (int)(-Math.Cos(degToRad(360-angle)) * step);
                cameraPlayer.PosY += (int)(Math.Sin(degToRad(360-angle)) * step);
            }
            
        }

        public void moveBackWard()
        {

        }

        public void incAngle()
        {
            cameraPlayer.Angle+=4;
            if (cameraPlayer.Angle > 359) cameraPlayer.Angle -= 360;
        }

        public void decAngle()
        {
            cameraPlayer.Angle-=4;
            if (cameraPlayer.Angle < 0) cameraPlayer.Angle += 360;
        }

        private int calculateSliceHeight(double dist)
        {
            //projected slice height=actual (slice height/distance to the slice)*distance from viewport
            return (int)((level.Height/dist)*distFromVp);
        }

        public List<Tuple<int,int>> calculateFrame()
        {
            //Console.WriteLine("x:{0}, y:{1}", cameraPlayer.PosX, cameraPlayer.PosY);
            List<Tuple<int,int>> slices = new List<Tuple<int,int>>(vPort.Width);
            for (int rays = 0; rays < vPort.Width; rays += 1)
            
            {
                //Console.WriteLine("ray num: {0}",rays);
                Tuple<int, double> horWall = findHorizontal(rays);
                //Console.WriteLine("vertical");
                Tuple<int, double> vertWall = findVertical(rays);
                Tuple<int, double> closetWall;
                if (horWall.Item2 == 0) closetWall = vertWall;
                else if (vertWall.Item2 == 0) closetWall = horWall;
                else if (horWall.Item2 > vertWall.Item2)closetWall = vertWall;
                else closetWall = horWall;
                Tuple<int, int> tempData = new Tuple<int, int>(closetWall.Item1, calculateSliceHeight(closetWall.Item2));
                slices.Add(tempData);
                //Console.WriteLine("hor dis: {0}, vert dis: {1}, nearest: {2}",horWall.Item2,vertWall.Item2,closetWall.Item2);
            }
            return slices;
        }


        private Tuple<int, double> findHorizontal(int iteration)
        {
            
            
            int xIndex, yIndex;

            double angle = calcLocalRayAngle(iteration);
            double dx;
            double dy;
            

            int offset; //check if we hit grid from top or bottom
            if (angle < 180)
            {
                offset = -1;
                dy =  -cameraPlayer.PosY % level.Height;
            }
            else
            {
                offset = 0;
                dy = level.Height - cameraPlayer.PosY % level.Height;
            }
            dy += offset;

            
            if (angle == 0 || angle == 180) return new Tuple<int, double>(0,0.0); //no horizontal intersection
            if (angle == 90 || angle==270) dx = 0;
            else if (angle > 0 && angle < 90) dx = (dy / Math.Tan(degToRad(angle))); //dy<0 we get dx<0, tan>0
            else if (angle > 90 && angle < 180) dx = -(dy / Math.Tan(degToRad(180 - angle))); //dy<0, tan>0, so -dx>0
            else if (angle > 180 && angle < 270) dx = (dy / Math.Tan(degToRad(angle - 180))); //dy>0, tan>0, we get dx>0
            else dx = (int)(-dy / Math.Tan(degToRad(360 - angle)));

            //Console.WriteLine("dx: {0}, dy: {1}", dx, dy);
           
            
            double x = dx + cameraPlayer.PosX;
            double y = dy + cameraPlayer.PosY;

            //Console.WriteLine("x: {0}, y: {1}",x,y);

            if (x < 0 || y < 0) return new Tuple<int,double>(0,0.0);
            if (x > level.Width * level.Chart.GetLength(0)-1 || y > level.Height * level.Chart.Length/level.Chart.GetLength(0)-1) return new Tuple<int, double>(0, 0.0);
            
            xIndex = (int)(x / level.Width);
            yIndex = (int)(y / level.Height);

            //x = (int)x;
            //y = (int)y;
            
            //Console.WriteLine("yIndex: {0}, xIndex: {1}",yIndex,xIndex);
            if (level.Chart[yIndex, xIndex] == 0)
            {
                if (angle < 180) dy = -level.Height;
                else dy = level.Height;

                if (angle == 0 || angle == 180) return new Tuple<int, double>(0, 0.0); //no horizontal intersection
                if (angle == 90 || angle == 270) dx = 0;
                else if (angle > 0 && angle < 90) dx = (dy / Math.Tan(degToRad(angle))); //dy<0 we get dx<0, tan>0
                else if (angle > 90 && angle < 180) dx = -(dy / Math.Tan(degToRad(180 - angle))); //dy<0, tan>0, so -dx>0
                else if (angle > 180 && angle < 270) dx = (dy / Math.Tan(degToRad(angle - 180))); //dy>0, tan>0, we get dx>0
                else dx = (-dy / Math.Tan(degToRad(360 - angle)));

                //Console.WriteLine("dx: {0}, dy: {1}", dx, dy);

                x += dx;
                y += dy;

                //Console.WriteLine("x: {0}, y: {1}", x, y);

                if (x < 0 || y < 0) return new Tuple<int, double>(0, 0.0);
                if (x > level.Width * level.Chart.GetLength(0) - 1 || y > level.Height * level.Chart.Length / level.Chart.GetLength(0) - 1) return new Tuple<int, double>(0, 0.0);

                xIndex = (int)(x / level.Width);
                yIndex = (int)(y / level.Height);


                while (level.Chart[yIndex, xIndex] == 0)
                {
                    x += dx;
                    y += dy;

                    //Console.WriteLine("x: {0}, y: {1}", x, y);

                    if (x < 0 || y < 0) return new Tuple<int, double>(0, 0.0);
                    if (x > level.Width * level.Chart.GetLength(0) - 1 || y > level.Height * level.Chart.Length / level.Chart.GetLength(0) - 1) return new Tuple<int, double>(0, 0.0);

                    xIndex = (int)(x / level.Width);
                    yIndex = (int)(y / level.Height);
                    // x = (int)x;
                    //y = (int)y;


                    //Console.WriteLine("yIndex: {0}, xIndex: {1}", yIndex, xIndex);
                }
            }
            double distance = Math.Sqrt(((int)x - cameraPlayer.PosX) * ((int)x - cameraPlayer.PosX) + ((int)y - cameraPlayer.PosY) * ((int)y - cameraPlayer.PosY));
            distance *= Math.Cos(degToRad(Math.Abs(angle-cameraPlayer.Angle)));        //project distance on viewport;no fish eye effect
            return new Tuple<int, double>(level.Chart[yIndex, xIndex], distance);
        }

        private Tuple<int,double> findVertical(int iteration)
        {
            int xIndex, yIndex;
            double dx;
            double dy;
            double angle = calcLocalRayAngle(iteration);
            int offset; //check if we hit grid from left or right
      
            if (angle< 90 || angle > 270)
            {
                offset = -1;
                dx = -cameraPlayer.PosX % level.Width;
            }
            else
            {
                offset = 0;
                dx= level.Width - cameraPlayer.PosX % level.Width;
            }

            dx += offset;
            
            if (angle == 90 || angle==270) return new Tuple<int,double>(0,0.0); //no vertical intersection
            if (angle == 0 || angle == 180) dy = 0;
            else if (angle < 90) dy = (dx * Math.Tan(degToRad(angle)));
            else if (angle > 90 && angle < 180) dy = -(dx * Math.Tan(degToRad(180 - angle)));
            else if (angle > 180 && angle < 270) dy = (dx * Math.Tan(degToRad(angle - 180))); //dy>0, tan>0, we get dx>0
            else dy = -(dx * Math.Tan(degToRad(360 - angle)));
            double x = dx + cameraPlayer.PosX;
            double y = dy + cameraPlayer.PosY;
            if (x < 0 || y < 0) return new Tuple<int,double>(0,0.0);
            if (x > level.Width * level.Chart.GetLength(0) - 1 || y > level.Height * level.Chart.Length / level.Chart.GetLength(0) - 1) return new Tuple<int, double>(0, 0.0);

            //Console.WriteLine("dx: {0}, dy: {1}", dx, dy);
            //Console.WriteLine("x: {0}, y: {1}", x, y);

            xIndex =(int)( x / level.Width);
            yIndex = (int)(y / level.Height);

            //x = (int)x;
            //y = (int)y;
                        
            //Console.WriteLine("yIndex: {0}, xIndex: {1}", yIndex, xIndex);
            if ((level.Chart[yIndex, xIndex] == 0))
            { 
                if (angle<90 || angle>270) dx = -level.Width;
                else dx = level.Width;
                
                if (angle == 90 || angle == 270) return new Tuple<int, double>(0, 0.0); //no vertical intersection
                if (angle == 0 || angle == 180) dy = 0;
                else if (angle < 90) dy = (dx * Math.Tan(degToRad(angle)));
                else if (angle > 90 && angle < 180) dy = -(dx * Math.Tan(degToRad(180 - angle)));
                else if (angle > 180 && angle < 270) dy = (dx * Math.Tan(degToRad(angle - 180))); //dy>0, tan>0, we get dx>0
                else dy = -(dx * Math.Tan(degToRad(360 - angle)));

                //Console.WriteLine("dx: {0}, dy: {1}", dx, dy);

                x += dx;
                y += dy;

                //Console.WriteLine("x: {0}, y: {1}", x, y);

                if (x < 0 || y < 0) return new Tuple<int, double>(0,0.0);
                if (x > level.Width * level.Chart.GetLength(0) - 1 || y > level.Height * level.Chart.Length / level.Chart.GetLength(0) - 1) return new Tuple<int, double>(0, 0.0);
               
                xIndex = (int)(x / level.Width);
                yIndex = (int)(y / level.Height);

                while (level.Chart[yIndex, xIndex] == 0)
                {
                    x += dx;
                    y += dy;

                    //Console.WriteLine("x: {0}, y: {1}", x, y);

                    if (x < 0 || y < 0) return new Tuple<int, double>(0, 0.0);
                    if (x > level.Width * level.Chart.GetLength(0) - 1 || y > level.Height * level.Chart.Length / level.Chart.GetLength(0) - 1) return new Tuple<int, double>(0, 0.0);

                    xIndex = (int)(x / level.Width);
                    yIndex = (int)(y / level.Height);
                }
                //x = (int)x;
               // y = (int)y;
            
                //Console.WriteLine("yIndex: {0}, xIndex: {1}", yIndex, xIndex);
            }

            double distance = Math.Sqrt(((int)x - cameraPlayer.PosX) * ((int)x - cameraPlayer.PosX) + ((int)y - cameraPlayer.PosY) * ((int)y - cameraPlayer.PosY));
            distance *= Math.Cos(degToRad(Math.Abs(angle - cameraPlayer.Angle)));
            return new Tuple<int, double>(level.Chart[yIndex, xIndex], distance);
        }

        private double calcLocalRayAngle(int iteration)
        {
            double angle = cameraPlayer.Angle - cameraPlayer.Fov/2 + iteration * angleBetweenRay;
            if (angle < 0) angle = 360 + angle;
            else if (angle > 359) angle %= 360;
            return angle;
        }
    }
}
