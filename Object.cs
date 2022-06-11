namespace WinFormsApp1
{
    public enum Direction
    {
        NONE = 0,
        RIGHT,
        DOWN,
        LEFT,
        UP
    }

    public class Object
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float W { get; set; }
        public float H { get; set; }
        public int Angle { get; set; } = -1;
        public Direction Direction { get; set; } = Direction.RIGHT;
        public Direction NextDirection { get; set; } = Direction.NONE;
    }
}
