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

        public void ClearStates()
        {
            foreach (var cell in Cells)
            {
                cell.HasPath = false;
                cell.WasDrawn = false;
            }
        }

        public bool FindStartAndFinish()
        {
            try
            {
                StartCell = Cells.Where(c => c.IdxY == Columns - 1 && c.GetOpenSides().Contains(Side.Bottom)).First();
                StartCell.Sides -= Side.Bottom;

                FinishCell = Cells.Where(c => c.IdxY == 0 && c.GetOpenSides().Contains(Side.Top)).First();
                FinishCell.Sides -= Side.Top;
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

}
