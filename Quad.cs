namespace WinFormsApp1
{
    public class Quad : Object
    {
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
}