using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeTest
{
    public class CellCollection
    {
        public Cell[] Cells = new Cell[0];

        public Cell StartCell { get; private set; }
        public Cell FinishCell { get; private set; }


        public int Columns = 0;
        public int Rows = 0;
        public int StepSize = 0;

        private Random _random = new Random();


        public CellCollection(List<Cell> cells, int columns, int rows, int stepSize)
        {
            Cells = cells.ToArray();
            Columns = columns;
            Rows = rows;
            StepSize = stepSize;
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
                if (x >= 0 && x <= Columns - 1 && y >= 0 && y <= Rows - 1)
                    return Cells[x * Columns + y];

                return null;
            }
        }

        public void ClearStates()
        {
            foreach (var cell in Cells)
            {
                cell.WasVisited = false;
                cell.WasDrawn = false;
            }
        }

        public bool FindStartAndFinish()
        {
            try
            {

                StartCell = Cells.Where(c => c.IdxY == Columns - 1 && c.GetOpenSides().Contains(Side.Bottom)).First();
                StartCell.AddSide(Side.Bottom);


                var openTopCells = Cells.Where(c => c.IdxY == 0 && c.GetOpenSides().Contains(Side.Top));
                if (!openTopCells.Any())
                {
                    var topCells = Cells.Where(c => c.IdxY == 0).ToArray();
                    FinishCell = topCells[_random.Next(topCells.Length)];
                }
                else
                {
                    FinishCell = openTopCells.First();
                    FinishCell.AddSide(Side.Top);
                }

                //var bottomCells = Cells.Where(c => c.IdxY == Columns - 1).ToArray();
                //StartCell = bottomCells[_random.Next(bottomCells.Length)];

                //var topCells = Cells.Where(c => c.IdxY == 0).ToArray();
                //FinishCell = topCells[_random.Next(topCells.Length)];


                //StartCell = Cells.Where(c => c.IdxY == Columns - 1 && c.GetOpenSides().Contains(Side.Bottom)).First();
                //StartCell.AddSide(Side.Bottom);

                //FinishCell = Cells.Where(c => c.IdxY == 0 && c.GetOpenSides().Contains(Side.Top)).First();
                //FinishCell.AddSide(Side.Top);


                //StartCell = Cells.Where(c => c.IdxY == Columns - 1 && c.GetOpenSides().Contains(Side.Bottom)).First();
                //StartCell.Sides -= Side.Bottom;

                //FinishCell = Cells.Where(c => c.IdxY == 0 && c.GetOpenSides().Contains(Side.Top)).First();
                //FinishCell.Sides -= Side.Top;
            }
            catch
            {
                return false;
            }

            if (StartCell == null || FinishCell == null)
                return false;

            return true;
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

        public List<Cell> GetNeighbors(Cell cell)
        {
            var ns = new List<Cell>();
            foreach (var side in SideIterator())
            {
                var n = NextCell(cell, side);
                if (n != null) 
                    ns.Add(n);
            }

            return ns;
        }

        public IEnumerable<Side> SideIterator()
        {
            for (int s = 1; s <= 8; s <<= 1)
                yield return (Side)s;
        }

        public Cell NextCell(Cell current, Side[] sides)
        {
            foreach (var side in sides)
            {
                var next = NextCell(current, side);
                if (next.WasVisited == false)
                    return next;
            }

            return null;
        }
    }

}
