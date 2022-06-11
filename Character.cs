namespace WinFormsApp1
{
    public class Character : Object
    {
        private string texture;
        private Bitmap bitmap;
        public PictureBox image = new PictureBox();

        public Character(float _x, float _y, float _w, float _h, string _texture)
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
            image.Image = bitmap;
        }

        public void Draw()
        {
            switch (Direction)
            {
                case Direction.LEFT:
                    image.Location = new Point((int)X, Helpers.Snap(Y));
                    break;
                case Direction.RIGHT:
                    image.Location = new Point((int)X - (int)(Constants.size - W), Helpers.Snap(Y));
                    break;
                case Direction.UP:
                    image.Location = new Point(Helpers.Snap(X), (int)Y);
                    break;
                case Direction.DOWN:
                    image.Location = new Point(Helpers.Snap(X), (int)Y - (int)(Constants.size - H));
                    break;
            }

            image.ClientSize = new Size(Constants.size, Constants.size);
        }
    }
}