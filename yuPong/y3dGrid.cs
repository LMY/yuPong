using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
//using System.IO;


namespace yuPong
{
    class y3dGrid : Object
    {
        private int grid_play; // = 1;

        private int gridnX;
        private int gridnY;

        private float gridS;
        private float[,] grid;
        private float[,] grid_speed;
        private float grid_height;              //=1;
        private float gridCr, gridCg, gridCb;   // 0.1, 0.3, 0.7
        private int gridType;	                // 0:wave, 1:histo(default)


        public int GridnX { get { return gridnX; } }
        public int GridnY { get { return gridnY; } }

        public int GridType     { get { return gridType; } set { gridType = value; } }
        public float SizeX { get { return gridnX* gridS; } }
        public float SizeY { get { return gridnY * gridS; } }


        public float getGrid(int y, int x)
        { return grid[y,x]+Center.Z; }
        
        public void setGrid(int y, int x, float h)
        { grid[y,x] = h; }


        public void toggleGridType()
        { gridType = (gridType == 0 ? 1 : 0); }




        public y3dGrid(int nnodes, float spacing)
            : base(Vector3.Zero)
        { init(nnodes, nnodes, spacing); }

        public y3dGrid(Vector3 c, int nnodes, float spacing)
            : base(c)
        { init(nnodes, nnodes, spacing); }


        public y3dGrid(int xnodes, int ynodes, float spacing)
            : base(Vector3.Zero)
        { init(xnodes, ynodes, spacing); }

        public y3dGrid(Vector3 c, int xnodes, int ynodes, float spacing)
            : base(c)
        { init(xnodes, ynodes, spacing); }

        private void init(int xnodes, int ynodes, float spacing)
        {
            grid_play = 1;
            grid = null;
            grid_speed = null;
            grid_height = 1;

            gridCr = 0.2f;
            gridCg = 0.2f;
            gridCb = 0.7f;

            gridType = 1;

            gridnX = xnodes;
            gridnY = ynodes;
            gridS = spacing;

            grid = new float[gridnY, gridnX];
            grid_speed = new float[gridnY, gridnX];
            clear();
        }


        public void clear()
        {
            for (int y = 0; y < gridnY; y++)
                for (int x = 0; x < gridnX; x++)
                {
                    grid[y,x] = 0;
                    grid_speed[y,x] = 0;
                }
        }



        public void smooth()
        {
            float[,] grid1 = new float[gridnY, gridnX];

            for (int y = 0; y < gridnY; y++)
                for (int x = 0; x < gridnX; x++)
                {
                    grid1[y, x] = 0;

                    for (int py=-1; py<=+1; py++)
                        for (int px=-1; px<=+1; px++)
                            if (px + x >= 0 && px + x < gridnX && py + y >= 0 && py + y < gridnY)
                                grid1[y,x] += grid[(y+py),(x+px)];
                            else
                                grid1[y,x] += grid[y,x];

                            grid1[y,x] /= 9;
                }

            grid=grid1;
        }


        /*
        public void map()
        {
            const float h = 10.0f;
            const float n = -10.0f;
            const int dim = 10;

            float[][] map = { 	{0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
								{0, h, h, h, h, h, h, h, h, 0 },
								{0, h, 0, 0, 0, 0, 0, 0, h, 0 },
								{0, h, 0, 0, 0, 0, h, h, h, 0 },
								{0, h, h, 0, 0, 0, h, 0, 0, 0 },
								{0, 0, h, 0, 0, 0, h, h, h, h },
								{0, 0, h, h, 0, 0, 0, 0, h, n },
								{0, 0, 0, h, 0, 0, 0, h, h, n },
								{0, 0, 0, h, h, h, h, h, n, n },
								{0, 0, 0, 0, 0, h, n, n, n, n }};
	        int x,y;
	        
            int rep = gridN/dim;

	        for (y=0; y<dim; y++)
		        for (x=0; x<dim; x++)
			        if (map[y][x] == 0)
				        map[y][x] = h;
			        else if (map[y][x] == h)
				        map[y][x] = 0;

	        for (y=0; y<gridN; y++)
		        for (x=0; x<gridN; x++)
			        grid[gridN*y+x] = map[y/rep][x/rep];
	     }*/


        public void pause()
        {
            grid_play = (grid_play == 0 ? 1 : 0);
        }
        
