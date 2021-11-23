using System.Diagnostics;
namespace MazeTest
{
    public partial class Form1 : Form
    {
        private Graphics _gfx;
        private Bitmap _overImg;
        private Bitmap _mazeImg;
        private Pen _pathPen = new Pen(Brushes.BlueViolet, 4);
        private Pen _pathPenComp = new Pen(Brushes.Red, 6);

        private Pen _startCellPen = new Pen(Brushes.GreenYellow, 10);
        private Pen _finishCellPen = new Pen(Brushes.Blue, 10);

        private CellCollection _cellCollection;
        private Stack<Cell> _mazeGenStack;
        private List<Cell> _mazeGenCells;

        const int _blackPixelVal = -16777216;

        private Random _random = new Random();

        public Form1()
        {
            InitializeComponent();
        }


        private void InitPens(int stepSize)
        {
            _pathPen = new Pen(new SolidBrush(Color.FromArgb(150, Color.Blue)), stepSize / 2);
            _pathPenComp = new Pen(new SolidBrush(Color.FromArgb(150, Color.Red)), stepSize / 2);

            _startCellPen = new Pen(Brushes.GreenYellow, stepSize + 2);
            _finishCellPen = new Pen(Brushes.Blue, stepSize + 2);

        }

        private void LoadMaze(string path)
        {
            if (path == null)
                return;

            _mazeImg = new Bitmap(Bitmap.FromFile(path));
            _overImg = (Bitmap)_mazeImg.Clone();
            _gfx = Graphics.FromImage(_overImg);

            _gfx.DrawImage(_mazeImg, new Point());

            pictureBox1.Image = _overImg;

            ProcessMazeCells();

            SolveButton.Enabled = true;
        }

        private void LoadMaze(Image image)
        {
            if (image == null)
                return;

            _mazeImg = new Bitmap(image);
            _overImg = (Bitmap)_mazeImg.Clone();
            _gfx = Graphics.FromImage(_overImg);

            _gfx.DrawImage(_mazeImg, new Point());

            pictureBox1.Image = _overImg;

            ProcessMazeCells();

            SolveButton.Enabled = true;
        }

        private void ProcessMazeCells()
        {
            var cellPen = new Pen(Color.FromArgb(150, Color.LimeGreen));
            var cellCoordFont = new Font("Tahoma", 6, FontStyle.Regular);

            var cells = new List<Cell>();

            int xOffset = 0;
            int yOffset = 0;
            int stepSize = FindStepSize();
            int halfStep = stepSize / 2;
            int idxX = 0;
            int idxY = 0;

            InitPens(stepSize);

            var columns = _mazeImg.Width / stepSize;
            var rows = _mazeImg.Height / stepSize;


            for (int x = xOffset; x < _mazeImg.Width - xOffset - halfStep; x += stepSize)
            {
                idxY = 0;
                for (int y = yOffset; y < _mazeImg.Height - yOffset - halfStep; y += stepSize)
                {
                    var centerX = x + halfStep;
                    var centerY = y + halfStep;

                    _gfx.DrawRectangle(cellPen, new Rectangle(x, y, stepSize, stepSize));
                    _gfx.FillEllipse(Brushes.Blue, centerX, centerY, 2, 2);

                    //string label = $"{idxX},{idxY}";
                    //var lblSize = _gfx.MeasureString(label, cellCoordFont);
                    //_gfx.DrawString(label, cellCoordFont, Brushes.Black, new PointF((float)(centerX - (lblSize.Width / 2f)), (float)(centerY - (lblSize.Height / 2f))));

                    // Get pixel colors at each side.
                    var pTop = _mazeImg.GetPixel(centerX, centerY - halfStep).ToArgb();
                    var pLeft = _mazeImg.GetPixel(centerX - halfStep, centerY).ToArgb();
                    var pBottom = _mazeImg.GetPixel(centerX, centerY + halfStep).ToArgb();
                    var pRight = _mazeImg.GetPixel(centerX + halfStep, centerY).ToArgb();


                    // Set sides where pixels are not pure black.
                    int sides = 15;
                    if (pTop > _blackPixelVal)
                        sides -= (int)Side.Top;

                    if (pLeft > _blackPixelVal)
                        sides -= (int)Side.Left;

                    if (pBottom > _blackPixelVal)
                        sides -= (int)Side.Bottom;

                    if (pRight > _blackPixelVal)
                        sides -= (int)Side.Right;

                    cells.Add(new Cell(centerX, centerY, idxX, idxY, stepSize, (Side)sides));

                    idxY++;
                }

                idxX++;
            }

            _cellCollection = new CellCollection(cells, columns, rows, stepSize);
            _cellCollection.FindStartAndFinish();

            DrawCells(_cellCollection.Cells, _cellCollection.StartCell, _cellCollection.FinishCell);



            pictureBox1.Refresh();
        }

