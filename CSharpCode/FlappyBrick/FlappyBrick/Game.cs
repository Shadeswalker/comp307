using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bridge.Html5;

namespace FlappyBrick
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
        Sprite brick, floor, ceiling;
        Sprite[] sprites;
        int lastFrame, score, gameStartCountdown;
        HashSet<int> keys = new HashSet<int>();
        bool spaceDownLastFrame = false;
        bool gameOver = false;
        Random rdm = new Random();
        int hole = 150;
        double obstacleSpeed = -0.25;

        const int SPACE_KEY_DOWN = 32;
        const double FLAPPING_SPEED = -0.5;
        const double GRAVITY = 0.05;

        public Game()
        {
            screen = Document.GetElementById<HTMLCanvasElement>("screen");//Document.QuerySelector<HTMLCanvasElement>("canvas");
            screenContext = screen.GetContext(CanvasTypes.CanvasContext2DType.CanvasRenderingContext2D);
            screenContext.FillStyle = "white";
            screenContext.Font = "40px Consolas, monospace";

            Document.OnKeyDown = e => keys.Add(e.KeyCode);
            Document.OnKeyUp = e => keys.Remove(e.KeyCode);

            int floorHeight = rdm.Next(20, screen.Height - 20 - hole);
            int ceilingHeight = screen.Height - floorHeight - hole;

            brick = new Sprite { Width = 50, Height = 25, X = screen.Width / 4, Y = screen.Height / 2 };
            floor = new Sprite { Width = 25, Height = floorHeight, X = screen.Width, Y = floorHeight / 2 };
            ceiling = new Sprite { Width = 25, Height = ceilingHeight, X = screen.Width, Y = screen.Height - ceilingHeight / 2 };
            sprites = new[] { brick, floor, ceiling };

            floor.VelocityX = obstacleSpeed;
            ceiling.VelocityX = obstacleSpeed;

            score = 0;
            gameStartCountdown = 3000;

            Tick();
        }

        void Tick()
        {
            int now = (int)Window.Performance.Now();
            Update(now - lastFrame);
            Draw();
            lastFrame = now;
            spaceDownLastFrame = keys.Contains(SPACE_KEY_DOWN);
            Window.RequestAnimationFrame(Tick);
        }

        void Update(int elapsed)
        {
            if (gameStartCountdown > 0)
            {
                gameStartCountdown -= elapsed;
                return;
            }
            else if (gameOver)
            {
                return;
            }

            if (keys.Contains(SPACE_KEY_DOWN) && !spaceDownLastFrame)
            {
                brick.VelocityY = FLAPPING_SPEED;
            }
            else
            {
                brick.VelocityY += GRAVITY;
            }

            brick.Y += elapsed * brick.VelocityY;
            if (brick.Y > screen.Height - brick.Height / 2)
            {
                brick.Y = screen.Height - brick.Height / 2;
                brick.VelocityY = 0;
            }
            else if (brick.Y < brick.Height / 2)
            {
                brick.Y = brick.Height / 2;
                brick.VelocityY = 0;
            }

            floor.X += elapsed * floor.VelocityX;
            ceiling.X += elapsed * ceiling.VelocityX;

            if (brick.CollidesWith(floor) || brick.CollidesWith(ceiling))
            {
                gameOver = true;
                return;
            }

            if (floor.X < 0)
            {
                score++;
                ResetObstacles();
            }

        }

        void Draw()
        {
            screenContext.ClearRect(0, 0, screen.Width, screen.Height);

            screenContext.TextAlign = CanvasTypes.CanvasTextAlign.Left;
            screenContext.FillText(score.ToString(), 50, 50);

            if (gameStartCountdown > 0)
            {
                screenContext.TextAlign = CanvasTypes.CanvasTextAlign.Center;
                screenContext.FillText(((gameStartCountdown / 1000) + 1).ToString(), screen.Width / 2, screen.Height / 2);
            }
            else if (gameOver)
            {
                screenContext.TextAlign = CanvasTypes.CanvasTextAlign.Center;
                screenContext.FillText("Game Over", screen.Width / 2, screen.Height / 2);
            }
            else
            {
                for (var i = 0; i < sprites.Length; i++)
                {
                    screenContext.FillRect((int)(sprites[i].X - sprites[i].Width / 2), (int)(sprites[i].Y - sprites[i].Height / 2), sprites[i].Width, sprites[i].Height);
                }
            }
        }

        void ResetObstacles()
        {
            if (hole > 50)
            {
                hole--;
            }

            if (obstacleSpeed > -0.5)
            {
                obstacleSpeed -= 0.01;
            }

            int floorHeight = rdm.Next(20, screen.Height - 20 - hole);
            int ceilingHeight = screen.Height - floorHeight - hole;

            floor.Height = floorHeight;
            floor.X = screen.Width;
            floor.Y = floorHeight / 2;

            ceiling.Height = ceilingHeight;
            ceiling.X = screen.Width;
            ceiling.Y = screen.Height - ceilingHeight / 2;

            floor.VelocityX = obstacleSpeed;
            ceiling.VelocityX = obstacleSpeed;
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
