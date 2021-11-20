using System.Diagnostics;
namespace MazeTest
{
    public partial class Form1 : Form
    {
        private Graphics _gfx;
        private Bitmap _overImg;
        private Bitmap _mazeImg;
        private Pen _pathPen = new Pen(Brushes.DodgerBlue, 4);
        private Pen _pathPenComp = new Pen(Brushes.Red, 5);

        private Cell _start;
        private Cell _finish;
        private Cell[] _cells;
        private bool _startSet = false;
        private bool _finishSet = false;

        private const int _columns = 80;//60;//40;
        private const int _rows = 80; //60;//40;

        public Form1()
        {
            InitializeComponent();
        }

        private void Test()
        {
            var mazeFile = $@"{Environment.CurrentDirectory}/maze.png";
            _mazeImg = new Bitmap(Bitmap.FromFile(mazeFile));
            _overImg = (Bitmap)_mazeImg.Clone();
            var cellPen = new Pen(Color.FromArgb(150, Color.LimeGreen));
            _gfx = Graphics.FromImage(_overImg);
            var cellCoordFont = new Font("Tahoma", 6, FontStyle.Regular);

            var cells = new List<Cell>();


            int xOffset = 0;
            int yOffset = 0;
            int step = 16;
            int halfStep = step / 2;
            int idxX = 0;
            int idxY = 0;
            for (int x = xOffset; x < _mazeImg.Width - xOffset - halfStep; x += step)
            {
                idxY = 0;
                for (int y = yOffset; y < _mazeImg.Height - yOffset - halfStep; y += step)
                {
                    var centerX = x + halfStep;
                    var centerY = y + halfStep;

                    _gfx.DrawRectangle(cellPen, new Rectangle(x, y, step, step));
                    _gfx.FillEllipse(Brushes.Blue, centerX, centerY, 2, 2);

                    //string label = $"{idxX},{idxY}";
                    //var lblSize = _gfx.MeasureString(label, cellCoordFont);
                    //_gfx.DrawString(label, cellCoordFont, Brushes.Black, new PointF((float)(centerX - (lblSize.Width / 2f)), (float)(centerY - (lblSize.Height / 2f))));

                    var pTop = _mazeImg.GetPixel(centerX, centerY - halfStep).ToArgb();
                    var pLeft = _mazeImg.GetPixel(centerX - halfStep, centerY).ToArgb();
                    var pBottom = _mazeImg.GetPixel(centerX, centerY + halfStep).ToArgb();
                    var pRight = _mazeImg.GetPixel(centerX + halfStep, centerY).ToArgb();

                    int sides = 0;
                    int whiteVal = -16777216;
                    if (pTop > whiteVal)
                        sides += (int)Side.Top;

                    if (pLeft > whiteVal)
                        sides += (int)Side.Left;

                    if (pBottom > whiteVal)
                        sides += (int)Side.Bottom;

                    if (pRight > whiteVal)
                        sides += (int)Side.Right;

                    cells.Add(new Cell(centerX, centerY, idxX, idxY, step, (Side)sides));

                    idxY++;
                }

                idxX++;
            }

            _cells = cells.ToArray();

            //var cellCollection = new CellCollection(cells, columns, rows);
            //var path = FindPath(cellCollection, cellCollection[10, 19], cellCollection[9, 0]);
            //DrawPath(path);

            pictureBox1.Image = _overImg;
        }

        private int _steps = 0;
        private void DrawPath(List<Cell> path, Pen pen, bool gradiant)
        {
            //_steps++;

            //if (_steps % 10 != 0)
            //    return;


            //_gfx.Clear(Color.White);
            //_gfx.DrawImage(_mazeImg, new Point());

            for (int i = 0; i < path.Count - 1; i++)
            {
                var a = path[i];
                var b = path[i + 1];
                if (gradiant)
                    pen.Color = GetVariableColor(Color.Blue, Color.Red, Color.Yellow, path.Count, i, 240, true);

                _gfx.DrawLine(pen, a.X, a.Y, b.X, b.Y);

                if (i == path.Count - 2)
                {
                    var poly = path[i].GetPoly();

                    _gfx.DrawPolygon(Pens.Blue, poly);
                }
            }

            pictureBox1.Image = _overImg;
            pictureBox1.Refresh();
            //Application.DoEvents();
        }

