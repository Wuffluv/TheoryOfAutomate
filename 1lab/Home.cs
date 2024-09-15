using System;
using System.Drawing;

namespace _1lab
{
    public class Home
    {
        public PointF Position { get; private set; }

        private static Random random = new Random();

        public Home(PointF position)
        {
            Position = position;
        }

        
    }
}
