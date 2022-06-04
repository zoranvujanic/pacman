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
        float deltaTime;
        float lastTime;
        Random random = new Random();

        const float size = 50.0f;
        int trenugao = 90;

        TexturedQuad kvadrat = new TexturedQuad(0, 0, 0.97f * size, 0.97f * size, "../../../res/pacman.png");
        //Quad kvadrat2 = new Quad(100, 200, 100, 100, Color.Black);
        List<Quad> zidovi = new List<Quad>();
        List<Quad> coini = new List<Quad>();

        Smer smer = Smer.DESNO;
        Smer bufferedSmer = Smer.DESNO;
        public Form1()
        {
            InitializeComponent();
        }

        char[,] mapa =
        {
            { '#', '#', '#', '#', '#', '#','#','#','#','#'},
            { '#', '#', '#', ' ', ' ', ' ','#',' ',' ','#' },
            { '#', 'p', '#', ' ', '#', ' ','#',' ','#','#' },
            { '#', ' ', ' ', ' ', ' ', ' ','#',' ','#','#' },
            { '#', ' ', '#', ' ', '#', '#','#',' ','#','#' },
            { '#', ' ', '#', ' ', '#', ' ',' ',' ',' ','#'},
            { '#', ' ', '#', ' ', ' ', ' ','#','#',' ','#'},
            { '#', '#', '#', '#', '#', '#','#','#','#','#'}
        };

        private void Form1_Load(object sender, EventArgs e)
        {
            sw.Start();
            this.Controls.Add(kvadrat.image);


            for (int i = 0; i < mapa.GetLength(0); i++)
            {
                for (int j = 0; j < mapa.GetLength(1); j++)
                {
                    if (mapa[i, j] == 'p')
                    {
                        kvadrat.X = j * size;
                        kvadrat.Y = i * size;
                    }
                    else if (mapa[i, j] == '#')
                    {
                        zidovi.Add(new Quad(j * size, i * size, size, size, Color.Blue));
                    }
                    else if (mapa[i, j] == ' ')
                    {
                        coini.Add(new Quad(j * size, i * size, size, size, Color.Yellow));
                    }
                }
            }

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            sw.Stop();
        }

        private void Form1_Activated(object sender, EventArgs e)
        {

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //kvadrat2.Draw(e.Graphics);
            for (int i = 0; i < zidovi.Count(); i++) zidovi[i].Draw(e.Graphics);
            for (int i = 0; i < coini.Count(); i++) coini[i].Draw(e.Graphics);
            kvadrat.UpdateLocationAndSize();
        }
        bool Collision(TexturedQuad a, Quad b)
        {
            /*
            return (((a.X + a.W >= b.X) && (a.X + a.W <= b.X + b.W))
                || ((a.X <= b.X + b.W) && (a.X >= b.X)))
                && (((a.Y + a.H >= b.Y) && (a.Y + a.H <= b.Y + b.H))
                || ((a.Y <= b.Y + b.H) && (a.Y >= b.Y)));
            */
            return (((a.X + a.W > b.X) && (a.X + a.W <= b.X + b.W))
               || ((a.X < b.X + b.W) && (a.X > b.X)))
               && (((a.Y + a.H > b.Y) && (a.Y + a.H <= b.Y + b.H))
               || ((a.Y < b.Y + b.H) && (a.Y > b.Y)));
        }
        public void GameUpdate()
        {
            //kvadrat.image.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            float speed = 200.0f;
            if (bufferedSmer == Smer.LEVO) kvadrat.X -= speed * deltaTime;
            else if (bufferedSmer == Smer.DESNO) kvadrat.X += speed * deltaTime;
            else if (bufferedSmer == Smer.GORE) kvadrat.Y -= speed * deltaTime;
            else if (bufferedSmer == Smer.DOLE) kvadrat.Y += speed * deltaTime;
            bool collided = false;
            for (int i = 0; i < zidovi.Count(); i++)
            {
                if (Collision(kvadrat, zidovi[i]))
                {
                    if (bufferedSmer == Smer.LEVO) kvadrat.X = zidovi[i].X + zidovi[i].W;
                    else if (bufferedSmer == Smer.DESNO) kvadrat.X = zidovi[i].X - kvadrat.W;
                    else if (bufferedSmer == Smer.GORE) kvadrat.Y = zidovi[i].Y + zidovi[i].H;
                    else if (bufferedSmer == Smer.DOLE) kvadrat.Y = zidovi[i].Y - kvadrat.H;
                    collided = true;
                }
            }
            if (!collided)
            {
                int angle = (int)bufferedSmer - (int)smer;
                smer = bufferedSmer;

                if(trenugao == 90) kvadrat.image.Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                else if(trenugao == 180) kvadrat.image.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                else if(trenugao == 270) kvadrat.image.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);


                if (bufferedSmer == Smer.LEVO) {
                    kvadrat.image.Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    trenugao = 270;
                }
                else if (bufferedSmer == Smer.DESNO) {
                    kvadrat.image.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    trenugao = 90;
                }
                else if (bufferedSmer == Smer.GORE) {
                    //kvadrat.image.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    trenugao = 0;
                }
                else if (bufferedSmer == Smer.DOLE) {
                    kvadrat.image.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    trenugao = 180;
                }
            }
            else if (smer != bufferedSmer)
            {
                if (smer == Smer.LEVO) kvadrat.X -= speed * deltaTime;
                else if (smer == Smer.DESNO) kvadrat.X += speed * deltaTime;
                else if (smer == Smer.GORE) kvadrat.Y -= speed * deltaTime;
                else if (smer == Smer.DOLE) kvadrat.Y += speed * deltaTime;
                for (int i = 0; i < zidovi.Count(); i++)
                {
                    if (Collision(kvadrat, zidovi[i]))
                    {
                        if (smer == Smer.LEVO) kvadrat.X = zidovi[i].X + zidovi[i].W;
                        else if (smer == Smer.DESNO) kvadrat.X = zidovi[i].X - kvadrat.W;
                        else if (smer == Smer.GORE) kvadrat.Y = zidovi[i].Y + zidovi[i].H;
                        else if (smer == Smer.DOLE) kvadrat.Y = zidovi[i].Y - kvadrat.H;
                        collided = true;
                    }
                }
            }
            for (int i = 0; i < coini.Count(); i++)
            {
                if (Collision(kvadrat, coini[i]))
                {
                    coini.RemoveAt(i);
                }
            }

            //kvadrat2.X += 20 * deltaTime;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            GameUpdate();
            Refresh();

            deltaTime = (sw.ElapsedMilliseconds - lastTime) / 1000.0f;
            lastTime = sw.ElapsedMilliseconds;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();
            else if (e.KeyCode == Keys.W || e.KeyCode == Keys.Up) bufferedSmer = Smer.GORE;
            else if (e.KeyCode == Keys.A || e.KeyCode == Keys.Left) bufferedSmer = Smer.LEVO;
            else if (e.KeyCode == Keys.S || e.KeyCode == Keys.Down) bufferedSmer = Smer.DOLE;
            else if (e.KeyCode == Keys.D || e.KeyCode == Keys.Right) bufferedSmer = Smer.DESNO;
        }
    }

    enum Smer
    {
        NONE = 0,
        DESNO,
        DOLE,
        LEVO,
        GORE
    }

    public class Pacman
    {
        int x;
        int y;
        Smer smer;
        float brizina;
    }

    public class Quad
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float W { get; set; }
        public float H { get; set; }
        private Color Color { get; set; }
        public Quad(float _x, float _y, float _w, float _h, Color _color)
        {
            X = _x;
            Y = _y;
            W = _w;
            H = _h;
            Color = _color;
        }
        public void Draw(Graphics g)
        {
            Brush brush = new SolidBrush(Color);
            g.FillRectangle(brush, (int)X, (int)Y, (int)W, (int)H);
            brush.Dispose();
        }

    }

    public class TexturedQuad
    {
        private string texture;
        private Bitmap bitmap;
        public PictureBox image = new PictureBox();
        public float X { get; set; }
        public float Y { get; set; }
        public float W { get; set; }
        public float H { get; set; }
        //private float _X, _Y, _W, _H;
        //public float X { get { return _X; } set { _X = value; image.Location = new Point((int)X, (int)Y); } }
        // public float Y { get { return _Y; } set { _Y = value; image.Location = new Point((int)X, (int)Y); } }
        //public float W { get { return _W; } set { _W = value; image.Size = new Size((int)W, (int)H); } }
        //public float H { get { return _H; } set { _H = value; image.Size = new Size((int)W, (int)H); } }

        public TexturedQuad(float _x, float _y, float _w, float _h, string _texture)
        {
            X = _x;
            Y = _y;
            W = _w;
            H = _h;
            texture = _texture;
            bitmap = new Bitmap(texture);

            image.SizeMode = PictureBoxSizeMode.StretchImage;
            image.Location = new Point((int)X, (int)Y);
            image.ClientSize = new Size((int)W, (int)H);
            image.Image = (Image)bitmap;
        }

        public void UpdateLocationAndSize()
        {
            image.Location = new Point((int)X, (int)Y);
            image.ClientSize = new Size((int)W, (int)H);
        }

    }


    public class Circle
    {
        private float X { get; set; }
        private float Y { get; set; }
        private float R { get; set; }
        private Color Color { get; set; }
        public Circle(float _x, float _y, float _r, Color _color)
        {
            X = _x;
            Y = _y;
            R = _r;
            Color = _color;
        }
        public void Draw(Graphics g)
        {
            Brush brush = new SolidBrush(Color);
            g.FillEllipse(brush, (int)X, (int)Y, (int)2 * R, (int)2 * R);
            brush.Dispose();
        }


    }
}