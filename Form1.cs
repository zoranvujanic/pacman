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

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            sw.Start();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            sw.Stop();
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            
        }

        TexturedQuad kvadrat = new TexturedQuad(100, 100, 100, 100, "../../../res/slika.png");
        private void Form1_Paint(object sender, PaintEventArgs e)
        {

            kvadrat.x += 200 * deltaTime;


            kvadrat.Draw(e.Graphics);
        }

        public void GameUpdate()
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            GameUpdate();
            Refresh();

            deltaTime = (sw.ElapsedMilliseconds - lastTime) / 1000.0f;
            lastTime = sw.ElapsedMilliseconds;
        }
    }

    enum Smer
    {
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
        public float x, y, w, h;
        public Color color;
        public Quad(float _x, float _y, float _w, float _h, Color _color)
        {
            x = _x;
            y = _y;
            w = _w;
            h = _h;
            color = _color;
        }
        public void Draw(Graphics g)
        {
            Brush brush = new SolidBrush(color);
            g.FillRectangle(brush, (int)x, (int)y, (int)w, (int)h);
            brush.Dispose();
        }
    }

    public class TexturedQuad
    {
        public float x, y, w, h;
        public string texture;
        public Image image;
        public TexturedQuad(float _x, float _y, float _w, float _h, string _texture)
        {
            x = _x;
            y = _y;
            w = _w;
            h = _h;
            texture = _texture;
            image = Image.FromFile(texture);
        }
        public void Draw(Graphics g)
        {
           g.DrawImage(image, x,y, new RectangleF(0,0, w, h), GraphicsUnit.Pixel);
        }
    }


    public class Circle
    {
        public float x, y, r;
        public Color color;
        public Circle(float _x, float _y, float _r, Color _color)
        {
            x = _x;
            y = _y;
            r = _r;
            color = _color;
        }
        public void Draw(Graphics g)
        {
            Brush brush = new SolidBrush(color);
            g.FillEllipse(brush, (int)x, (int)y, (int)2 * r, (int)2 * r);
            brush.Dispose();
        }
    }

}