        private int FindStepSize()
        {
            int steps = 0;
            bool found = false;
            bool firstEdge = false;
            while (!found)
            {
                var pixel = _mazeImg.GetPixel(steps, steps).ToArgb();

                if (!firstEdge && pixel > _blackPixelVal)
                {
                    firstEdge = true;
                }
                else if (firstEdge && pixel <= _blackPixelVal)
                {
                    break;
                }



                steps++;
            }

            return steps;
        }

        private List<Cell> FindPath(CellCollection cells, int drawDelay = 1)
        {
            cells.ClearStates();

            Cell start = cells.StartCell;
            Cell finish = cells.FinishCell;

            int delay = drawDelay;
            var path = new List<Cell>() { start };
            //int deadEnds = 0;
            //int steps = 0;
            var openSides = start.GetOpenSides();
            start.WasVisited = true;

            Cell next = cells.NextCell(start, openSides);
            path.Add(next);

            bool done = false;
            while (done == false)
            {
                //steps++;
                next = cells.NextCell(next, next.GetOpenSides());

                if (next != null)
                {
                    path.Add(next);
                    next.WasVisited = true;
                }
                else
                {
                    //deadEnds++;
                    while (next == null)
                    {
                        //steps++;
                        path.RemoveAt(path.Count - 1);
                        next = path[path.Count - 1];

                        //next = path.Last();
                        next = cells.NextCell(next, next.GetOpenSides());

                        //if (delay > 0)
                        //{
                        //DrawPath(path, _pathPen, true);
                        //    Task.Delay(delay).Wait();
                        //}
                    }

                    path.Add(next);
                    next.WasVisited = true;
                }


                if (next.IdxX == finish.IdxX && next.IdxY == finish.IdxY)
                    done = true;


                if (delay > 0)
                {
                    DrawPath(path, _pathPen, true, false);
                    //Task.Delay(delay).Wait();
                }

            }

            //Debug.WriteLine($"DeadEnds: {deadEnds}  Steps: {steps}");
            return path;
        }

      
        private void GenerateNewMaze(int columns, int rows, int wallThickness, int cellSize)
        {
            var cells = new List<Cell>(columns * rows);
            int sides = 15;
            int halfStep = cellSize / 2;

            for (int col = 0; col < columns; col++)
            {
                for (int row = 0; row < rows; row++)
                {
                    var centerX = col * cellSize + halfStep;
                    var centerY = row * cellSize + halfStep;

                    var newCell = new Cell(centerX, centerY, col, row, cellSize, (Side)sides);
                    newCell.Index = newCell.GetIndex(columns, col, row);
                    cells.Add(newCell);
                }
            }

            _mazeGenStack = new Stack<Cell>();
            _mazeGenCells = cells;

            var bottomCells = cells.Where(cell => cell.IdxY == rows - 1).ToArray();
            var startCell = bottomCells[_random.Next(bottomCells.Length)];
            startCell.State = CellState.Visited;
            using (var newMazeImg = new Bitmap(rows * cellSize + (wallThickness / 2), columns * cellSize + (wallThickness / 2), System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            using (var gfx = Graphics.FromImage(newMazeImg))
            {
                var timer = new System.Diagnostics.Stopwatch();
                timer.Restart();

                GenerateNewMaze(startCell, columns, rows);

                timer.Stop();
                System.Diagnostics.Debug.WriteLine(string.Format("Gen Time: {0} ms  {1} ticks", timer.Elapsed.TotalMilliseconds, timer.Elapsed.Ticks));


                startCell.RemoveSide(Side.Bottom);

                var openTopCells = _mazeGenCells.Where(c => c.IdxY == 0).ToArray();
                openTopCells[_random.Next(openTopCells.Length)].RemoveSide(Side.Top);


                DrawMaze(_mazeGenCells.ToArray(), gfx, wallThickness);

                newMazeImg.Save($@"C:\Temp\maze.png");
                LoadMaze(newMazeImg);
            }
        }


        private void GenerateNewMaze(Cell startCell, int columns, int rows)
        {
            int doneCount = 0;
            int targetCount = _mazeGenCells.Where(c => c.State == CellState.Default).Count();

            Cell currentCell = startCell;
            while (doneCount < targetCount)
            {
                int cellIndex = currentCell.Index;

                int northIdx = currentCell.IdxX * columns + (currentCell.IdxY - 1);
                int eastIdx = (currentCell.IdxX + 1) * columns + currentCell.IdxY;
                int southIdx = currentCell.IdxX * columns + (currentCell.IdxY + 1);
                int westIdx = (currentCell.IdxX - 1) * columns + currentCell.IdxY;

                bool northEdge = currentCell.IdxY == 0;
                bool eastEdge = currentCell.IdxX == columns - 1;
                bool southEdge = currentCell.IdxY == rows - 1;
                bool westEdge = currentCell.IdxX == 0;

                //DrawCells(_mazeGenStack.ToArray());
                //DrawCells(_mazeGenCells.ToArray());

                //pictureBox1.Refresh();
                //Application.DoEvents();

                var unvisitedNeighbors = new List<Cell>();

                if (!northEdge && (northIdx >= 0 && northIdx < _mazeGenCells.Count) && _mazeGenCells[northIdx].State == CellState.Default)
                    unvisitedNeighbors.Add(_mazeGenCells[northIdx]);

                if (!eastEdge && (eastIdx >= 0 && eastIdx < _mazeGenCells.Count) && _mazeGenCells[eastIdx].State == CellState.Default)
                    unvisitedNeighbors.Add(_mazeGenCells[eastIdx]);

                if (!southEdge && (southIdx >= 0 && southIdx < _mazeGenCells.Count) && _mazeGenCells[southIdx].State == CellState.Default)
                    unvisitedNeighbors.Add(_mazeGenCells[southIdx]);

                if (!westEdge && (westIdx >= 0 && westIdx < _mazeGenCells.Count) && _mazeGenCells[westIdx].State == CellState.Default)
                    unvisitedNeighbors.Add(_mazeGenCells[westIdx]);

                if (unvisitedNeighbors.Count > 0)
                {
                    var selectedNeighbour = unvisitedNeighbors[_random.Next(unvisitedNeighbors.Count)];

                    int selectedNeightbourIndex = selectedNeighbour.Index;

                    if (selectedNeightbourIndex == northIdx)
                    {
                        currentCell.RemoveSide(Side.Top);
                        selectedNeighbour.RemoveSide(Side.Bottom);
                    }
                    else if (selectedNeightbourIndex == eastIdx)
                    {
                        currentCell.RemoveSide(Side.Right);
                        selectedNeighbour.RemoveSide(Side.Left);
                    }
                    else if (selectedNeightbourIndex == southIdx)
                    {
                        currentCell.RemoveSide(Side.Bottom);
                        selectedNeighbour.RemoveSide(Side.Top);
                    }
                    else if (selectedNeightbourIndex == westIdx)
                    {
                        currentCell.RemoveSide(Side.Left);
                        selectedNeighbour.RemoveSide(Side.Right);
                    }

                    _mazeGenStack.Push(currentCell);

                    selectedNeighbour.State = CellState.Visited;

                    currentCell = selectedNeighbour;

                    doneCount++;
                }
                else
                {
                    currentCell.State = CellState.Empty;

                    var previousCell = _mazeGenStack.Pop();
                    previousCell.State = CellState.Empty;

                    currentCell = previousCell;
                }
            }
        }


        private void StressTest(int count)
        {
            var timer = new System.Diagnostics.Stopwatch();
            timer.Restart();

            for (int i = 0; i < count; i++)
            {
                _steps = 0;

                _cellCollection.ClearStates();

                timer.Start();

                var path = FindPath(_cellCollection, 0);

                timer.Stop();

            }

            System.Diagnostics.Debug.WriteLine(string.Format("StressTest: {0} ms  {1} ticks", timer.Elapsed.TotalMilliseconds, timer.Elapsed.Ticks));

        }

        private void Solve()
        {
            _gfx.DrawImage(_mazeImg, new Point());
            pictureBox1.Refresh();

            _cellCollection.ClearStates();
            //_cellCollection.FindStartAndFinish();

            var timer = new System.Diagnostics.Stopwatch();
            timer.Restart();

            _steps = 0;
            var path = FindPath(_cellCollection, 0);

            timer.Stop();
            System.Diagnostics.Debug.WriteLine(string.Format("Solution Time: {0} ms  {1} ticks", timer.Elapsed.TotalMilliseconds, timer.Elapsed.Ticks));

            DrawVisitedCells(_cellCollection.Cells);
            DrawPath(path, _pathPenComp, false, true);

            pictureBox1.Refresh();
        }


        private int _steps = 0;
        private void DrawPath(List<Cell> path, Pen pen, bool gradiant, bool force)
        {

            //_gfx.Clear(Color.White);
            //_gfx.DrawImage(_mazeImg, new Point());

            for (int i = 0; i < path.Count - 1; i++)
            {
                var a = path[i];
                var b = path[i + 1];


                //if (gradiant)
                //    pen.Color = GetVariableColor(Color.Blue, Color.Red, Color.Orange, path.Count, i, 200, true);

                //pen.Color = GetVariableColor(Color.Blue, Color.Red, Color.Orange, path.Count, i, 255, false);

                if ((!a.WasDrawn || !b.WasDrawn) || force == true)
                {
                    //if (gradiant)
                    //{
                    //    var nDrawn = path.Count(p => p.WasDrawn);
                    //    pen.Color = GetVariableColor(Color.Blue, Color.Red, Color.Orange, _cells.Count(), nDrawn, 200, true);

                    //    //pen.Color = GetVariableColor(Color.Blue, Color.Red, Color.Orange, path.Count, i, 200, true);

                    //}

                    _gfx.DrawLine(pen, a.X, a.Y, b.X, b.Y);

                    a.WasDrawn = true;
                    b.WasDrawn = true;

                }


                //_gfx.DrawLine(pen, a.X, a.Y, b.X, b.Y);

            }


            _gfx.DrawPolygon(_startCellPen, _cellCollection.StartCell.GetPoly());
            _gfx.DrawPolygon(_finishCellPen, _cellCollection.FinishCell.GetPoly());

            _steps++;
            if (!(_steps % 100 != 0) || force)
            {
                pictureBox1.Refresh();
                Application.DoEvents();
            }
        }

        private void DrawMaze(Cell[] cells, Graphics gfx, int wallThickness)
        {
            gfx.Clear(Color.White);

            using (var wallPen = new Pen(Color.Black, wallThickness / 2))
            {
                foreach (var cell in cells)
                {
                    foreach (var side in cell.GetSides())
                    {
                        var start = new Point();
                        var end = new Point();
                        int sideLen = cell.SideLen / 2;
                        switch (side)
                        {
                            case Side.Top:
                                start = new Point(cell.X - sideLen, cell.Y - sideLen);
                                end = new Point(cell.X + sideLen, cell.Y - sideLen);
                                break;

                            case Side.Left:
                                start = new Point(cell.X - sideLen, cell.Y - sideLen);
                                end = new Point(cell.X - sideLen, cell.Y + sideLen);
                                break;
                            case Side.Bottom:
                                start = new Point(cell.X - sideLen, cell.Y + sideLen);
                                end = new Point(cell.X + sideLen, cell.Y + sideLen);
                                break;
                            case Side.Right:
                                start = new Point(cell.X + sideLen, cell.Y - sideLen);
                                end = new Point(cell.X + sideLen, cell.Y + sideLen);
                                break;
                        }

                        gfx.DrawLine(wallPen, start, end);
                    }
                }
            }
        }

        private void DrawCells(Cell[] cells, Cell startCell, Cell finishcell)
        {
            _steps++;

            _gfx.Clear(Color.White);

            foreach (var cell in cells)
            {
                foreach (var side in cell.GetSides())
                {
                    var start = new Point();
                    var end = new Point();
                    int sideLen = cell.SideLen / 2;
                    switch (side)
                    {
                        case Side.Top:
                            start = new Point(cell.X - sideLen, cell.Y - sideLen);
                            end = new Point(cell.X + sideLen, cell.Y - sideLen);
                            break;

                        case Side.Left:
                            start = new Point(cell.X - sideLen, cell.Y - sideLen);
                            end = new Point(cell.X - sideLen, cell.Y + sideLen);
                            break;
                        case Side.Bottom:
                            start = new Point(cell.X - sideLen, cell.Y + sideLen);
                            end = new Point(cell.X + sideLen, cell.Y + sideLen);
                            break;
                        case Side.Right:
                            start = new Point(cell.X + sideLen, cell.Y - sideLen);
                            end = new Point(cell.X + sideLen, cell.Y + sideLen);
                            break;
                    }

                    _gfx.DrawLine(Pens.Black, start, end);
                }
            }


            _gfx.DrawRectangle(new Pen(Color.LimeGreen, 4), startCell.GetRectangle());
            _gfx.DrawRectangle(new Pen(Color.Red, 4), finishcell.GetRectangle());

            pictureBox1.Refresh();
            Application.DoEvents();
        }

        private void DrawVisitedCells(Cell[] cells)
        {
            //using (var brush = new SolidBrush(Color.FromArgb(150, Color.RosyBrown)))
            using (var brush = new SolidBrush(Color.FromArgb(150, Color.Blue)))
            {

                foreach (var cell in cells)
                {
                    if (cell.WasVisited)
                        _gfx.FillPolygon(brush, cell.GetPoly());
                }
            }

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

        private void LoadMazeButton_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            var res = dialog.ShowDialog();
            if (res == DialogResult.OK)
            {
                var mazePath = dialog.FileName;
                LoadMaze(mazePath);
            }
        }

        private void SolveButton_Click(object sender, EventArgs e)
        {
            Solve();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            //if (!_startSet || !_finishSet)
            //    return;


            //var scaleLoc = TranslateZoomMousePosition(e.Location);

            //Debug.WriteLine($"Clicked!  Raw: {e.Location}  P2Client: {pictureBox1.PointToClient(e.Location)}  P2Screen: {pictureBox1.PointToScreen(e.Location)}  Scaled: {scaleLoc}");

            //if (_cells == null)
            //    return;

            //foreach (var cell in _cells)
            //{
            //    if (PointIsInsidePolygon(cell.GetPoly(), scaleLoc))
            //    {
            //        Debug.WriteLine($"Cell Clicked: {cell}");


            //        if (_startSet == false && _finishSet == false)
            //        {
            //            _startCell = cell;
            //            _startCell.Sides -= Side.Bottom;

            //            _startSet = true;
            //            Debug.WriteLine($"Start cell: {_startCell}");
            //        }
            //        else if (_startSet == true && _finishSet == false)
            //        {
            //            _finishCell = cell;
            //            _finishCell.Sides -= Side.Top;

            //            _finishSet = true;
            //            Debug.WriteLine($"Finish cell: {_finishCell}");

            //        }
            //    }
            //}
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StressTest(2000);
        }

        private void GenMazeButton_Click(object sender, EventArgs e)
        {
            //GenerateNewMaze(_cellCollection.Columns, _cellCollection.Rows, 2, _cellCollection.StepSize);

            string[] parseDims = GenColumnsRowsTextBox.Text.Trim().Split(',');
            if (int.TryParse(parseDims[0], out int columns) && int.TryParse(parseDims[1], out int rows))
            {
                GenerateNewMaze(columns, rows, 2, 4);
            }

            //GenerateNewMaze(400, 400, 2, 4);

        }
    }





}