        private List<Cell> FindPath(CellCollection cells, Cell start, Cell finish)
        {
            foreach (var cell in cells.Cells)
                cell.HasPath = false;

            int delay = 1;
            var path = new List<Cell>() { start };
            int deadEnds = 0;
            int steps = 0;
            var openSides = start.GetOpenSides();
            start.HasPath = true;

            Cell next = cells.NextCell(start, openSides);
            path.Add(next);

            bool done = false;
            while (done == false)
            {
                steps++;
                next = cells.NextCell(next, next.GetOpenSides());

                if (next != null)
                {
                    path.Add(next);
                    next.HasPath = true;
                }
                else
                {
                    deadEnds++;
                    while (next == null)
                    {
                        steps++;
                        path.RemoveAt(path.Count - 1);
                        next = path.Last();
                        next = cells.NextCell(next, next.GetOpenSides());

                        //if (delay > 0)
                        //{
                        //    DrawPath(path);
                        //    Task.Delay(delay).Wait();
                        //}
                    }

                    path.Add(next);
                    next.HasPath = true;
                }


                if (next.IdxX == finish.IdxX && next.IdxY == finish.IdxY)
                    done = true;


                if (delay > 0)
                {
                    DrawPath(path, _pathPen, true);
                    Task.Delay(delay).Wait();
                }

            }
            Debug.WriteLine($"DeadEnds: {deadEnds}  Steps: {steps}");
            return path;
        }

