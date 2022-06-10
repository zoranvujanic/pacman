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

        float speed = 200.0f;
        int trenugao = 90;
        float timerDuhova = 0;

        TexturedQuad kvadrat = new TexturedQuad(0, 0, 0.8f * Globals.size, 0.8f * Globals.size, "../../../res/pacman.png");
        //Quad kvadrat2 = new Quad(100, 200, 100, 100, Color.Black);
        List<Quad> zidovi = new List<Quad>();
        List<Circle> coini = new List<Circle>();
        List<Circle> vockice = new List<Circle>();

        List<TexturedQuad> duhovi = new List<TexturedQuad>();
        List<Smer> smerDuhova = new List<Smer>();
        List<Smer> bufferedSmerDuhova = new List<Smer>();
        List<int> ugaoDuhova = new List<int>();

        Smer gsmer = Smer.DESNO;
        Smer gbufferedSmer = Smer.DESNO;
        public Form1()
        {
            InitializeComponent();
        }

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
            { '#', '#', '#', '#', '#', ' ','#','#','#','#','!', '#', '!', '#', '#', '#','#',' ','#','#','#','#','#'},
            { '#', '#', '#', '#', '#', ' ','#','!','!','!','!', '!', '!', '!', '!', '!','#',' ','#','#','#','#','#'},
            { '#', '#', '#', '#', '#', ' ','#','!','#',' ',' ', '!', ' ', ' ', '#', '!','#',' ','#','#','#','#','#'},
            { '!', '!', '!', '!', '!', '!','!','!','#','m','m', '!', 'm', 'm', '#', '!','!',' ','!','!','!','!','!'},
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

        private void Form1_Load(object sender, EventArgs e)
        {
            sw.Start();


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
                        coini.Add(new Circle(j * Globals.size, i * Globals.size, Globals.size / 3.0f, Color.Yellow));
                    }
                    else if (mapa[i, j] == '@')
                    {
                        vockice.Add(new Circle(j * Globals.size, i * Globals.size, Globals.size / 2.5f, Color.Red));
                    }
                    else if (mapa[i, j] == 'm')
                    {
                        duhovi.Add(new TexturedQuad(j * Globals.size, i * Globals.size, 1.0f * Globals.size, 1.0f * Globals.size, "../../../res/duh.png"));
                        smerDuhova.Add((Smer)random.Next((int)Smer.DESNO, (int)Smer.GORE));
                        bufferedSmerDuhova.Add((Smer)random.Next((int)Smer.DESNO, (int)Smer.GORE));
                        ugaoDuhova.Add(((int)smerDuhova[smerDuhova.Count() - 1] % 4) * 90);
                    }
                }
            }

            this.Controls.Add(kvadrat.image);
            for (int i = 0; i < duhovi.Count(); i++) this.Controls.Add(duhovi[i].image);
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
            for (int i = 0; i < vockice.Count(); i++) vockice[i].Draw(e.Graphics);
            for (int i = 0; i < duhovi.Count(); i++) duhovi[i].UpdateLocationAndSize(smerDuhova[i]);

            kvadrat.UpdateLocationAndSize(gsmer);
        }
        bool Collision(TexturedQuad a, Quad b)
        {

            return (((a.X + a.W > b.X) && (a.X + a.W < b.X + b.W))
               || ((a.X < b.X + b.W) && (a.X > b.X)))
               && (((a.Y + a.H > b.Y) && (a.Y + a.H < b.Y + b.H))
               || ((a.Y < b.Y + b.H) && (a.Y > b.Y)));

            //return (((Helpers.Snap(a.X) + a.W > b.X) && (Helpers.Snap(a.X) + a.W <= b.X + b.W))
            //    || ((Helpers.Snap(a.X) < b.X + b.W) && (Helpers.Snap(a.X) > b.X)))
            //    && (((Helpers.Snap(a.Y) + a.H > b.Y) && (Helpers.Snap(a.Y) + a.H <= b.Y + b.H))
            //    || ((Helpers.Snap(a.Y) < b.Y + b.H) && (Helpers.Snap(a.Y) > b.Y)));
        }
        bool Collision(TexturedQuad a, Circle b)
        {
            return (((a.X + a.W >= b.X) && (a.X + a.W <= b.X + 2 * b.R))
              || ((a.X <= b.X + 2 * b.R) && (a.X >= b.X)))
              && (((a.Y + a.H >= b.Y) && (a.Y + a.H <= b.Y + 2 * b.R))
              || ((a.Y <= b.Y + 2 * b.R) && (a.Y >= b.Y)));
            // return (((Helpers.Snap(a.X) + Helpers.Snap(a.W) > Helpers.Snap(b.X) && (Helpers.Snap(a.X) + Helpers.Snap(a.W) <= Helpers.Snap(b.X + 2 * b.R)))
            // || ((Helpers.Snap(a.X) < Helpers.Snap(b.X + 2 * b.R)) && (Helpers.Snap(a.X) > Helpers.Snap(b.X))))
            // && (((Helpers.Snap(a.Y) + Helpers.Snap(a.H) > Helpers.Snap(b.Y)) && (Helpers.Snap(a.Y) + Helpers.Snap(a.H) <= Helpers.Snap(b.Y + 2 * b.R))))
            //|| ((Helpers.Snap(a.Y) < Helpers.Snap(b.Y + 2 * b.R)) && (Helpers.Snap(a.Y) > Helpers.Snap(b.Y))));
        }

        public bool Pomeri(ref TexturedQuad objekat, Smer bufferedSmer, ref Smer smer, ref int ugao, float spid)
        {
            bool collided = false;
            bool isf = false;
            if (bufferedSmer != smer)
            {
                if (bufferedSmer == Smer.LEVO) objekat.X -= objekat.W;
                else if (bufferedSmer == Smer.DESNO) objekat.X += objekat.W;
                else if (bufferedSmer == Smer.GORE) objekat.Y -= objekat.H;
                else if (bufferedSmer == Smer.DOLE) objekat.Y += objekat.H;
                isf = true;
            }
            else
            {

                if (bufferedSmer == Smer.LEVO) objekat.X -= spid * deltaTime;
                else if (bufferedSmer == Smer.DESNO) objekat.X += spid * deltaTime;
                else if (bufferedSmer == Smer.GORE) objekat.Y -= spid * deltaTime;
                else if (bufferedSmer == Smer.DOLE) objekat.Y += spid * deltaTime;
            }
            for (int i = 0; i < zidovi.Count(); i++)
            {
                if (Collision(objekat, zidovi[i]))
                {
                    if (!isf)
                    {
                        if (bufferedSmer == Smer.LEVO) objekat.X = zidovi[i].X + zidovi[i].W;
                        else if (bufferedSmer == Smer.DESNO) objekat.X = zidovi[i].X - objekat.W;
                        else if (bufferedSmer == Smer.GORE) objekat.Y = zidovi[i].Y + zidovi[i].H;
                        else if (bufferedSmer == Smer.DOLE) objekat.Y = zidovi[i].Y - objekat.H;
                    }
                    else
                    {
                        if (bufferedSmer == Smer.LEVO) objekat.X += objekat.W;
                        else if (bufferedSmer == Smer.DESNO) objekat.X -= objekat.W;
                        else if (bufferedSmer == Smer.GORE) objekat.Y += objekat.H;
                        else if (bufferedSmer == Smer.DOLE) objekat.Y -= objekat.H;
                    }

                    collided = true;
                }
            }
            if (!collided)
            {
                if (smer != bufferedSmer)
                {
                    if (bufferedSmer == Smer.LEVO) objekat.X += objekat.W;
                    else if (bufferedSmer == Smer.DESNO) objekat.X -= objekat.W;
                    else if (bufferedSmer == Smer.GORE) objekat.Y += objekat.H;
                    else if (bufferedSmer == Smer.DOLE) objekat.Y -= objekat.H;

                    if (bufferedSmer == Smer.LEVO) objekat.X -= spid * deltaTime;
                    else if (bufferedSmer == Smer.DESNO) objekat.X += spid * deltaTime;
                    else if (bufferedSmer == Smer.GORE) objekat.Y -= spid * deltaTime;
                    else if (bufferedSmer == Smer.DOLE) objekat.Y += spid * deltaTime;
                }

                int angle = (int)bufferedSmer - (int)smer;
                smer = bufferedSmer;


                if (ugao == 90) objekat.image.Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                else if (ugao == 180) objekat.image.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                else if (ugao == 270) objekat.image.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);


                if (bufferedSmer == Smer.LEVO)
                {
                    objekat.image.Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    ugao = 270;
                }
                else if (bufferedSmer == Smer.DESNO)
                {
                    objekat.image.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    ugao = 90;
                }
                else if (bufferedSmer == Smer.GORE)
                {
                    //kvadrat.image.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    ugao = 0;
                }
                else if (bufferedSmer == Smer.DOLE)
                {
                    objekat.image.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    ugao = 180;
                }
            }
            else
            {

                if (smer == Smer.LEVO) objekat.X -= spid * deltaTime;
                else if (smer == Smer.DESNO) objekat.X += spid * deltaTime;
                else if (smer == Smer.GORE) objekat.Y -= spid * deltaTime;
                else if (smer == Smer.DOLE) objekat.Y += spid * deltaTime;
                for (int i = 0; i < zidovi.Count(); i++)
                {
                    if (Collision(objekat, zidovi[i]))
                    {
                        if (smer == Smer.LEVO) objekat.X = zidovi[i].X + zidovi[i].W;
                        else if (smer == Smer.DESNO) objekat.X = zidovi[i].X - objekat.W;
                        else if (smer == Smer.GORE) objekat.Y = zidovi[i].Y + zidovi[i].H;
                        else if (smer == Smer.DOLE) objekat.Y = zidovi[i].Y - objekat.H;
                        collided = true;
                    }
                }
            }
            return collided;
        }
        public bool PomeriDuha(ref TexturedQuad duh, Smer bufferedSmer, ref Smer smer, ref int ugao, float spid)
        {
            int i = (int)Math.Floor(duh.Y / Globals.size);
            int j = (int)Math.Floor(duh.X / Globals.size);

            if (bufferedSmer == Smer.LEVO)
            {
                duh.X -= spid * deltaTime;
                if((mapa[i, j] == '#'))
                {
                    smer = bufferedSmer;
                    duh.X += spid * deltaTime;
                }
            }
            else if (bufferedSmer == Smer.DESNO)
            {
                    duh.X += spid * deltaTime;
                if ((mapa[i, j + 1] == '#'))
                {
                    smer = bufferedSmer;
                    duh.X -= spid * deltaTime;
                }
            }
            else if (bufferedSmer == Smer.GORE)
            {
                duh.Y -= spid * deltaTime;
                if ((mapa[i, j] == '#'))
                {
                    smer = bufferedSmer;
                    duh.Y += spid * deltaTime;
                }
            }
            else if (bufferedSmer == Smer.DOLE)
            {
                duh.Y += spid * deltaTime;
                if ((mapa[i + 1, j] == '#'))
                {
                    smer = bufferedSmer;
                    duh.Y -= spid * deltaTime;
                }
            }
            //else if (bufferedSmer == Smer.DESNO)
            //{
            //    if (mapa[i, j + 1] == ' ' || (mapa[i, j + 1] == '!'))
            //    {
            //        smer = bufferedSmer;
            //        duh.X += spid * deltaTime;
            //    }
            //}
            //else if (bufferedSmer == Smer.DOLE)
            //{
            //    if (mapa[i + 1, j] == ' ' || (mapa[i + 1, j] == '!'))
            //    {
            //        smer = bufferedSmer;
            //        duh.Y += spid * deltaTime;
            //    }
            //}
            //else if (bufferedSmer == Smer.GORE)
            //{
            //    if (mapa[i - 1, j] == ' ' || (mapa[i - 1, j] == '!'))
            //    {
            //        smer = bufferedSmer;
            //        duh.Y -= spid * deltaTime;
            //    }
            //}



            return random.Next(0,10) > 1;
        }

        public void GameUpdate()
        {
            timerDuhova += deltaTime;
            if (coini.Count() == 0)
            {
                //Close();
            }

            if (kvadrat.X < 0) kvadrat.X = (mapa.GetLength(1) - 1) * Globals.size;
            if (kvadrat.X > (mapa.GetLength(1) - 1) * Globals.size) kvadrat.X = 0;

            Pomeri(ref kvadrat, gbufferedSmer, ref gsmer, ref trenugao, speed);


            for (int i = 0; i < duhovi.Count(); i++)
            {
                TexturedQuad duh = duhovi[i];
                Smer smerDuha = smerDuhova[i];
                int ugaoDuha = ugaoDuhova[i];
                PomeriDuha(ref duh, bufferedSmerDuhova[i], ref smerDuha, ref ugaoDuha, speed / 3);

                //while (PomeriDuha(ref duh, bufferedSmerDuhova[i], ref smerDuha, ref ugaoDuha, speed / 3))
                //{
                    //bufferedSmerDuhova[i] = (Smer)random.Next((int)Smer.DESNO, (int)Smer.GORE + 1);
                    //duh = duhovi[i];
                    //smerDuha = smerDuhova[i];
                    //ugaoDuha = ugaoDuhova[i];
                //}
            }



            //smerDuhova.Add((Smer)random.Next((int)Smer.DESNO, (int)Smer.GORE));
            for (int i = 0; i < coini.Count(); i++)
            {
                if (Collision(kvadrat, coini[i]))
                {
                    coini.RemoveAt(i);
                }
            }

            for (int i = 0; i < vockice.Count(); i++)
            {
                if (Collision(kvadrat, vockice[i]))
                {
                    vockice.RemoveAt(i);

                    //MOCI JESTI BOTOVE
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
            else if (e.KeyCode == Keys.W || e.KeyCode == Keys.Up) gbufferedSmer = Smer.GORE;
            else if (e.KeyCode == Keys.A || e.KeyCode == Keys.Left) gbufferedSmer = Smer.LEVO;
            else if (e.KeyCode == Keys.S || e.KeyCode == Keys.Down) gbufferedSmer = Smer.DOLE;
            else if (e.KeyCode == Keys.D || e.KeyCode == Keys.Right) gbufferedSmer = Smer.DESNO;
        }
    }

    public enum Smer
    {
        NONE = 0,
        DESNO,
        DOLE,
        LEVO,
        GORE
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

        public void UpdateLocationAndSize(Smer smer)
        {
            if (smer == Smer.LEVO) image.Location = new Point((int)X, (int)Helpers.Snap(Y));
            else if (smer == Smer.DESNO) image.Location = new Point((int)X, (int)Helpers.Snap(Y));
            else if (smer == Smer.GORE) image.Location = new Point((int)Helpers.Snap(X), (int)Y);
            else if (smer == Smer.DOLE) image.Location = new Point((int)Helpers.Snap(X), (int)Y);
            //image.ClientSize = new Size((int)W, (int)H);
            image.ClientSize = new Size((int)Globals.size, (int)Globals.size);
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
            g.FillEllipse(brush, (int)Helpers.Snap(X) + (Globals.size - 2 * R) / 2, (int)Helpers.Snap(Y) + (Globals.size - 2 * R) / 2, (int)2 * R, (int)2 * R);
            brush.Dispose();
        }


    }

    public static class Globals
    {
        public const float size = 30.0f;


    }
    public static class Helpers
    {
        public static float Snap(float a, float val = Globals.size)
        {
            //return a;
            return (int)(((int)a / (int)val) * val);
        }
    }
}