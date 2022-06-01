using System;
using System.Diagnostics;


namespace WinFormsApp1
{

    public partial class Form1 : Form
    {
        Stopwatch sw = new Stopwatch();

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
            Console.WriteLine("Test");
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
}