        private bool PointIsInsidePolygon(PointF[] polygon, PointF testPoint)
        {
            int i, j = 0;
            bool c = false;
            for (i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if (((polygon[i].Y > testPoint.Y) != (polygon[j].Y > testPoint.Y)) && (testPoint.X < (polygon[j].X - polygon[i].X) * (testPoint.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X))
                    c = !c;
            }

            return c;
        }

        private bool PointIsInsidePolygon(Point[] polygon, Point testPoint)
        {
            int i, j = 0;
            bool c = false;
            for (i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if (((polygon[i].Y > testPoint.Y) != (polygon[j].Y > testPoint.Y)) && (testPoint.X < (polygon[j].X - polygon[i].X) * (testPoint.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X))
                    c = !c;
            }

            return c;
        }


        private Size GetScaledImageSize(Size imageSize, Size windowSize, bool keepAspectRatio = true)
        {
            var scaleFactor = ScaleFactor(imageSize, windowSize, keepAspectRatio);
            var scaledSize = new Size((int)(imageSize.Width / scaleFactor.X), (int)(imageSize.Height / scaleFactor.Y));

            return scaledSize;
        }

        private PointF ScaleFactor(Size imageSize, Size windowSize, bool keepAspectRatio = true)
        {
            var wfactor = (float)imageSize.Width / windowSize.Width;
            var hfactor = (float)imageSize.Height / windowSize.Height;

            var resizeFactor = Math.Max(wfactor, hfactor);
            if (keepAspectRatio)
            {
                wfactor = resizeFactor;
                hfactor = resizeFactor;
            }

            return new PointF(wfactor, hfactor);
        }
        private Color GetVariableColor(Color startColor, Color midColor, Color endColor, float maxValue, float currentValue, int alpha, bool translucent = false)
        {
            float intensity = 0;
            byte r1, g1, b1, r2, g2, b2;
            float maxHalf = maxValue * 0.5f;

            if (currentValue <= maxHalf)
            {
                r1 = startColor.R;
                g1 = startColor.G;
                b1 = startColor.B;

                r2 = midColor.R;
                g2 = midColor.G;
                b2 = midColor.B;

                maxValue = maxHalf;
            }
            else
            {
                r1 = midColor.R;
                g1 = midColor.G;
                b1 = midColor.B;

                r2 = endColor.R;
                g2 = endColor.G;
                b2 = endColor.B;

                maxValue = maxHalf;
                currentValue = currentValue - maxValue;
            }

            if (currentValue > 0)
            {
                // Compute the intensity of the end color.
                intensity = (currentValue / maxValue);
            }

            intensity = Math.Min(intensity, 1.0f);

            byte newR, newG, newB;
            newR = (byte)(r1 + (r2 - r1) * intensity);
            newG = (byte)(g1 + (g2 - g1) * intensity);
            newB = (byte)(b1 + (b2 - b1) * intensity);

            if (translucent)
            {
                return Color.FromArgb(alpha, newR, newG, newB);
            }
            else
            {
                return Color.FromArgb(newR, newG, newB);
            }
        }


        private Point TranslateZoomMousePosition(Point coordinates)
        {
            // test to make sure our image is not null
            if (_mazeImg == null) return coordinates;
            // Make sure our control width and height are not 0 and our 
            // image width and height are not 0
            if (pictureBox1.Width == 0 || pictureBox1.Height == 0 || _mazeImg.Width == 0 || _mazeImg.Height == 0) return coordinates;
            // This is the one that gets a little tricky. Essentially, need to check 
            // the aspect ratio of the image to the aspect ratio of the control
            // to determine how it is being rendered
            float imageAspect = (float)_mazeImg.Width / _mazeImg.Height;
            float controlAspect = (float)pictureBox1.Width / pictureBox1.Height;
            float newX = coordinates.X;
            float newY = coordinates.Y;
            if (imageAspect > controlAspect)
            {
                // This means that we are limited by width, 
                // meaning the image fills up the entire control from left to right
                float ratioWidth = (float)_mazeImg.Width / Width;
                newX *= ratioWidth;
                float scale = (float)pictureBox1.Width / _mazeImg.Width;
                float displayHeight = scale * _mazeImg.Height;
                float diffHeight = pictureBox1.Height - displayHeight;
                diffHeight /= 2;
                newY -= diffHeight;
                newY /= scale;
            }
            else
            {
                // This means that we are limited by height, 
                // meaning the image fills up the entire control from top to bottom
                float ratioHeight = (float)_mazeImg.Height / pictureBox1.Height;
                newY *= ratioHeight;
                float scale = (float)pictureBox1.Height / _mazeImg.Height;
                float displayWidth = scale * _mazeImg.Width;
                float diffWidth = pictureBox1.Width - displayWidth;
                diffWidth /= 2;
                newX -= diffWidth;
                newX /= scale;
            }
            return new Point((int)newX, (int)newY);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Test();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            var scaleLoc = TranslateZoomMousePosition(e.Location);

            Debug.WriteLine($"Clicked!  Raw: {e.Location}  P2Client: {pictureBox1.PointToClient(e.Location)}  P2Screen: {pictureBox1.PointToScreen(e.Location)}  Scaled: {scaleLoc}");

            if (_cells == null)
                return;

            foreach (var cell in _cells)
            {
                if (PointIsInsidePolygon(cell.GetPoly(), scaleLoc))
                {
                    Debug.WriteLine($"Cell Clicked: {cell}");


                    if (_startSet == false && _finishSet == false)
                    {
                        _start = cell;
                        _start.Sides -= Side.Bottom;

                        _startSet = true;
                        Debug.WriteLine($"Start cell: {_start}");
                    }
                    else if (_startSet == true && _finishSet == false)
                    {
                        _finish = cell;
                        _finish.Sides -= Side.Top;

                        _finishSet = true;
                        Debug.WriteLine($"Finish cell: {_finish}");

                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!_startSet || !_finishSet)
                return;

            var cellCollection = new CellCollection(_cells, _columns, _rows);

            var timer = new System.Diagnostics.Stopwatch();
            timer.Restart();

            _gfx.DrawImage(_mazeImg, new Point());
            var path = FindPath(cellCollection, _start, _finish);


            timer.Stop();
            System.Diagnostics.Debug.WriteLine(string.Format("Solution Time: {0} ms  {1} ticks", timer.Elapsed.TotalMilliseconds, timer.Elapsed.Ticks));


            DrawPath(path, _pathPenComp, false);
        }
    }

    public class CellCollection
    {
        public Cell[] Cells = new Cell[0];

        public int Columns = 0;
        public int Rows = 0;


        public CellCollection(List<Cell> cells, int columns, int rows)
        {
            Cells = cells.ToArray();
            Columns = columns;
            Rows = rows;
        }

        public CellCollection(Cell[] cells, int columns, int rows)
        {
            Cells = cells.ToArray();
            Columns = columns;
            Rows = rows;
        }

        public Cell this[int x, int y]
        {
            get
            {
                return Cells[x * Columns + y];
            }
        }

        public Cell NextCell(Cell current, Side side)
        {
            switch (side)
            {
                case Side.Top:
                    return this[current.IdxX, current.IdxY - 1];

                case Side.Left:
                    return this[current.IdxX - 1, current.IdxY];

                case Side.Bottom:
                    return this[current.IdxX, current.IdxY + 1];

                case Side.Right:
                default:
                    return this[current.IdxX + 1, current.IdxY];
            }

        }

        public Cell NextCell(Cell current, Side[] sides)
        {
            foreach (var side in sides)
            {
                var next = NextCell(current, side);
                if (next.HasPath == false)
                    return next;
            }

            return null;
        }
    }

    public class Cell
    {
        public int IdxX;
        public int IdxY;

        public int X;
        public int Y;
        public Side Sides;
        public int SideLen;
        public bool HasPath = false;

        //public Cell(int x, int y, List<Side> openSides)
        //{
        //    X = x;
        //    Y = y;
        //    OpenSides = openSides;
        //}

        public Cell(int x, int y, Side sides)
        {
            X = x;
            Y = y;
            Sides = sides;
        }

        public Cell(int x, int y, int idxX, int idxY, int sideLen, Side sides)
        {
            X = x;
            Y = y;
            IdxX = idxX;
            IdxY = idxY;
            Sides = sides;
            SideLen = sideLen;
        }

        public Side[] GetOpenSides()
        {
            var sides = new List<Side>();
            for (int i = 1; i <= 8; i <<= 1)
            {
                Side side = (Side)i;
                if ((Sides & side) == side)
                    sides.Add(side);
            }

            return sides.ToArray();
        }

        //public PointF[] GetPoly()
        //{
        //    var poly = new PointF[4];

        //    int sideLenHalf = SideLen / 2;
        //    poly[0] = new PointF(X - sideLenHalf, Y - sideLenHalf); // Top-Left
        //    poly[1] = new PointF(X + sideLenHalf, Y - sideLenHalf); // Top-Right
        //    poly[2] = new PointF(X + sideLenHalf, Y + sideLenHalf); // Bot-Right
        //    poly[3] = new PointF(X - sideLenHalf, Y + sideLenHalf); // Bot-Left
        //    return poly;
        //}

        public Point[] GetPoly()
        {
            var poly = new Point[4];

            int sideLenHalf = SideLen / 2;
            poly[0] = new Point(X - sideLenHalf, Y - sideLenHalf); // Top-Left
            poly[1] = new Point(X + sideLenHalf, Y - sideLenHalf); // Top-Right
            poly[2] = new Point(X + sideLenHalf, Y + sideLenHalf); // Bot-Right
            poly[3] = new Point(X - sideLenHalf, Y + sideLenHalf); // Bot-Left
            return poly;
        }



        public PointF[] GetPoly(PointF scale)
        {
            var poly = new PointF[4];

            int sideLenHalf = SideLen / 2;
            poly[0] = new PointF((X - sideLenHalf) * scale.X, (Y - sideLenHalf) * scale.Y); // Top-Left
            poly[1] = new PointF((X + sideLenHalf) * scale.X, (Y - sideLenHalf) * scale.Y); // Top-Right
            poly[2] = new PointF((X + sideLenHalf) * scale.X, (Y + sideLenHalf) * scale.Y); // Bot-Right
            poly[3] = new PointF((X - sideLenHalf) * scale.X, (Y + sideLenHalf) * scale.Y); // Bot-Left
            return poly;
        }

        public override string ToString()
        {
            string sides = "";

            for (int i = 1; i <= 8; i <<= 1)
            {
                Side side = (Side)i;
                if ((Sides & side) == side)
                    sides += $" {side.ToString()} |";
            }

            return $"({X},{Y}) ({IdxX},{IdxY}) [{sides}]";
        }
    }

    [Flags]
    public enum Side
    {
        Top = 1,
        Left = 2,
        Bottom = 4,
        Right = 8
    }
}