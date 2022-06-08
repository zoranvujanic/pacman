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

        int trenugao = 90;

        TexturedQuad kvadrat = new TexturedQuad(0, 0, 0.98f * Globals.size, 0.98f * Globals.size, "../../../res/pacman.png");
        //Quad kvadrat2 = new Quad(100, 200, 100, 100, Color.Black);
        List<Quad> zidovi = new List<Quad>();
        List<Circle> coini = new List<Circle>();

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
                        kvadrat.X = j * Globals.size;
                        kvadrat.Y = i * Globals.size;
                    }
                    else if (mapa[i, j] == '#')
                    {
                        zidovi.Add(new Quad(j * Globals.size, i * Globals.size, Globals.size, Globals.size, Color.Blue));
                    }
                    else if (mapa[i, j] == ' ')
                    {
                        coini.Add(new Circle(j * Globals.size, i * Globals.size, Globals.size /2, Color.Yellow));
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

           //return (((a.X + a.W > b.X) && (a.X + a.W <= b.X + b.W))
           //   || ((a.X < b.X + b.W) && (a.X > b.X)))
           //   && (((a.Y + a.H > b.Y) && (a.Y + a.H <= b.Y + b.H))
           //   || ((a.Y < b.Y + b.H) && (a.Y > b.Y)));

            return (((Helpers.Snap(a.X) + a.W > b.X) && (Helpers.Snap(a.X) + a.W <= b.X + b.W))
                || ((Helpers.Snap(a.X) < b.X + b.W) && (Helpers.Snap(a.X) > b.X)))
                && (((Helpers.Snap(a.Y) + a.H > b.Y) && (Helpers.Snap(a.Y) + a.H <= b.Y + b.H))
                || ((Helpers.Snap(a.Y) < b.Y + b.H) && (Helpers.Snap(a.Y) > b.Y)));
        }
        bool Collision(TexturedQuad a, Circle b)
        {

            return (((Helpers.Snap(a.X) + Helpers.Snap(a.W) > Helpers.Snap(b.X) && (Helpers.Snap(a.X) + Helpers.Snap(a.W) <= Helpers.Snap(b.X + 2 * b.R)))
               || ((Helpers.Snap(a.X) < Helpers.Snap(b.X + 2 * b.R)) && (Helpers.Snap(a.X) > Helpers.Snap(b.X))))
               && (((Helpers.Snap(a.Y) + Helpers.Snap(a.H) > Helpers.Snap(b.Y)) && (Helpers.Snap(a.Y) + Helpers.Snap(a.H) <= Helpers.Snap(b.Y + 2 * b.R))))
               || ((Helpers.Snap(a.Y) < Helpers.Snap(b.Y + 2 * b.R)) && (Helpers.Snap(a.Y) > Helpers.Snap(b.Y))));
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
            g.FillRectangle(brush, (int)Helpers.Snap(X), (int)Helpers.Snap(Y), (int)Helpers.Snap(W), (int)Helpers.Snap(H));
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
            image.Location = new Point((int)Helpers.Snap(X), (int)Helpers.Snap(Y));
            image.ClientSize = new Size((int)W,(int)H);
        }

    }


    public class Circle
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float R { get; set; }
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
            g.FillEllipse(brush, (int)Helpers.Snap(X), (int)Helpers.Snap(Y), (int)2 * R, (int)2 * R);
            brush.Dispose();
        }


    }

    public static class Globals
    {
        public const float size = 50.0f;


    }
    public static class Helpers
    {
        public static float Snap(float a, float val = Globals.size)
        {
            return a;
            //return (int)(((int)a / (int)val) * val);
        }
    }
}