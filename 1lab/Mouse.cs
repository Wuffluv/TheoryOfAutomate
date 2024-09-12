using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace _1lab
{
    public class Mouse
    {
        public PointF Position { get; set; }

        public Mouse(PointF position)
        {
            Position = position;
        }
    }
}
