using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeTest
{
    [Flags]
    public enum Side
    {
        Top = 1,
        Left = 2,
        Bottom = 4,
        Right = 8
    }
}
