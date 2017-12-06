using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bridge.Html5;

namespace AMazeZing
{
    public static class Main
    {
        static Game game;

        [Ready]
        public static void OnReady()
        {
            game = new Game();
        }
    }

    public class Game
    {
        HTMLCanvasElement screen;
        CanvasRenderingContext2D screenContext;
        Sprite player, goal;
        Sprite[] sprites, walls;
        int lastFrame;
        HashSet<int> keys = new HashSet<int>();
        bool gameOver = false;
        Random rdm = new Random();

        const int LEFT_KEY = 65, RIGHT_KEY = 68, UP_KEY = 87, DOWN_KEY = 83;
        const int MAZE_SIZE = 12; // We assume a square maze
        const double PLAYER_SPEED = 0.3;

        public Game()
        {
            screen = Document.GetElementById<HTMLCanvasElement>("screen");
            screenContext = screen.GetContext(CanvasTypes.CanvasContext2DType.CanvasRenderingContext2D);
            screenContext.FillStyle = "white";
            screenContext.Font = "40px Consolas, monospace";

            Document.OnKeyDown = e => keys.Add(e.KeyCode);
            Document.OnKeyUp = e => keys.Remove(e.KeyCode);

            player = new Sprite { Width = 20, Height = 20, X = 25, Y = 25 };
            goal = new Sprite { Width = 30, Height = 30, X = screen.Width - 25, Y = screen.Height - 25 };

            walls = ComputeMaze();

            sprites = new Sprite[2 + walls.Length];
            sprites[0] = player;
            sprites[1] = goal;
            for (int i = 2; i < sprites.Length; i++)
            {
                sprites[i] = walls[i - 2];
            }

            Tick();
        }

        void Tick()
        {
            int now = (int)Window.Performance.Now();
            Update(now - lastFrame);
            Draw();
            lastFrame = now;
            Window.RequestAnimationFrame(Tick);
        }

        void Update(int elapsed)
        {
            if (gameOver)
            {
                return;
            }

            if (keys.Contains(UP_KEY))
            {
                player.VelocityY = -PLAYER_SPEED;
            }
            else if (keys.Contains(DOWN_KEY))
            {
                player.VelocityY = PLAYER_SPEED;
            }
            else
            {
                player.VelocityY = 0;
            }

            if (keys.Contains(RIGHT_KEY))
            {
                player.VelocityX = PLAYER_SPEED;
            }
            else if (keys.Contains(LEFT_KEY))
            {
                player.VelocityX = -PLAYER_SPEED;
            }
            else
            {
                player.VelocityX = 0;
            }

            player.X += elapsed * player.VelocityX;
            player.Y += elapsed * player.VelocityY;

            if (player.CollidesWith(goal))
            {
                gameOver = true;
                return;
            }

            for (int i = 0; i < walls.Length; i++)
            {
                if (player.CollidesWith(walls[i]))
                {
                    player.X -= elapsed * player.VelocityX;
                    player.Y -= elapsed * player.VelocityY;
                }
            }
        }

        void Draw()
        {
            screenContext.ClearRect(0, 0, screen.Width, screen.Height);

            if (gameOver)
            {
                screenContext.TextAlign = CanvasTypes.CanvasTextAlign.Center;
                screenContext.FillText("Congratulation!!!", screen.Width / 2, screen.Height / 2);
            }
            else
            {
                for (var i = 0; i < sprites.Length; i++)
                {
                    screenContext.FillRect((int)(sprites[i].X - sprites[i].Width / 2), (int)(sprites[i].Y - sprites[i].Height / 2), sprites[i].Width, sprites[i].Height);
                }
            }
        }

        enum entrance { NOT_INCLUDED, LEFT, RIGHT, UP, DOWN, START };

