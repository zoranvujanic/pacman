using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;


namespace WinFormsApp1
{

    public partial class Form1 : Form
    {
        Stopwatch sw = new Stopwatch();
        Random random = new Random();

        float deltaTime;
        float lastTime;

        int score = 0;
        bool canEatGhost = false;
        float ghostTimer = 0.0f;

        Character pacman = new Character(0, 0, 0.85f * Constants.size, 0.85f * Constants.size, "../../../res/pacman.png");
        Character ghost = new Character(0, 0, 0.85f * Constants.size, 0.85f * Constants.size, "../../../res/duh.png");

        List<Quad> walls = new List<Quad>();
        List<Circle> coins = new List<Circle>();
        List<Circle> fruits = new List<Circle>();

        static char[,] mapa =
        {
            { '#', '#', '#', '#', '#', '#','#','#','#','#','#', '#', '#', '#', '#', '#','#','#','#','#','#','#','#'},
            { '#', ' ', ' ', ' ', ' ', ' ',' ',' ',' ',' ',' ', '#', ' ', ' ', ' ', ' ',' ',' ',' ',' ',' ',' ','#'},
            { '#', '@', '#', '#', '#', ' ','#','#','#','#',' ', '#', ' ', '#', '#', '#','#',' ','#','#','#','@','#'},
            { '#', ' ', '#', '#', '#', ' ','#','#','#','#',' ', '#', ' ', '#', '#', '#','#',' ','#','#','#',' ','#'},
            { '#', ' ', ' ', ' ', ' ', ' ',' ',' ',' ',' ',' ', ' ', ' ', ' ', ' ', ' ',' ',' ',' ',' ',' ',' ','#'},
            { '#', ' ', '#', '#', '#', ' ','#',' ','#','#','#', '#', '#', '#', '#', ' ','#',' ','#','#','#',' ','#'},
            { '#', ' ', ' ', ' ', ' ', ' ','#',' ','#','#','#', '#', '#', '#', '#', ' ','#',' ',' ',' ',' ',' ','#'},
            { '#', '#', '#', '#', '#', ' ','#',' ',' ',' ',' ', '#', ' ', ' ', ' ', ' ','#',' ','#','#','#','#','#'},
            { '#', '#', '#', '#', '#', ' ','#','#','#','#',' ', '#', ' ', '#', '#', '#','#',' ','#','#','#','#','#'},
            { '#', '#', '#', '#', '#', ' ','#','!','!','!','!', '!', '!', '!', '!', '!','#',' ','#','#','#','#','#'},
            { '#', '#', '#', '#', '#', ' ','#','!','#','#','#', '!', '#', '#', '#', '!','#',' ','#','#','#','#','#'},
            { '!', '!', '!', '!', '!', '!','!','!','#','!','!', 'm', '!', '!', '#', '!','!','!','!','!','!','!','!'},
            { '#', '#', '#', '#', '#', ' ','#','!','#','#','#', '#', '#', '#', '#', '!','#',' ','#','#','#','#','#'},
            { '#', '#', '#', '#', '#', ' ','#','!','!','!','!', '!', '!', '!', '!', '!','#',' ','#','#','#','#','#'},
            { '#', '#', '#', '#', '#', ' ','#','!','#','#','#', '#', '#', '#', '#', '!','#',' ','#','#','#','#','#'},
            { '#', '#', '#', '#', '#', ' ','#','!','#','#','#', '#', '#', '#', '#', '!','#',' ','#','#','#','#','#'},
            { '#', ' ', ' ', ' ', ' ', ' ',' ',' ',' ',' ',' ', '#', ' ', ' ', ' ', ' ',' ',' ',' ',' ',' ',' ','#'},
            { '#', ' ', '#', '#', '#', ' ','#','#','#','#',' ', '#', ' ', '#', '#', '#','#',' ','#','#','#',' ','#'},
            { '#', '@', ' ', ' ', '#', ' ',' ',' ',' ',' ',' ', 'p', ' ', ' ', ' ', ' ',' ',' ',' ',' ',' ','@','#'},
            { '#', '#', '#', ' ', '#', ' ','#',' ','#','#','#', '#', '#', '#', '#', ' ','#',' ','#',' ','#','#','#'},
            { '#', ' ', ' ', ' ', ' ', ' ','#',' ',' ',' ',' ', '#', ' ', ' ', ' ', ' ','#',' ',' ',' ',' ',' ','#'},
            { '#', ' ', '#', '#', '#', '#','#','#','#','#',' ', '#', ' ', '#', '#', '#','#','#','#','#','#',' ','#'},
            { '#', ' ', '#', '#', '#', '#','#','#','#','#',' ', '#', ' ', '#', '#', '#','#','#','#','#','#',' ','#'},
            { '#', ' ', ' ', ' ', ' ', ' ',' ',' ',' ',' ',' ', ' ', ' ', ' ', ' ', ' ',' ',' ',' ',' ',' ',' ','#'},
            { '#', '#', '#', '#', '#', '#','#','#','#','#','#', '#', '#', '#', '#', '#','#','#','#','#','#','#','#'},
        };

