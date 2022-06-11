namespace WinFormsApp1
{
    public static class Helpers
    {
        public static int Snap(float a, int val = Constants.size)
        {
            return ((int)a / val) * val;
        }
        public static bool Collision(Object a, Object b)
        {
            return (((a.X + a.W > b.X) && (a.X + a.W < b.X + b.W))
               || ((a.X < b.X + b.W) && (a.X > b.X)))
               && (((a.Y + a.H > b.Y) && (a.Y + a.H < b.Y + b.H))
               || ((a.Y < b.Y + b.H) && (a.Y > b.Y)));
        }
        public static bool Collision(Object a, Circle b)
        {
            return (((a.X + a.W >= b.X) && (a.X + a.W <= b.X + b.W))
               || ((a.X <= b.X + b.W) && (a.X >= b.X)))
               && (((a.Y + a.H >= b.Y) && (a.Y + a.H <= b.Y + b.H))
               || ((a.Y <= b.Y + b.H) && (a.Y >= b.Y)));
        }
        public static bool Collision(Character a, Character b)
        {
            return (((a.X + a.W >= b.X) && (a.X + a.W <= b.X + b.W))
               || ((a.X <= b.X + b.W) && (a.X >= b.X)))
               && (((a.Y + a.H >= b.Y) && (a.Y + a.H <= b.Y + b.H))
               || ((a.Y <= b.Y + b.H) && (a.Y >= b.Y)));
        }
    }
}