        Sprite[] ComputeMaze()
        {
            // Initialize maze
            entrance[,] maze = new entrance[MAZE_SIZE, MAZE_SIZE];
            for (int i = 0; i < MAZE_SIZE; i++)
            {
                for (int j = 0; j < MAZE_SIZE; j++)
                {
                    maze[i, j] = entrance.NOT_INCLUDED;
                }
            }

            // Fixe maze entrance at 0,0
            maze[0, 0] = entrance.START;

            List<int> remainingTiles = new List<int>();

            for (int i = 1; i < MAZE_SIZE * MAZE_SIZE; i++)
            {
                remainingTiles.Add(i);
            }

            while (remainingTiles.Count > 0)
            {
                // Temporary maze use to compute new paths
                entrance[,] tempMaze = new entrance[MAZE_SIZE, MAZE_SIZE];

                for (int i = 0; i < MAZE_SIZE; i++)
                {
                    for (int j = 0; j < MAZE_SIZE; j++)
                    {
                        tempMaze[i, j] = entrance.NOT_INCLUDED;
                    }
                }

                int index = rdm.Next(0, remainingTiles.Count);
                int start = remainingTiles[index];

                int x = start % MAZE_SIZE;
                int y = start / MAZE_SIZE;
                int xIni = x;
                int yIni = y;

                // Get a path from the tempMaze
                while (maze[x, y] == entrance.NOT_INCLUDED)
                {
                    // Randomly choose if the next movement is along x or y
                    bool moveAlongX = rdm.Next(0, 2) == 0;

                    int position = moveAlongX ? x : y;
                    int next;

                    if (position == 0)
                    {
                        next = 1;
                    }
                    else if (position == MAZE_SIZE - 1)
                    {
                        next = MAZE_SIZE - 2;
                    }
                    else
                    {
                        next = rdm.Next(0, 2) == 0 ? position - 1 : position + 1;
                    }

                    if (moveAlongX)
                    {
                        if (tempMaze[next, y] == entrance.NOT_INCLUDED)
                        {
                            if (next < position)
                            {
                                tempMaze[next, y] = entrance.RIGHT;
                            }
                            else
                            {
                                tempMaze[next, y] = entrance.LEFT;
                            }
                        }

                        x = next;
                    }
                    else
                    {
                        if (tempMaze[x, next] == entrance.NOT_INCLUDED)
                        {
                            if (next < position)
                            {
                                tempMaze[x, next] = entrance.UP;
                            }
                            else
                            {
                                tempMaze[x, next] = entrance.DOWN;
                            }
                        }

                        y = next;
                    }
                }

                // Apply the path found to the maze
                while (tempMaze[x, y] != entrance.NOT_INCLUDED && !(x == xIni && y == yIni))
                {
                    if (tempMaze[x, y] == entrance.RIGHT)
                    {
                        x++;
                        maze[x, y] = entrance.LEFT;
                    }
                    else if (tempMaze[x, y] == entrance.UP)
                    {
                        y++;
                        maze[x, y] = entrance.DOWN;
                    }
                    else if (tempMaze[x, y] == entrance.LEFT)
                    {
                        x--;
                        maze[x, y] = entrance.RIGHT;
                    }
                    else
                    {
                        y--;
                        maze[x, y] = entrance.UP;
                    }

                    remainingTiles.Remove(x + y * MAZE_SIZE);
                }
            }

            List<Sprite> horizontalWalls = new List<Sprite>();
            List<Sprite> verticalWalls = new List<Sprite>();

            int wallWidth = 10;
            int wallLength = 50;

            for (int i = 0; i < (MAZE_SIZE + 1) * MAZE_SIZE; i++)
            {
                horizontalWalls.Add(new Sprite { Width = wallLength, Height = wallWidth, X = wallLength / 2 + (i % MAZE_SIZE) * (screen.Width / MAZE_SIZE), Y = (i / MAZE_SIZE) * (screen.Height / MAZE_SIZE) });
                verticalWalls.Add(new Sprite { Width = wallWidth, Height = wallLength, X = (i / MAZE_SIZE) * (screen.Width / MAZE_SIZE), Y = wallLength / 2 + (i % MAZE_SIZE) * (screen.Height / MAZE_SIZE)});
            }
            
            for (int i = 0; i < MAZE_SIZE; i++)
            {
                for (int j = 0; j < MAZE_SIZE; j++)
                {
                    if (maze[i, j] == entrance.LEFT)
                    {
                        verticalWalls.ElementAt(i * MAZE_SIZE + j).X = -100;
                    }
                    else if (maze[i, j] == entrance.RIGHT)
                    {
                        verticalWalls.ElementAt((i + 1) * MAZE_SIZE + j).X = -100;
                    }

                    if (maze[j, i] == entrance.DOWN)
                    {
                        horizontalWalls.ElementAt(i * MAZE_SIZE + j).X = -100;
                    }
                    else if (maze[j, i] == entrance.UP)
                    {
                        horizontalWalls.ElementAt((i + 1) * MAZE_SIZE + j).X = -100;
                    }
                }
            }

            Sprite[] allWalls = new Sprite[verticalWalls.Count + horizontalWalls.Count];

            for (int i = 0; i < verticalWalls.Count; i++)
            {
                allWalls[i] = verticalWalls.ElementAt(i);
            }

            for (int i = verticalWalls.Count; i < allWalls.Length; i++)
            {
                allWalls[i] = horizontalWalls.ElementAt(i - verticalWalls.Count);
            }

            return allWalls;
        }
    }

    public class Sprite
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double VelocityX { get; set; }
        public double VelocityY { get; set; }

        public bool CollidesWith(Sprite obstacle)
        {
            bool collision = X + Width / 2 >= obstacle.X - obstacle.Width / 2 &&
                             X - Width / 2 <= obstacle.X + obstacle.Width / 2 &&
                             Y + Height / 2 >= obstacle.Y - obstacle.Height / 2 &&
                             Y - Height / 2 <= obstacle.Y + obstacle.Height / 2;

            return collision;
        }
    }
}
