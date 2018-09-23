using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace metaball
{
    class Metaball
    {
        public Tuple<int,int> Speed {get;private set;}
        public Color[,] Body { get; private set;}
        public Tuple<int, int> Position {get; private set;}

        private MetaballEngine engine;
        
        public Metaball(int size, int x, int y, Color clr,int speed, int angle, MetaballEngine engine)
        {
            if (size<3) size=3;
            if (size%2==0) size++;   //make sure, size is not even
            Speed = new Tuple<int,int>(speed,angle%360);
            Body = new Color[size, size];
            Position = new Tuple<int, int>(x, y);
            GenerateBody(size, clr);
            this.engine = engine;
        }

        public void Move()
        {
            int dx, dy;
            int x, y;
            if (Speed.Item2<90)
            {
                dx = -(int)(Math.Cos(DegToRad(Speed.Item2)) * Speed.Item1);
                dy = -(int)(Math.Sin(DegToRad(Speed.Item2)) * Speed.Item1);
            }
            else if (Speed.Item2>=90 && Speed.Item2<180)
            {
                dx = (int)(Math.Cos(DegToRad(180-Speed.Item2)) * Speed.Item1);
                dy = -(int)(Math.Sin(DegToRad(180-Speed.Item2)) * Speed.Item1);
            }
            else if (Speed.Item2 >= 180 && Speed.Item2 < 270)
            {
                dx = (int)(Math.Cos(DegToRad(Speed.Item2-180)) * Speed.Item1);
                dy = (int)(Math.Sin(DegToRad(Speed.Item2-180)) * Speed.Item1);
    
            }
            else //angle>=270 && angle<360
            {
                dx = -(int)(Math.Cos(DegToRad(360 - Speed.Item2)) * Speed.Item1);
                dy = (int)(Math.Sin(DegToRad(360 - Speed.Item2)) * Speed.Item1);

            }

            /*if (Speed.Item2 < 90)
            {
                dx = -(int)(engine.Cos[Speed.Item2] * Speed.Item1);
                dy = -(int)(engine.Sin[Speed.Item2] * Speed.Item1);
            }
            else if (Speed.Item2 >= 90 && Speed.Item2 < 180)
            {
                dx = (int)(engine.Cos[180 - Speed.Item2] * Speed.Item1);
                dy = -(int)(engine.Sin[180 - Speed.Item2] * Speed.Item1);
            }
            else if (Speed.Item2 >= 180 && Speed.Item2 < 270)
            {
                dx = (int)(engine.Cos[Speed.Item2 - 180] * Speed.Item1);
                dy = (int)(engine.Sin[Speed.Item2 - 180] * Speed.Item1);

            }
            else //angle>=270 && angle<360
            {
                dx = -(int)(engine.Cos[360 - Speed.Item2] * Speed.Item1);
                dy = (int)(engine.Sin[360 - Speed.Item2] * Speed.Item1);

            }*/

            x = Position.Item2 + dx;
            y = Position.Item1 + dy;
            //loop coords
            if (x < 0) x += engine.World.GetLength(0);
            else if (x >= engine.World.GetLength(0)) x -= engine.World.GetLength(0);
            if (y < 0) y += engine.World.GetLength(0);
            else if (y >= engine.World.GetLength(0)) y -= engine.World.GetLength(0);


            Position = new Tuple<int, int>(y, x);
        }

        private double DegToRad(double angle)
        {
            return angle * Math.PI / 180;
        }

        
        private void GenerateBody(int size, Color clr)
        {
            //center is in size/2,size/2

            double radius;
            double fr;

            for (int i=0;i<=size/2;i++)
            {
                for (int j=0;j<=size/2;j++)
                {
                    radius = Math.Sqrt((size / 2 - i) * (size / 2 - i) + (size / 2 - j) * (size / 2 - j));
                    radius = radius / (size / 2);
                    if (radius <= 1)
                    {
                        if (radius < 1/3)
                            fr = 1 - 3 * radius * radius;
                        else
                            fr = (3 / 2) * (1 - radius) * (1 - radius);

                        Body[i, j] = Color.FromRgb((byte)(clr.R * fr), (byte)(clr.G * fr), (byte)(clr.B * fr));
                        Body[i, size - 1 - j] = Color.FromRgb((byte)(clr.R * fr), (byte)(clr.G * fr), (byte)(clr.B * fr));
                        Body[size - 1 - i, j] = Color.FromRgb((byte)(clr.R * fr), (byte)(clr.G * fr), (byte)(clr.B * fr));
                        Body[size - 1 - i, size - 1 - j] = Color.FromRgb((byte)(clr.R * fr), (byte)(clr.G * fr), (byte)(clr.B * fr));
                    }
                }
            }
        }
    }

    class MetaballEngine
    {
        public Color[,] World { get; private set; }
        public LinkedList<Metaball> MetaballList { get; private set;}
        public int Threshold { get; private set; }              //don't render cell below  this value
        public float[] Sin { get; private set; }
        public float[] Cos { get; private set; }

        public MetaballEngine(int size, int threshold)
        {
            MetaballList = new LinkedList<Metaball>();
            World = new Color[size, size];
            Threshold = threshold;
            GenerateSin();
            GenerateCos();
        }
        
        public Color[,] CalculateFrame()
        {
            int maxI, maxJ;
            int x, y;
            int posX, posY;
            int size=World.GetLength(0);
            World = new Color[size, size];
            foreach (Metaball ball in MetaballList)
            {
                ball.Move();           
                maxI = ball.Body.Length / ball.Body.GetLength(0);
                maxJ = ball.Body.GetLength(0);
                posX=ball.Position.Item2;
                posY=ball.Position.Item1;
                for (int i = 0; i < maxI; i++)
                {
                    y = i - posY;
                    if (y < 0) y += World.GetLength(0);
                    else if (y >= World.GetLength(0)) y -= World.GetLength(0);
                    for (int j = 0; j < maxJ; j++)
                    {
                        x = j - posX;
                        if (x < 0) x += World.GetLength(0);
                        else if (x >= World.GetLength(0)) x -= World.GetLength(0);
                        World[y, x] +=ball.Body[i, j];
                        //Console.WriteLine(World[y, x].R + " " + World[y, x].G + " " + World[y, x].B);
                    }
                }
            }
            return World;
        }

        public void AddMetaball(Metaball ball)
        {
            MetaballList.AddLast(ball);
        }

        private void GenerateSin()
        {
            Sin = new float[91];
            for (int i = 0; i < 91; i++)
                Sin[i] = (float)Math.Sin(DegToRad(i));
        }

        private void GenerateCos()
        {
            Cos = new float[91];
            for (int i = 0; i < 91; i++)
                Cos[i] = (float)Math.Cos(DegToRad(i));
        }

        private double DegToRad(double angle)
        {
            return angle * Math.PI / 180;
        }
    }

}
