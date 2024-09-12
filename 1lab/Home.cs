using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace _1lab
{
    public class Home
    {
        public PointF Position { get; private set; }

        public Home(PointF position)
        {
            Position = position;
        }
    }
}