        public new void verlet(float deltaT, float kAtt)
        {
            if (grid_play == 0)
                return;

            for (int y = 0; y < gridnY; y++)
                for (int x = 0; x < gridnX; x++)
                {
                    if (x != 0 && y != 0 && x != gridnX - 1 && y != gridnY - 1)
                    {
                        float f = 0;

                        for (int py = y - 1; py <= y + 1; py++)
                            for (int px = x - 1; px <= x + 1; px++)
                                f += (grid[py, px] - grid[y,x]);

                        grid_speed[y, x] += f * deltaT;
                        grid_speed[y, x] -= kAtt * deltaT * grid_speed[y, x];
                        grid[y, x] += grid_speed[y, x] * deltaT;
                        grid_speed[y, x] += f * deltaT;
                        grid_speed[y, x] -= kAtt * deltaT * grid_speed[y, x];
                    }
                }
        }
	


        public float getHeightAt(float x, float y)
        {
            float maxGridx = gridnX * gridS / 2;
            float maxGridY = gridnY * gridS / 2;

            if (x < -maxGridx || x > maxGridx || y < -maxGridY || y > maxGridY)
		        return 99999;

            int ix = (int)(x / gridS + gridnX / 2);
            int iy = (int)(y / gridS + gridnY / 2);

	        return getGrid(iy, ix);
        }



        public override void draw()
        {
	        const float vert_color = 0.2f;

            GL.Begin(BeginMode.Quads);

            for (int y = 0; y < gridnY; y++)
                for (int x = 0; x < gridnX; x++)
                {
                    float px = -gridS * gridnX / 2 + x * gridS;
                    float py = -gridS * gridnY / 2 + y * gridS;

                    GL.Color4(new Color4(gridCr, gridCg, gridCb, 1)); 

			        if (gridType == 0) {
                        if (x == gridnX - 1 || y == gridnY - 1)
					        continue;

				        GL.Normal3(0, -1, 0);
				        GL.Vertex3(Vector3.Add(Center, new Vector3(px, py, grid[y,x])));
				        GL.Vertex3(Vector3.Add(Center, new Vector3(px+gridS, py, grid[y,x+1])));
				        GL.Vertex3(Vector3.Add(Center, new Vector3(px+gridS, py+gridS, grid[(y+1), x+1])));
				        GL.Vertex3(Vector3.Add(Center, new Vector3(px, py+gridS, grid[(y+1), x])));
			        }
			        else if (gridType == 1) {
				        GL.Normal3(0, +1, 0);
				        GL.Vertex3(Vector3.Add(Center, new Vector3(px, py, grid[y, x])));
				        GL.Vertex3(Vector3.Add(Center, new Vector3(px+gridS, py, grid[y, x])));
				        GL.Vertex3(Vector3.Add(Center, new Vector3(px+gridS, py+gridS, grid[y, x])));
				        GL.Vertex3(Vector3.Add(Center, new Vector3(px, py+gridS, grid[y, x])));


                        if (x != gridnX - 1)
                        {
                            GL.Color4(new Color4(gridCr*vert_color, gridCg*vert_color, gridCb*vert_color, 1)); 

					        GL.Normal3(-1, 0, 0);
					        GL.Vertex3(Vector3.Add(Center, new Vector3(px+gridS, py, grid[y, x])));
					        GL.Vertex3(Vector3.Add(Center, new Vector3(px+gridS, py+gridS, grid[y, x])));
					        GL.Vertex3(Vector3.Add(Center, new Vector3(px+gridS, py+gridS, grid[y, (x+1)])));
					        GL.Vertex3(Vector3.Add(Center, new Vector3(px+gridS, py, grid[y, (x+1)])));
				        }

                        if (y != gridnY - 1)
                        {
					        GL.Color4(new Color4(gridCr*vert_color, gridCg*vert_color, gridCb*vert_color, 1)); 
			
					        GL.Normal3(0, -1, 0);
					        GL.Vertex3(Vector3.Add(Center, new Vector3(px, py+gridS, grid[y,x])));
					        GL.Vertex3(Vector3.Add(Center, new Vector3(px, py+gridS, grid[(y+1),x])));
					        GL.Vertex3(Vector3.Add(Center, new Vector3(px+gridS, py+gridS, grid[(y+1),x])));
					        GL.Vertex3(Vector3.Add(Center, new Vector3(px+gridS, py+gridS, grid[y,x])));
				        }
			        }
		        }

            GL.End();
        }

        public void random(float max_height)
        {
	        grid_height = max_height;

            for (int y = 0; y < gridnY; y++)
                for (int x = 0; x < gridnX; x++)
                    grid[y,x] = max_height * yPhysics.Random();		// (x%3)*0.2+(y%3)*0.2;
        }

        public void addnoise(float k)
        {
            for (int y = 1; y < gridnY-1; y++)
                for (int x = 1; x < gridnX-1; x++)
                    grid[y,x] += 2 * k * (yPhysics.Random() - 0.5f);
        }
    }



}