        public Form1()
        {
            InitializeComponent();
        }

        private void LoadMap()
        {
            for (int i = 0; i < mapa.GetLength(0); i++)
            {
                for (int j = 0; j < mapa.GetLength(1); j++)
                {
                    switch (mapa[i, j])
                    {
                        case 'p':
                            pacman.X = j * Constants.size;
                            pacman.Y = i * Constants.size;
                            coins.Add(new Circle(j * Constants.size, i * Constants.size, Constants.size / 3.0f, Color.Yellow));
                            break;
                        case 'm':
                            ghost.X = j * Constants.size;
                            ghost.Y = i * Constants.size;
                            break;
                        case '#':
                            walls.Add(new Quad(j * Constants.size, i * Constants.size, Constants.size, Constants.size, Color.Blue));
                            break;
                        case ' ':
                            coins.Add(new Circle(j * Constants.size, i * Constants.size, Constants.size / 3.0f, Color.Yellow));
                            break;
                        case '@':
                            fruits.Add(new Circle(j * Constants.size, i * Constants.size, Constants.size / 2.5f, Color.Red));
                            break;
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            sw.Start();

            LoadMap();

            Controls.Add(pacman.image);
            Controls.Add(ghost.image);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            sw.Stop();
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            //delete this
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            foreach (var wall in walls) wall.Draw(e.Graphics);
            foreach (var coin in coins) coin.Draw(e.Graphics);
            foreach (var fruit in fruits) fruit.Draw(e.Graphics);

            pacman.Draw();
            ghost.Draw();
        }

        public bool MoveCharacter(ref Character objekat)
        {
            if (objekat.X < 0) objekat.X = (mapa.GetLength(1) - 1) * Constants.size;
            if (objekat.X > (mapa.GetLength(1) - 1) * Constants.size) objekat.X = 0;

            bool collided = false;
            bool differentDirections = false;
            if (objekat.NextDirection != objekat.Direction)
            {
                switch (objekat.NextDirection)
                {
                    case Direction.LEFT:
                        objekat.X -= objekat.W;
                        break;
                    case Direction.RIGHT:
                        objekat.X += objekat.W;
                        break;
                    case Direction.UP:
                        objekat.Y -= objekat.H;
                        break;
                    case Direction.DOWN:
                        objekat.Y += objekat.H;
                        break;
                }
                differentDirections = true;
            }
            else
            {
                switch (objekat.NextDirection)
                {
                    case Direction.LEFT:
                        objekat.X -= Constants.speed * deltaTime;
                        break;
                    case Direction.RIGHT:
                        objekat.X += Constants.speed * deltaTime;
                        break;
                    case Direction.UP:
                        objekat.Y -= Constants.speed * deltaTime;
                        break;
                    case Direction.DOWN:
                        objekat.Y += Constants.speed * deltaTime;
                        break;
                }
            }
            foreach (var wall in walls)
            {
                if (Helpers.Collision(objekat, wall))
                {
                    if (!differentDirections)
                    {
                        switch (objekat.NextDirection)
                        {
                            case Direction.LEFT:
                                objekat.X = wall.X + wall.W;
                                break;
                            case Direction.RIGHT:
                                objekat.X = wall.X - objekat.W;
                                break;
                            case Direction.UP:
                                objekat.Y = wall.Y + wall.H;
                                break;
                            case Direction.DOWN:
                                objekat.Y = wall.Y - objekat.H;
                                break;
                        }
                    }
                    else
                    {
                        switch (objekat.NextDirection)
                        {
                            case Direction.LEFT:
                                objekat.X += objekat.W;
                                break;
                            case Direction.RIGHT:
                                objekat.X -= objekat.W;
                                break;
                            case Direction.UP:
                                objekat.Y += objekat.H;
                                break;
                            case Direction.DOWN:
                                objekat.Y -= objekat.H;
                                break;
                        }
                    }

                    collided = true;
                }
            }
            if (!collided)
            {
                if (objekat.Direction != objekat.NextDirection)
                {
                    switch (objekat.NextDirection)
                    {
                        case Direction.LEFT:
                            objekat.X += objekat.W;
                            objekat.X -= Constants.speed * deltaTime;
                            break;
                        case Direction.RIGHT:
                            objekat.X -= objekat.W;
                            objekat.X += Constants.speed * deltaTime;
                            break;
                        case Direction.UP:
                            objekat.Y += objekat.H;
                            objekat.Y -= Constants.speed * deltaTime;
                            break;
                        case Direction.DOWN:
                            objekat.Y -= objekat.H;
                            objekat.Y += Constants.speed * deltaTime;
                            break;
                    }
                }

                if (objekat.NextDirection != Direction.NONE)
                {
                    objekat.Direction = objekat.NextDirection;
                    return collided;
                }

                int angle = (int)objekat.NextDirection - (int)objekat.Direction;
                objekat.Direction = objekat.NextDirection;

                if (objekat.Angle == 90) objekat.image.Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                else if (objekat.Angle == 270) objekat.image.Image.RotateFlip(RotateFlipType.Rotate90FlipX);

                switch (objekat.NextDirection)
                {
                    case Direction.LEFT:
                        objekat.image.Image.RotateFlip(RotateFlipType.Rotate90FlipX);
                        objekat.Angle = 270;
                        break;
                    case Direction.RIGHT:
                        objekat.image.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        objekat.Angle = 90;
                        break;
                    case Direction.UP:
                        objekat.Angle = 0;
                        break;
                    case Direction.DOWN:
                        objekat.Angle = 180;
                        break;
                }

            }
            else
            {

                switch (objekat.Direction)
                {
                    case Direction.LEFT:
                        objekat.X -= Constants.speed * deltaTime;
                        break;
                    case Direction.RIGHT:
                        objekat.X += Constants.speed * deltaTime;
                        break;
                    case Direction.UP:
                        objekat.Y -= Constants.speed * deltaTime;
                        break;
                    case Direction.DOWN:
                        objekat.Y += Constants.speed * deltaTime;
                        break;
                }

                foreach (var wall in walls)
                {
                    if (Helpers.Collision(objekat, wall))
                    {
                        switch (objekat.Direction)
                        {
                            case Direction.LEFT:
                                objekat.X = wall.X + wall.W;
                                break;
                            case Direction.RIGHT:
                                objekat.X = wall.X - objekat.W;
                                break;
                            case Direction.UP:
                                objekat.Y = wall.Y + wall.H;
                                break;
                            case Direction.DOWN:
                                objekat.Y = wall.Y - objekat.H;
                                break;
                        }
                        collided = true;
                    }
                }
            }
            return collided;
        }

        private void ShowMessageBox(string text)
        {
            timer1.Stop();
            this.Hide();
            if (MessageBox.Show(text) == DialogResult.OK) Application.Exit();
            this.Show();
            timer1.Start();
        }
        private void CalculateCollisions()
        {
            for (int i = 0; i < coins.Count(); i++)
            {
                if (Helpers.Collision(pacman, coins[i]))
                {
                    coins.RemoveAt(i);
                    score++;
                }
            }

            for (int i = 0; i < fruits.Count(); i++)
            {
                if (Helpers.Collision(pacman, fruits[i]))
                {
                    fruits.RemoveAt(i);
                    score += 2;

                    BackColor = Color.FromArgb(255,0,0);
                    canEatGhost = true;
                    ghostTimer = Constants.fruitDuration;
                }
            }
        }
        private void ShowEndScreens()
        {
            if (coins.Count() == 0 && fruits.Count() == 0)
            {
                ShowMessageBox("a vidi ovaj zna nešto, evo ti: " + Convert.ToString(score));
            }

            if (Helpers.Collision(pacman, ghost))
            {
                if (canEatGhost) ShowMessageBox("a vidi ovaj zna nešto, evo ti: " + Convert.ToString(score));
                else ShowMessageBox("jest jest jest jest");
            }
        }
        private void SetLabels()
        {
            ghostTimer -= deltaTime;
            if (ghostTimer < 0.0f)
            {
                canEatGhost = false;
                BackColor = Color.Black;
            }

            label1.Text = "Skor: " + Convert.ToString(score);
            if (canEatGhost)
            {
                label2.Text = Convert.ToString(Math.Round(ghostTimer, 1));
            }
            else label2.Text = "";
        }

        public void GameUpdate()
        {
            SetLabels();

            MoveCharacter(ref pacman);
            MoveCharacter(ref ghost);

            CalculateCollisions();
            ShowEndScreens();
        }

        private void CalculateDeltaTime()
        {
            deltaTime = (sw.ElapsedMilliseconds - lastTime) / 1000.0f;
            lastTime = sw.ElapsedMilliseconds;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            GameUpdate();
            Refresh();
            CalculateDeltaTime();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Application.Exit();
                    break;
                case Keys.Up:
                    pacman.NextDirection = Direction.UP;
                    break;
                case Keys.Left:
                    pacman.NextDirection = Direction.LEFT;
                    break;
                case Keys.Down:
                    pacman.NextDirection = Direction.DOWN;
                    break;
                case Keys.Right:
                    pacman.NextDirection = Direction.RIGHT;
                    break;
                case Keys.W:
                    ghost.NextDirection = Direction.UP;
                    break;
                case Keys.A:
                    ghost.NextDirection = Direction.LEFT;
                    break;
                case Keys.S:
                    ghost.NextDirection = Direction.DOWN;
                    break;
                case Keys.D:
                    ghost.NextDirection = Direction.RIGHT;
                    break;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            //delete this
        }
    }
}