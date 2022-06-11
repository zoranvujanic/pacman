namespace WinFormsApp1
{
    public class Circle : Object
    {
        public float R { get; set; }
        public Color Color { get; set; }

        public Circle(float _x, float _y, float _r, Color _color)
        {
            X = _x;
            Y = _y;
            R = _r;
            W = 2 * _r;
            H = 2 * _r;
            Color = _color;
        }
        public void Draw(Graphics g)
        {
            Brush brush = new SolidBrush(Color);
            g.FillEllipse(brush, (int)Helpers.Snap(X) + (Constants.size - 2 * R) / 2, 
                                 (int)Helpers.Snap(Y) + (Constants.size - 2 * R) / 2, 
                                 (int)2 * R,
                                 (int)2 * R);
            brush.Dispose();
        }
    }
}