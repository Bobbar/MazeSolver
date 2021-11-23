using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeTest
{
    public class Cell
    {
        public int IdxX;
        public int IdxY;

        public int X;
        public int Y;
        public Side Sides;
        public int SideLen;
        public bool WasVisited = false;
        public bool WasDrawn = false;
        public CellState State = CellState.Default;
        public int Index;

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
                if ((Sides & side) == 0)
                    sides.Add(side);
            }

            return sides.ToArray();
        }

        public Side[] GetSides()
        {
            var sides = new List<Side>();
            for (int i = 1; i <= 8; i <<= 1)
            {
                Side side = (Side)i;
                if (((int)Sides & (int)side) == i)
                    sides.Add(side);
            }

            return sides.ToArray();
        }

        public Side SharedSide(Cell cell)
        {
            if (cell.IdxX == IdxX && cell.IdxY == IdxY - 1) // Top
                return Side.Top;
            else if (cell.IdxX == IdxX + 1 && cell.IdxY == IdxY) // Right
                return Side.Right;
            else if (cell.IdxX == IdxX && cell.IdxY == IdxY + 1) // Bottom
                return Side.Bottom;
            else if (cell.IdxX == IdxX - 1 && cell.IdxY == IdxY) // Left
                return Side.Left;

            throw new Exception("Does not share a side.");

        }

        //public Side[] GetOpenSides()
        //{
        //    var sides = new List<Side>();
        //    for (int i = 1; i <= 8; i <<= 1)
        //    {
        //        Side side = (Side)i;
        //        if ((Sides & side) == side)
        //            sides.Add(side);
        //    }

        //    return sides.ToArray();
        //}

        public int GetIndex(int columns, int idxX, int idxY)
        {
            return idxX * columns + idxY;
        }

        public void RemoveSide(Side side)
        {
            if (HasSide(side))
                Sides -= (int)side;
        }

        public void AddSide(Side side)
        {
            if (!HasSide(side))
                Sides += (int)side;
        }

        public bool HasSide(Side side)
        {
            return (Sides & side) != 0;
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


        public Rectangle GetRectangle()
        {
            var p = GetPoly();
            return new Rectangle(p[0], new Size(SideLen, SideLen));
        }



        //public PointF[] GetPoly(PointF scale)
        //{
        //    var poly = new PointF[4];

        //    int sideLenHalf = SideLen / 2;
        //    poly[0] = new PointF((X - sideLenHalf) * scale.X, (Y - sideLenHalf) * scale.Y); // Top-Left
        //    poly[1] = new PointF((X + sideLenHalf) * scale.X, (Y - sideLenHalf) * scale.Y); // Top-Right
        //    poly[2] = new PointF((X + sideLenHalf) * scale.X, (Y + sideLenHalf) * scale.Y); // Bot-Right
        //    poly[3] = new PointF((X - sideLenHalf) * scale.X, (Y + sideLenHalf) * scale.Y); // Bot-Left
        //    return poly;
        //}




        public override string ToString()
        {
            string sides = "";

            for (int i = 1; i <= 8; i <<= 1)
            {
                Side side = (Side)i;
                if ((Sides & side) == side)
                    sides += $" {side.ToString()} |";
            }

            return $"[{Index}] ({IdxX},{IdxY}) [{sides}]";


            //return $"({X},{Y}) ({IdxX},{IdxY}) [{sides}]";
        }
    }

    public enum CellState
    {
        Default,
        Empty,
        Visited
    }
}
